using System;
using System.Windows;
using EduOrgAMS.Client.Pages.ViewModel;

namespace EduOrgAMS.Client.Pages
{
    public partial class DatabasePage : PageContent
    {
        public DatabaseViewModel ViewModel
        {
            get
            {
                return DataContext as DatabaseViewModel;
            }
        }

        public DatabasePage()
        {
            InitializeComponent();
            DataContext = new DatabaseViewModel();
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
