using Application.Contracts;
using Application.Extensions;
using Application.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Application.Converters.FromPdf;
public class PdfToDocConverter : IFileConverter
{
    public async Task<FileModel> ConvertAsync(IFormFile file)
    {
        var outputPath = Path.ChangeExtension(file.FileName, FileTypes.DocxExtension);

        try
        {
            CreateWordDocument(file, outputPath);
            return await MapResponseFileAsync(outputPath);
        }
        catch (Exception)
        {
            DeleteFile(outputPath);
            throw;
        }
    }

    private static void CreateWordDocument(IFormFile file, string outputPath)
    {
        using var pdfStream = file.OpenReadStream();
        using var wordDocument = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document);

        var mainPart = CreateDocumentBase(wordDocument);

        using var pdfDocument = PdfDocument.Open(pdfStream);

        foreach (var page in pdfDocument.GetPages())
            AddPageToBody(mainPart.Document.Body!, page);
    }

    private static MainDocumentPart CreateDocumentBase(WordprocessingDocument wordDocument)
    {
        var mainPart = wordDocument.AddMainDocumentPart();
        mainPart.Document = new Document(new Body());
        return mainPart;
    }

    private static void AddPageToBody(Body body, Page page)
    {
        var pageText = ContentOrderTextExtractor.GetText(page);
        var lines = pageText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
            AddParagraph(body, line.Trim());
    }

    private static void AddParagraph(Body body, string text)
    {
        var paragraph = body.AppendChild(new Paragraph());
        var run = paragraph.AppendChild(new Run());
        run.AppendChild(new Text(text));
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