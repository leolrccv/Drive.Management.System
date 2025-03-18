using Application.Contracts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;

namespace Application.Converters;
public class PdfToDocConverter : IFileConverter
{
    public Stream Convert(IFormFile file)
    {
        var pdfStream = file.OpenReadStream();

        var tempFileName = Path.GetTempFileName();

        var docxPath = Path.ChangeExtension(tempFileName, ".docx");

        using var wordDoc = WordprocessingDocument.Create(docxPath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);

        var mainPart = wordDoc.AddMainDocumentPart();

        mainPart.Document = new Document();

        var body = mainPart.Document.AppendChild(new Body());

        using var pdf = PdfDocument.Open(pdfStream);

        foreach (var page in pdf.GetPages())
        {
            string text = page.Text;

            body.AppendChild(new Paragraph(new Run(new Text(text))));
        }

        var bytes = File.ReadAllBytes(docxPath);
        var memoryStream = new MemoryStream(bytes);

        if (File.Exists(docxPath)) File.Delete(docxPath);

        memoryStream.Position = 0;
        return memoryStream;
    }
}