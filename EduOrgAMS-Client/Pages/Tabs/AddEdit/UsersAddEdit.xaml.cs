using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;
using EduOrgAMS.Client.Dialogs;
using EduOrgAMS.Client.Utils;
using Microsoft.EntityFrameworkCore;
using Group = EduOrgAMS.Client.Database.Entities.Group;

namespace EduOrgAMS.Client.Pages.Tabs.AddEdit
{
    public partial class UsersAddEdit : AddEditContent
    {
        public User Item { get; }
        public List<Gender> Genders { get; private set; }
        public List<Role> Roles { get; private set; }
        public List<Group> Groups { get; private set; }

        public UsersAddEdit(User item,
            AddEditModeType mode)
            : base(mode)
        {
            InitializeComponent();
            DataContext = this;

            Item = item;
        }

        protected override async void OnCreated(object sender, EventArgs e)
        {
            base.OnCreated(sender, e);

            await using var context = DatabaseManager
                .CreateContext();

            await context.Genders.AsNoTracking().LoadAsync()
                .ConfigureAwait(true);
            await context.Roles.AsNoTracking().LoadAsync()
                .ConfigureAwait(true);
            await context.Groups.AsNoTracking().LoadAsync()
                .ConfigureAwait(true);

            Genders = new List<Gender>(
                context.Genders);
            Roles = new List<Role>(
                context.Roles);
            Groups = new List<Group>(
                context.Groups);

            GenderIdProperty.GetBindingExpression(
                    ItemsControl.ItemsSourceProperty)?
                .UpdateTarget();
            RoleIdProperty.GetBindingExpression(
                    ItemsControl.ItemsSourceProperty)?
                .UpdateTarget();
            GroupIdProperty.GetBindingExpression(
                    ItemsControl.ItemsSourceProperty)?
                .UpdateTarget();
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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                if (Item == null)
                    return;

                if (Item.BirthDate == 0)
                {
                    var message = LocalizationUtils
                        .GetLocalized("InvalidFieldValueErrorMessage");
                    var header = LocalizationUtils
                        .GetLocalized("UsersTab-Header-BirthDate");

                    await DialogManager.ShowErrorDialog($"{message} - '{header}'")
                        .ConfigureAwait(true);

                    return;
                }
                if (Item.RegistrationDate == 0)
                {
                    var message = LocalizationUtils
                        .GetLocalized("InvalidFieldValueErrorMessage");
                    var header = LocalizationUtils
                        .GetLocalized("UsersTab-Header-RegistrationDate");

                    await DialogManager.ShowErrorDialog($"{message} - '{header}'")
                        .ConfigureAwait(true);

                    return;
                }
                if (!string.IsNullOrEmpty(Item.PhoneNumber)
                    && !ValidateUtils.IsValidPatternString(Item.PhoneNumber,
                        @"^(?<phone_number>(?<country_code>\+7)(?:[\s]{1})(?:(?:(?:[\(]{1})(?=\d{3}[\)]{1}))?(?<area_code>(?:\d{3}))(?:(?<=[\(]{1}\d{3})(?:[\)]{1}))?)(?:[\s]{1})(?<prefix>\d{3})(?:[-\s]{1})(?<suffix>(?<suffix_part_1>\d{2})(?:[-\s]{1})(?<suffix_part_2>\d{2}))(?!\d))$",
                        RegexOptions.IgnoreCase))
                {
                    var message = LocalizationUtils
                        .GetLocalized("InvalidFieldValueErrorMessage");
                    var header = LocalizationUtils
                        .GetLocalized("UsersTab-Header-PhoneNumber");

                    await DialogManager.ShowErrorDialog($"{message} - '{header}'")
                        .ConfigureAwait(true);

                    return;
                }
                if (!string.IsNullOrEmpty(Item.Email)
                    && !ValidateUtils.IsValidPatternString(Item.Email,
                        @"^(?<email>[a-z0-9!#$%&'*+\=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9]))$",
                        RegexOptions.IgnoreCase))
                {
                    var message = LocalizationUtils
                        .GetLocalized("InvalidFieldValueErrorMessage");
                    var header = LocalizationUtils
                        .GetLocalized("UsersTab-Header-Email");

                    await DialogManager.ShowErrorDialog($"{message} - '{header}'")
                        .ConfigureAwait(true);

                    return;
                }

                if (Mode == AddEditModeType.Add)
                {
                    await DatabaseManager.AddAsync(Item)
                        .ConfigureAwait(true);
                }
                else if (Mode == AddEditModeType.Edit)
                {
                    DatabaseManager.Update(Item);
                }

                RaiseEvent(new RoutedEventArgs(OnSaveButtonClicked));
            }
            finally
            {
                IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                RaiseEvent(new RoutedEventArgs(OnCancelButtonClicked));
            }
            finally
            {
                IsEnabled = true;
            }
        }
    }
}
