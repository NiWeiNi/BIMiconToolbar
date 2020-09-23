using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIMiconToolbar.ViewOnSheet
{
    [TransactionAttribute(TransactionMode.Manual)]
    class ViewOnSheet : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Window1 VOSwindow = new Window1(commandData);
            VOSwindow.ShowDialog();
            
            return Result.Succeeded;
        }
    }
}
