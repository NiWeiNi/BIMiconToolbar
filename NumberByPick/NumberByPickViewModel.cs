using Autodesk.Revit.DB;
using System.Collections.ObjectModel;

namespace BIMiconToolbar.NumberByPick
{
    public class NumberByPickViewModel
    {
        public NumberByPickViewModel()
        {
            Initiliaze();
        }
        public string PrefixVM
        {
            get;
            set;
        }
        public double StartNumberVM
        {
            get;
            set;
        }

        public ObservableCollection<string> categories = new ObservableCollection<string>();

        public void Initiliaze()
        {
            NumberByPickModel model = new NumberByPickModel();

            model.Prefix = "Init value";
            model.StartNumber = 1;


            ObservableCollection<string> students = new ObservableCollection<string>();
            students.Add("Wall");
            students.Add("Door");
            categories = students;
        }
    }
}
