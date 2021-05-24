using System;
using System.Collections.ObjectModel;
using EduOrgAMS.Client.Localization.Entities;

namespace EduOrgAMS.Client.Localization
{
    public interface ILocalizable
    {
        ReadOnlyDictionary<string, LocalizationXamlModule> Locales { get; }
    }
}
