using Application.Commands.v1.Commons;

namespace Application.Commands.v1.ConvertFile;
public record ConvertFileCommandResponse(Stream Stream) : UploadFileCommandResponse(Stream);