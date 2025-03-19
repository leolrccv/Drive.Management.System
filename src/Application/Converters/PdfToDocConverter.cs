using Application.Contracts;
using Application.Extensions;
using Application.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Application.Converters;
public class PdfToDocConverter : IFileConverter
{
    public async Task<FileModel> ConvertAsync(IFormFile file)
    {
        using var pdfStream = file.OpenReadStream();

        var outputPath = Path.ChangeExtension(file.FileName, FileTypes.DocxExtension);

        using (var wordDocument = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
        {
            var mainPart = wordDocument.AddMainDocumentPart();

            mainPart.Document = new Document();
            Body body = mainPart.Document.AppendChild(new Body());

            using (PdfDocument pdfDocument = PdfDocument.Open(pdfStream))
            {
                foreach (var page in pdfDocument.GetPages())
                {
                    string pageText = ContentOrderTextExtractor.GetText(page);

                    var lines = pageText.Split('\n');
                    foreach (var line in lines)
                    {
                        string trimmedLine = line.Trim();

                        if (string.IsNullOrWhiteSpace(trimmedLine))
                            continue;

                        AddParagraph(body, trimmedLine);
                    }
                }
            }

            mainPart.Document.Save();
        }

        var bytes = await File.ReadAllBytesAsync(outputPath);
        var memoryStream = new MemoryStream(bytes);

        if (File.Exists(outputPath)) File.Delete(outputPath);

        memoryStream.Position = 0;
        return new FileModel(Path.ChangeExtension(file.FileName, FileTypes.DocxExtension), memoryStream);
    }

    private static void AddParagraph(Body body, string text)
    {
        var paragraph = body.AppendChild(new Paragraph());
        var run = paragraph.AppendChild(new Run());

        run.AppendChild(new Text(text));
    }
}