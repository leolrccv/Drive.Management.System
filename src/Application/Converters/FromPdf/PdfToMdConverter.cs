using Application.Contracts;
using Application.Extensions;
using Application.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Application.Converters.FromPdf;
public class PdfToMdConverter : IFileConverter
{
    public async Task<FileModel> ConvertAsync(IFormFile file)
    {
        using var sourceStream = file.OpenReadStream();

        var outputFilePath = Path.ChangeExtension(file.FileName, FileTypes.MdExtension);

        var markdownContent = new StringBuilder();

        using (var document = PdfDocument.Open(sourceStream))
        {
            foreach (var page in document.GetPages())
            {
                string pageText = ContentOrderTextExtractor.GetText(page);

                var lines = pageText.Split('\n');

                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    if (string.IsNullOrWhiteSpace(trimmedLine))
                        continue;

                    if (trimmedLine.Length < 50 && !trimmedLine.EndsWith('.'))
                    {
                        markdownContent.AppendLine($"# {trimmedLine}");
                        markdownContent.AppendLine();
                        continue;
                    }

                    markdownContent.AppendLine(trimmedLine);
                    markdownContent.AppendLine();
                }
            }
        }

        await File.WriteAllTextAsync(outputFilePath, markdownContent.ToString());

        var bytes = await File.ReadAllBytesAsync(outputFilePath);
        var memoryStream = new MemoryStream(bytes);

        if (File.Exists(outputFilePath)) File.Delete(outputFilePath);

        memoryStream.Position = 0;
        return new FileModel(outputFilePath, memoryStream);
    }
}