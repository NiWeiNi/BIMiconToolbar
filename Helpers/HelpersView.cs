using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace BIMicon.BIMiconToolbar.Helpers
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

        /// <summary>
        /// Function to create viewport
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="marker"></param>
        /// <param name="floorPlan"></param>
        /// <param name="i"></param>
        /// <param name="viewTemplate"></param>
        /// <param name="annoCategories"></param>
        /// <param name="sheet"></param>
        /// <param name="viewports"></param>
        public static void CreateViewport(Document doc, ViewSheet sheet, ref List<Viewport> viewports, View view)
        {
            // Create viewports
            Viewport viewP = Viewport.Create(doc, sheet.Id, view.Id, new XYZ());

            // Disable temporary hide
            view.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);

            // Store viewports
            viewports.Add(viewP);
        }

        /// <summary>
        /// Function to create view elevation
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="marker"></param>
        /// <param name="floorPlan"></param>
        /// <param name="i"></param>
        /// <param name="viewTemplate"></param>
        /// <param name="annoCategories"></param>
        /// <returns></returns>
        public static View CreateViewElevation(Document doc, ElevationMarker marker, View floorPlan, int i, 
                                            View viewTemplate, List<ElementId> annoCategories)
        {
            // Create view elevation
            View view = marker.CreateElevation(doc, floorPlan.Id, i);
            view.ViewTemplateId = viewTemplate.Id;

            // Hide annotation categories to reduce viewport outline to minimum size
            // This allows labels to align to the base
            view.HideCategoriesTemporary(annoCategories);

            // Regenerate document to pick view scale for title
            doc.Regenerate();

            return view;
        }

        /// <summary>
        /// Function to retrieve viewport dimensions
        /// </summary>
        /// <param name="viewports"></param>
        /// <returns></returns>
        public static Dictionary<Viewport, double[]> ViewportDimensions(List<Viewport> viewports)
        {
            // Dictionary to store viewport dimensions
            var viewportDims = new Dictionary<Viewport, double[]>();

            foreach (Viewport vp in viewports)
            {
                Outline vpOut = vp.GetBoxOutline();

                // Viewport dimensions
                XYZ maxPoint = vpOut.MaximumPoint;
                XYZ minPoint = vpOut.MinimumPoint;

                double vPxMax = maxPoint.X;
                double vPxMin = minPoint.X;

                double vPyMax = maxPoint.Y;
                double vPyMin = minPoint.Y;

                double vPxDist = vPxMax - vPxMin;
                double vPyDist = vPyMax - vPyMin;

                // Store results
                double[] dims = { vPxDist, vPyDist };
                viewportDims.Add(vp, dims);
            }

            return viewportDims;
        }

        /// <summary>
        /// Function to retrieve viewport label dimensions
        /// </summary>
        /// <param name="viewports"></param>
        /// <returns></returns>
        public static Dictionary<Viewport, double[]> LabelDimensions(List<Viewport> viewports)
        {
            // Dictionary to store viewport dimensions
            var labelDims = new Dictionary<Viewport, double[]>();

            foreach (Viewport vp in viewports)
            {
                Outline labelOut = vp.GetLabelOutline();

                // Label dimensions
                XYZ labelMaxPoint = labelOut.MaximumPoint;
                XYZ labelMinPoint = labelOut.MinimumPoint;

                double labelxMax = labelMaxPoint.X;
                double labelxMin = labelMinPoint.X;

                double labelyMax = labelMaxPoint.Y;
                double labelyMin = labelMinPoint.Y;

                double labelxDist = labelxMax - labelxMin;
                double labelyDist = labelyMax - labelyMin;

                // Store results
                double[] dims = { labelxDist, labelyDist };
                labelDims.Add(vp, dims);
            }

            return labelDims;
        }

        /// <summary>
        /// Function to return final coordinates of viewports
        /// </summary>
        /// <param name="viewportDims"></param>
        /// <param name="sheetWidth"></param>
        /// <param name="sheetHeight"></param>
        /// <returns></returns>
        public static List<XYZ> ViewportRowsColumns(Dictionary<Viewport, double[]> viewportDims, double sheetWidth, double sheetHeight)
        {
            // Lists to store single row of viewports and all rows on sheet
            List<Viewport> viewportSingleRow = new List<Viewport>();
            List<List<Viewport>> viewportRows = new List<List<Viewport>>();

            // Store viewport row dimension and all rows dimensions
            List<double> width = new List<double>();
            List<List<double>> viewportWidths = new List<List<double>>();

            // Maximum dimension
            double maxViewportHeight = 0;
            List<double> maxHeights = new List<double>();

            // Spacing between viewports
            double X = GeneralHelpers.MillimetersToFeet(30);
            double Y = GeneralHelpers.MillimetersToFeet(30);

            // Count number of elements in dict
            int vpCount = viewportDims.Count;
            int count = 0;

            // Divide the viewports in rows
            foreach (KeyValuePair<Viewport, double[]> entry in viewportDims)
            {
                Viewport vp = entry.Key;

                // Last element in list
                if (count + 1 == vpCount)
                {
                    // Store the heightest vertical viewport dimension
                    if (entry.Value[1] > maxViewportHeight)
                    {
                        maxViewportHeight = entry.Value[1];
                    }

                    // Clone single row list to total row list
                    viewportSingleRow.Add(vp);
                    viewportRows.Add(new List<Viewport>(viewportSingleRow));
                    
                    // Store max height and reset previous value
                    maxHeights.Add(entry.Value[1]);

                    // Store width
                    width.Add(entry.Value[0]);
                    viewportWidths.Add(new List<double>(width));
                }
                // Failsafe that last viewport does not exceed the max width
                else if (width.Sum() + width.Count * X < sheetWidth)
                {
                    width.Add(entry.Value[0]);
                    viewportSingleRow.Add(vp);

                    // Store the heightest vertical viewport dimension
                    if (entry.Value[1] > maxViewportHeight)
                    {
                        maxViewportHeight = entry.Value[1];
                    }

                    count++;
                }
                // Add exceeding viewport to next row
                else
                {
                    // Store the heightest vertical viewport dimension
                    if (entry.Value[1] > maxViewportHeight)
                    {
                        maxViewportHeight = entry.Value[1];
                    }

                    // Clone single row list to total row list
                    // Clear single row list and start the single row list over again
                    viewportRows.Add(new List<Viewport>(viewportSingleRow));
                    viewportSingleRow.Clear();
                    viewportSingleRow.Add(vp);

                    // Store max height and reset previous value
                    maxHeights.Add(entry.Value[1]);
                    maxViewportHeight = 0;

                    // Store width and reset max width
                    viewportWidths.Add(new List<double>(width));
                    width.Clear();
                    width.Add(entry.Value[0]);

                    count++;
                }
            }

            // Y distance for viewports
            double heightIncrease = sheetHeight;

            // Calculate center coordinates of viewports on sheets
            var coordinates = new List<XYZ>();

            for (int i = 0; i < viewportRows.Count; i++)
            {
                var vpList = viewportRows[i];

                double widthIncrease = X;

                for (int j = 0; j < vpList.Count; j++)
                {
                    // Retrieve max and min point
                    double vPwidth = viewportWidths[i][j];

                    // Calculate final center of viewports
                    XYZ vpCenter = new XYZ(widthIncrease + vPwidth / 2, (heightIncrease - (i + 1) * Y) - viewportDims[viewportRows[i][j]][1] / 2, 0);

                    // Increase spacing for next viewport
                    widthIncrease = widthIncrease + X + vPwidth;

                    coordinates.Add(vpCenter);

                    if (j == vpList.Count - 1)
                    {
                        heightIncrease -= viewportDims[viewportRows[i][j]][1] + (i + 1) * Y;
                    }
                }
            }

            return coordinates;
        }
        
    }
}
