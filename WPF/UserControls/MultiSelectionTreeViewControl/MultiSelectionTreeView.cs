using BIMicon.BIMiconToolbar.WPF.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.WPF.UserControls.MultiSelectionTreeViewControl
{
    public class MultiSelectionTreeView : ItemsControl
    {
        #region Constants and Fields

        public event EventHandler<PreviewSelectionChangedEventArgs> PreviewSelectionChanged;
        public event EventHandler SelectionChanged;
        public static readonly DependencyProperty LastSelectedItemProperty;

        public static DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems",
            typeof(IList),
            typeof(MultiSelectionTreeView),
            new FrameworkPropertyMetadata(
                null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsPropertyChanged));

        public static DependencyPropertyKey LastSelectedItemPropertyKey = DependencyProperty.RegisterReadOnly(
            "LastSelectedItem",
            typeof(object),
            typeof(MultiSelectionTreeView),
            new FrameworkPropertyMetadata(null));

        #endregion

        public object LastSelectedItem
        {
            get
            {
                return GetValue(LastSelectedItemProperty);
            }
            private set
            {
                SetValue(LastSelectedItemPropertyKey, value);
            }
        }

        private static void OnSelectedItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectionTreeView treeView = (MultiSelectionTreeView)d;
            if (e.OldValue != null)
            {
                INotifyCollectionChanged collection = e.OldValue as INotifyCollectionChanged;
                if (collection != null)
                {
                    collection.CollectionChanged -= treeView.OnSelectedItemsChanged;
                }
            }

            if (e.NewValue != null)
            {
                INotifyCollectionChanged collection = e.NewValue as INotifyCollectionChanged;
                if (collection != null)
                {
                    collection.CollectionChanged += treeView.OnSelectedItemsChanged;
                }
            }
        }

        internal IEnumerable<MultiSelectionTreeViewItem> GetTreeViewItemsFor(IEnumerable dataItems)
        {
            if (dataItems == null)
            {
                yield break;
            }

            foreach (var dataItem in dataItems)
            {
                foreach (var treeViewItem in RecursiveTreeViewItemEnumerable(this, true))
                {
                    if (treeViewItem.DataContext == dataItem)
                    {
                        yield return treeViewItem;
                        break;
                    }
                }
            }
        }

        internal IEnumerable GetAllDataItems()
        {
            foreach (var treeViewItem in RecursiveTreeViewItemEnumerable(this, true))
            {
                yield return treeViewItem.DataContext;
            }
        }

        internal static IEnumerable<MultiSelectionTreeViewItem> RecursiveTreeViewItemEnumerable(ItemsControl parent, bool includeInvisible)
        {
            return RecursiveTreeViewItemEnumerable(parent, includeInvisible, true);
        }

        internal static IEnumerable<MultiSelectionTreeViewItem> RecursiveTreeViewItemEnumerable(ItemsControl parent, bool includeInvisible, bool includeDisabled)
        {
            foreach (var item in parent.Items)
            {
                MultiSelectionTreeViewItem tve = (MultiSelectionTreeViewItem)parent.ItemContainerGenerator.ContainerFromItem(item);
                if (tve == null)
                {
                    // Container was not generated, therefore it is probably not visible, so we can ignore it.
                    continue;
                }
                if (!includeInvisible && !tve.IsVisible)
                {
                    continue;
                }
                if (!includeDisabled && !tve.IsEnabled)
                {
                    continue;
                }

                yield return tve;
                if (includeInvisible || tve.IsExpanded)
                {
                    foreach (var childItem in RecursiveTreeViewItemEnumerable(tve, includeInvisible, includeDisabled))
                    {
                        yield return childItem;
                    }
                }
            }
        }
        public IList SelectedItems
        {
            get
            {
                return (IList)GetValue(SelectedItemsProperty);
            }
            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }
        private void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
#if DEBUG
                    // Make sure we don't confuse MultiSelectTreeViewItems and their DataContexts while development
                    if (e.NewItems.OfType<MultiSelectionTreeViewItem>().Any())
                        throw new ArgumentException("A MultiSelectionTreeViewItem instance was added to the SelectedItems collection. Only their DataContext instances must be added to this list!");
#endif
                    object last = null;
                    foreach (var item in GetTreeViewItemsFor(e.NewItems))
                    {
                        if (!item.IsSelected)
                        {
                            item.IsSelected = true;
                        }

                        last = item.DataContext;
                    }

                    LastSelectedItem = last;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in GetTreeViewItemsFor(e.OldItems))
                    {
                        item.IsSelected = false;
                        if (item.DataContext == LastSelectedItem)
                        {
                            if (SelectedItems.Count > 0)
                            {
                                LastSelectedItem = SelectedItems[SelectedItems.Count - 1];
                            }
                            else
                            {
                                LastSelectedItem = null;
                            }
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var item in RecursiveTreeViewItemEnumerable(this, true))
                    {
                        if (item.IsSelected)
                        {
                            item.IsSelected = false;
                        }
                    }

                    LastSelectedItem = null;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            OnSelectionChanged();
        }

        protected void OnSelectionChanged()
        {
            var handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

    }
}
