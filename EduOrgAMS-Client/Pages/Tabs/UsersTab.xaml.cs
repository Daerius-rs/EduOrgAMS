using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EduOrgAMS.Client.Converters;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;
using EduOrgAMS.Client.Dialogs;
using EduOrgAMS.Client.Localization;
using EduOrgAMS.Client.Navigation;
using EduOrgAMS.Client.Pages.Tabs.AddEdit;
using EduOrgAMS.Client.Pages.ViewModel;
using EduOrgAMS.Client.Utils;
using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore;

namespace EduOrgAMS.Client.Pages.Tabs
{
    public partial class UsersTab : TabContent
    {
        private static Type ItemType { get; }
        private static ReadOnlyDictionary<string, PropertyInfo> ItemProperties { get; }

        private List<User> Items { get; set; }

        static UsersTab()
        {
            ItemType = typeof(User);
            ItemProperties = GetItemProperties();
        }

        public UsersTab()
        {
            InitializeComponent();
            DataContext = this;

            LocalizationManager.LanguageChanged += OnLanguageChanged;
        }

        ~UsersTab()
        {
            LocalizationManager.LanguageChanged -= OnLanguageChanged;
        }

        private static ReadOnlyDictionary<string, PropertyInfo> GetItemProperties()
        {
            var itemProperties = new Dictionary<string, PropertyInfo>(32);

            foreach (var property in ItemType.GetProperties(BindingFlags.Instance
                                                            | BindingFlags.Public))
            {
                if (property.IsDefined(typeof(NotMappedAttribute)))
                    continue;

                itemProperties.Add(
                    property.Name, property);
            }

            return new ReadOnlyDictionary<string, PropertyInfo>(
                itemProperties);
        }

        private string GetLocalizedHeader(string name)
        {
            var key = $"{GetType().Name}-Header-{name}";

            return LocalizationUtils.TryGetLocalized(
                key) ?? name;
        }

        private void UpdateHeaders()
        {
            for (var i = 0; i < TableGrid.Columns.Count; ++i)
            {
                if (TableGrid.Columns[i] is DataGridBoundColumn)
                {
                    var column = TableGrid.Columns[i] as DataGridBoundColumn;
                    var binding = column?.Binding as Binding;

                    if (binding == null)
                        continue;

                    var propertyName = binding.Path.Path;

                    column.Header = GetLocalizedHeader(propertyName);
                }
                else if (TableGrid.Columns[i] is DataGridComboBoxColumn)
                {
                    var column = TableGrid.Columns[i] as DataGridComboBoxColumn;
                    var binding = column?.SelectedValueBinding as Binding;

                    if (binding == null)
                        continue;

                    var propertyName = binding.Path.Path;

                    column.Header = GetLocalizedHeader(propertyName);
                }
            }
        }

        private Task UpdateData()
        {
            return UpdateData(DatabaseManager
                .CreateContext());
        }
        private async Task UpdateData(DatabaseContext context)
        {
            TableGrid.ItemsSource = null;

            TableGrid.Items.Clear();
            Items.Clear();

            await context.Users.AsNoTracking().LoadAsync()
                .ConfigureAwait(true);

            Items.AddRange(context.Users);

            TableGrid.ItemsSource = Items;
        }

