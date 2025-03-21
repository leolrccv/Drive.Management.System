using Application.Contracts;
using Application.Converters.Commons;
using Application.Models;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;

namespace Application.Commands.v1.ConvertFile;

public class ConvertFileCommandHandler(IAwsClient _awsClient) : IRequestHandler<ConvertFileCommand, ErrorOr<ConvertFileCommandResponse>>
{
    public async Task<ErrorOr<ConvertFileCommandResponse>> Handle(ConvertFileCommand request, CancellationToken cancellationToken)
    {
        var uploadStatus = await _awsClient.UploadToS3Async(new FileModel(request.File.FileName, request.File.OpenReadStream()));
        if (uploadStatus.IsError) return uploadStatus.Errors;

        var strategies = ConverterFactory.CreateStrategies(request.File.FileName);

        var zipStream = await ConvertFilesAsync(request.File, strategies, cancellationToken);

        return zipStream.IsError ? zipStream.Errors : new ConvertFileCommandResponse(zipStream.Value);
    }

    private async Task<ErrorOr<MemoryStream>> ConvertFilesAsync(IFormFile file, List<IFileConverter> strategies, CancellationToken cancellationToken)
    {
        var zipStream = new MemoryStream();

        using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var strategy in strategies)
            {
                var convertStatus = await ConvertFileAsync(file, strategy, zipArchive, cancellationToken);
                if (convertStatus.IsError) return convertStatus.Errors;
            }
        }

        zipStream.Position = 0;
        return zipStream;
    }

    private async Task<ErrorOr<Success>> ConvertFileAsync(IFormFile file, IFileConverter strategy, ZipArchive zipArchive, CancellationToken cancellationToken)
    {
        var converted = await strategy.ConvertAsync(file);
        if (converted.IsError) return converted.Errors;

        await AddToZipAsync(converted.Value, zipArchive, cancellationToken);
        return await _awsClient.UploadToS3Async(converted.Value);
    }

    private static async Task AddToZipAsync(FileModel convertedFile, ZipArchive zipArchive, CancellationToken cancellationToken)
    {
        var zipEntry = zipArchive.CreateEntry(convertedFile.FileName);

        using var entryStream = zipEntry.Open();
        await convertedFile.File.CopyToAsync(entryStream, cancellationToken);
    }
}