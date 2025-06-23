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
        public SvgToImageExporter()
            :this(@"C:\Program Files\Google\Chrome\Application\chrome.exe")
        {

        }
        public SvgToImageExporter(string chromeExecutablePath)
        {
            this.browserLaunchOptions = new BrowserTypeLaunchOptions()
            {
                ExecutablePath = chromeExecutablePath
            };
        }
        public async Task ExportSvgAsImageAsync(Document document, int width, int height, string outputPath)
        {
            var data = await ExportSvgAsImageAsync(document, width, height);
            await File.WriteAllBytesAsync(outputPath, data);
        }
        public async Task<byte[]> ExportSvgAsImageAsync(Document document, int width, int height)
        {
            var svgContent = document.GetDocumentAsString();
            var tempFileName = Path.GetTempFileName() + ".svg";
            try
            {
                await File.WriteAllTextAsync(tempFileName, svgContent);
                return await ExportSvgAsImageAsync(tempFileName, width, height);
            }
            finally
            {
                File.Delete(tempFileName);
            }
        }

        public async Task<byte[]> ExportSvgAsImageAsync(string svgFileName, int width, int height)
        {
            if (this.playwright == null)
            {
                this.playwright = await Playwright.CreateAsync();
            }
            if (this.browser == null)
            {
                this.browser = await playwright.Chromium.LaunchAsync(this.browserLaunchOptions);
            }
            var page = await this.browser.NewPageAsync();

            await page.SetViewportSizeAsync(width, height);
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
