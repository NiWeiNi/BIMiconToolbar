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
            using (ViewOnSheetForm form = new ViewOnSheetForm())
            {
                form.ShowDialog();
            }

            return Result.Succeeded;
        }
    }
}
