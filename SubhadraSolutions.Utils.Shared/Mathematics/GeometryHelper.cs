using SubhadraSolutions.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SubhadraSolutions.Utils.Mathematics;

public static class GeometryHelper
{
    //public static double TOL = Math.Pow(10, -9); // Floating point error is likely to be above 1 epsilon
    public static double TOL = 0; // Floating point error is likely to be above 1 epsilon

    public static bool AlmostEqual(double a, double b, double? tolerance)
    {
        if (tolerance == null)
        {
            tolerance = TOL;
        }

        return Math.Abs(a - b) < tolerance.Value;
    }

    public static bool AlmostEqual(double a, double b)
    {
        return AlmostEqual(a, b, null);
    }

    public static double AngleBetweenTwoLines(PointF line1P1, PointF line1P2, PointF line2P1, PointF line2P2)
    {
        var theta1 = Math.Atan2(line1P1.Y - line1P2.Y, line1P1.X - line1P2.X);
        var theta2 = Math.Atan2(line2P1.Y - line2P2.Y, line2P1.X - line2P2.X);
        return theta1 - theta2;
    }

    public static bool CircleHasMinimumDistanceWithPolygon(PointF circleCenter, float radius, PointF[] polygon,
        float distance)
    {
        if (distance < 0)
        {
            return false;
        }

        return CircleOverlapsWithPolygon(circleCenter, radius + distance, polygon);
    }

    public static bool CircleOverlapsWithPolygon(PointF circleCenter, float radius, PointF[] polygon)
    {
        var radiusSquared = radius * radius;

        var vertex = polygon[polygon.Length - 1];

        var nearestDistance = float.MaxValue;
        var nearestIsInside = false;
        var nearestVertex = -1;
        var lastIsInside = false;

        for (var i = 0; i < polygon.Length; i++)
        {
            var nextVertex = polygon[i];

            var axis = new PointF(circleCenter.X - vertex.X, circleCenter.Y - vertex.Y);
            var axisLengthSquared = axis.X * axis.X + axis.Y * axis.Y;
            var distance = axisLengthSquared - radiusSquared;

            if (distance <= 0)
            {
                return true;
            }

            var isInside = false;

            var edge = new PointF(nextVertex.X - vertex.X, nextVertex.Y - vertex.Y);

            var edgeLengthSquared = edge.X * edge.X + edge.Y * edge.Y;

            if (edgeLengthSquared != 0)
            {
                var dot = edge.X * axis.X + edge.Y * axis.Y;

                if (dot >= 0 && dot <= edgeLengthSquared)
                {
                    var div = dot / edgeLengthSquared;
                    var temp = new PointF(edge.X * div, edge.Y * div);
                    var projection = new PointF(vertex.X + temp.X, vertex.Y + temp.Y);

                    axis = new PointF(projection.X - circleCenter.X, projection.Y - circleCenter.Y);

                    axisLengthSquared = axis.X * axis.X + axis.Y * axis.Y;
                    if (axisLengthSquared <= radiusSquared)
                    {
                        return true;
                    }

                    if (edge.X > 0)
                    {
                        if (axis.Y > 0)
                        {
                            return false;
                        }
                    }
                    else if (edge.X < 0)
                    {
                        if (axis.Y < 0)
                        {
                            return false;
                        }
                    }
                    else if (edge.Y > 0)
                    {
                        if (axis.X < 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (axis.X > 0)
                        {
                            return false;
                        }
                    }

                    isInside = true;
                }
            }

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestIsInside = isInside || lastIsInside;
                nearestVertex = i;
            }

            vertex = nextVertex;
            lastIsInside = isInside;
        }

        if (nearestVertex == 0)
        {
            return nearestIsInside || lastIsInside;
        }

        return nearestIsInside;
    }

    public static bool CircleReallyInPolygon(PointF[] path, PointF center, float radius)
    {
        for (var i = 0; i < path.Length; i++)
            if (PointSegmentDistance(center, path[i], path[(i + 1) % path.Length]) <= radius)
            {
                return false;
            }

        return true;
    }

    public static double DegreesToRadians(double angleInDegrees)
    {
        return angleInDegrees * (Math.PI / 180);
    }

    public static PointF? FindIntersection(PointF line1P1, PointF line1P2, PointF line2P1, PointF line2P2,
        double tolerance = 0.01)
    {
        double x1 = line1P1.X, y1 = line1P1.Y;
        double x2 = line1P2.X, y2 = line1P2.Y;

        double x3 = line2P1.X, y3 = line2P1.Y;
        double x4 = line2P2.X, y4 = line2P2.Y;

        //equations of the form x=c (two vertical lines)
        if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance && Math.Abs(x1 - x3) < tolerance)
        {
            return null;
        }

