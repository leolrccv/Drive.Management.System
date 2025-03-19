using Application.Converters.Commons;
using ErrorOr;
using MediatR;
using System.IO.Compression;

namespace Application.Commands.v1;

public class ConvertFileCommandHandler() : IRequestHandler<ConvertFileCommand, ErrorOr<ConvertFileCommandResponse>>
{
    public async Task<ErrorOr<ConvertFileCommandResponse>> Handle(ConvertFileCommand request, CancellationToken cancellationToken)
    {
        var strategies = ConverterFactory.CreateStrategies(request.File.FileName);

        var zipStream = new MemoryStream();

        using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var strategy in strategies)
            {
                var converted = await strategy.ConvertAsync(request.File);
                var zipEntry = zipArchive.CreateEntry(converted.FileName);

                using var entryStream = zipEntry.Open();
                await converted.File.CopyToAsync(entryStream, cancellationToken);
            }
        }

        zipStream.Position = 0;
        return new ConvertFileCommandResponse(zipStream);
    }
}