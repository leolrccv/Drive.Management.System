using Application.Contracts;
using Application.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ErrorOr;
using MediatR;
using System.Text;

namespace Application.Commands.v1.AnalyzeFile;
public class AnalyzeFileCommandHandler(IGeminiClient _geminiClient) : IRequestHandler<AnalyzeFileCommand, ErrorOr<AnalyzeFileCommandResponse>>
{
    public async Task<ErrorOr<AnalyzeFileCommandResponse>> Handle(AnalyzeFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var fileStream = request.FormFile.OpenReadStream();

            var fileContent = ConvertWordToText(fileStream);

            var prompt = "Resuma o seguinte conteudo de arquivo: " + fileContent;

            var geminiModel = new GeminiModel([new Content([new Part(prompt)])]);

            var response = await _geminiClient.PostAsync(geminiModel, cancellationToken);

            return MapResponse(response);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static AnalyzeFileCommandResponse MapResponse(GeminiResponse response)
    {
        var textResponse = response.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
            ?? "Nenhuma resposta gerada pela IA.";

        return new AnalyzeFileCommandResponse(textResponse);
    }

    private static string ConvertWordToText(Stream sourceStream)
    {
        var sb = new StringBuilder();

        using var wordDoc = WordprocessingDocument.Open(sourceStream, false);

        var body = wordDoc.MainDocumentPart?.Document?.Body
            ?? throw new InvalidOperationException("Document body not found");

        foreach (var paragraph in body.Elements<Paragraph>())
        {
            var text = paragraph.InnerText.Trim();

            if (string.IsNullOrWhiteSpace(text)) continue;

            sb.AppendLine(text);
            sb.AppendLine();
        }

        return sb.ToString();
    }
}