using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIMiconToolbar.Helpers
{
    class HelpersView
    {
        /// <summary>
        /// Function to create a sheet
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="titleBlockId"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static ViewSheet CreateSheet(Document doc, ElementId titleBlockId, string sheetName)
        {
            // Create sheet and set name
            ViewSheet sheet = ViewSheet.Create(doc, titleBlockId);
            sheet.Name = sheetName;

            return sheet;
        }


        public static void CreateViewport(Document doc, ElevationMarker marker, View floorPlan, int i, View viewTemplate,
                                        List<ElementId> annoCategories, ViewSheet sheet, ref List<Viewport> viewports)
        {
            View view = marker.CreateElevation(doc, floorPlan.Id, i);
            view.ViewTemplateId = viewTemplate.Id;

            // Hide annotation categories to reduce viewport outline to minimum size
            // This allows labels to align to the base
            view.HideCategoriesTemporary(annoCategories);

            // Regenerate document to pick view scale for title
            doc.Regenerate();

            // Create viewports
            Viewport viewP = Viewport.Create(doc, sheet.Id, view.Id, new XYZ());

            // Disable temporary hide
            view.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);

            // Store viewports
            viewports.Add(viewP);
        }
    }
}
