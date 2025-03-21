using Application.Contracts;
using Application.Errors;
using Application.Extensions;
using Application.Models;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Application.Converters.FromPdf;
public class PdfToMdConverter : IFileConverter
{
    public async Task<ErrorOr<FileModel>> ConvertAsync(IFormFile file)
    {
        var outputFilePath = Path.ChangeExtension(file.FileName, FileTypes.MdExtension);

        try
        {
            var markdownContent = ConvertPdfToMd(file);
            return await MapFileResponseAsync(outputFilePath, markdownContent);
        }
        catch (Exception)
        {
            DeleteFile(outputFilePath);
            return ConverterError.Pdf.ToMd;
        }
    }

    private static string ConvertPdfToMd(IFormFile file)
    {
        using var sourceStream = file.OpenReadStream();
        var markdownContent = new StringBuilder();

        using var document = PdfDocument.Open(sourceStream);

        foreach (var page in document.GetPages())
            AppendPageContent(markdownContent, page);

        return markdownContent.ToString();
    }

    private static void AppendPageContent(StringBuilder markdownContent, Page page)
    {
        var pageText = ContentOrderTextExtractor.GetText(page);
        var lines = pageText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
            AppendFormattedText(markdownContent, line);
    }

    private static void AppendFormattedText(StringBuilder markdownText, string text)
    {
        var isHeading = text.Length < 50 && !text.EndsWith('.');

        text = isHeading ? $"# {text}" : text;

        markdownText.AppendLine(text);
        markdownText.AppendLine();
    }

    private static async Task<FileModel> MapFileResponseAsync(string filePath, string content)
    {
        var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream, leaveOpen: true);

        await writer.WriteAsync(content);
        await writer.FlushAsync();

        memoryStream.Position = 0;
        return new FileModel(filePath, memoryStream);
    }

    private static void DeleteFile(string filePath)
    {
        if (File.Exists(filePath)) File.Delete(filePath);
    }
}