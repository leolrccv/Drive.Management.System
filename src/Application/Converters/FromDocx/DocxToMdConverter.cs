using Application.Contracts;
using Application.Errors;
using Application.Extensions;
using Application.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Application.Converters.FromDocx;
public class DocxToMdConverter : IFileConverter
{
    public async Task<ErrorOr<FileModel>> ConvertAsync(IFormFile file)
    {
        var outputFilePath = GetMarkdownFilePath(file.FileName);

        try
        {
            using var sourceStream = file.OpenReadStream();

            var markdownContent = ConvertWordToMarkdown(sourceStream);

            return await MapFileResponseAsync(outputFilePath, markdownContent);
        }
        catch (Exception)
        {
            DeleteFile(outputFilePath);
            return ConverterError.Docx.ToMd;
        }
    }

    private static string ConvertWordToMarkdown(Stream sourceStream)
    {
        var markdownText = new StringBuilder();

        using var wordDoc = WordprocessingDocument.Open(sourceStream, false);

        var body = wordDoc.MainDocumentPart.Document.Body;

        foreach (var paragraph in GetValidParagraphs(body))
        {
            var text = paragraph.InnerText.Trim();
            AppendFormattedText(markdownText, text);
        }

        return markdownText.ToString();
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
        await File.WriteAllTextAsync(filePath, content);

        var bytes = await File.ReadAllBytesAsync(filePath);
        var memoryStream = new MemoryStream(bytes);

        DeleteFile(filePath);

        return new FileModel(filePath, memoryStream);
    }

    private static void DeleteFile(string filePath)
    {
        if (File.Exists(filePath)) File.Delete(filePath);
    }

    private static IEnumerable<Paragraph> GetValidParagraphs(Body body) =>
        body.Elements<Paragraph>().Where(e => !string.IsNullOrWhiteSpace(e.InnerText));

    private static string GetMarkdownFilePath(string fileName) =>
        Path.ChangeExtension(fileName, FileTypes.MdExtension);
}