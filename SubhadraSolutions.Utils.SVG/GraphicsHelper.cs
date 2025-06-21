using System;
using System.Collections.Generic;
using System.Globalization;
using VectSharp;

namespace SubhadraSolutions.Utils.SVG
{
    public class GraphicsHelper
    {
        public static ExtendedGraphics BuildCircle(double centerX, double centerY, double radius, Brush brush)
        {
            var path = new GraphicsPath();
            //path = path.MoveTo(centerX, centerY);
            path = path.Arc(centerX, centerY, radius, 0, Math.PI * 2);
            var g = new ExtendedGraphics();
            g.StrokePath(path, brush);
            return g;
        }

        public static ExtendedGraphics BuildLine(double xFrom, double yFrom, double xTo, double yTo, Brush brush)
        {
            var path = new GraphicsPath();
            path = path.MoveTo(xFrom, yFrom);
            path = path.LineTo(xTo, yTo);
            var g = new ExtendedGraphics();
            g.StrokePath(path, brush);
            return g;
        }

        public static ExtendedGraphics BuildPolygon(System.Drawing.PointF[] points, Brush brush)
        {
            var lines = new List<ExtendedGraphics>();
            for (int i = 0; i < points.Length; i++)
            {
                var p = points[i];
                var np = points[(i + 1) % points.Length];
                var line = BuildLine(p.X, p.Y, np.X, np.Y, brush);
                lines.Add(line);
            }
            return Merge(lines);
        }

        public static ExtendedGraphics BuildRectangle(double x, double y, double width, double height, Brush brush)
        {
            var g = new ExtendedGraphics();
            g.StrokeRectangle(x, y, width, height, brush);
            return g;
        }

        //public static ExtendedGraphics BuildText(double x, double y, string text, Font font, Brush brush)
        //{
        //    var g = new ExtendedGraphics();
        //    var splits = text.Split('\n');
        //    for (int i = 0; i < splits.Length; i++)
        //    {
        //        var split = splits[i];
        //        var size = g.MeasureText(split, font);
        //        var newX = x - size.Width / 2;
        //        var newY = y - size.Height / 2;
        //        newY -= ((splits.Length / 2.0) - i) * (size.Height * 1.2);
        //        g.FillText(newX, newY, split, font, brush);
        //    }
        //    return g;
        //}
        public static ExtendedGraphics BuildText(double x, double y, string text, Font font, Brush brush)
        {
            var g = new ExtendedGraphics();
            var gp = new GraphicsPath();
            var size = g.MeasureText(text, font);
            var newX = x - size.Width / 2;
            var newY = y - size.Height / 2;
            g.FillText(newX, newY, text, font, brush);
            //var path = SVGHelper.ConvertTextToPathSvg(text, font.FontFamily.FamilyName, (float)font.FontSize);
            //var svgPath= ParseSvgPath(path);
            //g.StrokePath(svgPath, brush);

            return g;
        }

        //public static ExtendedGraphics BuildText(double x, double y, string text, Font font, Brush brush)
        //{
        //    var xx = BuildText(text, font, brush);

        //    var g = new ExtendedGraphics();
        //    var size = xx.GetBounds().Size;
        //    var newX = x - size.Width / 2;
        //    var newY = y - size.Height / 2;
        //    g.FillText(newX, newY, text, font, brush);
        //    return g;
        //}

        public static ExtendedGraphics BuildText(string text, Font font, Brush brush)
        {
            //SvgElement e = new Svg.SvgText(text);
            //e.FontFamily = font.FontFamily.FamilyName;
            var g = new ExtendedGraphics();
            g.FillText(0, 0, text, font, brush);
            return g;
        }

        public static ExtendedGraphics Merge(IEnumerable<ExtendedGraphics> graphics)
        {
            var group = new ExtendedGraphics();
            foreach (var graphicsItem in graphics)
            {
                group.DrawGraphics(0, 0, graphicsItem);
            }
            return group;
        }

        public static GraphicsPath ParseSvgPath(string d)
        {
            var path = new GraphicsPath();
            string[] tokens = d.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            Point current = new Point(0, 0);

            while (i < tokens.Length)
            {
                string cmd = tokens[i++];

                switch (cmd)
                {
                    case "M":
                        current = ReadPoint(tokens, ref i);
                        path.MoveTo(current.X, current.Y);
                        break;

                    case "L":
                        current = ReadPoint(tokens, ref i);
                        path.LineTo(current.X, current.Y);
                        break;

                    case "C":
                        var p1 = ReadPoint(tokens, ref i);
                        var p2 = ReadPoint(tokens, ref i);
                        var p3 = ReadPoint(tokens, ref i);
                        path.CubicBezierTo(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);
                        current = p3;
                        break;

                    case "Z":
                    case "z":
                        path.Close();
                        break;

                    default:
                        throw new Exception($"Unsupported SVG command: {cmd}");
                }
            }

            return path;
        }

        private static Point ReadPoint(string[] tokens, ref int index)
        {
            double x = double.Parse(tokens[index++], CultureInfo.InvariantCulture);
            double y = double.Parse(tokens[index++], CultureInfo.InvariantCulture);
            return new Point(x, y);
        }
    }
}