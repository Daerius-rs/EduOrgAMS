using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;
using EduOrgAMS.Client.Localization;
using EduOrgAMS.Client.Utils;
using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore;

namespace EduOrgAMS.Client.Pages.Tabs
{
    public partial class RolesTab : TabContent
    {
        private static Type ItemType { get; }
        private static ReadOnlyDictionary<string, PropertyInfo> ItemProperties { get; }

        private List<Role> Items { get; set; }

        static RolesTab()
        {
            ItemType = typeof(Role);
            ItemProperties = GetItemProperties();
        }

        public RolesTab()
        {
            InitializeComponent();
            DataContext = this;

            LocalizationManager.LanguageChanged += OnLanguageChanged;
        }

        ~RolesTab()
        {
            LocalizationManager.LanguageChanged -= OnLanguageChanged;
        }

        private static ReadOnlyDictionary<string, PropertyInfo> GetItemProperties()
        {
            var itemProperties = new Dictionary<string, PropertyInfo>(32);

            foreach (var property in ItemType.GetProperties(BindingFlags.Instance
                                                            | BindingFlags.Public))
            {
                if (property.IsDefined(typeof(NotMappedAttribute)))
                    continue;

                itemProperties.Add(
                    property.Name, property);
            }

            return new ReadOnlyDictionary<string, PropertyInfo>(
                itemProperties);
        }

        private string GetLocalizedHeader(string name)
        {
            var key = $"{GetType().Name}-Header-{name}";

            return LocalizationUtils.TryGetLocalized(
                key) ?? name;
        }

        private void UpdateHeaders()
        {
            for (var i = 0; i < TableGrid.Columns.Count; ++i)
            {
                var column = TableGrid.Columns[i] as DataGridBoundColumn;
                var binding = column?.Binding as Binding;

                if (binding == null)
                    continue;

                var propertyName = binding.Path.Path;

                column.Header = GetLocalizedHeader(propertyName);
            }
        }

        // ReSharper disable once EntityNameCapturedOnly.Local
        private async Task Update()
        {
            TableGrid.ItemsSource = null;

            TableGrid.Items.Clear();
            TableGrid.Columns.Clear();

            TableGrid.Columns.Add(new DataGridNumericUpDownColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(Role.Id))
                }
            });
            TableGrid.Columns.Add(new DataGridTextColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(Role.Name))
                }
            });
            TableGrid.Columns.Add(new DataGridTextColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(Role.Permissions))
                }
            });
            TableGrid.Columns.Add(new DataGridCheckBoxColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(Role.IsAdmin))
                }
            });

            UpdateHeaders();

            Items.Clear();

            await using var context = DatabaseManager
                .CreateContext();

            await context.Roles.LoadAsync()
                .ConfigureAwait(true);

            Items.AddRange(context.Roles);

            TableGrid.ItemsSource = Items;
        }

        protected override async void OnCreated(object sender, EventArgs e)
        {
            base.OnCreated(sender, e);

            Items = new List<Role>(128);

            await Update()
                .ConfigureAwait(true);
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

        private void OnLanguageChanged(object sender, LanguageChangedEventArgs e)
        {
            UpdateHeaders();
        }

        private void TableGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TableGrid.SelectedCells.Count < 1)
                return;

            var item = TableGrid.SelectedCells[0].Item as Role;

            if (item == null)
                return;

            var column = TableGrid.SelectedCells[0].Column as DataGridBoundColumn;

            if (column == null)
                return;

            var binding = column.Binding as Binding;
            
            if (binding == null)
                return;

            var propertyName = binding.Path.Path;

            //if (propertyName == nameof(Role.Permissions))
            //{
            //    MessageBox.Show($"Selected cell: {item.Id} - {item.Name} - {propertyName}");
            //}
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            await Update()
                .ConfigureAwait(true);
        }
    }
}
