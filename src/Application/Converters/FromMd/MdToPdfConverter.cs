using Application.Contracts;
using Application.Errors;
using Application.Extensions;
using Application.Models;
using ErrorOr;
using HTMLQuestPDF.Extensions;
using Markdig;
using Microsoft.AspNetCore.Http;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace Application.Converters.FromMd;

public class MdToPdfConverter : IFileConverter
{
    public async Task<ErrorOr<FileModel>> ConvertAsync(IFormFile file)
    {
        try
        {
            var htmlContent = await ConvertMarkdownToHtmlAsync(file);

            var documentStream = ConvertToPdf(htmlContent);

            return MapResponseFile(file.FileName, documentStream);
        }
        catch (Exception)
        {
            return ConverterError.Md.ToPdf;
        }
    }

    private static async Task<string> ConvertMarkdownToHtmlAsync(IFormFile file)
    {
        using var streamReader = new StreamReader(file.OpenReadStream());
        var content = await streamReader.ReadToEndAsync();
        return Markdown.ToHtml(content);
    }

    private static MemoryStream ConvertToPdf(string htmlContent)
    {
        var memoryStream = new MemoryStream();

        Document.Create(container =>
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(1, Unit.Centimetre);
            page.DefaultTextStyle(x => x.FontSize(12));
            page.Content().Column(col => col.Item()
                .HTML(handler => handler.SetHtml(htmlContent)));
        })).GeneratePdf(memoryStream);

        memoryStream.Position = 0;
        return memoryStream;
    }

    private static FileModel MapResponseFile(string originalFileName, MemoryStream file)
    {
        var fileName = Path.ChangeExtension(originalFileName, FileTypes.PdfExtension);
        return new FileModel(fileName, file);
    }
}