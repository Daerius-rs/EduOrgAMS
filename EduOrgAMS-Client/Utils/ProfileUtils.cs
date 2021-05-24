using System;
using System.Threading.Tasks;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Dialogs;
using EduOrgAMS.Client.Settings;

namespace EduOrgAMS.Client.Utils
{
    public static class ProfileUtils
    {
        public static event EventHandler<UserPhotoChangedEventArgs> AvatarChanged;

        public static void OnAvatarChanged(object sender, UserPhotoChangedEventArgs e)
        {
            AvatarChanged?.Invoke(sender, e);
        }

        public static async Task ChangeAvatar(string url,
            bool checkEmptyUrl = true)
        {
            try
            {
                if (checkEmptyUrl && string.IsNullOrWhiteSpace(url))
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

                var oldPhoto = user.AvatarUrl;
                user.AvatarUrl = url;

                var changesCount = await DatabaseManager.SaveChangesAsync(context)
                    .ConfigureAwait(true);

                if (changesCount == 0)
                {
                    var message = LocalizationUtils.GetLocalized("UpdatingDataErrorMessage");

                    await DialogManager.ShowErrorDialog(message)
                        .ConfigureAwait(true);
                    return;
                }

                OnAvatarChanged(null,
                    new UserPhotoChangedEventArgs(oldPhoto, user.AvatarUrl, user.Id));
            }
            catch (Exception ex)
            {
                await DialogManager.ShowErrorDialog(ex.Message)
                    .ConfigureAwait(true);
            }
        }

        public static Task RemoveAvatar()
        {
            return ChangeAvatar(string.Empty, false);
        }
    }
}