        //throw new Exception("Both lines overlap vertically, ambiguous intersection points.");
        //equations of the form y=c (two horizontal lines)
        if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance && Math.Abs(y1 - y3) < tolerance)
        //throw new Exception("Both lines overlap horizontally, ambiguous intersection points.");
        {
            return null;
        }

        //equations of the form x=c (two vertical lines)
        if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance)
        {
            return default(Point);
        }

        //equations of the form y=c (two horizontal lines)
        if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance)
        {
            return default(PointF);
        }

        //general equation of line is y = mx + c where m is the slope
        //assume equation of line 1 as y1 = m1x1 + c1
        //=> -m1x1 + y1 = c1 ----(1)
        //assume equation of line 2 as y2 = m2x2 + c2
        //=> -m2x2 + y2 = c2 -----(2)
        //if line 1 and 2 intersect then x1=x2=x & y1=y2=y where (x,y) is the intersection point
        //so we will get below two equations
        //-m1x + y = c1 --------(3)
        //-m2x + y = c2 --------(4)

        double x, y;

        //lineA is vertical x1 = x2
        //slope will be infinity
        //so lets derive another solution
        if (Math.Abs(x1 - x2) < tolerance)
        {
            //compute slope of line 2 (m2) and c2
            var m2 = (y4 - y3) / (x4 - x3);
            var c2 = -m2 * x3 + y3;

            //equation of vertical line is x = c
            //if line 1 and 2 intersect then x1=c1=x
            //subsitute x=x1 in (4) => -m2x1 + y = c2
            // => y = c2 + m2x1
            x = x1;
            y = c2 + m2 * x1;
        }
        //lineB is vertical x3 = x4
        //slope will be infinity
        //so lets derive another solution
        else if (Math.Abs(x3 - x4) < tolerance)
        {
            //compute slope of line 1 (m1) and c2
            var m1 = (y2 - y1) / (x2 - x1);
            var c1 = -m1 * x1 + y1;

            //equation of vertical line is x = c
            //if line 1 and 2 intersect then x3=c3=x
            //subsitute x=x3 in (3) => -m1x3 + y = c1
            // => y = c1 + m1x3
            x = x3;
            y = c1 + m1 * x3;
        }
        //lineA & lineB are not vertical
        //(could be horizontal we can handle it with slope = 0)
        else
        {
            //compute slope of line 1 (m1) and c2
            var m1 = (y2 - y1) / (x2 - x1);
            var c1 = -m1 * x1 + y1;

            //compute slope of line 2 (m2) and c2
            var m2 = (y4 - y3) / (x4 - x3);
            var c2 = -m2 * x3 + y3;

            //solving equations (3) & (4) => x = (c1-c2)/(m2-m1)
            //plugging x value in equation (4) => y = c2 + m2 * x
            x = (c1 - c2) / (m2 - m1);
            y = c2 + m2 * x;

            //verify by plugging intersection point (x, y)
            //in orginal equations (1) & (2) to see if they intersect
            //otherwise x,y values will not be finite and will fail this check
            if (!(Math.Abs(-m1 * x + y - c1) < tolerance
                  && Math.Abs(-m2 * x + y - c2) < tolerance))
            {
                return default(PointF);
            }
        }

        //x,y can intersect outside the line segment since line is infinitely long
        //so finally check if x, y is within both the line segments
        if (IsInsideLine(line1P1, line1P2, x, y) &&
            IsInsideLine(line2P1, line2P2, x, y))
        {
            return new PointF((float)x, (float)y);
        }

        //return default null (no intersection)
        return default(PointF);
    }

    public static float FixAngle(double angleInDegrees)
    {
        var abs = Math.Abs(angleInDegrees);
        var isNegative = angleInDegrees < 0;
        while (abs > 360) abs -= 360;
        return (float)(abs == 360 ? 0 : isNegative ? -abs : abs);
    }

    public static double GetAngleBetweenTwoPointsInDegrees(PointF center, PointF p)
    {
        double dx = p.X - center.X;
        // Minus to correct for coord re-mapping
        double dy = -(p.Y - center.Y);

        var inRads = Math.Atan2(dy, dx);

        return RadiansToDegrees(inRads);
    }

    public static PointF GetCentroid(PointF[] poly)
    {
        var accumulatedArea = 0.0f;
        var centerX = 0.0f;
        var centerY = 0.0f;

        for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
        {
            var temp = poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
            accumulatedArea += temp;
            centerX += (poly[i].X + poly[j].X) * temp;
            centerY += (poly[i].Y + poly[j].Y) * temp;
        }

        if (Math.Abs(accumulatedArea) < 1E-7f)
        {
            return PointF.Empty; // Avoid division by zero
        }

        accumulatedArea *= 3f;
        return new PointF(centerX / accumulatedArea, centerY / accumulatedArea);
    }

    public static double GetDistanceBetweenPoints(PointF a, PointF b)
    {
        var value = GetSquaredDistance(a, b);
        return Math.Sqrt(value);
    }

    public static double GetDistanceBetweenTwoLines(PointF l1p1, PointF l1p2, PointF l2p1, PointF l2p2)
    {
        if (SegmentsIntersect(l1p1, l1p2, l2p1, l2p2))
        {
            return 0;
        }

        var result = new[]
        {
            PointSegmentDistance(l1p1, l2p1, l2p2),
            PointSegmentDistance(l1p2, l2p1, l2p2),
            PointSegmentDistance(l2p1, l1p1, l1p2),
            PointSegmentDistance(l2p2, l1p1, l1p2)
        }.Min();
        return result;
    }

    public static PointF GetMidPointOfLine(PointF p1, PointF p2)
    {
        return new PointF((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
    }

    public static PointF GetPointOnLineWithDistance(PointF p1, PointF p2, double distance)
    {
        var d = GetDistanceBetweenPoints(p1, p2);
        var x = p1.X - distance * (p1.X - p2.X) / d;
        var y = p1.Y - distance * (p1.Y - p2.Y) / d;
        return new PointF((float)x, (float)y);
    }

    public static PointF[] GetPointsOfCornersWhenSidesHave90Degrees(float[] lengthsOfSides)
    {
        var points = new PointF[lengthsOfSides.Length];
        //points[0] = new PointF(0, 0);
        var previousPoint = new PointF(0, 0);

        for (var i = 0; i < lengthsOfSides.Length; i++)
        {
            if (i % 2 == 0)
            {
                points[i] = previousPoint with { X = previousPoint.X + lengthsOfSides[i] };
            }
            else
            {
                points[i] = previousPoint with { Y = previousPoint.Y + lengthsOfSides[i] };
            }

            previousPoint = points[i];
        }

        return points;
    }

    public static PointF GetPointWithDistanceAndAngle(PointF referencePoint, float angleInRadians, float distance)
    {
        //Get SOH
        var op = (float)Math.Sin(angleInRadians) * distance;
        //Get CAH
        var ad = (float)Math.Cos(angleInRadians) * distance;
        //Add to old Vector
        return new PointF(referencePoint.X + ad, referencePoint.Y + op);
    }

    public static double GetSquaredDistance(PointF a, PointF b)
    {
        var deltax = b.X - a.X;
        var deltay = b.Y - a.Y;
        return deltax * deltax + deltay * deltay;
    }

    public static double Hypot(double x, double y)
    {
        return Math.Sqrt(x * x + y * y);
    }

    public static bool Intersect(IList<PointF> A, PointF? aOffset, IList<PointF> B, PointF? bOffset)
    {
        var Aoffsetx = aOffset == null ? 0 : aOffset.Value.X;
        var Aoffsety = aOffset == null ? 0 : aOffset.Value.Y;
        var Boffsetx = bOffset == null ? 0 : bOffset.Value.X;
        var Boffsety = bOffset == null ? 0 : bOffset.Value.Y;

        //A = GeneralHelper.CloneList(A);
        //B = GeneralHelper.CloneList(B);

        for (var i = 0; i < A.Count - 1; i++)
            for (var j = 0; j < B.Count - 1; j++)
            {
                var a1 = new PointF(A[i].X + Aoffsetx, A[i].Y + Aoffsety);
                var a2 = new PointF(A[i + 1].X + Aoffsetx, A[i + 1].Y + Aoffsety);
                var b1 = new PointF(B[j].X + Boffsetx, B[j].Y + Boffsety);
                var b2 = new PointF(B[j + 1].X + Boffsetx, B[j + 1].Y + Boffsety);

                var prevbindex = j == 0 ? B.Count - 1 : j - 1;
                var prevaindex = i == 0 ? A.Count - 1 : i - 1;
                var nextbindex = j + 1 == B.Count - 1 ? 0 : j + 2;
                var nextaindex = i + 1 == A.Count - 1 ? 0 : i + 2;

                // go even further back if we happen to hit on a loop end point
                if (B[prevbindex] == B[j] || (AlmostEqual(B[prevbindex].X, B[j].X) && AlmostEqual(B[prevbindex].Y, B[j].Y)))
                {
                    prevbindex = prevbindex == 0 ? B.Count - 1 : prevbindex - 1;
                }

                if (A[prevaindex] == A[i] || (AlmostEqual(A[prevaindex].X, A[i].X) && AlmostEqual(A[prevaindex].Y, A[i].Y)))
                {
                    prevaindex = prevaindex == 0 ? A.Count - 1 : prevaindex - 1;
                }

                // go even further forward if we happen to hit on a loop end point
                if (B[nextbindex] == B[j + 1] ||
                    (AlmostEqual(B[nextbindex].X, B[j + 1].X) && AlmostEqual(B[nextbindex].Y, B[j + 1].Y)))
                {
                    nextbindex = nextbindex == B.Count - 1 ? 0 : nextbindex + 1;
                }

                if (A[nextaindex] == A[i + 1] ||
                    (AlmostEqual(A[nextaindex].X, A[i + 1].X) && AlmostEqual(A[nextaindex].Y, A[i + 1].Y)))
                {
                    nextaindex = nextaindex == A.Count - 1 ? 0 : nextaindex + 1;
                }

                var a0 = new PointF(A[prevaindex].X + Aoffsetx, A[prevaindex].Y + Aoffsety);
                var b0 = new PointF(B[prevbindex].X + Boffsetx, B[prevbindex].Y + Boffsety);

                var a3 = new PointF(A[nextaindex].X + Aoffsetx, A[nextaindex].Y + Aoffsety);
                var b3 = new PointF(B[nextbindex].X + Boffsetx, B[nextbindex].Y + Boffsety);

                if (OnSegment(a1, a2, b1) || (AlmostEqual(a1.X, b1.X) && AlmostEqual(a1.Y, b1.Y)))
                {
                    // if a point is on a segment, it could intersect or it could not. Check via the neighboring points
                    var b0in = PointInPolygon(A, b0);
                    var b2in = PointInPolygon(A, b2);
                    if ((b0in == true && b2in == false) || (b0in == false && b2in == true))
                    {
                        return true;
                    }

                    continue;
                }

                if (OnSegment(a1, a2, b2) || (AlmostEqual(a2.X, b2.X) && AlmostEqual(a2.Y, b2.Y)))
                {
                    // if a point is on a segment, it could intersect or it could not. Check via the neighboring points
                    var b1in = PointInPolygon(A, b1);
                    var b3in = PointInPolygon(A, b3);

                    if ((b1in == true && b3in == false) || (b1in == false && b3in == true))
                    {
                        return true;
                    }

                    continue;
                }

                if (OnSegment(b1, b2, a1) || (AlmostEqual(a1.X, b2.X) && AlmostEqual(a1.Y, b2.Y)))
                {
                    // if a point is on a segment, it could intersect or it could not. Check via the neighboring points
                    var a0in = PointInPolygon(B, a0);
                    var a2in = PointInPolygon(B, a2);

                    if ((a0in == true && a2in == false) || (a0in == false && a2in == true))
                    {
                        return true;
                    }

                    continue;
                }

                if (OnSegment(b1, b2, a2) || (AlmostEqual(a2.X, b1.X) && AlmostEqual(a2.Y, b1.Y)))
                {
                    // if a point is on a segment, it could intersect or it could not. Check via the neighboring points
                    var a1in = PointInPolygon(B, a1);
                    var a3in = PointInPolygon(B, a3);

                    if ((a1in == true && a3in == false) || (a1in == false && a3in == true))
                    {
                        return true;
                    }

                    continue;
                }

                var p = LineIntersect(b1, b2, a1, a2);

                if (p != null)
                {
                    return true;
                }
            }

        return false;
    }

    public static bool IsPerfectRectangle(IList<PointF> points)
    {
        if (points.Count != 4)
        {
            return false;
        }

        var x1 = points[0].X;
        var y1 = points[0].Y;
        var x2 = points[1].X;
        var y2 = points[1].Y;
        var x3 = points[2].X;
        var y3 = points[2].Y;
        var x4 = points[3].X;
        var y4 = points[3].Y;
        double m1 = (y2 - y1) / (x2 - x1);
        double m2 = (y2 - y3) / (x2 - x3);
        double m3 = (y4 - y3) / (x4 - x3);

        return m1 * m2 == -1 && m2 * m3 == -1;
    }

    //public static KeyValuePair<PointF, PointF> GetIntersectPoints(PointF[] polygon, PointF origin, double angleInRadians)
    //{
    //    var eps = 1e-9;
    //    origin = new PointF((float)(origin.X + (eps * Math.Cos(angleInRadians))), (float)(origin.Y + (eps * Math.Sin(angleInRadians))));
    //    var x0 = origin.X;
    //    var y0 = origin.Y;
    //    var shiftedOrigin = new PointF((float)(x0 + Math.Cos(angleInRadians)), (float)(y0 + Math.Sin(angleInRadians)));
    //    var idx = 0;
    //    if (Math.Abs(shiftedOrigin.X - x0) < eps)
    //    {
    //        idx = 1;
    //    }
    //    var i = -1;
    //    var n = polygon.Length;
    //    var b = polygon[n - 1];
    //    var minSqDistLeft = 1.79E+308;
    //    var minSqDistRight = 1.79E+308;
    //    PointF closestPointLeft = null;
    //    PointF closestPointRight = null;
    //    while (++i < n)
    //    {
    //        var a = b;
    //        b = polygon[i];
    //        var p = GetLineIntersection(origin, shiftedOrigin, a, b);
    //        if ((p != null) && IsPointInSegmentBox(p, a, b))
    //        {
    //            var pxyAsArray = new double[] { p.X, p.Y };
    //            var originxyAsArray = new double[] { origin.X, origin.Y };
    //            var sqDist = GeometryHelper.GetSquaredDistance(origin, p);
    //            if (pxyAsArray[idx] < originxyAsArray[idx])
    //            {
    //                if (sqDist < minSqDistLeft)
    //                {
    //                    minSqDistLeft = sqDist;
    //                    closestPointLeft = p;
    //                }
    //            }
    //            else if (pxyAsArray[idx] > originxyAsArray[idx])
    //            {
    //                if (sqDist < minSqDistRight)
    //                {
    //                    minSqDistRight = sqDist;
    //                    closestPointRight = p;
    //                }
    //            }
    //        }
    //    }
    //    return new KeyValuePair<PointF, PointF>(closestPointLeft, closestPointRight);
    //}
    public static bool IsPointInSegmentBox(PointF p, PointF p1, PointF q1)
    {
        var eps = 1e-9;
        var px = p.X;
        var py = p.Y;
        if (px < Math.Min(p1.X, q1.X) - eps || px > Math.Max(p1.X, q1.X) + eps || py < Math.Min(p1.Y, q1.Y) - eps ||
            py > Math.Max(p1.Y, q1.Y) + eps)
        {
            return false;
        }

        return true;
    }

    //    return false;
    //}
    public static bool IsRectangleLike(IList<PointF> points, double? tolerance = null)
    {
        var bounds = MinAndMax.Build(points);
        tolerance = tolerance ?? TOL;

        for (var i = 0; i < points.Count; i++)
        {
            if (!AlmostEqual(points[i].X, bounds.XMin) && !AlmostEqual(points[i].X, bounds.XMin + bounds.Width))
            {
                return false;
            }

            if (!AlmostEqual(points[i].Y, bounds.YMin) &&
                !AlmostEqual(points[i].Y, bounds.YMin + bounds.Height))
            {
                return false;
            }
        }

        return true;
    }

    // returns the intersection of AB and EF
    // or null if there are no intersections or other numerical error
    // if the infinite flag is set, AE and EF describe infinite lines without endpoints, they are finite line segments otherwise
    public static PointF? LineIntersect(PointF A, PointF B, PointF E, PointF F, bool infinite = false)
    {
        var a1 = B.Y - A.Y;
        var b1 = A.X - B.X;
        var c1 = B.X * A.Y - A.X * B.Y;
        var a2 = F.Y - E.Y;
        var b2 = E.X - F.X;
        var c2 = F.X * E.Y - E.X * F.Y;

        var denom = a1 * b2 - a2 * b1;
        if (denom == 0)
        {
            return null;
        }

        var x = (b1 * c2 - b2 * c1) / denom;
        var y = (a2 * c1 - a1 * c2) / denom;

        // lines are colinear
        /*var crossABE = (E.Y - A.Y) * (B.X - A.X) - (E.X - A.X) * (B.Y - A.Y);
        var crossABF = (F.Y - A.Y) * (B.X - A.X) - (F.X - A.X) * (B.Y - A.Y);
        if(_almostEqual(crossABE,0) && _almostEqual(crossABF,0)){
            return null;
        }*/

        if (!infinite)
        {
            // coincident points do not count as intersecting
            if (Math.Abs(A.X - B.X) > TOL && (A.X < B.X ? x < A.X || x > B.X : x > A.X || x < B.X))
            {
                return null;
            }

            if (Math.Abs(A.Y - B.Y) > TOL && (A.Y < B.Y ? y < A.Y || y > B.Y : y > A.Y || y < B.Y))
            {
                return null;
            }

            if (Math.Abs(E.X - F.X) > TOL && (E.X < F.X ? x < E.X || x > F.X : x > E.X || x < F.X))
            {
                return null;
            }

            if (Math.Abs(E.Y - F.Y) > TOL && (E.Y < F.Y ? y < E.Y || y > F.Y : y > E.Y || y < F.Y))
            {
                return null;
            }
        }

        return new PointF(x, y);
    }

    public static PointF[] MoveCoordinates(PointF[] coordinates, float xToIncrement, float yToIncrement)
    {
        //var minmax = GetMinMax(coordinates);
        var newCoordinates = (PointF[])coordinates.Clone();
        MoveCoordinatesInplace(newCoordinates, xToIncrement, yToIncrement);
        return newCoordinates;
    }

    public static void MoveCoordinatesInplace(PointF[] coordinates, float xToIncrement, float yToIncrement)
    {
        //var minmax = GetMinMax(coordinates);
        for (var i = 0; i < coordinates.Length; i++)
        {
            var existing = coordinates[i];
            coordinates[i] = new PointF(existing.X + xToIncrement, existing.Y + yToIncrement);
        }
    }

    //public static bool PolygonsOverlap(PointF[] polygon1, PointF[] polygon2)
    //{
    //    foreach(var p in polygon2)
    //    {
    //        var result = PointInPolygon(polygon1, null, p);
    //        if(result!=null && result.Value)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
    public static KeyValuePair<List<PointF>, MinAndMax> MoveToZeroZeroCoordinate(IList<PointF> points)
    {
        var bounds = MinAndMax.Build(points);
        var result = new List<PointF>(points.Count);
        for (var i = 0; i < points.Count; i++)
        {
            var point = points[i];
            var newPoint = new PointF(point.X - bounds.XMin, point.Y - bounds.YMin);
            result.Add(newPoint);
        }

        return new KeyValuePair<List<PointF>, MinAndMax>(result, bounds);
    }

    public static PointF[] MoveToZeroZeroCoordinates(PointF[] coordinates)
    {
        var minmax = MinAndMax.Build(coordinates);
        var newCoordinates = new PointF[coordinates.Length];
        for (var i = 0; i < coordinates.Length; i++)
        {
            var existing = coordinates[i];
            newCoordinates[i] = new PointF(existing.X - minmax.XMin, existing.Y - minmax.YMin);
        }

        return newCoordinates;
    }

    public static bool IsAngleBetweenTwoAngles(double angleInDegrees, double from, double to)
    {
        angleInDegrees = MakePositiveDegrees(angleInDegrees);
        from = MakePositiveDegrees(from);
        to = MakePositiveDegrees(to);

        if (from < to)
        {
            return from <= angleInDegrees && angleInDegrees <= to;
        }
        return from <= angleInDegrees || angleInDegrees <= to;
    }
    public static double AngleDifference(double a, double b)
    {
        a = MakePositiveDegrees(a);
        b = MakePositiveDegrees(b);
        return Math.Abs(a - b);
    }

    public static double MakePositive(double d, double n)
    {
        if (d < 0)
        {
            while (d < 0)
            {
                d += n;
            }
            return d;
        }
        return d % n;
    }
    public static double AbsoluteAngleDifference(double a, double b)
    {
        a = MakePositiveDegrees(a);
        b = MakePositiveDegrees(b);
        return Math.Min(Math.Min(Math.Abs(a - b), Math.Abs((a + 360) - b)), Math.Abs((b + 360) - a));
    }

    public static double MakePositiveDegrees(double degrees)
    {
        return MakePositive(degrees, 360d);
    }


    // normalize vector into a unit vector
    public static PointF Normalize(PointF point)
    {
        if (AlmostEqual(point.X * point.X + point.Y * point.Y,
                1))
        {
            return point; // given vector was already a unit vector
        }

        var len = Math.Sqrt(point.X * point.X + point.Y * point.Y);
        var inverse = 1 / len;

        return new PointF((float)(point.X * inverse), (float)(point.Y * inverse));
    }

    // returns true if p lies on the line segment defined by ab, but not at any endpoints
    // may need work!
    public static bool OnSegment(PointF a, PointF b, PointF p)
    {
        // vertical line
        if (AlmostEqual(a.X, b.X) && AlmostEqual(p.X, a.X))
        {
            if (!AlmostEqual(p.Y, b.Y) && !AlmostEqual(p.Y, a.Y) && p.Y < Math.Max(b.Y, a.Y) &&
                p.Y > Math.Min(b.Y, a.Y))
            {
                return true;
            }

            return false;
        }

        // horizontal line
        if (AlmostEqual(a.Y, b.Y) && AlmostEqual(p.Y, a.Y))
        {
            if (!AlmostEqual(p.X, b.X) && !AlmostEqual(p.X, a.X) && p.X < Math.Max(b.X, a.X) &&
                p.X > Math.Min(b.X, a.X))
            {
                return true;
            }

            return false;
        }

        //range check
        if ((p.X < a.X && p.X < b.X) || (p.X > a.X && p.X > b.X) || (p.Y < a.Y && p.Y < b.Y) ||
            (p.Y > a.Y && p.Y > b.Y))
        {
            return false;
        }

        // exclude end points
        if ((AlmostEqual(p.X, a.X) && AlmostEqual(p.Y, a.Y)) ||
            (AlmostEqual(p.X, b.X) && AlmostEqual(p.Y, b.Y)))
        {
            return false;
        }

        var cross = (p.Y - a.Y) * (b.X - a.X) - (p.X - a.X) * (b.Y - a.Y);

        if (Math.Abs(cross) > TOL)
        {
            return false;
        }

        var dot = (p.X - a.X) * (b.X - a.X) + (p.Y - a.Y) * (b.Y - a.Y);

        if (dot < 0 || AlmostEqual(dot, 0))
        {
            return false;
        }

        var len2 = (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);

        if (dot > len2 || AlmostEqual(dot, len2))
        {
            return false;
        }

        return true;
    }

    public static float? PointDistance(PointF p, PointF s1, PointF s2, PointF normal)
    {
        return PointDistance(p, s1, s2, normal, false);
    }

    public static float? PointDistance(PointF p, PointF s1, PointF s2, PointF normal, bool infinite)
    {
        normal = Normalize(normal);

        var dir = new PointF(normal.Y, -normal.X);

        var pdot = p.X * dir.X + p.Y * dir.Y;
        var s1dot = s1.X * dir.X + s1.Y * dir.Y;
        var s2dot = s2.X * dir.X + s2.Y * dir.Y;

        var pdotnorm = p.X * normal.X + p.Y * normal.Y;
        var s1dotnorm = s1.X * normal.X + s1.Y * normal.Y;
        var s2dotnorm = s2.X * normal.X + s2.Y * normal.Y;

        if (!infinite)
        {
            if (((pdot < s1dot || AlmostEqual(pdot, s1dot)) && (pdot < s2dot || AlmostEqual(pdot, s2dot))) ||
                ((pdot > s1dot || AlmostEqual(pdot, s1dot)) &&
                 (pdot > s2dot ||
                  AlmostEqual(pdot, s2dot))))
            {
                return null; // dot doesn't collide with segment, or lies directly on the vertex
            }

            if (AlmostEqual(pdot, s1dot) && AlmostEqual(pdot, s2dot) && pdotnorm > s1dotnorm && pdotnorm > s2dotnorm)
            {
                return Math.Min(pdotnorm - s1dotnorm, pdotnorm - s2dotnorm);
            }

            if (AlmostEqual(pdot, s1dot) && AlmostEqual(pdot, s2dot) && pdotnorm < s1dotnorm && pdotnorm < s2dotnorm)
            {
                return -Math.Min(s1dotnorm - pdotnorm, s2dotnorm - pdotnorm);
            }
        }

        return -(pdotnorm - s1dotnorm + (s1dotnorm - s2dotnorm) * (s1dot - pdot) / (s1dot - s2dot));
    }

    public static bool? PointInCircle(PointF center, double radius, PointF point)
    {
        var squareDistance = GetSquaredDistance(center, point);
        var diameter = radius * 2;
        if (diameter == squareDistance)
        {
            return null;
        }

        return squareDistance <= diameter;
    }

    public static bool? PointInEllipse(PointF center, float radiusX, float radiusY, PointF point)
    {
        var normalized = new PointF(point.X - center.X, point.Y - center.Y);

        var result = (double)(normalized.X * normalized.X)
            / (radiusX * radiusY) + (double)(normalized.Y * normalized.Y) / (radiusY * radiusY);
        if (result == 0)
        {
            return null;
        }

        return result <= 1.0;
    }

    // return true if point is in the polygon, false if outside, and null if exactly on a point or edge
    public static bool? PointInPolygon(IList<PointF> polygonPath, PointF point,
        IEnumerable<PointF[]> polygonHoles = null)
    {
        var num = 0;
        var count = polygonPath.Count;
        if (count < 3)
        {
            return false;
        }

        var intPoint = polygonPath[0];
        for (var i = 1; i <= count; i++)
        {
            var intPoint2 = i == count ? polygonPath[0] : polygonPath[i];
            if (intPoint2.Y == point.Y &&
                (intPoint2.X == point.X || (intPoint.Y == point.Y && intPoint2.X > point.X == intPoint.X < point.X)))
            {
                return null;
            }

            if (intPoint.Y < point.Y != intPoint2.Y < point.Y)
            {
                if (intPoint.X >= point.X)
                {
                    if (intPoint2.X > point.X)
                    {
                        num = 1 - num;
                    }
                    else
                    {
                        var num2 = (intPoint.X - point.X) * (double)(intPoint2.Y - point.Y) -
                                   (intPoint2.X - point.X) * (double)(intPoint.Y - point.Y);
                        if (num2 == 0.0)
                        {
                            return null;
                        }

                        if (num2 > 0.0 == intPoint2.Y > intPoint.Y)
                        {
                            num = 1 - num;
                        }
                    }
                }
                else if (intPoint2.X > point.X)
                {
                    var num3 = (intPoint.X - point.X) * (double)(intPoint2.Y - point.Y) -
                               (intPoint2.X - point.X) * (double)(intPoint.Y - point.Y);
                    if (num3 == 0.0)
                    {
                        return null;
                    }

                    if (num3 > 0.0 == intPoint2.Y > intPoint.Y)
                    {
                        num = 1 - num;
                    }
                }
            }

            intPoint = intPoint2;
        }

        if (num == 0)
        {
            return false;
        }

        if (num == -1)
        {
            return null;
        }

        if (polygonHoles == null)
        {
            return true;
        }

        foreach (var l in polygonHoles)
        {
            var b = PointInPolygon(l, point);
            if (b != null && b.Value)
            {
                return false;
            }
        }

        return true;
    }

    public static float? PointLineDistance(PointF p, PointF s1, PointF s2, PointF normal, bool s1inclusive,
        bool s2inclusive)
    {
        normal = Normalize(normal);

        var dir = new PointF(normal.Y, -normal.X);

        var pdot = p.X * dir.X + p.Y * dir.Y;
        var s1dot = s1.X * dir.X + s1.Y * dir.Y;
        var s2dot = s2.X * dir.X + s2.Y * dir.Y;

        var pdotnorm = p.X * normal.X + p.Y * normal.Y;
        var s1dotnorm = s1.X * normal.X + s1.Y * normal.Y;
        var s2dotnorm = s2.X * normal.X + s2.Y * normal.Y;

        // point is exactly along the edge in the normal direction
        if (AlmostEqual(pdot, s1dot) && AlmostEqual(pdot, s2dot))
        {
            // point lies on an endpoint
            if (AlmostEqual(pdotnorm, s1dotnorm))
            {
                return null;
            }

            if (AlmostEqual(pdotnorm, s2dotnorm))
            {
                return null;
            }

            // point is outside both endpoints
            if (pdotnorm > s1dotnorm && pdotnorm > s2dotnorm)
            {
                return Math.Min(pdotnorm - s1dotnorm, pdotnorm - s2dotnorm);
            }

            if (pdotnorm < s1dotnorm && pdotnorm < s2dotnorm)
            {
                return -Math.Min(s1dotnorm - pdotnorm, s2dotnorm - pdotnorm);
            }

            // point lies between endpoints
            var diff1 = pdotnorm - s1dotnorm;
            var diff2 = pdotnorm - s2dotnorm;
            if (diff1 > 0)
            {
                return diff1;
            }

            return diff2;
        }
        // point

        if (AlmostEqual(pdot, s1dot))
        {
            if (s1inclusive)
            {
                return pdotnorm - s1dotnorm;
            }

            return null;
        }

        if (AlmostEqual(pdot, s2dot))
        {
            if (s2inclusive)
            {
                return pdotnorm - s2dotnorm;
            }

            return null;
        }

        if ((pdot < s1dot && pdot < s2dot) ||
            (pdot > s1dot && pdot > s2dot))
        {
            return null; // point doesn't collide with segment
        }

        return pdotnorm - s1dotnorm + (s1dotnorm - s2dotnorm) * (s1dot - pdot) / (s1dot - s2dot);
    }

    //        var intersect = ((yi > point.Y) != (yj > point.Y)) && (point.X < (xj - xi) * (point.Y - yi) / (yj - yi) + xi);
    //        if (intersect) inside = !inside;
    //    }
    //    if (!inside)
    //    {
    //        return false;
    //    }
    //    if (polygonHoles == null)
    //    {
    //        return true;
    //    }
    //    foreach (var l in polygonHoles)
    //    {
    //        var b = PointInPolygon(l, null, point, null);
    //        if (b != null && b.Value)
    //        {
    //            return false;
    //        }
    //    }
    //    return inside;
    //}
    public static bool PointReallyInPolygon(PointF[] path, PointF pt, IEnumerable<PointF[]> polygonHoles = null)
    {
        var result = PointInPolygon(path, pt, polygonHoles);
        return result != null && result.Value;
    }

    public static double PointSegmentDistance(PointF p, PointF p1, PointF p2)
    {
        var dx = p2.X - p1.X;
        var dy = p2.Y - p1.Y;
        if (dx == 0 && dy == 0)
        {
            return Hypot(p.X - p1.X, p.Y - p1.Y);
        }

        var t = ((p.X - p1.X) * dx + (p.Y - p1.Y) * dy) / (dx * dx + dy * dy);

        if (t < 0)
        {
            dx = p.X - p1.X;
            dy = p.Y - p1.Y;
        }
        else
        {
            if (t > 1)
            {
                dx = p.X - p2.X;
                dy = p.Y - p2.Y;
            }
            else
            {
                var near_x = p1.X + t * dx;
                var near_y = p1.Y + t * dy;
                dx = p.X - near_x;
                dy = p.Y - near_y;
            }
        }

        return Hypot(dx, dy);
    }

    public static double PolygonArea(IList<PointF> points)
    {
        double area = 0;
        for (int i = 0, j = points.Count - 1; i < points.Count; j = i++)
            area += (points[j].X + points[i].X) * (points[j].Y - points[i].Y);
        return Math.Abs(0.5 * area);
    }

    //Works only with Convex polygons
    public static bool PolygonContains(PointF[] containerPolygon, PointF[] polygon,
        IEnumerable<PointF[]> polygonHoles = null)
    {
        //all points in inner Polygon should be contained in polygon
        for (var i = 0; i < polygon.Length; i++)
        {
            var result = PointInPolygon(containerPolygon, polygon[i], polygonHoles);
            if (result != null && !result.Value)
            {
                return false;
            }
        }

        return true;
    }

    //    for (var i = 0; i < nfp.Count; i++)
    //    {
    //        for (var j = 0; j < nfp[i].Count; j++)
    //        {
    //            if (GeometryUtil.AlmostEqual(p.X, nfp[i][j].X) && GeometryUtil.AlmostEqual(p.Y, nfp[i][j].Y))
    //            {
    //                return true;
    //            }
    //        }
    //    }
    // todo: swap this for a more efficient sweep-line implementation
    // returnEdges: if set, return all edges on A that have intersections
    // given two polygons that touch at at least one point, but do not intersect. Return the outer perimeter of both polygons as a single continuous polygon
    // A and B must have the same winding direction
    public static object PolygonHull(IList<PointF> A, PointF? aOffset, IList<PointF> B, PointF? bOffset)
    {
        if (A != null || A.Count < 3 || B != null || B.Count < 3)
        {
            return null;
        }

        //var i, j;

        var Aoffsetx = aOffset == null ? 0 : aOffset.Value.X;
        var Aoffsety = aOffset == null ? 0 : aOffset.Value.Y;
        var Boffsetx = bOffset == null ? 0 : bOffset.Value.X;
        var Boffsety = bOffset == null ? 0 : bOffset.Value.Y;

        // start at an extreme point that is guaranteed to be on the final polygon
        var miny = A[0].Y;
        var startPolygon = A;
        var startIndex = 0;

        for (var i = 0; i < A.Count; i++)
            if (A[i].Y + Aoffsety < miny)
            {
                miny = A[i].Y + Aoffsety;
                startPolygon = A;
                startIndex = i;
            }

        for (var i = 0; i < B.Count; i++)
            if (B[i].Y + Boffsety < miny)
            {
                miny = B[i].Y + Boffsety;
                startPolygon = B;
                startIndex = i;
            }

        // for simplicity we'll define polygon A as the starting polygon
        if (startPolygon == B)
        {
            B = A;
            A = startPolygon;

            Aoffsetx = aOffset == null ? 0 : aOffset.Value.X;
            Aoffsety = aOffset == null ? 0 : aOffset.Value.Y;
            Boffsetx = bOffset == null ? 0 : bOffset.Value.X;
            Boffsety = bOffset == null ? 0 : bOffset.Value.Y;
        }

        //A = GeneralHelper.CloneList(A);
        //B = GeneralHelper.CloneList(B);

        var C = new List<PointF>();
        var current = startIndex;
        int? intercept1 = null;
        int? intercept2 = null;

        // scan forward from the starting point
        for (var i = 0; i < A.Count + 1; i++)
        {
            current = current == A.Count ? 0 : current;
            var next = current == A.Count - 1 ? 0 : current + 1;
            var touching = false;
            for (var j = 0; j < B.Count; j++)
            {
                var nextj = j == B.Count - 1 ? 0 : j + 1;
                if (AlmostEqual(A[current].X + Aoffsetx, B[j].X + Boffsetx) &&
                    AlmostEqual(A[current].Y + Aoffsety, B[j].Y + Boffsety))
                {
                    C.Add(new PointF(A[current].X + Aoffsetx, A[current].Y + Aoffsety));
                    intercept1 = j;
                    touching = true;
                    break;
                }

                if (OnSegment(new PointF(A[current].X + Aoffsetx, A[current].Y + Aoffsety),
                        new PointF(A[next].X + Aoffsetx, A[next].Y + Aoffsety),
                        new PointF(B[j].X + Boffsetx, B[j].Y + Boffsety)))
                {
                    C.Add(new PointF(A[current].X + Aoffsetx, A[current].Y + Aoffsety));
                    C.Add(new PointF(B[j].X + Boffsetx, B[j].Y + Boffsety));
                    intercept1 = j;
                    touching = true;
                    break;
                }

                if (OnSegment(new PointF(B[j].X + Boffsetx, B[j].Y + Boffsety),
                        new PointF(B[nextj].X + Boffsetx, B[nextj].Y + Boffsety),
                        new PointF(A[current].X + Aoffsetx, A[current].Y + Aoffsety)))
                {
                    C.Add(new PointF(A[current].X + Aoffsetx, A[current].Y + Aoffsety));
                    C.Add(new PointF(B[nextj].X + Boffsetx, B[nextj].Y + Boffsety));
                    intercept1 = nextj;
                    touching = true;
                    break;
                }
            }

            if (touching)
            {
                break;
            }

            C.Add(new PointF(A[current].X + Aoffsetx, A[current].Y + Aoffsety));

            current++;
        }

        // scan backward from the starting point
        current = startIndex - 1;
        for (var i = 0; i < A.Count + 1; i++)
        {
            current = current < 0 ? A.Count - 1 : current;
            var next = current == 0 ? A.Count - 1 : current - 1;
            var touching = false;
            for (var j = 0; j < B.Count; j++)
            {
                var nextj = j == B.Count - 1 ? 0 : j + 1;
                if (AlmostEqual(A[current].X + Aoffsetx, B[j].X + Boffsetx) &&
                    AlmostEqual(A[current].Y, B[j].Y + Boffsety))
                {
                    C.Insert(0, new PointF(A[current].X + Aoffsetx, A[current].Y + Aoffsety));
                    intercept2 = j;
                    touching = true;
                    break;
                }

                if (OnSegment(new PointF(A[current].X + Aoffsetx, A[current].Y + Aoffsety),
                        new PointF(A[next].X + Aoffsetx, A[next].Y + Aoffsety),
                        new PointF(B[j].X + Boffsetx, B[j].Y + Boffsety)))
                {
                    C.Insert(0, new PointF(A[current].X + Aoffsetx, A[current].Y + Aoffsety));
                    C.Insert(0, new PointF(B[j].X + Boffsetx, B[j].Y + Boffsety));
                    intercept2 = j;
                    touching = true;
                    break;
                }

                if (OnSegment(new PointF(B[j].X + Boffsetx, B[j].Y + Boffsety),
                        new PointF(B[nextj].X + Boffsetx, B[nextj].Y + Boffsety),
                        new PointF(A[current].X + Aoffsetx, A[current].Y + Aoffsety)))
                {
                    C.Insert(0, new PointF(A[current].X + Aoffsetx, A[current].Y + Aoffsety));
                    intercept2 = j;
                    touching = true;
                    break;
                }
            }

            if (touching)
            {
                break;
            }

            C.Insert(0, new PointF(A[current].X + Aoffsetx, A[current].Y + Aoffsety));

            current--;
        }

        if (intercept1 == null || intercept2 == null)
        // polygons not touching?
        {
            return null;
        }

        // the relevant points on B now lie between intercept1 and intercept2
        current = intercept1.Value + 1;
        for (var i = 0; i < B.Count; i++)
        {
            current = current == B.Count ? 0 : current;
            C.Add(new PointF(B[current].X + Boffsetx, B[current].Y + Boffsety));

            if (current == intercept2)
            {
                break;
            }

            current++;
        }

        // dedupe
        for (var i = 0; i < C.Count; i++)
        {
            var next = i == C.Count - 1 ? 0 : i + 1;
            if (AlmostEqual(C[i].X, C[next].X) && AlmostEqual(C[i].Y, C[next].Y))
            {
                C.RemoveAt(i);
                i--;
            }
        }

        return C;
    }

    public static bool PolygonsHaveMinimumDistance(PointF[] polygon1, PointF[] polygon2, float distance)
    {
        for (var i = 0; i < polygon1.Length; i++)
            for (var j = 0; j < polygon2.Length; j++)
            {
                var d = GetDistanceBetweenTwoLines(polygon1[i], polygon1[(i + 1) % polygon1.Length], polygon2[j],
                    polygon2[(j + 1) % polygon2.Length]);
                if (d <= distance)
                {
                    return true;
                }
            }

        return false;
    }

    public static bool PolygonsOverlap(PointF[] polygon1, PointF[] polygon2)
    {
        var centroid = GetCentroid(polygon2);
        var isCentroidInPolygon = PointInPolygon(polygon1, centroid);
        if (isCentroidInPolygon != null && isCentroidInPolygon.Value)
        {
            return true;
        }

        return PolygonsHaveMinimumDistance(polygon1, polygon2, 0);
    }

    public static float RadiansToDegrees(double angleInRadians)
    {
        return FixAngle(angleInRadians * (180 / Math.PI));
    }

    //public static bool PolygonContains(PointF[] containerPolygon, PointF[] polygon)
    //{
    //    var polygonCentroid = GetCentroid(polygon);
    //    var pointIn = PointInPolygon(containerPolygon, polygonCentroid);
    //    if (pointIn != null && !pointIn.Value)
    //    {
    //        return false;
    //    }
    //    return PolygonsOverlap(containerPolygon, polygon);
    //}
    public static PointF[] RearrangeForLeftMostPointFirst(PointF[] points)
    {
        if (points.Length == 0)
        {
            return Array.Empty<PointF>();
        }

        var minXIndex = 0;
        var minX = points[0].X;
        for (var i = 0; i < points.Length; i++)
        {
            var x = points[i].X;
            if (x < minX)
            {
                minX = x;
                minXIndex = i;
            }
        }

        if (minXIndex == 0)
        {
            return (PointF[])points.Clone();
        }

        var newArray = new PointF[points.Length];
        Array.Copy(points, minXIndex, newArray, 0, points.Length - minXIndex);
        Array.Copy(points, 0, newArray, points.Length - minXIndex, minXIndex);
        return newArray;
    }

    public static PointF RotateCoordinate(PointF coordinate, PointF origin, double angleInRadians)
    {
        var newX = origin.X + ((coordinate.X - origin.X) * Math.Cos(angleInRadians) -
                               (coordinate.Y - origin.Y) * Math.Sin(angleInRadians));
        var newY = origin.Y + ((coordinate.Y - origin.Y) * Math.Cos(angleInRadians) +
                               (coordinate.X - origin.X) * Math.Sin(angleInRadians));
        return new PointF((float)newX, (float)newY);
    }

    //public static double RadiansToDegrees(double angleInRadians)
    //{
    //    return angleInRadians * (180 / Math.PI);
    //}
    //public static PointF RotatePoint(double x, double y, double originX, double originY, double angleInRadians)
    //{
    //    var newX = originX + (((x - originX) * Math.Cos(angleInRadians)) - ((y - originY) * Math.Sin(angleInRadians)));
    //    var newY = originY + (((y - originY) * Math.Cos(angleInRadians)) + ((x - originX) * Math.Sin(angleInRadians)));
    //    return new PointF((float)newX, (float)newY);
    //}
    public static PointF[] RotateCoordinates(PointF[] coordinates, PointF origin, double angleInRadians)
    {
        var newCoordinates = new PointF[coordinates.Length];
        for (var i = 0; i < coordinates.Length; i++)
        {
            var p = RotateCoordinate(coordinates[i], origin, angleInRadians);
            newCoordinates[i] = p;
        }

        return newCoordinates;
    }

    //public static double DegreesToRadians(double angleInDegrees)
    //{
    //    return angleInDegrees * (Math.PI / 180);
    //}
    public static void RotateCoordinatesInplace(PointF[] coordinates, PointF origin, double angleInRadians)
    {
        for (var i = 0; i < coordinates.Length; i++)
        {
            var p = RotateCoordinate(coordinates[i], origin, angleInRadians);
            coordinates[i] = p;
        }
    }

    public static PointF RotatePoint(PointF point, double angleInRadians)
    {
        return RotatePoint(point.X, point.Y, angleInRadians);
    }

    public static PointF RotatePoint(float x, float y, double angleInRadians)
    {
        var x1 = x * Math.Cos(angleInRadians) - y * Math.Sin(angleInRadians);
        var y1 = x * Math.Sin(angleInRadians) + y * Math.Cos(angleInRadians);

        return new PointF((float)x1, (float)y1);
    }

    public static PointF RotatePoint(PointF point, PointF origin, double angleInRadians)
    {
        return RotatePoint(point.X, point.Y, origin.X, origin.Y, angleInRadians);
    }

    public static PointF RotatePoint(double x, double y, double originX, double originY, double angleInRadians)
    {
        RotatePoint(x, y, originX, originY, angleInRadians, out var newX, out var newY);
        return new PointF((float)newX, (float)newY);
    }

    public static void RotatePoint(double x, double y, double originX, double originY, double angleInRadians,
        out double newX, out double newY)
    {
        newX = originX + ((x - originX) * Math.Cos(angleInRadians) - (y - originY) * Math.Sin(angleInRadians));
        newY = originY + ((y - originY) * Math.Cos(angleInRadians) + (x - originX) * Math.Sin(angleInRadians));
    }

    public static IEnumerable<PointF> RotatePoints(IEnumerable<PointF> points, double angleInDegrees)
    {
        var angleInRadians = DegreesToRadians(angleInDegrees);
        foreach (var p in points)
        {
            yield return RotatePoint(p, angleInRadians);
        }
    }

    public static List<PointF> RotatePoints(IList<PointF> points, double angleInDegrees)
    {
        var angleInRadians = DegreesToRadians(angleInDegrees);
        var result = new List<PointF>(points.Count);
        for (var i = 0; i < points.Count; i++)
        {
            var p = RotatePoint(points[i], angleInRadians);
            result.Add(p);
        }

        return result;
    }

    public static List<PointF> RotatePoints(IList<PointF> points, PointF origin, int angleInDegrees)
    {
        var angleInRadians = DegreesToRadians(angleInDegrees);
        //Point origin = points[0];
        var newPoints = new List<PointF>(points.Count);
        //newPoints[0] = points[0];
        for (var i = 0; i < points.Count; i++)
        {
            var p = RotatePoint(points[i], origin, angleInRadians);
            newPoints.Add(p);
        }

        return newPoints;
    }

    public static IEnumerable<PointF> RotatePointsEnumerable(IEnumerable<PointF> points, double angleInDegrees)
    {
        var angleInRadians = DegreesToRadians(angleInDegrees);
        foreach (var point in points)
        {
            var p = RotatePoint(point, angleInRadians);
            yield return p;
        }
    }

    public static PointF[] ScalePoints(PointF[] points, PointF centroid, double scale)
    {
        var newPoints = new PointF[points.Length];
        for (var i = 0; i < points.Length; i++)
        {
            var point = points[i];
            var x = centroid.X + (point.X - centroid.X) * scale;

            var y = centroid.Y + (point.Y - centroid.Y) * scale;

            newPoints[i] = new PointF((float)x, (float)y);
        }

        return newPoints;
    }

    //        newPoints[i] = new PointF((float)x, (float)y);
    //    }
    //    return newPoints;
    //}
    public static PointF[] ScalePoints(PointF[] points, double scale)
    {
        var newPoints = new PointF[points.Length];
        for (var i = 0; i < points.Length; i++)
        {
            var point = points[i];
            var x = point.X * scale;

            var y = point.Y * scale;

            newPoints[i] = new PointF((float)x, (float)y);
        }

        return newPoints;
    }

    //        if (AlmostEqual(xi, xj) && AlmostEqual(yi, yj))
    //        { // ignore very small lines
    //            continue;
    //        }
    // returns the normal distance from p to a line segment defined by s1 s2
    // this is basically algo 9 in [1], generalized for any vector direction
    // eg. normal of [-1, 0] returns the horizontal distance between the point and the line segment
    // sxinclusive: if true, include endpoints instead of excluding them
    public static float? SegmentDistance(PointF A, PointF B, PointF E, PointF F, PointF direction)
    {
        var normal = new PointF(direction.Y, -direction.X);

        var reverse = new PointF(-direction.X, -direction.Y);

        var dotA = A.X * normal.X + A.Y * normal.Y;
        var dotB = B.X * normal.X + B.Y * normal.Y;
        var dotE = E.X * normal.X + E.Y * normal.Y;
        var dotF = F.X * normal.X + F.Y * normal.Y;

        var crossA = A.X * direction.X + A.Y * direction.Y;
        var crossB = B.X * direction.X + B.Y * direction.Y;
        var crossE = E.X * direction.X + E.Y * direction.Y;
        var crossF = F.X * direction.X + F.Y * direction.Y;

        var crossABmin = Math.Min(crossA, crossB);
        var crossABmax = Math.Max(crossA, crossB);

        var crossEFmax = Math.Max(crossE, crossF);
        var crossEFmin = Math.Min(crossE, crossF);

        var ABmin = Math.Min(dotA, dotB);
        var ABmax = Math.Max(dotA, dotB);

        var EFmax = Math.Max(dotE, dotF);
        var EFmin = Math.Min(dotE, dotF);

        // segments that will merely touch at one point
        if (AlmostEqual(ABmax, EFmin, TOL) || AlmostEqual(ABmin, EFmax, TOL))
        {
            return null;
        }

        // segments miss eachother completely
        if (ABmax < EFmin || ABmin > EFmax)
        {
            return null;
        }

        double overlap;

        if ((ABmax > EFmax && ABmin < EFmin) || (EFmax > ABmax && EFmin < ABmin))
        {
            overlap = 1;
        }
        else
        {
            var minMax = Math.Min(ABmax, EFmax);
            var maxMin = Math.Max(ABmin, EFmin);

            var maxMax = Math.Max(ABmax, EFmax);
            var minMin = Math.Min(ABmin, EFmin);

            overlap = (minMax - maxMin) / (maxMax - minMin);
        }

        var crossABE = (E.Y - A.Y) * (B.X - A.X) - (E.X - A.X) * (B.Y - A.Y);
        var crossABF = (F.Y - A.Y) * (B.X - A.X) - (F.X - A.X) * (B.Y - A.Y);

        // lines are colinear
        if (AlmostEqual(crossABE, 0) && AlmostEqual(crossABF, 0))
        {
            //var ABnorm = new PointF(B.Y - A.Y, A.X - B.X );

            double ABnormX = B.Y - A.Y;
            double ABnormY = A.X - B.X;
            //var EFnorm = new PointF(F.Y - E.Y, E.X - F.X);
            double EFnormX = F.Y - E.Y;
            double EFnormY = E.X - F.X;

            var ABnormlength = Math.Sqrt(ABnormX * ABnormX + ABnormY * ABnormY);
            ABnormX /= ABnormlength;
            ABnormY /= ABnormlength;

            var EFnormlength = Math.Sqrt(EFnormX * EFnormX + EFnormY * EFnormY);
            EFnormX /= EFnormlength;
            EFnormY /= EFnormlength;

            // segment normals must point in opposite directions
            if (Math.Abs(ABnormY * EFnormX - ABnormX * EFnormY) < TOL && ABnormY * EFnormY + ABnormX * EFnormX < 0)
            {
                // normal of AB segment must point in same direction as given direction vector
                var normdot = ABnormY * direction.Y + ABnormX * direction.X;
                // the segments merely slide along eachother
                if (AlmostEqual(normdot, 0, TOL))
                {
                    return null;
                }

                if (normdot < 0)
                {
                    return 0;
                }
            }

            return null;
        }

        var distances = new List<float>();

        // coincident points
        if (AlmostEqual(dotA, dotE))
        {
            distances.Add(crossA - crossE);
        }
        else if (AlmostEqual(dotA, dotF))
        {
            distances.Add(crossA - crossF);
        }
        else if (dotA > EFmin && dotA < EFmax)
        {
            var d = PointDistance(A, E, F, reverse);
            if (d != null && AlmostEqual(d.Value, 0))
            {
                //  A currently touches EF, but AB is moving away from EF
                var dB = PointDistance(B, E, F, reverse, true);
                if (dB < 0 || AlmostEqual(dB.Value * overlap, 0))
                {
                    d = null;
                }
            }

            if (d != null)
            {
                distances.Add(d.Value);
            }
        }

        if (AlmostEqual(dotB, dotE))
        {
            distances.Add(crossB - crossE);
        }
        else if (AlmostEqual(dotB, dotF))
        {
            distances.Add(crossB - crossF);
        }
        else if (dotB > EFmin && dotB < EFmax)
        {
            var d = PointDistance(B, E, F, reverse);

            if (d != null && AlmostEqual(d.Value, 0))
            {
                // crossA>crossB A currently touches EF, but AB is moving away from EF
                var dA = PointDistance(A, E, F, reverse, true);
                if (dA < 0 || AlmostEqual(dA.Value * overlap, 0))
                {
                    d = null;
                }
            }

            if (d != null)
            {
                distances.Add(d.Value);
            }
        }

        if (dotE > ABmin && dotE < ABmax)
        {
            var d = PointDistance(E, A, B, direction);
            if (d != null && AlmostEqual(d.Value, 0))
            {
                // crossF<crossE A currently touches EF, but AB is moving away from EF
                var dF = PointDistance(F, A, B, direction, true);
                if (dF < 0 || AlmostEqual(dF.Value * overlap, 0))
                {
                    d = null;
                }
            }

            if (d != null)
            {
                distances.Add(d.Value);
            }
        }

        if (dotF > ABmin && dotF < ABmax)
        {
            var d = PointDistance(F, A, B, direction);
            if (d != null && AlmostEqual(d.Value, 0))
            {
                // && crossE<crossF A currently touches EF, but AB is moving away from EF
                var dE = PointDistance(E, A, B, direction, true);
                if (dE < 0 || AlmostEqual(dE.Value * overlap, 0))
                {
                    d = null;
                }
            }

            if (d != null)
            {
                distances.Add(d.Value);
            }
        }

        if (distances.Count == 0)
        {
            return null;
        }

        //TODO: Improve
        return distances.Min();
    }

    public static bool SegmentsIntersect(PointF l1p1, PointF l1p2, PointF l2p1, PointF l2p2)
    {
        var dx1 = l1p2.X - l1p1.X;
        var dy1 = l1p2.Y - l1p1.Y;
        var dx2 = l2p2.X - l2p1.X;
        var dy2 = l2p2.Y - l2p1.Y;
        var delta = dx2 * dy1 - dy2 * dx1;
        if (delta == 0)
        {
            return false;
        }

        var s = (dx1 * (l2p1.Y - l1p1.Y) + dy1 * (l1p1.X - l2p1.X)) / delta;
        var t = (dx2 * (l1p1.Y - l2p1.Y) + dy2 * (l2p1.X - l1p1.X)) / -delta;
        return s is >= 0 and <= 1 && t is >= 0 and <= 1;
    }

    // returns true if points are within the given distance
    public static bool WithinDistance(PointF p1, PointF p2, double distance)
    {
        var dx = p1.X - p2.X;
        var dy = p1.Y - p2.Y;
        return dx * dx + dy * dy < distance * distance;
    }

    // return true if point is in the polygon, false if outside, and null if exactly on a point or edge
    //public static bool? PointInPolygon(IList<PointF> polygonPoints, PointF? polygonOffset, PointF point, IEnumerable<PointF[]> polygonHoles = null)
    //{
    //    if (polygonPoints.Count < 3)
    //    {
    //        return null;
    //    }

    //    var inside = false;
    //    var offsetx = polygonOffset == null ? 0 : polygonOffset.Value.X;
    //    var offsety = polygonOffset == null ? 0 : polygonOffset.Value.Y;

    //    for (int i = 0, j = polygonPoints.Count - 1; i < polygonPoints.Count; j = i++)
    //    {
    //        var xi = polygonPoints[i].X + offsetx;
    //        var yi = polygonPoints[i].Y + offsety;
    //        var xj = polygonPoints[j].X + offsetx;
    //        var yj = polygonPoints[j].Y + offsety;

    //        if (AlmostEqual(xi, point.X) && AlmostEqual(yi, point.Y))
    //        {
    //            return null; // no result
    //        }

    //        if (OnSegment(new PointF(xi, yi), new PointF(xj, yj), point))
    //        {
    //            return null; // exactly on the segment
    //        }
    //// returns an interior NFP for the special case where A is a rectangle
    //public static RectangleF? NoFitPolygonRectangle(IList<PointF> A, IList<PointF> B)
    //{
    //    var abounds = GeometryHelper.CalculateBounds(A);
    //    var bbounds = GeometryHelper.CalculateBounds(B);
    //    if (bbounds.XMax - bbounds.XMin > abounds.XMax - abounds.XMin)
    //    {
    //        return null;
    //    }
    //    if (bbounds.YMax - bbounds.YMin > abounds.YMax - abounds.YMin)
    //    {
    //        return null;
    //    }
    //    return new RectangleF(new PointF(abounds.XMin - bbounds.XMin + B[0].X, abounds.YMin - bbounds.YMin + B[0].Y),
    //    new PointF(abounds.XMax - bbounds.XMax + B[0].X, abounds.YMin - bbounds.YMin + B[0].Y),
    //    new PointF(abounds.XMax - bbounds.XMax + B[0].X, abounds.YMax - bbounds.YMax + B[0].Y),
    //    new PointF(abounds.XMin - abounds.XMin + B[0].X, abounds.YMax - bbounds.YMax + B[0].Y)
    //    );
    //}

    //// given a static polygon A and a movable polygon B, compute a no fit polygon by orbiting B about A
    //// if the inside flag is set, B is orbited inside of A rather than outside
    //// if the searchEdges flag is set, all edges of A are explored for NFPs - multiple
    //public static List<List<PointF>> NoFitPolygon(IList<PointF> A, PointF? aOffset, IList<PointF> B, PointF? bOffset, bool inside, bool searchEdges)
    //{
    //    var Aoffsetx = aOffset == null ? 0 : aOffset.Value.X;
    //    var Aoffsety = aOffset == null ? 0 : aOffset.Value.Y;
    //    var Boffsetx = bOffset == null ? 0 : bOffset.Value.X;
    //    var Boffsety = bOffset == null ? 0 : bOffset.Value.Y;

    //    if (A == null || A.Count < 3 || B == null || B.Count < 3)
    //    {
    //        return null;
    //    }

    //    A.offsetx = 0;
    //    A.offsety = 0;

    //    //var i, j;

    //    var minA = A[0].Y;
    //    var minAindex = 0;

    //    var maxB = B[0].Y;
    //    var maxBindex = 0;
    //    var marked = new HashSet<PointF>();
    //    for (int i = 1; i < A.Count; i++)
    //    {
    //        //A[i].marked = false;
    //        if (A[i].Y < minA)
    //        {
    //            minA = A[i].Y;
    //            minAindex = i;
    //        }
    //    }

    //    for (int i = 1; i < B.Count; i++)
    //    {
    //        //B[i].marked = false;
    //        if (B[i].Y > maxB)
    //        {
    //            maxB = B[i].Y;
    //            maxBindex = i;
    //        }
    //    }
    //    PointF? startpoint = null;
    //    if (!inside)
    //    {
    //        // shift B such that the bottom-most point of B is at the top-most point of A. This guarantees an initial placement with no intersections
    //        startpoint = new PointF(A[minAindex].X - B[maxBindex].X, A[minAindex].Y - B[maxBindex].Y);

    //    }
    //    else
    //    {
    //        // no reliable heuristic for inside
    //        startpoint = SearchStartPoint(A, B, true, null);
    //    }

    //    var NFPlist = new List<List<PointF>>();

    //    while (startpoint != null)
    //    {
    //        B.offsetx = startpoint.Value.X;
    //        B.offsety = startpoint.Value.Y;

    //        // maintain a list of touching points/edges
    //        var touching = new List<Touching>();

    //        V prevvector = null; // keep track of previous vector
    //        var NFP = new List<PointF>();
    //        NFP.Add(new PointF(B[0].X + B.offsetx.Value, B[0].Y + B.offsety.Value));

    //        var referencex = B[0].X + B.offsetx;
    //        var referencey = B[0].Y + B.offsety;
    //        var startx = referencex;
    //        var starty = referencey;
    //        var counter = 0;

    //        while (counter < 10 * (A.Count + B.Count))
    //        { // sanity check, prevent infinite loop
    //          // find touching vertices/edges
    //            for (int i = 0; i < A.Count; i++)
    //            {
    //                var nexti = (i == A.Count - 1) ? 0 : i + 1;
    //                for (int j = 0; j < B.Count; j++)
    //                {
    //                    var nextj = (j == B.Count - 1) ? 0 : j + 1;
    //                    if (GeometryUtil.AlmostEqual(A[i].X, B[j].X + B.offsetx.Value) && GeometryUtil.AlmostEqual(A[i].Y, B[j].Y + B.offsety.Value))
    //                    {
    //                        touching.Add(new Touching { type = 0, A = i, B = j });
    //                    }
    //                    else if (GeometryUtil.OnSegment(A[i], A[nexti], new PointF(B[j].X + B.offsetx.Value, y: B[j].Y + B.offsety.Value)))
    //                    {
    //                        touching.Add(new Touching { type = 1, A = nexti, B = j });
    //                    }
    //                    else if (GeometryUtil.OnSegment(new PointF(B[j].X + B.offsetx.Value, y: B[j].Y + B.offsety.Value), new PointF(B[nextj].X + B.offsetx.Value, B[nextj].Y + B.offsety.Value), A[i]))
    //                    {
    //                        touching.Add(new Touching { type = 2, A = i, B = nextj });
    //                    }
    //                }
    //            }

    //            // generate translation vectors from touching vertices/edges
    //            var vectors = new List<V>();
    //            for (int i = 0; i < touching.Count; i++)
    //            {
    //                var vertexA = A[touching[i].A];
    //                marked.Add(vertexA);
    //                //vertexA.marked = true;

    //                // adjacent A vertices
    //                var prevAindex = touching[i].A - 1;
    //                var nextAindex = touching[i].A + 1;

    //                prevAindex = (prevAindex < 0) ? A.Count - 1 : prevAindex; // loop
    //                nextAindex = (nextAindex >= A.Count) ? 0 : nextAindex; // loop

    //                var prevA = A[prevAindex];
    //                var nextA = A[nextAindex];

    //                // adjacent B vertices
    //                var vertexB = B[touching[i].B];

    //                var prevBindex = touching[i].B - 1;
    //                var nextBindex = touching[i].B + 1;

    //                prevBindex = (prevBindex < 0) ? B.Count - 1 : prevBindex; // loop
    //                nextBindex = (nextBindex >= B.Count) ? 0 : nextBindex; // loop

    //                var prevB = B[prevBindex];
    //                var nextB = B[nextBindex];

    //                if (touching[i].type == 0)
    //                {
    //                    var vA1 = new V
    //                    {
    //                        x = prevA.X - vertexA.X,
    //                        y = prevA.Y - vertexA.Y,
    //                        start = vertexA,
    //                        end = prevA
    //                    };

    //                    var vA2 = new V
    //                    {
    //                        x = nextA.X - vertexA.X,
    //                        y = nextA.Y - vertexA.Y,
    //                        start = vertexA,
    //                        end = nextA
    //                    };

    //                    // B vectors need to be inverted
    //                    var vB1 = new V
    //                    {
    //                        x = vertexB.X - prevB.X,
    //                        y = vertexB.Y - prevB.Y,
    //                        start = prevB,
    //                        end = vertexB
    //                    };

    //                    var vB2 = new V
    //                    {
    //                        x = vertexB.X - nextB.X,
    //                        y = vertexB.Y - nextB.Y,
    //                        start = nextB,
    //                        end = vertexB
    //                    };

    //                    vectors.Add(vA1);
    //                    vectors.Add(vA2);
    //                    vectors.Add(vB1);
    //                    vectors.Add(vB2);
    //                }
    //                else if (touching[i].type == 1)
    //                {
    //                    vectors.Add(new V
    //                    {
    //                        x = vertexA.X - (vertexB.X + B.offsetx.Value),
    //                        y = vertexA.Y - (vertexB.Y + B.offsety.Value),
    //                        start = prevA,
    //                        end = vertexA
    //                    });

    //                    vectors.Add(new V
    //                    {
    //                        x = prevA.X - (vertexB.X + B.offsetx.Value),
    //                        y = prevA.Y - (vertexB.Y + B.offsety.Value),
    //                        start = vertexA,
    //                        end = prevA
    //                    });
    //                }
    //                else if (touching[i].type == 2)
    //                {
    //                    vectors.Add(new V
    //                    {
    //                        x = vertexA.X - (vertexB.X + B.offsetx.Value),
    //                        y = vertexA.Y - (vertexB.Y + B.offsety.Value),
    //                        start = prevB,
    //                        end = vertexB
    //                    });

    //                    vectors.Add(new V
    //                    {
    //                        x = vertexA.X - (prevB.X + B.offsetx.Value),
    //                        y = vertexA.Y - (prevB.Y + B.offsety.Value),
    //                        start = vertexB,
    //                        end = prevB
    //                    });
    //                }
    //            }

    //            // todo: there should be a faster way to reject vectors that will cause immediate intersection. For now just check them all

    //            V translate = null;
    //            double maxd = 0;

    //            for (int i = 0; i < vectors.Count; i++)
    //            {
    //                if (vectors[i].x == 0 && vectors[i].y == 0)
    //                {
    //                    continue;
    //                }

    //                // if this vector points us back to where we came from, ignore it.
    //                // ie cross product = 0, dot product < 0
    //                if (prevvector != null && vectors[i].y * prevvector.y + vectors[i].x * prevvector.x < 0)
    //                {
    //                    // compare magnitude with unit vectors
    //                    var vectorlength = Math.Sqrt(vectors[i].x * vectors[i].x + vectors[i].y * vectors[i].y);
    //                    var unitv = new PointF((float)(vectors[i].x / vectorlength), (float)(vectors[i].y / vectorlength));

    //                    var prevlength = Math.Sqrt(prevvector.x * prevvector.x + prevvector.y * prevvector.y);
    //                    var prevunit = new PointF((float)(prevvector.x / prevlength), (float)(prevvector.y / prevlength));

    //                    // we need to scale down to unit vectors to normalize vector length. Could also just do a tan here
    //                    if (Math.Abs(unitv.Y * prevunit.X - unitv.X * prevunit.Y) < 0.0001)
    //                    {
    //                        continue;
    //                    }
    //                }

    //                var d = PolygonSlideDistance(A, B, new PointF(vectors[i].x, vectors[i].y), true);
    //                var vecd2 = vectors[i].x * vectors[i].x + vectors[i].y * vectors[i].y;

    //                if (d == null || d * d > vecd2)
    //                {
    //                    var vecd = Math.Sqrt(vectors[i].x * vectors[i].x + vectors[i].y * vectors[i].y);
    //                    d = vecd;
    //                }

    //                if (d != null && d > maxd)
    //                {
    //                    maxd = d.Value;
    //                    translate = vectors[i];
    //                }
    //            }

    //            if (translate == null || GeometryUtil.AlmostEqual(maxd, 0))
    //            {
    //                // didn't close the loop, something went wrong here
    //                NFP = null;
    //                break;
    //            }
    //            marked.Add(translate.start);
    //            marked.Add(translate.end);
    //            //translate.start.marked = true;
    //            //translate.end.marked = true;

    //            prevvector = translate;

    //            // trim
    //            var vlength2 = translate.x * translate.x + translate.y * translate.y;
    //            if (maxd * maxd < vlength2 && !GeometryUtil.AlmostEqual(maxd * maxd, vlength2))
    //            {
    //                var scale = Math.Sqrt((maxd * maxd) / vlength2);
    //                translate.x = (float)(translate.x * scale);
    //                translate.y = (float)(translate.y * scale);
    //            }

    //            referencex += translate.x;
    //            referencey += translate.y;

    //            if (GeometryUtil.AlmostEqual(referencex.Value, startx.Value) && GeometryUtil.AlmostEqual(referencey.Value, starty.Value))
    //            {
    //                // we've made a full loop
    //                break;
    //            }

    //            // if A and B start on a touching horizontal line, the end point may not be the start point
    //            var looped = false;
    //            if (NFP.Count > 0)
    //            {
    //                for (int i = 0; i < NFP.Count - 1; i++)
    //                {
    //                    if (GeometryUtil.AlmostEqual(referencex.Value, NFP[i].X) && GeometryUtil.AlmostEqual(referencey.Value, NFP[i].Y))
    //                    {
    //                        looped = true;
    //                    }
    //                }
    //            }

    //            if (looped)
    //            {
    //                // we've made a full loop
    //                break;
    //            }

    //            NFP.Add(new PointF(referencex.Value, referencey.Value));

    //            B.offsetx += translate.x;
    //            B.offsety += translate.y;

    //            counter++;
    //        }

    //        if (NFP != null && NFP.Count > 0)
    //        {
    //            NFPlist.Add(NFP);
    //        }

    //        if (!searchEdges)
    //        {
    //            // only get outer NFP or first inner NFP
    //            break;
    //        }

    //        startpoint = SearchStartPoint(A, B, inside, NFPlist);

    //    }

    //    return NFPlist;
    //}

    //// searches for an arrangement of A and B such that they do not overlap
    //// if an NFP is given, only search for startpoints that have not already been traversed in the given NFP
    //public static PointF? SearchStartPoint(IList<PointF> A, PointF? aOffset, IList<PointF> B, PointF? bOffset, bool inside, IList<List<PointF>> NFP)
    //{
    //      var Aoffsetx = aOffset == null ? 0 : aOffset.Value.X;
    //        var Aoffsety = aOffset == null ? 0 : aOffset.Value.Y;
    //        var Boffsetx = bOffset == null ? 0 : bOffset.Value.X;
    //        var Boffsety = bOffset == null ? 0 : bOffset.Value.Y;
    //    // clone arrays
    //    A = GeneralHelper.CloneList(A);
    //    B = GeneralHelper.CloneList(B);

    //    // close the loop for polygons
    //    if (A[0] != A[A.Count - 1])
    //    {
    //        A.Add(A[0]);
    //    }

    //    if (B[0] != B[B.Count - 1])
    //    {
    //        B.Add(B[0]);
    //    }
    //    var marked = new HashSet<PointF>();
    //    for (var i = 0; i < A.Count - 1; i++)
    //    {
    //        if (!marked.Contains(A[i]))
    //        {
    //            marked.Add(A[i]);
    //            for (var j = 0; j < B.Count; j++)
    //            {
    //                B.offsetx = A[i].X - B[j].X;
    //                B.offsety = A[i].Y - B[j].Y;

    //                bool? Binside = null;
    //                for (var k = 0; k < B.Count; k++)
    //                {
    //                    var inpoly = A.PointInPolygon(new PointF(B[k].X + B.offsetx.Value, B[k].Y + B.offsety.Value));
    //                    if (inpoly != null)
    //                    {
    //                        Binside = inpoly;
    //                        break;
    //                    }
    //                }

    //                if (Binside == null)
    //                { // A and B are the same
    //                    return null;
    //                }

    //                var startPoint = new PointF(B.offsetx.Value, B.offsety.Value);
    //                if (((Binside.Value && inside) || (!Binside.Value && !inside)) && !Intersect(A, B) && !InNfp(startPoint, NFP))
    //                {
    //                    return startPoint;
    //                }

    //                // slide B along vector
    //                float vx = A[i + 1].X - A[i].X;
    //                float vy = A[i + 1].Y - A[i].Y;

    //                var d1 = PolygonProjectionDistance(A, B, new PointF(vx, vy));
    //                var d2 = PolygonProjectionDistance(B, A, new PointF(-vx, -vy));

    //                double? d = null;

    //                // todo: clean this up
    //                if (d1 == null && d2 == null)
    //                {
    //                    // nothin
    //                }
    //                else if (d1 == null)
    //                {
    //                    d = d2;
    //                }
    //                else if (d2 == null)
    //                {
    //                    d = d1;
    //                }
    //                else
    //                {
    //                    d = Math.Min(d1.Value, d2.Value);
    //                }

    //                // only slide until no longer negative
    //                // todo: clean this up
    //                if (d != null && !GeometryUtil.AlmostEqual(d.Value, 0) && d > 0)
    //                {
    //                }
    //                else
    //                {
    //                    continue;
    //                }

    //                var vd2 = vx * vx + vy * vy;

    //                if (d * d < vd2 && !GeometryUtil.AlmostEqual(d.Value * d.Value, vd2))
    //                {
    //                    var vd = Math.Sqrt(vx * vx + vy * vy);
    //                    vx = (float)(vx * d.Value / vd);
    //                    vy = (float)(vy * d.Value / vd);
    //                }

    //                B.offsetx += (float)vx;
    //                B.offsety += (float)vy;

    //                for (int k = 0; k < B.Count; k++)
    //                {
    //                    var inpoly = A.PointInPolygon(new PointF(B[k].X + B.offsetx.Value, B[k].Y + B.offsety.Value));
    //                    if (inpoly != null)
    //                    {
    //                        Binside = inpoly;
    //                        break;
    //                    }
    //                }
    //                startPoint = new PointF(B.offsetx.Value, B.offsety.Value);
    //                if (((Binside.Value && inside) || (!Binside.Value && !inside)) && !Intersect(A, B) && !InNfp(startPoint, NFP))
    //                {
    //                    return startPoint;
    //                }
    //            }
    //        }
    //    }
    //    return null;
    //}
    //private static bool InNfp(PointF p, IList<List<PointF>> nfp)
    //{
    //    if (nfp != null || nfp.Count == 0)
    //    {
    //        return false;
    //    }
    //public static PointF GetLineIntersection(PointF p1, PointF q1, PointF p2, PointF q2)
    //{
    //    var eps = 1e-9;
    //    var dx1 = p1.X - q1.X;
    //    var dy1 = p1.Y - q1.Y;
    //    var dx2 = p2.X - q2.X;
    //    var dy2 = p2.Y - q2.Y;
    //    var denom = dx1 * dy2 - dy1 * dx2;
    //    if (Math.Abs(denom) < eps)
    //    {
    //        return null;
    //    }
    //    var cross1 = p1.X * q1.Y - p1.Y * q1.X;
    //    var cross2 = p2.X * q2.Y - p2.Y * q2.X;
    //    var px = (cross1 * dx2 - cross2 * dx1) / denom;
    //    var py = (cross1 * dy2 - cross2 * dy1) / denom;
    //    return new PointF(px, py);
    //}
    //public static bool IsPointInPolygon(PointF coordinate, IGeometry polygon)
    //{
    //    var point = polygon.Factory.CreatePoint(coordinate);
    //    return polygon.Contains(point);
    //}
    //public static PointF[] ScalePoints(PointF[] points, PointF centroid, double scale)
    //{
    //    var newPoints = new PointF[points.Length];
    //    for (int i = 0; i < points.Length; i++)
    //    {
    //        var point = points[i];
    //        var x = centroid.X + ((point.X - centroid.X) * scale);

    //        var y = centroid.Y + ((point.Y - centroid.Y) * scale);
    /// <summary>
    ///     Returns true if given point(x,y) is inside the given line segment
    /// </summary>
    /// <param name="line"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private static bool IsInsideLine(PointF lineP1, PointF lineP2, double x, double y)
    {
        return ((x >= lineP1.X && x <= lineP2.X)
                || (x >= lineP2.X && x <= lineP1.X))
               && ((y >= lineP1.Y && y <= lineP2.Y)
                   || (y >= lineP2.Y && y <= lineP1.Y));
    }
}