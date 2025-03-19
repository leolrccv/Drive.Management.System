using Application.Contracts;
using Application.Extensions;
using Application.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;
using Markdig;
using Microsoft.AspNetCore.Http;

namespace Application.Converters;

public class MdToDocConverter : IFileConverter
{
    public async Task<FileModel> ConvertAsync(IFormFile file)
    {
        using var streamReader = new StreamReader(file.OpenReadStream());
        var markdownContent = await streamReader.ReadToEndAsync();

        var htmlContent = Markdown.ToHtml(markdownContent);

        var memoryStream = new MemoryStream();

        using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
        {
            var mainPart = wordDocument.AddMainDocumentPart();

            mainPart.Document = new Document();

            var body = new Body();

            var converter = new HtmlConverter(mainPart);
            var paragraphs = converter.Parse(htmlContent);

            foreach (var para in paragraphs)
                body.Append(para);

            mainPart.Document.Append(body);
            mainPart.Document.Save();
        }

        memoryStream.Position = 0;
        return new FileModel(Path.ChangeExtension(file.FileName, FileTypes.DocxExtension), memoryStream);
    }
}