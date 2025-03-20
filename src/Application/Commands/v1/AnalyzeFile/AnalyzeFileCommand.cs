using ErrorOr;
using MediatR;

namespace Application.Commands.v1.AnalyzeFile;
public record AnalyzeFileCommand(string FileName, string Question = "Resuma o arquivo") : IRequest<ErrorOr<AnalyzeFileCommandResponse>>;