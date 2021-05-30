using System;
using System.Linq;
using System.Threading.Tasks;
using EduOrgAMS.Client.Cryptography;
using EduOrgAMS.Client.Database.Entities;
using EduOrgAMS.Client.Dialogs;
using EduOrgAMS.Client.Generating;
using EduOrgAMS.Client.Utils;
using Microsoft.EntityFrameworkCore;

namespace EduOrgAMS.Client.Database
{
    public static class DatabaseHelper
    {
        public static async Task<(bool Success, string Token, User User)> Login(
            string login, string password, bool showErrors = true)
        {
            try
            {
                await using var context = DatabaseManager.CreateContext();

                var account = await context.Accounts
                    .Include(acc => acc.User)
                    .FirstOrDefaultAsync(acc =>
                        acc.Login == login)
                    .ConfigureAwait(true);

                if (account == null)
                {
                    if (showErrors)
                    {
                        var title = LocalizationUtils.GetLocalized("LoginErrorTitle");
                        var message = LocalizationUtils.GetLocalized("InvalidLoginOrPasswordErrorMessage");

                        await DialogManager.ShowMessageDialog(title, message)
                            .ConfigureAwait(true);
                    }

                    return (false, null, User.Default);
                }

                var loginSuccessfully = HashManager.VerifyPasswordHash(
                    login, password, account.Password);

                if (!loginSuccessfully)
                {
                    if (showErrors)
                    {
                        var title = LocalizationUtils.GetLocalized("LoginErrorTitle");
                        var message = LocalizationUtils.GetLocalized("InvalidLoginOrPasswordErrorMessage");

                        await DialogManager.ShowMessageDialog(title, message)
                            .ConfigureAwait(true);
                    }

                    return (false, null, User.Default);
                }

                if (TimeUtils.ToUnixTimeStamp(DateTime.UtcNow) > account.TokenExpirationDate)
                {
                    var newTokenExpirationDate = TimeUtils.ToUnixTimeStamp(
                        DateTime.UtcNow.AddMonths(6));
                    string newToken;

                    do
                    {
                        newToken = GeneratingManager.GenerateToken(
                            login, newTokenExpirationDate);
                    } while (context.Accounts.FirstOrDefault(acc => acc.Token == newToken) != null);

                    account.TokenExpirationDate = newTokenExpirationDate;
                    account.Token = newToken;

                    await DatabaseManager.SaveChangesAsync(
                            context)
                        .ConfigureAwait(true);
                }

                return (true, account.Token, account.User);
            }
            catch (Exception)
            {
                return (false, null, User.Default);
            }
        }
        public static async Task<(bool Success, string Token, User User)> Login(
            string token, bool showErrors = true)
        {
            try
            {
                await using var context = DatabaseManager.CreateContext();

                var account = await context.Accounts
                    .Include(acc => acc.User)
                    .FirstOrDefaultAsync(
                        acc => acc.Token == token)
                    .ConfigureAwait(true);

                if (account == null)
                {
                    if (showErrors)
                    {
                        var title = LocalizationUtils.GetLocalized("LoginErrorTitle");
                        var message = LocalizationUtils.GetLocalized("InvalidTokenErrorMessage");

                        await DialogManager.ShowMessageDialog(title, message)
                            .ConfigureAwait(true);
                    }

                    return (false, null, User.Default);
                }

                if (TimeUtils.ToUnixTimeStamp(DateTime.UtcNow) > account.TokenExpirationDate)
                {
                    if (showErrors)
                    {
                        var title = LocalizationUtils.GetLocalized("LoginErrorTitle");
                        var message = LocalizationUtils.GetLocalized("TokenExpiredErrorMessage");

                        await DialogManager.ShowMessageDialog(title, message)
                            .ConfigureAwait(true);
                    }

                    return (false, null, User.Default);
                }

                return (true, account.Token, account.User);
            }
            catch (Exception)
            {
                return (false, null, User.Default);
            }
        }

