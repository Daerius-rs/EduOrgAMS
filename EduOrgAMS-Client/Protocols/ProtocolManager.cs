using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using EduOrgAMS.Client.Logging;

namespace EduOrgAMS.Client.Protocols
{
    public static class ProtocolManager
    {
        private static ReadOnlyDictionary<string, IUserProtocol> UserProtocols { get; }

        static ProtocolManager()
        {
            UserProtocols = GetUserProtocols();
        }

        private static ReadOnlyDictionary<string, IUserProtocol> GetUserProtocols()
        {
            try
            {
                var types = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => type.IsClass && typeof(IUserProtocol).IsAssignableFrom(type))
                    .ToArray();
                var protocols = new Dictionary<string, IUserProtocol>(types.Length);

                foreach (var type in types)
                {
                    var protocol =
                        Activator.CreateInstance(type, true) as IUserProtocol;

                    if (protocol?.Schema == null)
                        continue;

                    protocols.Add(protocol.Schema.Name, protocol);
                }

                return new ReadOnlyDictionary<string, IUserProtocol>(
                    protocols);
            }
            catch (Exception ex)
            {
                LogManager.Log.Error(ex, "User protocols get error");
                return new ReadOnlyDictionary<string, IUserProtocol>(
                    new Dictionary<string, IUserProtocol>(0));
            }
        }

        public static void RegisterAll()
        {
            foreach (var protocol in UserProtocols.Values)
            {
                protocol.Register();
            }
        }

        public static void ParseUri(string uriString)
        {
            if (string.IsNullOrEmpty(uriString))
                return;

            if (!Uri.TryCreate(uriString, UriKind.Absolute, out var uri)
                || !UserProtocols.ContainsKey(uri.Scheme))
            {
                return;
            }

            UserProtocols[uri.Scheme].Schema?.ParseUri(uriString);
        }
    }
}
