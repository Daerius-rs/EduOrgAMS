using System;
using EduOrgAMS.Client.Logging;
using EduOrgAMS.Client.Protocols.Schemas;
using Microsoft.Win32;
using Environment = RIS.Environment;

namespace EduOrgAMS.Client.Protocols
{
    public class EOAMSProtocol : IUserProtocol
    {
        public string Name { get; }
        public IUserProtocolSchema Schema { get; }

        private EOAMSProtocol()
        {
            Name = "EduOrgAMS App Protocol";
            Schema = new EOAMSSchema();
        }

        public bool Register()
        {
            return RegisterInternal();
        }
        private bool RegisterInternal()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name)
                    || string.IsNullOrWhiteSpace(Schema.Name))
                {
                    return false;
                }

                using (var schemaKey =
                    Registry.CurrentUser.CreateSubKey($@"SOFTWARE\Classes\{Schema.Name}"))
                {
                    if (schemaKey == null)
                        return false;

                    string filePath = Environment.ExecProcessFilePath;

                    schemaKey.SetValue(string.Empty,
                        "URL:" + Name);
                    schemaKey.SetValue("URL Protocol",
                        string.Empty);

                    using (var defaultIconKey =
                        schemaKey.CreateSubKey("DefaultIcon"))
                    {
                        if (defaultIconKey == null)
                            return false;

                        defaultIconKey.SetValue(string.Empty, $"\"{filePath}\"");
                    }

                    using (var commandKey =
                        schemaKey.CreateSubKey(@"shell\open\command"))
                    {
                        if (commandKey == null)
                            return false;

                        commandKey.SetValue(string.Empty,
                            $"\"{filePath}\" \"startupUri:%1\"");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogManager.Log.Error(ex, "Protocol register error");
                return false;
            }
        }

        public bool Exists()
        {
            return ExistsInternal();
        }
        private bool ExistsInternal()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name)
                    || string.IsNullOrWhiteSpace(Schema.Name))
                {
                    return false;
                }

                using (var schemaKey =
                    Registry.CurrentUser.OpenSubKey($@"SOFTWARE\Classes\{Schema.Name}"))
                {
                    if (schemaKey != null)
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                LogManager.Log.Error(ex, "Protocol check exists error");
                return false;
            }
        }
    }
}
