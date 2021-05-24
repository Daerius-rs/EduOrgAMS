using System;
using System.Windows;
using EduOrgAMS.Client.Pages.ViewModel;

namespace EduOrgAMS.Client.Pages
{
    public partial class MainPage : PageContent
    {
        public MainViewModel ViewModel
        {
            get
            {
                return DataContext as MainViewModel;
            }
        }

        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        protected override void OnEnter(object sender, RoutedEventArgs e)
        {
            base.OnEnter(sender, e);

            if (!IsOnEnterActive)
            {
                e.Handled = true;
                return;
            }
        }
    }
}
