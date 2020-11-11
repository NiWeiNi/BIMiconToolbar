using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMiconToolbar.InteriorElevations
{
    [TransactionAttribute(TransactionMode.Manual)]
    class InteriorElevations : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Variables to store user input
            List<int> selectedIntIds;
            Element titleBlock;
            View viewTemplate;
            ViewFamilyType viewFamilyType;

            // Prompt window to collect user input
            using (InteriorElevationsWindow customWindow = new InteriorElevationsWindow(commandData))
            {
                customWindow.ShowDialog();
                selectedIntIds = customWindow.IntegerIds;
                titleBlock = customWindow.SelectedComboItemTitleBlock.Tag as Element;
                viewTemplate = customWindow.SelectedComboItemViewTemplate.Tag as View;
                viewFamilyType = customWindow.SelectedComboItemViewType.Tag as ViewFamilyType;
            }

            // No required elements loaded
            if (titleBlock == null && viewTemplate == null && viewFamilyType == null)
            {
                TaskDialog.Show("Warning", "Please load a Elevation, a Title Block and create a View Template");
                return Result.Cancelled;
            }
            // No title block and elevation loaded
            else if (titleBlock == null && viewFamilyType == null)
            {
                TaskDialog.Show("Warning", "Please load a Elevation and a Title Block");
                return Result.Cancelled;
            }
            // No title block and view template
            else if (titleBlock == null && viewTemplate == null)
            {
                TaskDialog.Show("Warning", "Please load a Title Block and create a View Template");
                return Result.Cancelled;
            }
            // No elevation and view template
            else if (viewFamilyType == null && viewTemplate == null)
            {
                TaskDialog.Show("Warning", "Please load an Elevation and create a View Template");
                return Result.Cancelled;
            }
            // No elevation
            else if (viewFamilyType == null)
            {
                TaskDialog.Show("Warning", "Please load an Elevation");
                return Result.Cancelled;
            }
            // No title block
            else if (titleBlock == null)
            {
                TaskDialog.Show("Warning", "Please load a Title Block");
                return Result.Cancelled;
            }
            // No view template
            else if (titleBlock == null)
            {
                TaskDialog.Show("Warning", "Please create a view template");
                return Result.Cancelled;
            }
            // Room selected
            else if (selectedIntIds != null)
            {
                // Select first plan view
                FilteredElementCollector floorPlansCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views);
                View floorPlan = floorPlansCollector.Cast<View>().Where(v =>
                                   v.ViewType == ViewType.FloorPlan).Where(v => v.IsTemplate == false).FirstOrDefault();

                if (floorPlan == null)
                {
                    TaskDialog.Show("Warning", "Plese create a floor plan");
                    return Result.Cancelled;
                }

                // Collect rooms
                foreach (int id in selectedIntIds)
                {
                    Room room = doc.GetElement(new ElementId(id)) as Room;

                    // Retrieve boundaries
                    IList<IList<BoundarySegment>> boundaries = Helpers.Helpers.SpatialBoundaries(room);

                    if (boundaries != null)
                    {
                        // Transaction
                        Transaction t = new Transaction(doc, "Create Interior Elevations");
                        t.Start();

                        if (boundaries.Count == 1)
                        {
                            List<XYZ> points = Helpers.Helpers.BoundaPoints(boundaries);
                            XYZ centroid = Helpers.Helpers.Centroid(points);

                            // Create sheet
                            ViewSheet sheet = ViewSheet.Create(doc, titleBlock.Id);
                            sheet.Name = room.Number + "-" + "INTERIOR ELEVATIONS";

                            // Create elevation marker
                            ElevationMarker marker = ElevationMarker.CreateElevationMarker(doc, viewFamilyType.Id, centroid, viewTemplate.Scale);

                            // Get settings of current document
                            Settings documentSettings = doc.Settings;

                            // Retrieve annotation categories
                            Categories cats = documentSettings.Categories;

                            var annoCategories = new List<ElementId>();

                            foreach(Category cat in cats)
                            {
                                if (cat.CategoryType == CategoryType.Annotation)
                                {
                                    annoCategories.Add(cat.Id);
                                }
                            }

                            // Viewport dimensions
                            var vPOutlines = new List<Outline>();
                            var labelOutlines = new List<Outline>();

                            // Place views on sheet
                            for (int i = 0; i < 4; i++)
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

                                // Retrieve outlines
                                Outline vPOutline = viewP.GetBoxOutline();
                                Outline labelOutline = viewP.GetLabelOutline();
                                vPOutlines.Add(vPOutline);
                                labelOutlines.Add(labelOutline);

                                // Disable temporary hide
                                view.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                            }

                            foreach(Outline vpOut in vPOutlines)
                            {
                                // TODO: move views to final position
                            }

                            // Commit transaction
                            t.Commit();
                        }
                    }
                }
            }

            return Result.Succeeded;
        }
    }
}
