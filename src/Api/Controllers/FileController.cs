using Api.Providers;
using Application.Commands.v1.AnalyzeFile;
using Application.Commands.v1.ConvertFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/drive-management")]
public class FileController(ILogger<FileController> _logger, IMediator _mediator, IErrorProvider _errorProvider)
    : ApiControllerBase(_logger, _mediator, _errorProvider)
{
    [HttpPost("v1/file/convert")]
    public async Task<IActionResult> ConvertAsync([FromForm] IFormFile file)
    {
        var response = await _mediator.Send(new ConvertFileCommand(file));
        return File(response.Value.Stream, "application/zip", "converted_files.zip");
    }

    [HttpPost("v1/file/analyze")]
    public async Task<IActionResult> AnalyzeFileAsync(AnalyzeFileCommand request) =>
      await ProcessRequestAsync(request, StatusCodes.Status200OK);
}