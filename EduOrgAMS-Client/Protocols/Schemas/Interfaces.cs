using System;

namespace EduOrgAMS.Client.Protocols.Schemas
{
    public interface IUserProtocolSchema
    {
        string Name { get; }

        bool ParseUri(string uriString);
    }
}
