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

        public static List<List<XYZ>> ViewportRowsColumns(Dictionary<Viewport, double[]> viewportDims, double sheetWidth, double sheetHeight)
        {
            // Viewport rows
            List<Viewport> viewportSingleRow = new List<Viewport>();
            List<List<Viewport>> viewportRows = new List<List<Viewport>>();

            // Store viewport dimension
            List<double> width = new List<double>();
            List<List<double>> viewportWidths = new List<List<double>>();

            // Maximum dimension
            double maxViewportHeight = 0;
            List<double> maxHeights = new List<double>();

            // Divide the viewports in rows
            foreach (KeyValuePair<Viewport, double[]> entry in viewportDims)
            {
                Viewport vp = entry.Key;
                
                // Check the latest view does not exceed the max width
                if (width.Sum() + width.Count * Helpers.MillimetersToFeet(30) <= sheetWidth)
                {
                    width.Add(entry.Value[0]);
                    viewportSingleRow.Add(vp);

                    if (entry.Value[1] > maxViewportHeight)
                    {
                        maxViewportHeight = entry.Value[1];
                    }
                }
                // Add exceding viewport to next row
                else
                {
                    viewportRows.Add(viewportSingleRow);
                    maxHeights.Add(maxViewportHeight);
                    viewportWidths.Add(width);
                    width.Clear();
                    width.Add(entry.Value[0]);
                    viewportSingleRow.Clear();
                    viewportSingleRow.Add(vp);
                    maxViewportHeight = 0;
                }
            }

            double X = Helpers.MillimetersToFeet(30);
            double Y = sheetHeight - Helpers.MillimetersToFeet(30);

            var vpCoordinates = new List<List<XYZ>>();

            for (int i = 0; i < viewportRows.Count; i++)
            {
                var vpList = viewportRows[i];
                var coordinates = new List<XYZ>();

                for (int j = 0; j < vpList.Count; j++)
                {
                    double vPwidth = viewportWidths[i][j];

                    XYZ maxP = new XYZ(X + vPwidth, Y, 0);
                    XYZ minP = new XYZ(X, Y - viewportDims[viewportRows[i][j]][1], 0);

                    X = X + vPwidth;

                    XYZ vpCenter = (maxP - minP) / 2;
                    coordinates.Add(vpCenter);
                }

                vpCoordinates.Add(coordinates);
                coordinates.Clear();
            }

            return vpCoordinates;
        }
    }
}
