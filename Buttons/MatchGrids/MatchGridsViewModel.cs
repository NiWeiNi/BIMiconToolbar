using Autodesk.Revit.DB;
using BIMicon.BIMiconToolbar.Helpers;
using BIMicon.BIMiconToolbar.Models;
using BIMicon.BIMiconToolbar.Models.Enums;
using BIMicon.BIMiconToolbar.Models.Forms;
using BIMicon.BIMiconToolbar.Models.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Message = BIMicon.BIMiconToolbar.Models.Message;

namespace BIMicon.BIMiconToolbar.Buttons.MatchGrids
{
    internal class MatchGridsViewModel : ViewModelBase
    {
        public readonly Document Doc;
        private List<View> ViewsInProject { get; set; }
        public ObservableCollection<BaseElement> Views { get; set; }
        public ObservableCollection<BaseElement> ViewsTemplate { get; set; }
        public List<BaseElement> ViewsFiltered { get; set; }
        public MatchGridsModel MatchGridsModel { get; set; }
        public RelayCommand OKExecute => new RelayCommand(execute => OKExecuteCommand());
        public ICollection<BaseElement> SelectedViews { get; set; }
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value != null)
                {
                    _searchText = value;
                    UpdateSearch();
                }
            }
        }
        private BaseElement _selectedViewTemplate;
        public BaseElement SelectedViewTemplate
        {
            get => _selectedViewTemplate;
            set
            {
                if (value != null)
                {
                    _selectedViewTemplate = value;
                    FilterViewsByParallelView();
                }
            }
        }
        public MatchGridsViewModel(Document doc) 
        { 
            Doc = doc;
            LoadViewsInModel();
            LoadSelectableViews();
        }

        private void LoadViewsInModel()
        {
            FilteredElementCollector viewsCollector = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_Views);
            ViewsInProject = viewsCollector.Cast<View>()
                            .Where(sh =>
                            sh.ViewType == ViewType.AreaPlan ||
                            sh.ViewType == ViewType.CeilingPlan ||
                            sh.ViewType == ViewType.Elevation ||
                            sh.ViewType == ViewType.EngineeringPlan ||
                            sh.ViewType == ViewType.FloorPlan ||
                            sh.ViewType == ViewType.Section ||
                            sh.ViewType == ViewType.EngineeringPlan)
                            .Where(view => !view.IsTemplate)
                            .ToList();
        }

        private void LoadSelectableViews()
        {
            ViewsTemplate = new ObservableCollection<BaseElement>(ViewsInProject
                .Select(v => new BaseElement() { Name = v.ViewType.ToString() + " - " + v.Name, Id = v.Id.IntegerValue })
                .OrderBy(v => v.Name));

            Views = new ObservableCollection<BaseElement>(ViewsInProject
                .Select(v => new BaseElement() { Name = v.ViewType.ToString() + " - " + v.Name, Id = v.Id.IntegerValue })
                .OrderBy(v => v.Name));
        }

        private void FilterViewsByParallelView()
        {
            Views.Clear();

            View selectedView = Doc.GetElement(new ElementId(_selectedViewTemplate.Id)) as View;
            XYZ viewDirection = selectedView.ViewDirection;

            List<View> FilteredViewsByComboBox = ViewsInProject
                .Where(x => x.Id.IntegerValue != _selectedViewTemplate.Id)
                .Where(x => HelpersGeometry.AreVectorsParallel(viewDirection, x.ViewDirection))
                .ToList();

            foreach (View v in FilteredViewsByComboBox)
            {
                BaseElement bs = new BaseElement() { Name = v.ViewType.ToString() + " - " + v.Name, Id = v.Id.IntegerValue };
                Views.Add(bs);
            }

            ViewsFiltered = Views.ToList();
        }

        private void UpdateSearch()
        {
            var FilteredElements = ViewsFiltered.Where(x => Parsing.Contains(x.Name, SearchText, StringComparison.InvariantCultureIgnoreCase));

            // Remove elements not in search term
            for (int i = Views.Count - 1; i >= 0; i--)
            {
                var item = Views[i];
                if (!FilteredElements.Contains(item))
                {
                    Views.Remove(item);
                }
            }

            // Bring back elements when input search text changes
            foreach (var item in FilteredElements)
            {
                if (!Views.Contains(item))
                {
                    Views.Add(item);
                }
            }
        }

        public void OKExecuteCommand()
        {
            try
            {
                MatchGridsModel.Execute();
            }
            catch
            {
                Message message = new Message(MessageType.Error, "Failed to execute", "Something went wrong");
                UserInfoWPF userMessage = new UserInfoWPF(message);
                userMessage.ShowDialog();
            }
            finally
            {
                OnRequestClose();
            }
        }
    }
}
