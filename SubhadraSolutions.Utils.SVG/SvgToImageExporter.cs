using Microsoft.Playwright;
using SubhadraSolutions.Utils.Abstractions;
using VectSharp;

namespace SubhadraSolutions.Utils.SVG
{
    public class SvgToImageExporter : AbstractDisposable
    {
        private readonly BrowserTypeLaunchOptions browserLaunchOptions;
        private IPlaywright playwright;
        private IBrowser browser;
        private IPage page;
        private readonly int width;
        private readonly int height;
        public SvgToImageExporter(int width, int height)
            :this(width, height, @"C:\Program Files\Google\Chrome\Application\chrome.exe")
        {

        }
        public SvgToImageExporter(int width, int height, string chromeExecutablePath)
        {
            this.width = width;
            this.height = height;
            this.browserLaunchOptions = new BrowserTypeLaunchOptions()
            {
                ExecutablePath = chromeExecutablePath
            };
        }
        public async Task ExportSvgAsImageAsync(Document document,string outputPath)
        {
            var data = await ExportSvgAsImageAsync(document);
            await File.WriteAllBytesAsync(outputPath, data);
        }
        public async Task<byte[]> ExportSvgAsImageAsync(Document document)
        {
            var svgContent = document.GetDocumentAsString();
            var tempFileName = Path.GetTempFileName() + ".svg";
            try
            {
                await File.WriteAllTextAsync(tempFileName, svgContent);
                return await ExportSvgAsImageAsync(tempFileName);
            }
            finally
            {
                File.Delete(tempFileName);
            }
        }

        public async Task<byte[]> ExportSvgAsImageAsync(string svgFileName)
        {
            if (this.playwright == null)
            {
                this.playwright = await Playwright.CreateAsync();
            }
            if (this.browser == null)
            {
                this.browser = await playwright.Chromium.LaunchAsync(this.browserLaunchOptions);
                this.page = await this.browser.NewPageAsync();
                await this.page.SetViewportSizeAsync(this.width, this.height);
            }
            await page.GotoAsync(Path.GetFullPath(svgFileName));

            var item = await page.QuerySelectorAsync("svg");
            var options = new ElementHandleScreenshotOptions
            {
                OmitBackground = true
            };
            var data = await item.ScreenshotAsync(options);
            return data;
        }
        protected override void Dispose(bool disposing)
        {
            if (this.browser != null)
            {
                this.browser.DisposeAsync();
            }
            if(this.playwright != null)
            {
                this.playwright.Dispose();
            }
        }
    }
}
