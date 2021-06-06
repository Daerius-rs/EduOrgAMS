using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduOrgAMS.Client.Pages.Tabs.AddEdit
{
    public partial class SemestersAddEdit : AddEditContent
    {
        public Semester Item { get; }
        public List<Course> Courses { get; private set; }

        public SemestersAddEdit(Semester item,
            AddEditModeType mode)
            : base(mode)
        {
            InitializeComponent();
            DataContext = this;

            Item = item;
        }

        protected override async void OnCreated(object sender, EventArgs e)
        {
            base.OnCreated(sender, e);

            await using var context = DatabaseManager
                .CreateContext();

            await context.Courses.LoadAsync()
                .ConfigureAwait(true);

            Courses = new List<Course>(
                context.Courses);

            CourseIdProperty.GetBindingExpression(
                    ItemsControl.ItemsSourceProperty)?
                .UpdateTarget();
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
