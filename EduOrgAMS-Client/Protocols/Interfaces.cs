using System;
using EduOrgAMS.Client.Protocols.Schemas;

namespace EduOrgAMS.Client.Protocols
{
    public interface IUserProtocol
    {
        string Name { get; }
        IUserProtocolSchema Schema { get; }

        bool Register();

        bool Exists();
    }
}
