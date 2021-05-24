using System;

namespace EduOrgAMS.Client.Utils
{
    public class UserPhotoChangedEventArgs : EventArgs
    {
        public string OldPhoto { get; }
        public string NewPhoto { get; }
        public int UserId { get; }

        public UserPhotoChangedEventArgs(string oldPhoto, string newPhoto, int userId)
        {
            OldPhoto = oldPhoto;
            NewPhoto = newPhoto;
            UserId = userId;
        }
    }
}
