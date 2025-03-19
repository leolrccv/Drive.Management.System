using Application.Contracts;
using Application.Extensions;
using Application.Models;
using Markdig;
using Microsoft.AspNetCore.Http;
using PuppeteerSharp;

namespace Application.Converters.FromMd;

public class MdToPdfConverter : IFileConverter
{
    public async Task<FileModel> ConvertAsync(IFormFile file)
    {
        using var streamReader = new StreamReader(file.OpenReadStream());
        var markdownContent = await streamReader.ReadToEndAsync();

        string htmlContent = Markdown.ToHtml(markdownContent);

        // Configurar Puppeteer para converter HTML para PDF
        await new BrowserFetcher().DownloadAsync();
        using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        using var page = await browser.NewPageAsync();

        // Carregar HTML no Puppeteer
        await page.SetContentAsync(htmlContent);

        var inputFilePath = Path.GetTempFileName();
        var outputFilePath = Path.ChangeExtension(inputFilePath, FileTypes.PdfExtension);

        await page.PdfAsync(outputFilePath);

        var bytes = await File.ReadAllBytesAsync(outputFilePath);
        var memoryStream = new MemoryStream(bytes);

        if (File.Exists(inputFilePath)) File.Delete(inputFilePath);
        if (File.Exists(outputFilePath)) File.Delete(outputFilePath);

        memoryStream.Position = 0;
        return new FileModel(Path.ChangeExtension(file.FileName, FileTypes.PdfExtension), memoryStream);
    }
}