using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMiconToolbar.SectionGeometry
{
    /// <summary>
    /// A class to count and report the 
    /// number of objects encountered.
    /// </summary>
    class JtObjCounter : Dictionary<string, int>
    {
        /// <summary>
        /// Count a new occurence of an object
        /// </summary>
        public void Increment(object obj)
        {
            string key = null == obj
              ? "null"
              : obj.GetType().Name;

            if (!ContainsKey(key))
            {
                Add(key, 0);
            }
            ++this[key];
        }

        /// <summary>
        /// Report the number of objects encountered.
        /// </summary>
        public void Print()
        {
            List<string> keys = new List<string>(Keys);
            keys.Sort();
            foreach (string key in keys)
            {
                Debug.Print("{0,5} {1}", this[key], key);
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        /// <summary>
        /// Maximum distance for line to be 
        /// considered to lie in plane
        /// </summary>
        const double _eps = 1.0e-6;

        /// <summary>
        /// User instructions for running this external command
        /// </summary>
        const string _instructions = "Please launch this "
          + "command in a section view with fine level of "
          + "detail and far bound clipping set to 'Clip with line'";

        /// <summary>
        /// Predicate returning true if the given line 
        /// lies in the given plane
        /// </summary>
        static bool IsLineInPlane(
          Line line,
          Plane plane)
        {
            XYZ p0 = line.GetEndPoint(0);
            XYZ p1 = line.GetEndPoint(1);
            UV uv0, uv1;
            double d0, d1;

            plane.Project(p0, out uv0, out d0);
            plane.Project(p1, out uv1, out d1);

            Debug.Assert(0 <= d0,
              "expected non-negative distance");
            Debug.Assert(0 <= d1,
              "expected non-negative distance");

            return (_eps > d0) && (_eps > d1);
        }

        /// <summary>
        /// Recursively handle geometry element to 
        /// retrieve all curves it contains that lie
        /// in the given plane
        /// </summary>
        static void GetCurvesInPlane(
          List<Curve> curves,
          JtObjCounter geoCounter,
          Plane plane,
          GeometryElement geo)
        {
            geoCounter.Increment(geo);

            if (null != geo)
            {
                foreach (GeometryObject obj in geo)
                {
                    geoCounter.Increment(obj);

                    Solid sol = obj as Solid;

                    if (null != sol)
                    {
                        EdgeArray edges = sol.Edges;

                        foreach (Edge edge in edges)
                        {
                            Curve curve = edge.AsCurve();

                            Debug.Assert(curve is Line,
                              "we currently only support lines here");

                            geoCounter.Increment(curve);

                            if (IsLineInPlane(curve as Line, plane))
                            {
                                curves.Add(curve);
                            }
                        }
                    }
                    else
                    {
                        GeometryInstance inst = obj as GeometryInstance;

                        if (null != inst)
                        {
                            GetCurvesInPlane(curves, geoCounter,
                              plane, inst.GetInstanceGeometry());
                        }
                        else
                        {
                            Debug.Assert(false,
                              "unsupported geometry object "
                              + obj.GetType().Name);
                        }
                    }
                }
            }
        }

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View section_view = commandData.View;
            Parameter p = section_view.get_Parameter(
              BuiltInParameter.VIEWER_BOUND_FAR_CLIPPING);
            //ViewType.Section != section_view.ViewType ||
            if (ViewDetailLevel.Fine != section_view.DetailLevel
              || 1 != p.AsInteger())
            {
                message = _instructions;
                return Result.Failed;
            }

            FilteredElementCollector a
              = new FilteredElementCollector(
                doc, section_view.Id);

            Options opt = new Options()
            {
                ComputeReferences = false,
                IncludeNonVisibleObjects = false,
                View = section_view
            };

            SketchPlane plane1 = section_view.SketchPlane; // this is null

            Plane plane2 = Plane.CreateByNormalAndOrigin(
              section_view.ViewDirection,
              section_view.Origin);

            JtObjCounter geoCounter = new JtObjCounter();

            List<Curve> curves = new List<Curve>();

            foreach (Element e in a)
            {
                geoCounter.Increment(e);

                GeometryElement geo = e.get_Geometry(opt);

                GetCurvesInPlane(curves,
                  geoCounter, plane2, geo);
            }

            Debug.Print("Objects analysed:");
            geoCounter.Print();

            Debug.Print(
              "{0} cut geometry lines found in section plane.",
              curves.Count);

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Create Section Cut Model Curves");

                SketchPlane plane3 = SketchPlane.Create(
                  doc, plane2);

                foreach (Curve c in curves)
                {
                    doc.Create.NewModelCurve(c, plane3);
                }

                tx.Commit();
            }
            return Result.Succeeded;
        }
    }
}
