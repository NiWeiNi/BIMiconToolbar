using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.WPF.UserControls.MultiSelectionTreeViewControl
{
    public class MultiSelectionTreeViewItem : HeaderedItemsControl
    {
        public static DependencyProperty DisplayNameProperty = DependencyProperty.Register(
            "DisplayName",
            typeof(string),
            typeof(MultiSelectionTreeViewItem));

        public static DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            "IsExpanded",
            typeof(bool),
            typeof(MultiSelectionTreeViewItem),
            new FrameworkPropertyMetadata(false));

        public static DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(MultiSelectionTreeViewItem),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsSelectedChanged)));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        private ItemsControl ParentItemsControl
        {
            get
            {
                return ItemsControlFromItemContainer(this);
            }
        }

        private MultiSelectionTreeView lastParentTreeView;

        internal MultiSelectionTreeView ParentTreeView
        {
            get
            {
                for (ItemsControl itemsControl = ParentItemsControl;
                    itemsControl != null;
                    itemsControl = ItemsControlFromItemContainer(itemsControl))
                {
                    MultiSelectionTreeView treeView = itemsControl as MultiSelectionTreeView;
                    if (treeView != null)
                    {
                        return lastParentTreeView = treeView;
                    }
                }
                return null;
            }
        }

        protected static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // The item has been selected through its IsSelected property. Update the SelectedItems
            // list accordingly (this is the authoritative collection). No PreviewSelectionChanged
            // event is fired - the item is already selected.
            MultiSelectionTreeViewItem item = d as MultiSelectionTreeViewItem;
            if (item != null)
            {
                if ((bool)e.NewValue)
                {
                    if (!item.ParentTreeView.SelectedItems.Contains(item.DataContext))
                    {
                        item.ParentTreeView.SelectedItems.Add(item.DataContext);
                    }
                }
                else
                {
                    item.ParentTreeView.SelectedItems.Remove(item.DataContext);
                }
            }
        }
    }
}
