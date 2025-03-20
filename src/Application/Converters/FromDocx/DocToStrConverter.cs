using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;

namespace Application.Converters.FromDocx;
public class DocToStrConverter
{
    public static string Convert(Stream fileStream)
    {
        var sb = new StringBuilder();

        using var wordDoc = WordprocessingDocument.Open(fileStream, false);

        var body = wordDoc.MainDocumentPart?.Document?.Body
            ?? throw new InvalidOperationException("Document body not found");

        WriteText(sb, body);

        return sb.ToString();
    }

    private static void WriteText(StringBuilder sb, Body body)
    {
        foreach (var paragraph in GetValidParagraphs(body))
            sb.AppendLine(paragraph.InnerText.Trim());
    }

    private static IEnumerable<Paragraph> GetValidParagraphs(Body body) =>
        body.Elements<Paragraph>().Where(e => !string.IsNullOrWhiteSpace(e.InnerText));
}