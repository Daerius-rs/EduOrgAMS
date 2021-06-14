using System;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;
using MahApps.Metro.Controls;
using EduOrgAMS.Client.Dialogs;
using EduOrgAMS.Client.Localization;
using EduOrgAMS.Client.Navigation;
using EduOrgAMS.Client.Pages.ViewModel;
using EduOrgAMS.Client.Settings;
using EduOrgAMS.Client.Utils;
using EduOrgAMS.Client.Widgets;
using Math = RIS.Mathematics.Math;

namespace EduOrgAMS.Client.Pages
{
    public partial class UserProfilePage : PageContent
    {
        public static readonly DependencyProperty IsEditModeProperty =
            DependencyProperty.Register(nameof(IsEditMode), typeof(bool), typeof(UserProfilePage),
                new PropertyMetadata(false));

        private readonly SemaphoreSlim _profileUpdateLock = new SemaphoreSlim(1, 1);
        private int _profileUpdateWaitingCount;
        private bool _loadingGridShowing;

        public UserProfileViewModel ViewModel
        {
            get
            {
                return DataContext as UserProfileViewModel;
            }
        }

        public bool IsEditMode
        {
            get
            {
                return (bool)GetValue(IsEditModeProperty);
            }
            set
            {
                SetValue(IsEditModeProperty, value);
            }
        }

        public UserProfilePage()
        {
            InitializeComponent();
            DataContext = new UserProfileViewModel();

            LocalizationManager.LanguageChanged += OnLanguageChanged;
            SettingsManager.PersistentSettings.CurrentUserChanged += OnCurrentUserChanged;
        }

        ~UserProfilePage()
        {
            LocalizationManager.LanguageChanged -= OnLanguageChanged;
            SettingsManager.PersistentSettings.CurrentUserChanged -= OnCurrentUserChanged;
        }

        public Task UpdateProfile()
        {
            return UpdateProfile(ViewModel.CurrentUser.Id);
        }
        public async Task UpdateProfile(int id)
        {
            await ShowLoadingGrid(true)
                .ConfigureAwait(true);

            try
            {
                Interlocked.Increment(ref _profileUpdateWaitingCount);

                await _profileUpdateLock.WaitAsync()
                    .ConfigureAwait(true);
            }
            finally
            {
                Interlocked.Decrement(ref _profileUpdateWaitingCount);
            }

            try
            {
                btnEditMode.IsChecked = false;

                if (id < 0)
                {
                    if (!NavigationController.Instance.IsCurrentPage<UserProfilePage>())
                        return;

                    NavigationController.Instance.GoBack(true);

                    string message = LocalizationUtils.GetLocalized("UserNotFound");

                    await DialogManager.ShowErrorDialog(message)
                        .ConfigureAwait(true);

                    return;
                }

                var result = await DatabaseManager.FindAsync<User>(id)
                    .ConfigureAwait(true);

                if (result == null)
                {
                    if (!NavigationController.Instance.IsCurrentPage<UserProfilePage>())
                        return;

                    NavigationController.Instance.GoBack(true);

                    string message = LocalizationUtils.GetLocalized("UserNotFound");

                    await DialogManager.ShowErrorDialog(message)
                        .ConfigureAwait(true);

                    return;
                }

                ViewModel.CurrentUser = result;

                UpdateStatBlock(wpStatBlock1, wpStatBlock2);
            }
            finally
            {
                await Task.Delay(500)
                    .ConfigureAwait(true);

                if (_profileUpdateWaitingCount == 0)
                {
                    await ShowLoadingGrid(false)
                        .ConfigureAwait(true);
                }

                _profileUpdateLock.Release();
            }
        }

        public void UpdateStatBlock(params StackPanel[] statBlocks)
        {
            foreach (var statBlock in statBlocks)
            {
                UpdateStatBlock(statBlock);
            }
        }
        public void UpdateStatBlock(StackPanel statBlock)
        {
            UpdateLayout();

            var visibilityMultiBinding =
                BindingOperations.GetMultiBindingExpression(statBlock,
                    VisibilityProperty);

            if (visibilityMultiBinding == null)
                return;

            foreach (var binding in
                visibilityMultiBinding.BindingExpressions)
            {
                binding.UpdateTarget();
            }

            visibilityMultiBinding.UpdateTarget();

            UpdateLayout();
        }