        public static async Task<(bool Success, string Token, User User)> Register(
            string login, string password, User user, bool showErrors = true)
        {
            try
            {
                await using var context = DatabaseManager.CreateContext();

                var registrationDate = TimeUtils.ToUnixTimeStamp(
                    DateTime.UtcNow);

                if (context.Accounts.FirstOrDefault(acc => acc.Login == login) != null)
                {
                    if (showErrors)
                    {
                        var title = LocalizationUtils.GetLocalized("RegisterErrorTitle");
                        var message = LocalizationUtils.GetLocalized("LoginAlreadyExistErrorMessage");

                        await DialogManager.ShowMessageDialog(title, message)
                            .ConfigureAwait(true);
                    }

                    return (false, null, User.Default);
                }

                if (user.Id != default)
                    user.Id = default;

                user.RegistrationDate = registrationDate;

                var userEntry = await context.Users.AddAsync(user)
                    .ConfigureAwait(true);

                var userChangesCount = await DatabaseManager.SaveChangesAsync(
                        context)
                    .ConfigureAwait(true);

                if (userChangesCount == 0)
                {
                    if (showErrors)
                    {
                        var title = LocalizationUtils.GetLocalized("RegisterErrorTitle");
                        var message = LocalizationUtils.GetLocalized("UserRegisterErrorMessage");

                        await DialogManager.ShowMessageDialog(title, message)
                            .ConfigureAwait(true);
                    }

                    return (false, null, User.Default);
                }

                var passwordHash = HashManager.GetPasswordHash(
                    login, password);
                var tokenExpirationDate = TimeUtils.ToUnixTimeStamp(
                    DateTime.UtcNow.AddMonths(6));
                string token;

                do
                {
                    token = GeneratingManager.GenerateToken(
                        login, tokenExpirationDate);
                } while (context.Accounts.FirstOrDefault(acc => acc.Token == token) != null);

                var account = new Account
                {
                    Login = login,
                    Password = passwordHash,
                    TokenExpirationDate = tokenExpirationDate,
                    Token = token,
                    UserId = userEntry.Entity.Id
                };

                await context.Accounts.AddAsync(account)
                    .ConfigureAwait(true);

                var accountChangesCount = await DatabaseManager.SaveChangesAsync(
                        context)
                    .ConfigureAwait(true);

                if (accountChangesCount == 0)
                {
                    if (showErrors)
                    {
                        var title = LocalizationUtils.GetLocalized("RegisterErrorTitle");
                        var message = LocalizationUtils.GetLocalized("UserRegisterErrorMessage");

                        await DialogManager.ShowMessageDialog(title, message)
                            .ConfigureAwait(true);
                    }

                    return (false, null, User.Default);
                }

                return (true, account.Token, account.User);
            }
            catch (Exception)
            {
                return (false, null, User.Default);
            }
        }

        public static async Task<bool> ChangePassword(string login,
            string oldPassword, string newPassword, bool showErrors = true)
        {
            try
            {
                await using var context = DatabaseManager.CreateContext();

                var account = await context.Accounts.FirstOrDefaultAsync(
                        acc => acc.Login == login)
                    .ConfigureAwait(true);

                if (account == null)
                {
                    if (showErrors)
                    {
                        string message = LocalizationUtils.GetLocalized("InvalidLoginOrPasswordErrorMessage");

                        await DialogManager.ShowErrorDialog(message)
                            .ConfigureAwait(true);
                    }

                    return false;
                }

                var loginSuccessfully = HashManager.VerifyPasswordHash(
                    login, oldPassword, account.Password);

                if (!loginSuccessfully)
                {
                    if (showErrors)
                    {
                        string message = LocalizationUtils.GetLocalized("InvalidLoginOrPasswordErrorMessage");

                        await DialogManager.ShowErrorDialog(message)
                            .ConfigureAwait(true);
                    }

                    return false;
                }

                var newPasswordHash = HashManager.GetPasswordHash(
                    login, newPassword);

                account.Password = newPasswordHash;

                var accountChangesCount = await DatabaseManager.SaveChangesAsync(
                        context)
                    .ConfigureAwait(true);

                if (accountChangesCount == 0)
                {
                    if (showErrors)
                    {
                        string message = LocalizationUtils.GetLocalized("ChangingPasswordErrorMessage");

                        await DialogManager.ShowErrorDialog(message)
                            .ConfigureAwait(true);
                    }

                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
