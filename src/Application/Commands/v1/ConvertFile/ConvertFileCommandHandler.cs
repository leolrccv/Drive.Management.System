﻿using Application.Contracts;
using Application.Converters.Commons;
using Application.Models;
using ErrorOr;
using MediatR;
using System.IO.Compression;

namespace Application.Commands.v1.ConvertFile;

public class ConvertFileCommandHandler(IAwsClient _awsClient) : IRequestHandler<ConvertFileCommand, ErrorOr<ConvertFileCommandResponse>>
{
    public async Task<ErrorOr<ConvertFileCommandResponse>> Handle(ConvertFileCommand request, CancellationToken cancellationToken)
    {
        await _awsClient.UploadToS3Async(new FileModel(request.File.FileName, request.File.OpenReadStream()));

        var strategies = ConverterFactory.CreateStrategies(request.File.FileName);

        var zipStream = new MemoryStream();

        using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var strategy in strategies)
            {
                var converted = await strategy.ConvertAsync(request.File);
                var zipEntry = zipArchive.CreateEntry(converted.FileName);

                using var entryStream = zipEntry.Open();
                await converted.File!.CopyToAsync(entryStream, cancellationToken);

                await _awsClient.UploadToS3Async(converted);
            }
        }

        zipStream.Position = 0;
        return new ConvertFileCommandResponse(zipStream);
    }
}