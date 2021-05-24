using System;
using System.Threading.Tasks;
using System.Windows;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;
using EduOrgAMS.Client.Dialogs;
using EduOrgAMS.Client.Settings.Entities;
using User = EduOrgAMS.Client.Settings.Entities.User;

namespace EduOrgAMS.Client.Widgets
{
    public partial class StoredAccount : WidgetContent
    {
        public static readonly RoutedEvent OnAccountClicked =
            EventManager.RegisterRoutedEvent(nameof(AccountClick), RoutingStrategy.Direct, typeof(EventHandler<RoutedEventArgs>), typeof(StoredAccount));
        public static readonly RoutedEvent OnAccountDeleted =
            EventManager.RegisterRoutedEvent(nameof(AccountDelete), RoutingStrategy.Direct, typeof(EventHandler<RoutedEventArgs>), typeof(StoredAccount));
        public static readonly DependencyProperty AccountProperty =
            DependencyProperty.Register(nameof(Account), typeof(User), typeof(StoredAccount),
                new PropertyMetadata(new User(null, null, -1, UserStoreType.Unknown)));
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register(nameof(UserName), typeof(string), typeof(StoredAccount),
                new PropertyMetadata("Unknown"));
        public static readonly DependencyProperty UserAvatarSourceProperty =
            DependencyProperty.Register(nameof(UserAvatarSource), typeof(string), typeof(StoredAccount),
                new PropertyMetadata((string)null));
        public static readonly DependencyProperty UserStatusProperty =
            DependencyProperty.Register(nameof(UserRole), typeof(Role), typeof(StoredAccount),
                new PropertyMetadata(Role.Default));

        public event EventHandler<RoutedEventArgs> AccountClick
        {
            add
            {
                AddHandler(OnAccountClicked, value);
            }
            remove
            {
                RemoveHandler(OnAccountClicked, value);
            }
        }
        public event EventHandler<RoutedEventArgs> AccountDelete
        {
            add
            {
                AddHandler(OnAccountDeleted, value);
            }
            remove
            {
                RemoveHandler(OnAccountDeleted, value);
            }
        }

        public User Account
        {
            get
            {
                return (User)GetValue(AccountProperty);
            }
            set
            {
                SetValue(AccountProperty, value);
            }
        }
        public string UserName
        {
            get
            {
                return (string)GetValue(UserNameProperty);
            }
            set
            {
                SetValue(UserNameProperty, value);
            }
        }
        public string UserAvatarSource
        {
            get
            {
                return (string)GetValue(UserAvatarSourceProperty);
            }
            set
            {
                SetValue(UserAvatarSourceProperty, value);
            }
        }
        public Role UserRole
        {
            get
            {
                return (Role)GetValue(UserStatusProperty);
            }
            set
            {
                SetValue(UserStatusProperty, value);
            }
        }

        public StoredAccount()
        {
            InitializeComponent();
            DataContext = this;
        }

        public async Task UpdateAccount()
        {
            if (Account.Id == -1)
                return;

            var result = await DatabaseManager.FindAsync<Database.Entities.User>(
                    Account.Id)
                .ConfigureAwait(true);

            if (result == null)
                return;

            UserName = result.FullName;
            UserAvatarSource = result.AvatarUrl;
            UserRole = result.Role;
        }

        public async Task UpdateRole()
        {
            if (Account.Id == -1)
                return;

            var result = await DatabaseManager.FindAsync<Database.Entities.User>(
                    Account.Id)
                .ConfigureAwait(true);

            if (result == null)
                return;

            UserRole = result.Role;
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateAccount()
                .ConfigureAwait(true);
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OnAccountClicked));
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            btnDelete.IsEnabled = false;

            var confirmResult = await DialogManager.ShowConfirmationDialog()
                .ConfigureAwait(true);

            if (confirmResult != MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
            {
                btnDelete.IsEnabled = true;
                return;
            }

            Visibility = Visibility.Collapsed;

            RaiseEvent(new RoutedEventArgs(OnAccountDeleted));

            btnDelete.IsEnabled = true;
        }
    }
}
