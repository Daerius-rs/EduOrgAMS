using System;
using System.Windows;
using EduOrgAMS.Client.Database.Entities;

namespace EduOrgAMS.Client.Widgets
{
    public partial class UserRole : WidgetContent
    {
        public static readonly DependencyProperty StatusValueProperty =
            DependencyProperty.Register(nameof(RoleValue), typeof(Role), typeof(UserRole),
                new PropertyMetadata(Role.Default));

        public Role RoleValue
        {
            get
            {
                return (Role)GetValue(StatusValueProperty);
            }
            set
            {
                SetValue(StatusValueProperty, value);
            }
        }

        public UserRole()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