        public Task ShowLoadingGrid(bool status)
        {
            if (status)
            {
                _loadingGridShowing = true;
                loadingIndicator.IsActive = true;
                loadingGrid.Opacity = 1.0;
                loadingGrid.IsHitTestVisible = true;
                loadingGrid.Visibility = Visibility.Visible;

                return Task.CompletedTask;
            }

            _loadingGridShowing = false;
            loadingIndicator.IsActive = false;

            return Task.Run(async () =>
            {
                for (double i = 1.0; i > 0.0; i -= 0.025)
                {
                    var opacity = i;

                    if (_loadingGridShowing)
                        break;

                    if (Math.AlmostEquals(opacity, 0.7, 0.01))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            loadingGrid.IsHitTestVisible = false;
                        });
                    }

                    Dispatcher.Invoke(() =>
                    {
                        loadingGrid.Opacity = opacity;
                    });

                    await Task.Delay(4)
                        .ConfigureAwait(false);
                }

                Dispatcher.Invoke(() =>
                {
                    loadingGrid.Visibility = Visibility.Collapsed;
                });
            });
        }

        protected override async void OnEnter(object sender, RoutedEventArgs e)
        {
            base.OnEnter(sender, e);

            UpdateLayout();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            if (!IsOnEnterActive)
            {
                e.Handled = true;
                return;
            }

            await UpdateProfile()
                .ConfigureAwait(true);
        }

        protected override void OnExit(object sender, RoutedEventArgs e)
        {
            base.OnExit(sender, e);

            if (!IsOnExitActive)
            {
                e.Handled = true;
                return;
            }

            UpdateLayout();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        protected override async void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.ViewModelPropertyChanged(sender, e);

            if (e.PropertyName.Length == 0)
            {
                await UpdateProfile()
                    .ConfigureAwait(true);
            }
        }

        private void OnLanguageChanged(object sender, LanguageChangedEventArgs e)
        {
            ProfileStatGender
                .GetBindingExpression(UserProfileStat.StatValueProperty)?
                .UpdateTarget();
        }

        private async void OnCurrentUserChanged(object sender, UserChangedEventArgs e)
        {
            await UpdateProfile()
                .ConfigureAwait(true);
        }

        private void CopyUserLink_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText($"eoams://app/user/id/{ViewModel.CurrentUser.Id}");
        }

        private void CopyUserId_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ViewModel.CurrentUser.Id.ToString());
        }

        private void btnEditMode_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatBlock(wpStatBlock1, wpStatBlock2);
        }

        private async void EditSinglelineText_Click(object sender, RoutedEventArgs e)
        {
            UserProfileStat element = sender as UserProfileStat;

            BindingExpression binding = element?.GetBindingExpression(UserProfileStat.StatValueProperty);

            if (binding == null)
                return;

            User sourceClass = (User)binding.ResolvedSource;
            PropertyInfo sourceProperty = typeof(User).GetProperty(binding.ResolvedSourcePropertyName);

            if (sourceClass == null || sourceProperty == null)
                return;

            string title = LocalizationUtils.GetLocalized("ProfileEditingTitle");
            string enterName = LocalizationUtils.GetLocalized("EnterTitle");

            string oldValue = (string)sourceProperty.GetValue(sourceClass);
            string value = await DialogManager.ShowSinglelineTextDialog(title,
                    $"{enterName} '{element.StatTitle}'", oldValue)
                .ConfigureAwait(true);

            if (value == null)
                return;

            await using var context = DatabaseManager.CreateContext();

            var user = await context.Users.FindAsync(
                    SettingsManager.PersistentSettings.CurrentUser.Id)
                .ConfigureAwait(true);

            if (user == null)
            {
                var message = LocalizationUtils.GetLocalized("GettingProfileErrorMessage");

                await DialogManager.ShowErrorDialog(message)
                    .ConfigureAwait(true);
                return;
            }

            sourceProperty.SetValue(sourceClass, value);
            sourceProperty.SetValue(user, value);

            var changesCount = await DatabaseManager.SaveChangesAsync(context)
                .ConfigureAwait(true);

            if (changesCount == 0)
            {
                var message = LocalizationUtils.GetLocalized("UpdatingDataErrorMessage");

                await DialogManager.ShowErrorDialog(message)
                    .ConfigureAwait(true);

                sourceProperty.SetValue(sourceClass, oldValue);
            }

            var statBlock = element.TryFindParent<StackPanel>();

            if (statBlock == null)
                return;

            UpdateStatBlock(statBlock);
        }

        private async void EditMultilineText_Click(object sender, RoutedEventArgs e)
        {
            UserProfileStat element = sender as UserProfileStat;

            BindingExpression binding = element?.GetBindingExpression(UserProfileStat.StatValueProperty);

            if (binding == null)
                return;

            User sourceClass = (User)binding.ResolvedSource;
            PropertyInfo sourceProperty = typeof(User).GetProperty(binding.ResolvedSourcePropertyName);

            if (sourceClass == null || sourceProperty == null)
                return;

            string title = LocalizationUtils.GetLocalized("ProfileEditingTitle");
            string enterName = LocalizationUtils.GetLocalized("EnterTitle");

            string oldValue = (string)sourceProperty.GetValue(sourceClass);
            string value = await DialogManager.ShowMultilineTextDialog(title,
                    $"{enterName} '{element.StatTitle}'", oldValue)
                .ConfigureAwait(true);

            if (value == null)
                return;

            await using var context = DatabaseManager.CreateContext();

            var user = await context.Users.FindAsync(
                    SettingsManager.PersistentSettings.CurrentUser.Id)
                .ConfigureAwait(true);

            if (user == null)
            {
                var message = LocalizationUtils.GetLocalized("GettingProfileErrorMessage");

                await DialogManager.ShowErrorDialog(message)
                    .ConfigureAwait(true);
                return;
            }

            sourceProperty.SetValue(sourceClass, value);
            sourceProperty.SetValue(user, value);

            var changesCount = await DatabaseManager.SaveChangesAsync(context)
                .ConfigureAwait(true);

            if (changesCount == 0)
            {
                var message = LocalizationUtils.GetLocalized("UpdatingDataErrorMessage");

                await DialogManager.ShowErrorDialog(message)
                    .ConfigureAwait(true);

                sourceProperty.SetValue(sourceClass, oldValue);
            }

            var statBlock = element.TryFindParent<StackPanel>();

            if (statBlock == null)
                return;

            UpdateStatBlock(statBlock);
        }

        private async void EditPhoneNumber_Click(object sender, RoutedEventArgs e)
        {
            UserProfileStat element = sender as UserProfileStat;

            BindingExpression binding = element?.GetBindingExpression(UserProfileStat.StatValueProperty);

            if (binding == null)
                return;

            User sourceClass = (User)binding.ResolvedSource;
            PropertyInfo sourceProperty = typeof(User).GetProperty(binding.ResolvedSourcePropertyName);

            if (sourceClass == null || sourceProperty == null)
                return;

            string title = LocalizationUtils.GetLocalized("ProfileEditingTitle");
            string enterName = LocalizationUtils.GetLocalized("EnterTitle");

            string oldValue = (string)sourceProperty.GetValue(sourceClass);
            string value = await DialogManager.ShowSinglelineTextDialog(title,
                    $"{enterName} '{element.StatTitle}'", oldValue)
                .ConfigureAwait(true);

            if (value == null)
                return;

            if (!string.IsNullOrEmpty(value)
                && !ValidateUtils.IsValidPatternString(value,
                    @"^(?<phone_number>(?<country_code>\+7)(?:[\s]{1})(?:(?:(?:[\(]{1})(?=\d{3}[\)]{1}))?(?<area_code>(?:\d{3}))(?:(?<=[\(]{1}\d{3})(?:[\)]{1}))?)(?:[\s]{1})(?<prefix>\d{3})(?:[-\s]{1})(?<suffix>(?<suffix_part_1>\d{2})(?:[-\s]{1})(?<suffix_part_2>\d{2}))(?!\d))$",
                    RegexOptions.IgnoreCase))
            {
                var message = LocalizationUtils
                    .GetLocalized("InvalidFieldValueErrorMessage");

                await DialogManager.ShowErrorDialog($"{message} - '{element.StatTitle}'")
                    .ConfigureAwait(true);

                return;
            }

            await using var context = DatabaseManager.CreateContext();

            var user = await context.Users.FindAsync(
                    SettingsManager.PersistentSettings.CurrentUser.Id)
                .ConfigureAwait(true);

            if (user == null)
            {
                var message = LocalizationUtils.GetLocalized("GettingProfileErrorMessage");

                await DialogManager.ShowErrorDialog(message)
                    .ConfigureAwait(true);
                return;
            }

            sourceProperty.SetValue(sourceClass, value);
            sourceProperty.SetValue(user, value);

            var changesCount = await DatabaseManager.SaveChangesAsync(context)
                .ConfigureAwait(true);

            if (changesCount == 0)
            {
                var message = LocalizationUtils.GetLocalized("UpdatingDataErrorMessage");

                await DialogManager.ShowErrorDialog(message)
                    .ConfigureAwait(true);

                sourceProperty.SetValue(sourceClass, oldValue);
            }

            var statBlock = element.TryFindParent<StackPanel>();

            if (statBlock == null)
                return;

            UpdateStatBlock(statBlock);
        }

        private async void EditEmail_Click(object sender, RoutedEventArgs e)
        {
            UserProfileStat element = sender as UserProfileStat;

            BindingExpression binding = element?.GetBindingExpression(UserProfileStat.StatValueProperty);

            if (binding == null)
                return;

            User sourceClass = (User)binding.ResolvedSource;
            PropertyInfo sourceProperty = typeof(User).GetProperty(binding.ResolvedSourcePropertyName);

            if (sourceClass == null || sourceProperty == null)
                return;

            string title = LocalizationUtils.GetLocalized("ProfileEditingTitle");
            string enterName = LocalizationUtils.GetLocalized("EnterTitle");

            string oldValue = (string)sourceProperty.GetValue(sourceClass);
            string value = await DialogManager.ShowSinglelineTextDialog(title,
                    $"{enterName} '{element.StatTitle}'", oldValue)
                .ConfigureAwait(true);

            if (value == null)
                return;

            if (!string.IsNullOrEmpty(value)
                && !ValidateUtils.IsValidPatternString(value,
                    @"^(?<email>[a-z0-9!#$%&'*+\=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9]))$",
                    RegexOptions.IgnoreCase))
            {
                var message = LocalizationUtils
                    .GetLocalized("InvalidFieldValueErrorMessage");

                await DialogManager.ShowErrorDialog($"{message} - '{element.StatTitle}'")
                    .ConfigureAwait(true);

                return;
            }

            await using var context = DatabaseManager.CreateContext();

            var user = await context.Users.FindAsync(
                    SettingsManager.PersistentSettings.CurrentUser.Id)
                .ConfigureAwait(true);

            if (user == null)
            {
                var message = LocalizationUtils.GetLocalized("GettingProfileErrorMessage");

                await DialogManager.ShowErrorDialog(message)
                    .ConfigureAwait(true);
                return;
            }

            sourceProperty.SetValue(sourceClass, value);
            sourceProperty.SetValue(user, value);

            var changesCount = await DatabaseManager.SaveChangesAsync(context)
                .ConfigureAwait(true);

            if (changesCount == 0)
            {
                var message = LocalizationUtils.GetLocalized("UpdatingDataErrorMessage");

                await DialogManager.ShowErrorDialog(message)
                    .ConfigureAwait(true);

                sourceProperty.SetValue(sourceClass, oldValue);
            }

            var statBlock = element.TryFindParent<StackPanel>();

            if (statBlock == null)
                return;

            UpdateStatBlock(statBlock);
        }

        private void Avatar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(ViewModel.CurrentUser.AvatarUrl))
                return;

            NavigationController.Instance.RequestOverlay<ImagePreviewOverlayPage>(new ImagePreviewOverlayViewModel()
            {
                ImageSource = ViewModel.CurrentUser.AvatarUrl
            });

            e.Handled = true;
        }

        private void Avatar_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!ViewModel.EditAllowed)
                e.Handled = true;
        }
    }
}
