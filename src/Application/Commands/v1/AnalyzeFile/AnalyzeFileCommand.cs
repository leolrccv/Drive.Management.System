using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.v1.AnalyzeFile;
public record AnalyzeFileCommand(IFormFile FormFile) : IRequest<ErrorOr<AnalyzeFileCommandResponse>>;