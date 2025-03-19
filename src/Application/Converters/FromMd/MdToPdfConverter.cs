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
        var outputFilePath = Path.ChangeExtension(file.FileName, FileTypes.PdfExtension);

        try
        {
            var htmlContent = await ConvertMarkdownToHtmlAsync(file);
            await GeneratePdfFromHtmlAsync(htmlContent, outputFilePath);
            return await MapResponseFileAsync(outputFilePath);
        }
        catch (Exception)
        {
            DeleteFile(outputFilePath);
            throw;
        }
    }

    private static async Task<string> ConvertMarkdownToHtmlAsync(IFormFile file)
    {
        using var streamReader = new StreamReader(file.OpenReadStream());
        var content = await streamReader.ReadToEndAsync();
        return Markdown.ToHtml(content);
    }

    private static async Task GeneratePdfFromHtmlAsync(string htmlContent, string outputFilePath)
    {
        await new BrowserFetcher().DownloadAsync();

        using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        using var page = await browser.NewPageAsync();

        await page.SetContentAsync(htmlContent);
        await page.PdfAsync(outputFilePath);
    }

    private static async Task<FileModel> MapResponseFileAsync(string outputFilePath)
    {
        var bytes = await File.ReadAllBytesAsync(outputFilePath);
        var memoryStream = new MemoryStream(bytes);

        DeleteFile(outputFilePath);

        return new FileModel(outputFilePath, memoryStream);
    }

    private static void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}