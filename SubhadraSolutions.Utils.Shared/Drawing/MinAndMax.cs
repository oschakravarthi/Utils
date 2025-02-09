using System;
using System.Collections.Generic;
using System.Drawing;

namespace SubhadraSolutions.Utils.Drawing;

public class MinAndMax
{
    public PointF Center => new(XMin + Width / 2, YMin + Height / 2);

    public float Height => Math.Abs(YMax - YMin);

    public float Width => Math.Abs(XMax - XMin);

    public float XMax { get; private set; } = float.MinValue;

    public float XMin { get; private set; } = float.MaxValue;

    public float YMax { get; private set; } = float.MinValue;

    public float YMin { get; private set; } = float.MaxValue;

    public static MinAndMax Build(IEnumerable<PointF> points)
    {
        var minmax = new MinAndMax();
        minmax.AddPoints(points);
        return minmax;
    }

    public void AddPoint(PointF point)
    {
        if (point.X < XMin)
        {
            XMin = point.X;
        }

        if (point.X > XMax)
        {
            XMax = point.X;
        }

        if (point.Y < YMin)
        {
            YMin = point.Y;
        }

        if (point.Y > YMax)
        {
            YMax = point.Y;
        }
    }

    public void AddPoints(IEnumerable<PointF> points)
    {
        foreach (var p in points)
        {
            AddPoint(p);
        }
    }

    public PointF[] GetCorners()
    {
        return [new(XMin, YMin), new(XMax, YMin), new(XMax, YMax), new(XMin, YMax)];
    }

    public RectangleF GetRectangle()
    {
        return new RectangleF(XMin, YMin, Width, Height);
    }
}