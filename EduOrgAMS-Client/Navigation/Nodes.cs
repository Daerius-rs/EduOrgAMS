using System;
using EduOrgAMS.Client.Pages;
using EduOrgAMS.Client.Pages.ViewModel;

namespace EduOrgAMS.Client.Navigation
{
    public sealed class NavigationHistoryNode
    {
        public PageContent Content { get; set; }
        public PageContent SubContent { get; set; }
        public PageViewModel DataContext { get; set; }
        public PageViewModel SubDataContext { get; set; }
        public PageContentType Type { get; set; }
        public bool SkipWhenGoBack { get; set; }
    }
}
