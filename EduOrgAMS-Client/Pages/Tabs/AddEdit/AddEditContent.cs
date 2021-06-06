using System;
using System.Windows;
using System.Windows.Controls;

namespace EduOrgAMS.Client.Pages.Tabs.AddEdit
{
    public abstract class AddEditContent : UserControl
    {
        public static readonly RoutedEvent OnSaveButtonClicked =
            EventManager.RegisterRoutedEvent(nameof(SaveButtonClick), RoutingStrategy.Direct, typeof(EventHandler<RoutedEventArgs>), typeof(AddEditContent));
        public static readonly RoutedEvent OnCancelButtonClicked =
            EventManager.RegisterRoutedEvent(nameof(CancelButtonClick), RoutingStrategy.Direct, typeof(EventHandler<RoutedEventArgs>), typeof(AddEditContent));

        public event EventHandler<RoutedEventArgs> SaveButtonClick
        {
            add
            {
                AddHandler(OnSaveButtonClicked, value);
            }
            remove
            {
                RemoveHandler(OnSaveButtonClicked, value);
            }
        }
        public event EventHandler<RoutedEventArgs> CancelButtonClick
        {
            add
            {
                AddHandler(OnCancelButtonClicked, value);
            }
            remove
            {
                RemoveHandler(OnCancelButtonClicked, value);
            }
        }

        public bool IsCreated { get; private set; }
        public bool IsOnEnterActive { get; set; }
        public bool IsOnExitActive { get; set; }
        public AddEditStateType State { get; protected set; }
        public AddEditModeType Mode { get; }

        protected AddEditContent(AddEditModeType mode)
        {
            Initialized += OnCreated;
            Initialized += OnInitialized;
            Loaded += OnEnter;
            Unloaded += OnExit;
            DataContextChanged += OnDataContextChanged;

            IsCreated = false;
            IsOnEnterActive = true;
            IsOnExitActive = true;
            State = AddEditStateType.Unknown;
            Mode = mode;
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
            State = AddEditStateType.Loaded;

            if (!IsOnEnterActive)
            {
                e.Handled = true;
                return;
            }
        }

        protected virtual void OnExit(object sender, RoutedEventArgs e)
        {
            State = AddEditStateType.Unloaded;

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
