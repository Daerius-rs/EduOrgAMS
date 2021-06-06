using System;
using System.Windows;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;

namespace EduOrgAMS.Client.Pages.Tabs.AddEdit
{
    public partial class ProfessionsAddEdit : AddEditContent
    {
        public Profession Item { get; }

        public ProfessionsAddEdit(Profession item,
            AddEditModeType mode)
            : base(mode)
        {
            InitializeComponent();
            DataContext = this;

            Item = item;
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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                if (Item == null)
                    return;

                if (Mode == AddEditModeType.Add)
                {
                    await DatabaseManager.AddAsync(Item)
                        .ConfigureAwait(true);
                }
                else if (Mode == AddEditModeType.Edit)
                {
                    DatabaseManager.Update(Item);
                }

                RaiseEvent(new RoutedEventArgs(OnSaveButtonClicked));
            }
            finally
            {
                IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                RaiseEvent(new RoutedEventArgs(OnCancelButtonClicked));
            }
            finally
            {
                IsEnabled = true;
            }
        }
    }
}
