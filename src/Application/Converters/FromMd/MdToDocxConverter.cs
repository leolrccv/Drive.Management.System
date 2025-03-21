using Application.Contracts;
using Application.Errors;
using Application.Extensions;
using Application.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ErrorOr;
using HtmlToOpenXml;
using Markdig;
using Microsoft.AspNetCore.Http;

namespace Application.Converters.FromMd;

public class MdToDocxConverter : IFileConverter
{
    public async Task<ErrorOr<FileModel>> ConvertAsync(IFormFile file)
    {
        try
        {
            var htmlContent = await ConvertMarkdownToHtmlAsync(file);

            var documentStream = ConvertToDoc(htmlContent);

            return MapResponseFile(file.FileName, documentStream);
        }
        catch (Exception)
        {
            return ConverterError.Md.ToDocx;
        }
    }

    private static async Task<string> ConvertMarkdownToHtmlAsync(IFormFile file)
    {
        using var streamReader = new StreamReader(file.OpenReadStream());
        var content = await streamReader.ReadToEndAsync();
        return Markdown.ToHtml(content);
    }

    private static MemoryStream ConvertToDoc(string htmlContent)
    {
        var memoryStream = new MemoryStream();

        using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
        {
            var mainPart = CreateDocumentBase(wordDocument);
            ConvertHtmlToDoc(mainPart, htmlContent);
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    private static MainDocumentPart CreateDocumentBase(WordprocessingDocument wordDocument)
    {
        var mainPart = wordDocument.AddMainDocumentPart();
        mainPart.Document = new Document(new Body());
        return mainPart;
    }

    private static void ConvertHtmlToDoc(MainDocumentPart mainPart, string htmlContent)
    {
        var body = mainPart.Document.Body!;

        var converter = new HtmlConverter(mainPart);
        var paragraphs = converter.Parse(htmlContent);

        foreach (var paragraph in paragraphs)
            body.Append(paragraph);

        mainPart.Document.Save();
    }

    private static FileModel MapResponseFile(string originalFileName, MemoryStream file)
    {
        var fileName = Path.ChangeExtension(originalFileName, FileTypes.DocxExtension);
        return new FileModel(fileName, file);
    }
}