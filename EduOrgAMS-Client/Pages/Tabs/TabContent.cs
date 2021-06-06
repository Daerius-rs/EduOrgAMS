using System;
using System.Windows;
using System.Windows.Controls;

namespace EduOrgAMS.Client.Pages.Tabs
{
    public abstract class TabContent : UserControl
    {
        public bool IsCreated { get; private set; }
        public bool IsOnEnterActive { get; set; }
        public bool IsOnExitActive { get; set; }
        public TabStateType State { get; protected set; }

        protected TabContent()
        {
            Initialized += OnCreated;
            Initialized += OnInitialized;
            Loaded += OnEnter;
            Unloaded += OnExit;
            DataContextChanged += OnDataContextChanged;

            IsCreated = false;
            IsOnEnterActive = true;
            IsOnExitActive = true;
            State = TabStateType.Unknown;
        }

        protected virtual void OnCreated(object sender, EventArgs e)
        {
            if (IsCreated)
                return;

            Initialized -= OnCreated;

            IsCreated = true;
        }

        protected virtual void OnInitialized(object sender, EventArgs e)
        {

        }

        protected virtual void OnEnter(object sender, RoutedEventArgs e)
        {
            State = TabStateType.Loaded;

            if (!IsOnEnterActive)
            {
                e.Handled = true;
                return;
            }
        }

        protected virtual void OnExit(object sender, RoutedEventArgs e)
        {
            State = TabStateType.Unloaded;

            if (!IsOnExitActive)
            {
                e.Handled = true;
                return;
            }
        }

        protected virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
