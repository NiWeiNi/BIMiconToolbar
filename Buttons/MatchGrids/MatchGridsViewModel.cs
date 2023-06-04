using Autodesk.Revit.DB;
using BIMicon.BIMiconToolbar.Helpers;
using BIMicon.BIMiconToolbar.Models;
using BIMicon.BIMiconToolbar.Models.MVVM.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BIMicon.BIMiconToolbar.Buttons.MatchGrids
{
    internal class MatchGridsViewModel : ViewModelBase
    {
        private readonly Document _doc;
        private List<View> viewsInProject { get; set; }
        public ObservableCollection<BaseElement> Views { get; set; }
        public ObservableCollection<BaseElement> ViewsTemplate { get; set; }
        public RelayCommand OKExecute => new RelayCommand(execute => OKExecuteCommand());

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
            _doc = doc;
            LoadViewsInModel();
            LoadSelectableViews();
        }

        private void LoadViewsInModel()
        {
            FilteredElementCollector viewsCollector = new FilteredElementCollector(_doc).OfCategory(BuiltInCategory.OST_Views);
            viewsInProject = viewsCollector.Cast<View>()
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
            ViewsTemplate = new ObservableCollection<BaseElement>(viewsInProject
                .Select(v => new BaseElement() { Name = v.ViewType.ToString() + " - " + v.Name, Id = v.Id.IntegerValue })
                .OrderBy(v => v.Name));

            Views = new ObservableCollection<BaseElement>(viewsInProject
                .Select(v => new BaseElement() { Name = v.ViewType.ToString() + " - " + v.Name, Id = v.Id.IntegerValue })
                .OrderBy(v => v.Name));
        }

        private void FilterViewsByParallelView()
        {
            Views.Clear();

            View selectedView = _doc.GetElement(new ElementId(_selectedViewTemplate.Id)) as View;
            XYZ viewDirection = selectedView.ViewDirection;

            var FilteredViewsByComboBox = viewsInProject
                .Where(x => x.Id.IntegerValue != _selectedViewTemplate.Id)
                .Where(x => HelpersGeometry.AreVectorsParallel(viewDirection, x.ViewDirection))
                .ToList();

            foreach (View v in FilteredViewsByComboBox)
            {
                BaseElement bs = new BaseElement() { Name = v.ViewType.ToString() + " - " + v.Name, Id = v.Id.IntegerValue };
                Views.Add(bs);
            }
        }

        public void OKExecuteCommand()
        {
            MessageWindows.AlertMessage("Warning", "test");
        }
    }
}
