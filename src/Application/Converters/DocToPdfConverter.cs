using Application.Contracts;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Application.Converters;
public class DocToPdfConverter : IFileConverter
{
    public Stream Convert(IFormFile file)
    {
        using var sourceStream = file.OpenReadStream();

        var inputFilePath = Path.GetTempFileName();

        using var fileStream = new FileStream(inputFilePath, FileMode.Create, FileAccess.Write);
        sourceStream.CopyTo(fileStream);

        string outputFilePath = Path.ChangeExtension(inputFilePath, ".pdf");

        var startInfo = new ProcessStartInfo
        {
            FileName = "libreoffice", 
            Arguments = $"--headless --convert-to pdf --outdir {Path.GetDirectoryName(outputFilePath)} {inputFilePath}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(startInfo))
        {
            process.WaitForExit();

            if(process.ExitCode != 0)
            {
                var error = process.StandardError.ReadToEnd();
                throw new Exception(error);
            }
        }

        // Retornar o arquivo convertido como Stream
        var bytes = File.ReadAllBytes(outputFilePath);
        var memoryStream = new MemoryStream(bytes);

        if (File.Exists(inputFilePath)) File.Delete(inputFilePath);
        if (File.Exists(outputFilePath)) File.Delete(outputFilePath);

        memoryStream.Position = 0; // Resetar a posição para leitura
        return memoryStream;
    }
    //public Stream Convert(IFormFile file)
    //{
    //    using var docxFile = file.OpenReadStream();

    //    Document document = new();
    //    document.LoadFromStream(docxFile, FileFormat.Docx);

    //    MemoryStream pdfStream = new();

    //    document.SaveToStream(pdfStream, FileFormat.PDF);

    //    pdfStream.Position = 0;

    //    return pdfStream;
    //}
}