using Application.Contracts;
using Application.Converters.FromDocx;
using Application.Models;
using ErrorOr;
using MediatR;

namespace Application.Commands.v1.AnalyzeFile;
public class AnalyzeFileCommandHandler(IGeminiClient _geminiClient) : IRequestHandler<AnalyzeFileCommand, ErrorOr<AnalyzeFileCommandResponse>>
{
    public async Task<ErrorOr<AnalyzeFileCommandResponse>> Handle(AnalyzeFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var fileModel = await new DocToStrConverter().ConvertAsync(request.FormFile);

            var prompt = "Resuma o seguinte conteudo de arquivo: " + fileModel.Content;

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
}