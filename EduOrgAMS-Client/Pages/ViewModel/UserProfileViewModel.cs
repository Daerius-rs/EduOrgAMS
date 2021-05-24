using System;
using EduOrgAMS.Client.Database.Entities;
using EduOrgAMS.Client.Settings;

namespace EduOrgAMS.Client.Pages.ViewModel
{
    public class UserProfileViewModel : PageViewModel
    {
        private User _currentUser = new User
        {
            Id = -1
        };
        public User CurrentUser
        {
            get
            {
                return CurrentUser;
            }
            set
            {
                CurrentUser = value;

                OnPropertyChanged(nameof(CurrentUser));
                OnPropertyChanged(nameof(EditAllowed));
            }
        }

        public bool EditAllowed
        {
            get
            {
                return CurrentUser.Id == SettingsManager.PersistentSettings.CurrentUser.Id;
            }
        }

        public UserProfileViewModel()
            : base(typeof(UserProfilePage))
        {

        }
    }
}
