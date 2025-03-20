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
    [HttpPost("v1/file")]
    public async Task<IActionResult> ConvertAsync([FromForm] IFormFile file) =>
        await ProcessRequestAsync(new ConvertFileCommand(file), StatusCodes.Status200OK);


    [HttpPost("v1/file/test")]
    public async Task<IActionResult> Test([FromForm] IFormFile file)
    {
        var response = await _mediator.Send(new ConvertFileCommand(file));

        return File(response.Value.Stream, "application/zip", "converted_files.zip");
    }

    [HttpPost("v1/analyze-file")]
    public async Task<IActionResult> AnalyzeFileAsync(AnalyzeFileCommand request) =>
      await ProcessRequestAsync(request, StatusCodes.Status200OK);
}