        private async Task Update()
        {
            TableGrid.Columns.Clear();

            await using var context = DatabaseManager
                .CreateContext();

            await context.Genders.AsNoTracking().LoadAsync()
                .ConfigureAwait(true);
            await context.Roles.AsNoTracking().LoadAsync()
                .ConfigureAwait(true);
            await context.Groups.AsNoTracking().LoadAsync()
                .ConfigureAwait(true);

            TableGrid.Columns.Add(new DataGridNumericUpDownColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(User.Id))
                }
            });
            TableGrid.Columns.Add(new DataGridTextColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(User.LastName))
                }
            });
            TableGrid.Columns.Add(new DataGridTextColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(User.FirstName))
                }
            });
            TableGrid.Columns.Add(new DataGridTextColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(User.Patronymic))
                }
            });
            TableGrid.Columns.Add(new DataGridComboBoxColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                ItemsSource = new List<Gender>(
                    context.Genders),
                DisplayMemberPath = $"{nameof(Gender.Name)}",
                SelectedValuePath = $"{nameof(Gender.Id)}",
                SelectedValueBinding = new Binding
                {
                    Path = new PropertyPath(nameof(User.GenderId))
                }
            });
            TableGrid.Columns.Add(new DataGridTextColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(User.BirthDate)),
                    Converter = new UnixTimeToStringConverter()
                }
            });
            TableGrid.Columns.Add(new DataGridTextColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(User.PhoneNumber))
                }
            });
            TableGrid.Columns.Add(new DataGridTextColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(User.Email))
                }
            });
            TableGrid.Columns.Add(new DataGridTextColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(User.Address))
                }
            });
            TableGrid.Columns.Add(new DataGridTextColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(User.AvatarUrl))
                }
            });
            TableGrid.Columns.Add(new DataGridComboBoxColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                ItemsSource = new List<Role>(
                    context.Roles),
                DisplayMemberPath = $"{nameof(Role.Name)}",
                SelectedValuePath = $"{nameof(Role.Id)}",
                SelectedValueBinding = new Binding
                {
                    Path = new PropertyPath(nameof(User.RoleId))
                }
            });
            TableGrid.Columns.Add(new DataGridTextColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(User.RegistrationDate)),
                    Converter = new UnixTimeToStringConverter()
                }
            });
            TableGrid.Columns.Add(new DataGridCheckBoxColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(User.Dismissed))
                }
            });
            TableGrid.Columns.Add(new DataGridComboBoxColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                ItemsSource = new List<Group>(
                    context.Groups),
                DisplayMemberPath = $"{nameof(Group.Name)}",
                SelectedValuePath = $"{nameof(Group.Id)}",
                SelectedValueBinding = new Binding
                {
                    Path = new PropertyPath(nameof(User.GroupId))
                }
            });

            UpdateHeaders();

            await UpdateData(context)
                .ConfigureAwait(true);
        }

        private bool OverlayIsOpen()
        {
            return OverlayGrid.Visibility == Visibility.Visible;
        }

        private void ShowOverlay()
        {
            OverlayGrid.Visibility = Visibility.Visible;
        }

        private void HideOverlay()
        {
            OverlayGrid.Visibility = Visibility.Collapsed;
        }

        private void ClearOverlay()
        {
            OverlayLayout.Content = null;
        }

        private void RequestOverlay(UserControl control)
        {
            OverlayLayout.Content = control;

            ShowOverlay();
        }

        private UsersAddEdit ShowAddEdit(User item,
            AddEditModeType mode)
        {
            var control = new UsersAddEdit(
                item, mode);

            control.SaveButtonClick += AddEdit_SaveButtonClick;
            control.CancelButtonClick += AddEdit_CancelButtonClick;

            RequestOverlay(control);

            return control;
        }

        protected override async void OnCreated(object sender, EventArgs e)
        {
            base.OnCreated(sender, e);

            Items = new List<User>(128);

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                await Update()
                    .ConfigureAwait(true);
            }
        }

        protected override void OnEnter(object sender, RoutedEventArgs e)
        {
            base.OnEnter(sender, e);

            if (!IsOnEnterActive)
            {
                e.Handled = true;
                return;
            }
        }

        private void OnLanguageChanged(object sender, LanguageChangedEventArgs e)
        {
            UpdateHeaders();
        }

        private void OverlayBackground_Click(object sender, RoutedEventArgs e)
        {
            HideOverlay();
            ClearOverlay();
        }

        private async void AddEdit_SaveButtonClick(object sender, RoutedEventArgs e)
        {
            HideOverlay();
            ClearOverlay();

            await UpdateData()
                .ConfigureAwait(true);

            if (sender is AddEditContent control)
            {
                control.SaveButtonClick -= AddEdit_SaveButtonClick;
                control.CancelButtonClick -= AddEdit_CancelButtonClick;
            }
        }

        private void AddEdit_CancelButtonClick(object sender, RoutedEventArgs e)
        {
            HideOverlay();
            ClearOverlay();

            if (sender is AddEditContent control)
            {
                control.SaveButtonClick -= AddEdit_SaveButtonClick;
                control.CancelButtonClick -= AddEdit_CancelButtonClick;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var item = new User();

            ShowAddEdit(item, AddEditModeType.Add);
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (TableGrid.SelectedItem == null && TableGrid.SelectedCells.Count == 0)
            {
                var message = LocalizationUtils
                    .GetLocalized("NoRowOrCellSelectedInTableErrorMessage");

                await DialogManager.ShowErrorDialog(
                        message)
                    .ConfigureAwait(true);

                return;
            }

            var item = TableGrid.SelectedItem as User
                       ?? TableGrid.SelectedCells[0].Item as User;

            if (item == null)
                return;

            ShowAddEdit(item, AddEditModeType.Edit);
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TableGrid.SelectedItem == null && TableGrid.SelectedCells.Count == 0)
            {
                var message = LocalizationUtils
                    .GetLocalized("NoRowOrCellSelectedInTableErrorMessage");

                await DialogManager.ShowErrorDialog(
                        message)
                    .ConfigureAwait(true);

                return;
            }

            var confirmResult = await DialogManager.ShowConfirmationDialog()
                .ConfigureAwait(true);

            if (confirmResult != MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
            {
                return;
            }

            var item = TableGrid.SelectedItem as User
                       ?? TableGrid.SelectedCells[0].Item as User;

            if (item == null)
                return;

            DatabaseManager.Remove(item);
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            await UpdateData()
                .ConfigureAwait(true);
        }

        private async void OpenUserProfile_Click(object sender, RoutedEventArgs e)
        {
            if (TableGrid.SelectedItem == null && TableGrid.SelectedCells.Count == 0)
            {
                var message = LocalizationUtils
                    .GetLocalized("NoRowOrCellSelectedInTableErrorMessage");

                await DialogManager.ShowErrorDialog(
                        message)
                    .ConfigureAwait(true);

                return;
            }

            var item = TableGrid.SelectedItem as User
                       ?? TableGrid.SelectedCells[0].Item as User;

            if (item == null)
                return;

            NavigationController.Instance.RequestPage<UserProfilePage>(new UserProfileViewModel
            {
                CurrentUser =
                {
                    Id = item.Id
                }
            });
        }
    }
}
