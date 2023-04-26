using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BIMicon.BIMiconToolbar.Models
{
    /// <summary>
    /// Model adapted from https://github.com/benperk/TheBestCSharpProgrammerInTheWorld/blob/master/csharpguitar/TreeView/TreeViewModel.cs
    /// Thank you best C# programmer in the world
    /// </summary>
    public class TreeView : INotifyPropertyChanged
    {
        #region Properties

        public BaseElement BaseElement { get; set; }
        public string Name { get; private set; }
        public List<TreeView> Children { get; private set; }
        public bool IsInitiallySelected { get; private set; }
        bool? _isChecked = false;
        TreeView _parent;
        public TreeView RootTreeView { get; set; }

        #endregion

        public TreeView(string name)
        {
            Name = name;
            Children = new List<TreeView>();
        }
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked) return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue) Children.ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null) _parent.VerifyCheckedState();

            NotifyPropertyChanged("IsChecked");
        }

        void VerifyCheckedState()
        {
            bool? state = null;

            for (int i = 0; i < Children.Count; ++i)
            {
                bool? current = Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }

            SetIsChecked(state, false, true);
        }

        public void Initialize()
        {
            foreach (TreeView child in Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        public static void AddItemsToTreeView(Dictionary<string, List<Element>> dictionaryElements, TreeView rootTreeView)
        {
            foreach (var item in dictionaryElements)
            {
                List<Element> listElements = item.Value;
                string name = item.Key;

                TreeView parentTreeView;
                if (dictionaryElements.Count > 1)
                {
                    BaseElement parentBaseElement = new BaseElement
                    {
                        Id = 0,
                        Name = name
                    };

                    parentTreeView = new TreeView(parentBaseElement.Name)
                    {
                        BaseElement = parentBaseElement
                    };

                    rootTreeView.Children.Add(parentTreeView);
                }
                else
                {
                    parentTreeView = rootTreeView;
                }

                List<Element> orderedList;
                if (listElements.FirstOrDefault() is SpatialElement)
                    orderedList = listElements.OrderBy(x => x.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString()).ToList();
                else
                    orderedList = listElements.OrderBy(x => x.Name).ToList();

                foreach (Element element in orderedList)
                {
                    string nameDisplay;
                    if (element is SpatialElement)
                        nameDisplay = element.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString() + " - " + element.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
                    else
                        nameDisplay = element.Name;

                    BaseElement childBaseElement = new BaseElement
                    {
                        Id = element.Id.IntegerValue,
                        Name = nameDisplay
                    };

                    TreeView childTreeView = new TreeView(childBaseElement.Name)
                    {
                        BaseElement = childBaseElement
                    };

                    parentTreeView.Children.Add(childTreeView);
                }
            }
        }

        public static List<TreeView> SetTree(Dictionary<string, List<Element>> dictionaryElements)
        {
            List<TreeView> treeViewList = new List<TreeView>();

            TreeView treeView;
            if (dictionaryElements.Count > 1)
                treeView = new TreeView("Select all");
            else
                treeView = new TreeView("Select all " + dictionaryElements.FirstOrDefault().Key);

            treeViewList.Add(treeView);

            AddItemsToTreeView(dictionaryElements, treeView);

            treeView.Initialize();

            return treeViewList;
        }

        //public static void GetTree(TreeView treeView, out List<BaseElement> selected)
        //{
        //    selected = new List<BaseElement>();

        //    // Retrieve selected items
        //    foreach (TreeView childrenTreeView in treeView.Children)
        //    {
        //        if (childrenTreeView.Children.Count == 0 && childrenTreeView.IsChecked == true)
        //            selected.Add(childrenTreeView.BaseElement);
        //        else if (childrenTreeView.Children.Count > 0)
        //            GetTree(childrenTreeView, out selected);
        //    }

        //    //***********************************************************
        //    //From your window capture selected your treeview control like:   TreeViewModel root = (TreeViewModel)TreeViewControl.Items[0];
        //    //                                                                List<string> selected = new List<string>(TreeViewModel.GetTree());
        //    //***********************************************************
        //}

        #region INotifyPropertyChanged Members

        void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
