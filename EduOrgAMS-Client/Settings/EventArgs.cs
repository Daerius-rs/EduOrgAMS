using System;
using System.Collections.ObjectModel;
using EduOrgAMS.Client.Settings.Entities;

namespace EduOrgAMS.Client.Settings
{
    public class AvailableUsersChangedEventArgs : EventArgs
    {
        public ReadOnlyDictionary<string, User> OldAvailableUsers { get; }
        public ReadOnlyDictionary<string, User> NewAvailableUsers { get; }

        public AvailableUsersChangedEventArgs(
            ReadOnlyDictionary<string, User> oldAvailableUsers,
            ReadOnlyDictionary<string, User> newAvailableUsers)
        {
            OldAvailableUsers = oldAvailableUsers;
            NewAvailableUsers = newAvailableUsers;
        }
    }

    public class UserChangedEventArgs : EventArgs
    {
        public User OldUser { get; }
        public User NewUser { get; }

        public UserChangedEventArgs(User oldUser, User newUser)
        {
            OldUser = oldUser;
            NewUser = newUser;
        }
    }
}
