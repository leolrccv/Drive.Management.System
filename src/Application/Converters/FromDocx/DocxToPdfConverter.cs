using Application.Contracts;
using Application.Errors;
using Application.Extensions;
using Application.Models;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Application.Converters.FromDocx;
public class DocxToPdfConverter : IFileConverter
{
    public async Task<ErrorOr<FileModel>> ConvertAsync(IFormFile file)
    {
        var inputFilePath = Path.GetTempFileName();
        var outputFilePath = Path.ChangeExtension(inputFilePath, FileTypes.PdfExtension);

        try
        {
            await SaveInputFileAsync(file, inputFilePath);

            await ConvertToPdfAsync(inputFilePath, outputFilePath);

            return await MapFileResponseAsync(file.FileName, outputFilePath);
        }
        catch (Exception)
        {
            DeleteFiles(inputFilePath, outputFilePath);
            return ConverterError.Docx.ToPdf;
        }
    }

    private static async Task SaveInputFileAsync(IFormFile file, string inputFilePath)
    {
        using var sourceStream = file.OpenReadStream();
        using var fileStream = new FileStream(inputFilePath, FileMode.Create, FileAccess.Write);
        await sourceStream.CopyToAsync(fileStream);
    }

    private static async Task ConvertToPdfAsync(string inputFilePath, string outputFilePath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "libreoffice",
            Arguments = $"--headless --convert-to pdf --outdir {Path.GetDirectoryName(outputFilePath)} {inputFilePath}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo)
            ?? throw new Exception("Failed to start conversion process");

        await process.WaitForExitAsync();

        DeleteFiles(inputFilePath);

        if (process.ExitCode == 0) return;

        var error = await process.StandardError.ReadToEndAsync();
        throw new Exception($"Conversion failed: {error}");
    }

    private static async Task<FileModel> MapFileResponseAsync(string originalFileName, string outputFilePath)
    {
        var bytes = await File.ReadAllBytesAsync(outputFilePath);
        var memoryStream = new MemoryStream(bytes);

        DeleteFiles(outputFilePath);

        return new FileModel(Path.ChangeExtension(originalFileName, FileTypes.PdfExtension), memoryStream);
    }

    private static void DeleteFiles(params string[] filePaths)
    {
        foreach (var filePath in filePaths)
            if (File.Exists(filePath)) File.Delete(filePath);
    }
}