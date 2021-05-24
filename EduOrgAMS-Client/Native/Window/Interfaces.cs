using System;

namespace EduOrgAMS.Client.Native.Window
{
    interface INativeRestorableWindow
    {
        public bool DuringRestoreToMaximized { get; }
    }
}
