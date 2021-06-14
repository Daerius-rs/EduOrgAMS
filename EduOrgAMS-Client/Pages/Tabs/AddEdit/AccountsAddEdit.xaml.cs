using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using EduOrgAMS.Client.Cryptography;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;
using EduOrgAMS.Client.Dialogs;
using EduOrgAMS.Client.Utils;
using Microsoft.EntityFrameworkCore;

namespace EduOrgAMS.Client.Pages.Tabs.AddEdit
{
    public partial class AccountsAddEdit : AddEditContent
    {
        public Account Item { get; }
        public List<User> Users { get; private set; }

        public AccountsAddEdit(Account item,
            AddEditModeType mode)
            : base(mode)
        {
            InitializeComponent();
            DataContext = this;

            Item = item;

            if (Mode == AddEditModeType.Add)
            {
                LoginProperty.IsEnabled = true;
            }
            else if (Mode == AddEditModeType.Edit)
            {
                LoginProperty.IsEnabled = false;
            }
        }

        protected override async void OnCreated(object sender, EventArgs e)
        {
            base.OnCreated(sender, e);

            await using var context = DatabaseManager
                .CreateContext();

            await context.Users.AsNoTracking().LoadAsync()
                .ConfigureAwait(true);

            Users = new List<User>(
                context.Users);

            UserIdProperty.GetBindingExpression(
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

        private async void EditPassword_Click(object sender, RoutedEventArgs e)
        {
            string title = LocalizationUtils.GetLocalized("EnterPasswordTitle");
            string enterName = LocalizationUtils.GetLocalized("EnterTitle");
            string newPasswordName = LocalizationUtils.GetLocalized("NewPassword");

            string newPassword = await DialogManager.ShowPasswordDialog(title,
                    $"{enterName} {newPasswordName.ToLower()}",
                    true)
                .ConfigureAwait(true);

            if (newPassword == null)
                return;

            var newPasswordHash = HashManager.GetPasswordHash(
                Item.Login, newPassword);

            PasswordProperty.Text = newPasswordHash;
            Item.Password = newPasswordHash;
        }
    }
}
