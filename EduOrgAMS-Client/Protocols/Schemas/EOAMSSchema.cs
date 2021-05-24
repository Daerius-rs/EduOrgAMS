using System;
using System.Linq;
using System.Reflection;
using EduOrgAMS.Client.Database.Entities;
using EduOrgAMS.Client.Logging;
using EduOrgAMS.Client.Navigation;
using EduOrgAMS.Client.Pages;
using EduOrgAMS.Client.Pages.ViewModel;
using EduOrgAMS.Client.Settings;
using EduOrgAMS.Client.Utils;
using RIS.Reflection.Mapping;

namespace EduOrgAMS.Client.Protocols.Schemas
{
    public class EOAMSSchema : IUserProtocolSchema
    {
        private readonly MethodMap<EOAMSSchema> _schemaMap;

        public string Name { get; }

        public EOAMSSchema()
        {
            _schemaMap = new MethodMap<EOAMSSchema>(
                this,
                new[]
                {
                    typeof(string)
                },
                typeof(bool));
            Name = "eoams";
        }

        public bool ParseUri(string uriString)
        {
            try
            {
                if (string.IsNullOrEmpty(uriString)
                    || string.IsNullOrWhiteSpace(Name))
                {
                    return false;
                }

                if (!Uri.TryCreate(uriString, UriKind.Absolute, out var uri)
                    || uri.Scheme != Name)
                {
                    return false;
                }

                var requestComponents = uri.Segments
                    .Select(segment => segment.Trim(' ', '/'))
                    .Where(segment => !string.IsNullOrEmpty(segment))
                    .ToArray();

                LogManager.DebugLog.Info($"Request components - {string.Join(',', requestComponents)}");

                if (requestComponents.Length >= 2)
                {
                    string methodName = string.Join('/',
                        requestComponents.SkipLast(1));
                    string args = requestComponents[^1];

                    LogManager.DebugLog.Info($"Request method - Name={methodName},Args={args}");

                    return _schemaMap.Invoke<bool>(
                        methodName, args);
                }

                return false;
            }
            catch (Exception ex)
            {
                LogManager.Log.Error(ex, "Schema parse uri error");
                return false;
            }
        }



        [MappedMethod("user/id")]
        private static bool ShowUserById(string args)
        {
            try
            {
                if (string.IsNullOrEmpty(SettingsManager.PersistentSettings.CurrentUser?.Login))
                    return false;

                if (!int.TryParse(args, out var id))
                    return false;
                if (id < 0)
                    return false;

                MainWindow.Instance.Dispatcher.Invoke(() =>
                {
                    NavigationController.Instance.RequestPage<UserProfilePage>(new UserProfileViewModel
                    {
                        CurrentUser = new User
                        {
                            Id = id
                        }
                    });
                });

                return true;
            }
            catch (Exception ex)
            {
                var method = MethodBase.GetCurrentMethod();

                ProtocolSchemaUtils.LogMethodError(
                    ex, method, args);
                
                return false;
            }
        }
    }
}
