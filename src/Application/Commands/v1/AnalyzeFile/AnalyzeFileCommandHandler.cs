using Application.Contracts;
using Application.Converters.FromDocx;
using Application.Extensions;
using Application.Models;
using ErrorOr;
using MediatR;

namespace Application.Commands.v1.AnalyzeFile;
public class AnalyzeFileCommandHandler(IAwsClient _awsClient, IGeminiClient _geminiClient) : IRequestHandler<AnalyzeFileCommand, ErrorOr<AnalyzeFileCommandResponse>>
{
    private const string Prompt = "Responda a pergunta com base no conteudo. Pergunta: {0}? Conteúdo: {1}";

    public async Task<ErrorOr<AnalyzeFileCommandResponse>> Handle(AnalyzeFileCommand request, CancellationToken cancellationToken)
    {
        var download = await _awsClient.DownloadFromS3Async(Path.ChangeExtension(request.FileName, FileTypes.DocxExtension));
        if (download.IsError) return download.Errors;

        using var file = download.Value;

        var content = DocToStrConverter.Convert(file);

        var prompt = string.Format(Prompt, request.Question, content);

        var geminiModel = new GeminiModel([new Content([new Part(prompt)])]);

        var response = await _geminiClient.PostAsync(geminiModel, cancellationToken);

        return response.IsError ? response.Errors : MapResponse(response.Value);
    }

    private static AnalyzeFileCommandResponse MapResponse(GeminiResponse response)
    {
        var textResponse = response.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
            ?? "Nenhuma resposta gerada pela IA.";

        return new AnalyzeFileCommandResponse(textResponse);
    }
}