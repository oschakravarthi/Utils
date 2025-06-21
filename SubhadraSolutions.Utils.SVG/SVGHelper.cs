//using SkiaSharp;
//using Svg.Skia;
//using System;
//using System.IO;

//namespace SubhadraSolutions.Utils.SVG
//{
//    public static class SVGHelper
//    {
//        public static string ConvertTextToPathSvg(string text, string fontFamily = "Arial", float fontSize = 72f)
//        {
//            var skTypeface = SKTypeface.FromFamilyName(fontFamily);

//            var skFont = new SKFont(skTypeface);
//            // Measure text path
//            using var path = skFont.GetTextPath(text);
//            return path.ToSvgPathData();
//            // Convert to SVG path data
//            //using var stream = new MemoryStream();
//            //using (var writer = new StreamWriter(stream))
//            //{
//            //    writer.Write("<svg xmlns=\"http://www.w3.org/2000/svg\">\n");
//            //    writer.Write($"<path d=\"{path.ToSvgPathData()}\" fill=\"black\"/>\n");
//            //    writer.Write("</svg>");
//            //    writer.Flush();
//            //    stream.Position = 0;

//            //    using var reader = new StreamReader(stream);
//            //    return reader.ReadToEnd();
//            //}
//        }

//    }
//}
