using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace BIMicon.BIMiconToolbar.Helpers
{
    class HelpersGeometry
    {
        public static double AngleBetweenVectors(XYZ vectorA, XYZ vectorB)
        {
            double dotProduct = vectorA.X * vectorB.X + vectorA.Y * vectorB.Y + vectorA.Z * vectorB.Z;
            double modulusProduct = GetVectorModulus(vectorA) * GetVectorModulus(vectorB);

            return Math.Acos(dotProduct / modulusProduct);
        }


        public static double GetVectorModulus(XYZ vector)
        {
            return Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2) + Math.Pow(vector.Z, 2));
        }

        /// <summary>
        /// Check if two vectors are parallel
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns></returns>
        public static bool AreVectorsParallel(XYZ vectorA, XYZ vectorB)
        {
            bool areParallel = false;
            // Calculate cross product and magnitude of the result
            XYZ vector = vectorA.CrossProduct(vectorB);
            double magnitude = Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2) + Math.Pow(vector.Z, 2));
            // If magnitude is less than thresshold (due to Revit inaccuracy) then they are parallel
            if (magnitude < 1.0e-6)
                areParallel = true;
            return areParallel;
        }

        /// <summary>
        /// Function to check if point is inside a rectangle
        /// </summary>
        /// <param name="point"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int IsPointInsideRectangle(XYZ point, XYZ min, XYZ max)
        {
            // Condition to fulfill for a point to be inside a rectangle,
            // point between min and max of the rectangle
            if (point.X > min.X && point.X < max.X && point.Y > min.Y && point.Y < max.Y)
            {
                return 1;
            }
            // In rectangle
            else if (point.X >= min.X && point.X <= max.X && point.Y >= min.Y && point.Y <= max.Y)
            {
                return 2;
            }
            // Outside rectangle
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Function to divide a spline into same length segments
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="numberSegments"></param>
        /// <returns></returns>
        public static XYZ[] DivideEquallySpline(Curve curve, int numberSegments)
        {
            // Retrieve length of the curve
            double curveLength = curve.Length;

            // Create a list of equi-distant points.
            List<XYZ> points = new List<XYZ>(1);

            // Define segments length and step to integrate
            double segmentLength = curveLength / numberSegments;
            double normalParamStep = 1.0 / (curveLength * 10 * numberSegments);
            double dist = 0;

            // Approximation to obtain equidistant points
            XYZ p = curve.GetEndPoint(0);

            for (double i = 0; i < 1; i += normalParamStep)
            {
                XYZ q = curve.Evaluate(i, true);
                dist += p.DistanceTo(q);

                // Add start point
                if (points.Count == 0)
                {
                    points.Add(p);
                }

                // Distance to previous point over the segment length, so it is a good aproximation
                if (dist >= segmentLength)
                {
                    points.Add(q);
                    dist = 0;
                }

                p = q;
            }      
            return points.ToArray();
        }

        /// <summary>
        /// Function to place detail circles on points
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="location"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static void CreateCircle(Document doc, XYZ location, double radius)
        {
            XYZ norm = XYZ.BasisZ;

            double startAngle = 0;
            double endAngle = 2 * Math.PI;

            Plane plane = Plane.CreateByNormalAndOrigin(norm, location);

            Arc arc = Arc.Create(plane, radius, startAngle, endAngle);

            doc.Create.NewDetailCurve(doc.ActiveView, arc);
        }
    }
}
