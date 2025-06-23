using Microsoft.Playwright;
using VectSharp;

namespace SubhadraSolutions.Utils.SVG
{
    public static class SvgExtensions
    {
        //public static string GetDocumentAsString(this Document doc)
        //{
        //    var xmlDoc = doc.Pages[0].SaveAsSVG(textOption: SVGContextInterpreter.TextOptions.DoNotEmbed, useStyles: false);
        //    //var xmlDoc = doc.Pages[0].SaveAsSVG();
        //    var attribute = xmlDoc.DocumentElement.Attributes["viewBox"];
        //    if (attribute != null)
        //    {
        //        var ints = attribute.Value.Split(' ').Select(x => Convert.ToInt32(x)).ToList();
        //        var widthAttribute = xmlDoc.CreateAttribute("width");
        //        widthAttribute.Value = (ints[2] - ints[0]).ToString();

        //        var heightAttribute = xmlDoc.CreateAttribute("height");
        //        heightAttribute.Value = (ints[3] - ints[1]).ToString();

        //        xmlDoc.DocumentElement.Attributes.Append(widthAttribute);
        //        xmlDoc.DocumentElement.Attributes.Append(heightAttribute);
        //        //xmlDoc.DocumentElement.Attributes.Remove(attribute);
        //    }
        //    return xmlDoc.LastChild.OuterXml;
        //}

        public static async Task ExportSvgAsImageAsync(this Document document, int width, int height, string outputPath)
        {
            var svgContent = document.GetDocumentAsString();
            var tempFileName = Path.GetTempFileName();
            await File.WriteAllTextAsync(tempFileName, svgContent);
            try
            {
                //await Playwright.CreateAsync();
                using var playwright = await Playwright.CreateAsync();
                var browserLaunchOptions = new BrowserTypeLaunchOptions()
                {
                    ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
                };
                await using var browser = await playwright.Chromium.LaunchAsync(browserLaunchOptions);
                var page = await browser.NewPageAsync();

                await page.SetViewportSizeAsync(width, height);
                await page.GotoAsync(Path.GetFullPath(tempFileName));

                var item = await page.QuerySelectorAsync("svg");
                var options = new ElementHandleScreenshotOptions
                {
                    OmitBackground = true
                };
                var data = await item.ScreenshotAsync(options);
                await File.WriteAllBytesAsync(outputPath, data);
            }
            finally
            {
                File.Delete(tempFileName);
            }
        }

        //public static void ExportSvgAsImage(this Document document, string outputPath, string fontFilePath)
        //{
        //    var svgContent = document.GetDocumentAsString();
        //    // Load the custom font
        //    var typeface = SKTypeface.FromFile(fontFilePath);
        //    if (typeface == null)
        //    {
        //        Console.WriteLine("Failed to load font.");
        //        return;
        //    }

        //    // Register the font with Skia
        //    var fontManager = SKFontManager.Default;

        //    // Load the SVG
        //    var svg = new SKSvg();
        //    using (var stream = new MemoryStream())
        //    {
        //        var writer = new StreamWriter(stream);
        //        writer.Write(svgContent);
        //        writer.Flush();
        //        stream.Position = 0;
        //        svg.Load(stream);
        //    }

        //    var picture = svg.Picture;
        //    var width = (int)svg.Picture.CullRect.Width;
        //    var height = (int)svg.Picture.CullRect.Height;

        //    // Create image surface
        //    using var bitmap = new SKBitmap(width, height);
        //    using var canvas = new SKCanvas(bitmap);
        //    canvas.Clear(SKColors.White);

        //    SKFont font = new SKFont()
        //    {
        //        Typeface = typeface,
        //    };
        //    var paint = new SKPaint(font)
        //    {
        //        IsAntialias = true,
        //    };

        //    // Draw the SVG onto the canvas
        //    canvas.DrawPicture(picture, paint);
        //    canvas.Flush();

        //    // Save as PNG
        //    using var image = SKImage.FromBitmap(bitmap);
        //    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        //    using var fileStream = File.OpenWrite(outputPath);
        //    data.SaveTo(fileStream);

        //    Console.WriteLine("SVG exported as image with custom font.");
        //}
    }
}