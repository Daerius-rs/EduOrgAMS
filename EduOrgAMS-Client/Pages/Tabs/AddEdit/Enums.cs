using System;

namespace EduOrgAMS.Client.Pages.Tabs.AddEdit
{
    public enum AddEditStateType : byte
    {
        Unknown = 0,
        Loaded = 1,
        Unloaded = 2
    }

    public enum AddEditModeType : byte
    {
        Add = 1,
        Edit = 2
    }
}
