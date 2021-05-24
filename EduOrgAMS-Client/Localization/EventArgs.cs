using System;
using EduOrgAMS.Client.Localization.Entities;

namespace EduOrgAMS.Client.Localization
{
    public class LanguageChangedEventArgs : EventArgs
    {
        public LocalizationXamlModule OldLanguage { get; }
        public LocalizationXamlModule NewLanguage { get; }

        public LanguageChangedEventArgs(
            LocalizationXamlModule oldLanguage,
            LocalizationXamlModule newLanguage)
        {
            OldLanguage = oldLanguage;
            NewLanguage = newLanguage;
        }
    }
}
