﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;
using EduOrgAMS.Client.Dialogs;
using EduOrgAMS.Client.Localization;
using EduOrgAMS.Client.Pages.Tabs.AddEdit;
using EduOrgAMS.Client.Utils;
using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore;

namespace EduOrgAMS.Client.Pages.Tabs
{
    public partial class GroupsTab : TabContent
    {
        private static Type ItemType { get; }
        private static ReadOnlyDictionary<string, PropertyInfo> ItemProperties { get; }

        private List<Group> Items { get; set; }

        static GroupsTab()
        {
            ItemType = typeof(Group);
            ItemProperties = GetItemProperties();
        }

        public GroupsTab()
        {
            InitializeComponent();
            DataContext = this;

            LocalizationManager.LanguageChanged += OnLanguageChanged;
        }

        ~GroupsTab()
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
                if (TableGrid.Columns[i] is DataGridBoundColumn)
                {
                    var column = TableGrid.Columns[i] as DataGridBoundColumn;
                    var binding = column?.Binding as Binding;

                    if (binding == null)
                        continue;

                    var propertyName = binding.Path.Path;

                    column.Header = GetLocalizedHeader(propertyName);
                }
                else if (TableGrid.Columns[i] is DataGridComboBoxColumn)
                {
                    var column = TableGrid.Columns[i] as DataGridComboBoxColumn;
                    var binding = column?.SelectedValueBinding as Binding;

                    if (binding == null)
                        continue;

                    var propertyName = binding.Path.Path;

                    column.Header = GetLocalizedHeader(propertyName);
                }
            }
        }

        private async Task Update()
        {
            TableGrid.ItemsSource = null;

            TableGrid.Items.Clear();
            TableGrid.Columns.Clear();

            await using var context = DatabaseManager
                .CreateContext();

            await context.Professions.LoadAsync()
                .ConfigureAwait(true);
            await context.Users.LoadAsync()
                .ConfigureAwait(true);

            TableGrid.Columns.Add(new DataGridNumericUpDownColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(Group.Id))
                }
            });
            TableGrid.Columns.Add(new DataGridNumericUpDownColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(Group.RecruitYear))
                }
            });
            TableGrid.Columns.Add(new DataGridNumericUpDownColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                Binding = new Binding
                {
                    Path = new PropertyPath(nameof(Group.BaseClassesCount))
                }
            });
            TableGrid.Columns.Add(new DataGridComboBoxColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                ItemsSource = new List<Profession>(
                    context.Professions),
                DisplayMemberPath = $"{nameof(Profession.Name)}",
                SelectedValuePath = $"{nameof(Profession.Id)}",
                SelectedValueBinding = new Binding
                {
                    Path = new PropertyPath(nameof(Group.ProfessionId))
                }
            });
            TableGrid.Columns.Add(new DataGridComboBoxColumn
            {
                Visibility = Visibility.Visible,
                IsReadOnly = true,
                ItemsSource = new List<User>(
                    context.Users),
                DisplayMemberPath = $"{nameof(User.FullName)}",
                SelectedValuePath = $"{nameof(User.Id)}",
                SelectedValueBinding = new Binding
                {
                    Path = new PropertyPath(nameof(Group.CuratorId))
                }
            });

            UpdateHeaders();

            Items.Clear();

            await context.Groups.LoadAsync()
                .ConfigureAwait(true);

            Items.AddRange(context.Groups);

            TableGrid.ItemsSource = Items;
        }

        private bool OverlayIsOpen()
        {
            return OverlayGrid.Visibility == Visibility.Visible;
        }

        private void ShowOverlay()
        {
            OverlayGrid.Visibility = Visibility.Visible;
        }

        private void HideOverlay()
        {
            OverlayGrid.Visibility = Visibility.Collapsed;
        }

        private void ClearOverlay()
        {
            OverlayLayout.Content = null;
        }

        private void RequestOverlay(UserControl control)
        {
            OverlayLayout.Content = control;

            ShowOverlay();
        }

        private GroupsAddEdit ShowAddEdit(Group item,
            AddEditModeType mode)
        {
            var control = new GroupsAddEdit(
                item, mode);

            control.SaveButtonClick += AddEdit_SaveButtonClick;
            control.CancelButtonClick += AddEdit_CancelButtonClick;

            RequestOverlay(control);

            return control;
        }

        protected override async void OnCreated(object sender, EventArgs e)
        {
            base.OnCreated(sender, e);

            Items = new List<Group>(128);

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                await Update()
                    .ConfigureAwait(true);
            }
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

        private void OverlayBackground_Click(object sender, RoutedEventArgs e)
        {
            HideOverlay();
            ClearOverlay();
        }

        private async void AddEdit_SaveButtonClick(object sender, RoutedEventArgs e)
        {
            HideOverlay();
            ClearOverlay();

            await Update()
                .ConfigureAwait(true);

            if (sender is AddEditContent control)
            {
                control.SaveButtonClick -= AddEdit_SaveButtonClick;
                control.CancelButtonClick -= AddEdit_CancelButtonClick;
            }
        }

        private void AddEdit_CancelButtonClick(object sender, RoutedEventArgs e)
        {
            HideOverlay();
            ClearOverlay();

            if (sender is AddEditContent control)
            {
                control.SaveButtonClick -= AddEdit_SaveButtonClick;
                control.CancelButtonClick -= AddEdit_CancelButtonClick;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var item = new Group();

            ShowAddEdit(item, AddEditModeType.Add);
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (TableGrid.SelectedItem == null && TableGrid.SelectedCells.Count == 0)
            {
                var message = LocalizationUtils
                    .GetLocalized("NoRowOrCellSelectedInTableErrorMessage");

                await DialogManager.ShowErrorDialog(
                        message)
                    .ConfigureAwait(true);

                return;
            }

            var item = TableGrid.SelectedItem as Group
                       ?? TableGrid.SelectedCells[0].Item as Group;

            if (item == null)
                return;

            ShowAddEdit(item, AddEditModeType.Edit);
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TableGrid.SelectedItem == null && TableGrid.SelectedCells.Count == 0)
            {
                var message = LocalizationUtils
                    .GetLocalized("NoRowOrCellSelectedInTableErrorMessage");

                await DialogManager.ShowErrorDialog(
                        message)
                    .ConfigureAwait(true);

                return;
            }

            var confirmResult = await DialogManager.ShowConfirmationDialog()
                .ConfigureAwait(true);

            if (confirmResult != MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
            {
                return;
            }

            var item = TableGrid.SelectedItem as Group
                       ?? TableGrid.SelectedCells[0].Item as Group;

            if (item == null)
                return;

            DatabaseManager.Remove(item);
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            await Update()
                .ConfigureAwait(true);
        }
    }
}