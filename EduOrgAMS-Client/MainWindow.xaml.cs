using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EduOrgAMS.Client.Connection;
using EduOrgAMS.Client.Database;
using MahApps.Metro.Controls;
using EduOrgAMS.Client.Dialogs;
using EduOrgAMS.Client.Localization;
using EduOrgAMS.Client.Localization.Entities;
using EduOrgAMS.Client.Native.Window;
using EduOrgAMS.Client.Navigation;
using EduOrgAMS.Client.Pages;
using EduOrgAMS.Client.Settings;
using EduOrgAMS.Client.Styles;
using EduOrgAMS.Client.Styles.Loading.Entities;
using EduOrgAMS.Client.Utils;
using RIS;
using Math = RIS.Mathematics.Math;

namespace EduOrgAMS.Client
{
    public sealed partial class MainWindow : MetroWindow, INativeRestorableWindow, ILocalizable
    {
        private static readonly object InstanceSyncRoot = new object();
        private static volatile MainWindow _instance;
        public static MainWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceSyncRoot)
                    {
                        if (_instance == null)
                            _instance = new MainWindow();
                    }
                }

                return _instance;
            }
        }

        private HwndSource _hwndSource;
        private WindowState _previousState;

        private int _loadingGridStatus;
        public bool LoadingGridStatus
        {
            get
            {
                return _loadingGridStatus > 0;
            }
            private set
            {
                Interlocked.Exchange(ref _loadingGridStatus, value ? 1 : 0);
            }
        }
        private Task _loadingGridTask;
        private int _connectionFailedGridStatus;
        public bool ConnectionFailedGridStatus
        {
            get
            {
                return _connectionFailedGridStatus > 0;
            }
            private set
            {
                Interlocked.Exchange(ref _connectionFailedGridStatus, value ? 1 : 0);
            }
        }
        private Task _connectionFailedGridTask;

        private int _titleLockStatus;
        public bool TitleLockStatus
        {
            get
            {
                return _titleLockStatus > 0;
            }
            private set
            {
                Interlocked.Exchange(ref _titleLockStatus, value ? 1 : 0);
            }
        }
        private bool _loadingGridTitleLockStatus;
        private bool _connectionFailedGridTitleLockStatus;

        private LoadingStyle _loadingStyle;

        public ReadOnlyDictionary<string, LocalizationXamlModule> Locales { get; private set; }
        public string AppVersion { get; private set; }
        public bool DuringRestoreToMaximized { get; private set; }

        private MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            _instance = this;

            RootLayout.Children.Add(NavigationController.Instance);

            Width = SettingsManager.AppSettings.WindowWidth;
            Height = SettingsManager.AppSettings.WindowHeight;
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = SettingsManager.AppSettings.WindowPositionX - (Width / 2.0);
            Top = SettingsManager.AppSettings.WindowPositionY - (Height / 2.0);
            WindowState = (WindowState)SettingsManager.AppSettings.WindowState;

            _previousState = WindowState;
            DuringRestoreToMaximized = WindowState == WindowState.Maximized;

            AppVersion = $"v{SettingsManager.AppSettings.AppVersion}";

            LoadingGridStatus = false;
            _loadingGridTask = Task.CompletedTask;
            ConnectionFailedGridStatus = false;
            _connectionFailedGridTask = Task.CompletedTask;

            LoadLoadingStyle();

            LocalizationManager.ReloadLocales();

            if (Locales.Count == 0)
                return;

            LocalizationManager.SetDefaultLanguage()
                .ConfigureAwait(true);

            if (Locales.TryGetValue(SettingsManager.AppSettings.Language, out var localizationModule))
            {
                slcLanguage.SelectedItem = new KeyValuePair<string, LocalizationXamlModule>(
                    SettingsManager.AppSettings.Language, localizationModule);
            }
            else if (Locales.TryGetValue("en-US", out localizationModule))
            {
                slcLanguage.SelectedItem = new KeyValuePair<string, LocalizationXamlModule>(
                    "en-US", localizationModule);

                SettingsManager.AppSettings.Language = "en-US";
                SettingsManager.AppSettings.Save();
            }
            else
            {
                var locale = Locales.First();

                slcLanguage.SelectedItem = locale;

                SettingsManager.AppSettings.Language = locale.Key;
                SettingsManager.AppSettings.Save();
            }

            ConnectionManager.ConnectionStateChanged += OnConnectionStateChanged;
            LocalizationManager.LanguageChanged += OnLanguageChanged;
        }

        ~MainWindow()
        {
            ConnectionManager.ConnectionStateChanged -= OnConnectionStateChanged;
            LocalizationManager.LanguageChanged -= OnLanguageChanged;
        }

        private void LoadLoadingStyle()
        {
            _loadingStyle = StylesManager.GetRandomLoadingStyle();

            if (_loadingStyle.ForegroundImageUri != null)
            {

                loadingForegroundImage.Source = new BitmapImage(
                    _loadingStyle.ForegroundImageUri);
                connectionFailedForegroundImage.Source = new BitmapImage(
                    _loadingStyle.ForegroundImageUri);

                loadingForegroundImage.Visibility = Visibility.Visible;
                connectionFailedForegroundImage.Visibility = Visibility.Visible;
            }
            else
            {
                loadingForegroundImage.Source = null;
                connectionFailedForegroundImage.Source = null;

                loadingForegroundImage.Visibility = Visibility.Hidden;
                connectionFailedForegroundImage.Visibility = Visibility.Hidden;
            }

            if (_loadingStyle.BackgroundImageUri != null)
            {

                loadingBackgroundImage.Source = new BitmapImage(
                    _loadingStyle.BackgroundImageUri);
                connectionFailedBackgroundImage.Source = new BitmapImage(
                    _loadingStyle.BackgroundImageUri);

                loadingBackgroundImage.Visibility = Visibility.Visible;
                connectionFailedBackgroundImage.Visibility = Visibility.Visible;
            }
            else
            {
                loadingBackgroundImage.Source = null;
                connectionFailedBackgroundImage.Source = null;

                loadingBackgroundImage.Visibility = Visibility.Hidden;
                connectionFailedBackgroundImage.Visibility = Visibility.Hidden;
            }

            loadingIndicator.HorizontalAlignment = _loadingStyle.LoadingIndicatorHorizontalAlignment;
            loadingIndicator.VerticalAlignment= _loadingStyle.LoadingIndicatorVerticalAlignment;

            connectionFailedIndicator.HorizontalAlignment = _loadingStyle.LoadingIndicatorHorizontalAlignment;
            connectionFailedIndicator.VerticalAlignment = _loadingStyle.LoadingIndicatorVerticalAlignment;
        }

        public void LinkOpenEnable(bool isEnabled)
        {
            if (NavigationController.Instance.RootLayout.DisplayMode == SplitViewDisplayMode.Inline
                || loadingGrid.Visibility == Visibility.Visible
                || connectionFailedGrid.Visibility == Visibility.Visible)
            {
                btnLinkOpen.IsEnabled = false;
                return;
            }

            btnLinkOpen.IsEnabled = isEnabled;
        }

        public bool IsOpenSettings()
        {
            return SettingsFlyout.IsOpen;
        }

        public void ShowSettings()
        {
            SettingsFlyout.IsOpen = true;
        }

        public void HideSettings()
        {
            SettingsFlyout.IsOpen = false;
        }

        public async Task ShowLoadingGrid(bool status)
        {
            await _loadingGridTask
                .ConfigureAwait(true);

            var newStatus = status ? 1 : 0;
            var oldStatus = Interlocked.Exchange(
                ref _loadingGridStatus, newStatus);

            if (oldStatus == newStatus)
                return;

            bool changeStyleNeeded;

            if (!TitleLockStatus)
            {
                TitleLockStatus = true;
                _loadingGridTitleLockStatus = true;

                changeStyleNeeded = true;
            }
            else
            {
                changeStyleNeeded = false;
            }

            if (status)
            {
                LinkOpenEnable(false);
                loadingIndicator.IsActive = true;
                loadingGrid.Opacity = 1.0;
                loadingGrid.IsHitTestVisible = true;
                loadingGrid.Visibility = Visibility.Visible;

                if (changeStyleNeeded)
                {
                    Resources.MergedDictionaries
                        .Add(_loadingStyle.Dictionary);
                    Resources.MergedDictionaries
                        .Add(_loadingStyle.MahAppsDictionary);
                }

                _loadingGridTask = Task.CompletedTask;
                return;
            }

            loadingIndicator.IsActive = false;

            if (_loadingGridTitleLockStatus)
                changeStyleNeeded = true;

            var color = ((SolidColorBrush)FindResource("Window.Main.TitleBackground")).Color;

            _loadingGridTask = Task.Run(async () =>
            {
                Action<double> changeOpacityFunction;

                if (changeStyleNeeded)
                {
                    changeOpacityFunction = opacity =>
                    {
                        loadingGrid.Opacity = opacity;

                        var brush = new SolidColorBrush(
                            Color.FromArgb((byte)(255 * opacity),
                                color.R, color.G, color.B));

                        WindowTitleBrush = brush;
                        NonActiveWindowTitleBrush = brush;
                    };
                }
                else
                {
                    changeOpacityFunction = opacity =>
                    {
                        loadingGrid.Opacity = opacity;
                    };
                }

                var isHitTestVisible = true;

                for (double i = 1.0; i > 0.0; i -= 0.025)
                {
                    var opacity = i;

                    if (isHitTestVisible
                        && Math.AlmostEquals(opacity, 0.7, 0.01))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            loadingGrid.IsHitTestVisible = false;

                            Resources.MergedDictionaries
                                .Remove(_loadingStyle.MahAppsDictionary);
                        });

                        isHitTestVisible = false;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        changeOpacityFunction(opacity);
                    });

                    await Task.Delay(4)
                        .ConfigureAwait(false);
                }

                Dispatcher.Invoke(() =>
                {
                    if (changeStyleNeeded)
                    {
                        Resources.MergedDictionaries
                            .Remove(_loadingStyle.Dictionary);

                        SetResourceReference(WindowTitleBrushProperty, "Window.Main.TitleBackground");
                        SetResourceReference(NonActiveWindowTitleBrushProperty, "Window.Main.NonActiveTitleBackground");
                    }

                    loadingGrid.IsHitTestVisible = false;
                    loadingGrid.Visibility = Visibility.Collapsed;
                    LinkOpenEnable(true);

                    if (changeStyleNeeded)
                    {
                        TitleLockStatus = false;
                        _loadingGridTitleLockStatus = false;
                    }
                });
            });
        }

        public async Task ShowConnectionFailedGrid(bool status)
        {
            await _connectionFailedGridTask
                .ConfigureAwait(true);

            var newStatus = status ? 1 : 0;
            var oldStatus = Interlocked.Exchange(
                ref _connectionFailedGridStatus, newStatus);

            if (oldStatus == newStatus)
                return;

            bool changeStyleNeeded;

            if (!TitleLockStatus)
            {
                TitleLockStatus = true;
                _connectionFailedGridTitleLockStatus = true;

                changeStyleNeeded = true;
            }
            else
            {
                changeStyleNeeded = false;
            }

            if (status)
            {
                LinkOpenEnable(false);
                connectionFailedIndicator.IsActive = true;
                connectionFailedGrid.Opacity = 1.0;
                connectionFailedGrid.IsHitTestVisible = true;
                connectionFailedGrid.Visibility = Visibility.Visible;

                if (changeStyleNeeded)
                {
                   Resources.MergedDictionaries
                        .Add(_loadingStyle.Dictionary);
                   Resources.MergedDictionaries
                       .Add(_loadingStyle.MahAppsDictionary);
                }

                _connectionFailedGridTask = Task.CompletedTask;
                return;
            }

            connectionFailedIndicator.IsActive = false;

            if (_connectionFailedGridTitleLockStatus)
                changeStyleNeeded = true;

            var color = ((SolidColorBrush)FindResource("Window.Main.TitleBackground")).Color;

            _connectionFailedGridTask = Task.Run(async () =>
            {
                Action<double> changeOpacityFunction;

                if (changeStyleNeeded)
                {
                    changeOpacityFunction = opacity =>
                    {
                        connectionFailedGrid.Opacity = opacity;

                        var brush = new SolidColorBrush(
                            Color.FromArgb((byte)(255 * opacity),
                                color.R, color.G, color.B));

                        WindowTitleBrush = brush;
                        NonActiveWindowTitleBrush = brush;
                    };
                }
                else
                {
                    changeOpacityFunction = opacity =>
                    {
                        connectionFailedGrid.Opacity = opacity;
                    };
                }

                var isHitTestVisible = true;

                for (double i = 1.0; i > 0.0; i -= 0.025)
                {
                    var opacity = i;

                    if (isHitTestVisible
                        && Math.AlmostEquals(opacity, 0.7, 0.01))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            connectionFailedGrid.IsHitTestVisible = false;

                            Resources.MergedDictionaries
                                .Remove(_loadingStyle.MahAppsDictionary);
                        });

                        isHitTestVisible = false;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        changeOpacityFunction(opacity);
                    });

                    await Task.Delay(4)
                        .ConfigureAwait(false);
                }

                Dispatcher.Invoke(() =>
                {
                    if (changeStyleNeeded)
                    {
                        Resources.MergedDictionaries
                            .Remove(_loadingStyle.Dictionary);

                        SetResourceReference(WindowTitleBrushProperty, "Window.Main.TitleBackground");
                        SetResourceReference(NonActiveWindowTitleBrushProperty, "Window.Main.NonActiveTitleBackground");
                    }

                    connectionFailedGrid.IsHitTestVisible = false;
                    connectionFailedGrid.Visibility = Visibility.Collapsed;
                    LinkOpenEnable(true);

                    if (changeStyleNeeded)
                    {
                        TitleLockStatus = false;
                        _connectionFailedGridTitleLockStatus = false;
                    }
                });
            });
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _hwndSource = (HwndSource) PresentationSource.FromVisual(this);
            _hwndSource?.AddHook(WindowUtils.HwndSourceHook);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            _previousState = WindowState;

            base.OnStateChanged(e);

            if (_previousState != WindowState.Minimized)
            {
                DuringRestoreToMaximized = WindowState == WindowState.Maximized;
            }
        }

        private async void OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            await Dispatcher.Invoke(() =>
            {
                return e.NewState switch
                {
                    ConnectionStateType.Connected => ShowConnectionFailedGrid(false),
                    ConnectionStateType.Disconnected => ShowConnectionFailedGrid(true),
                    _ => Task.CompletedTask,
                };
            }).ConfigureAwait(true);
        }

        private void OnLanguageChanged(object sender, LanguageChangedEventArgs e)
        {

        }

        private async void OpenLink_Click(object sender, RoutedEventArgs e)
        {
            if (!btnLinkOpen.IsEnabled)
                return;

            var title = LocalizationUtils.GetLocalized("LinkOpeningTitle");
            var message = LocalizationUtils.GetLocalized("EnterURL");

            var link = await DialogManager.ShowSinglelineTextDialog(
                    title, message)
                .ConfigureAwait(true);

            if (string.IsNullOrWhiteSpace(link))
                return;

            var startInfo = new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception)
            {
                var exception = new Exception(
                    $"An error occurred when opening the link '{link}'");
                Events.OnError(new RErrorEventArgs(exception,
                    exception.Message));
            }
        }

        private async void slcLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPair = (KeyValuePair<string, LocalizationXamlModule>)slcLanguage.SelectedItem;

            await LocalizationManager.SwitchLanguage(selectedPair.Key)
                .ConfigureAwait(true);
        }

        private async void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string title = LocalizationUtils.GetLocalized("ChangingPasswordTitle");
            string enterName = LocalizationUtils.GetLocalized("EnterTitle");
            string oldPasswordName = LocalizationUtils.GetLocalized("OldPassword");
            string newPasswordName = LocalizationUtils.GetLocalized("NewPassword");

            string oldPassword = await DialogManager.ShowPasswordDialog(title,
                    $"{enterName} {oldPasswordName.ToLower()}",
                    false)
                .ConfigureAwait(true);

            if (oldPassword == null)
                return;

            string newPassword = await DialogManager.ShowPasswordDialog(title,
                    $"{enterName} {newPasswordName.ToLower()}",
                    true)
                .ConfigureAwait(true);

            if (newPassword == null)
                return;
            
            var changeSuccessfully = await DatabaseHelper.ChangePassword(
                    SettingsManager.PersistentSettings.CurrentUser.Login,
                    oldPassword, newPassword)
               .ConfigureAwait(true);

            if (!changeSuccessfully)
                return;
        }

        private async void btnSignInToAnotherAccount_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsManager.PersistentSettings.CurrentUser.IsTemporary())
            {
                var confirmResult = await DialogManager.ShowConfirmationDialog()
                    .ConfigureAwait(true);

                if (confirmResult != MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                {
                    return;
                }
            }

            SettingsManager.PersistentSettings.ResetCurrentUser();

            HideSettings();

            NavigationController.Instance.RequestPage<LoginPage>();
        }

        private async void btnSignOutAccount_Click(object sender, RoutedEventArgs e)
        {
            var confirmResult = await DialogManager.ShowConfirmationDialog()
                .ConfigureAwait(true);

            if (confirmResult != MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
            {
                return;
            }

            SettingsManager.PersistentSettings.RemoveUser(
                SettingsManager.PersistentSettings.CurrentUser.Login);

            NavigationController.Instance.RequestPage<LoginPage>();
        }

        private void Telegram_Click(object sender, RoutedEventArgs e)
        {
            const string link = "https://t.me/daerius_rs";

            LinkUtils.OpenLink(link);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            SettingsManager.AppSettings.WindowWidth = Width;
            SettingsManager.AppSettings.WindowHeight = Height;
            SettingsManager.AppSettings.WindowPositionX = Left + (Width / 2.0);
            SettingsManager.AppSettings.WindowPositionY = Top + (Height / 2.0);
            SettingsManager.AppSettings.WindowState = (int)WindowState;

            SettingsManager.AppSettings.Save();
        }
    }
}
