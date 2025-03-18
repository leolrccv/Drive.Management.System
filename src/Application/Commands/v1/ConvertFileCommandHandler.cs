using Application.Converters;
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
                using var convertedFile = strategy.Convert(request.File);

                var zipEntry = zipArchive.CreateEntry($"{Path.GetFileNameWithoutExtension(request.File.FileName)}.docx");

                using var entryStream = zipEntry.Open();

                await convertedFile.CopyToAsync(entryStream);

                convertedFile.Dispose();
            }
        }

        zipStream.Position = 0; // Reseta a posição para o início
        return new ConvertFileCommandResponse(zipStream);
    }
}