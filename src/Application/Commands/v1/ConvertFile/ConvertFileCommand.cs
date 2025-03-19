using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.v1.ConvertFile;
public record ConvertFileCommand(IFormFile File) : IRequest<ErrorOr<ConvertFileCommandResponse>>;