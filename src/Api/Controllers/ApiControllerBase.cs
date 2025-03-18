﻿using Api.Errors;
using Api.Models;
using Api.Providers;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
public class ApiControllerBase(ILogger _logger, IMediator _mediator, IErrorProvider _errorProvider) : ControllerBase
{
    protected async Task<IActionResult> ProcessRequestAsync<TResponse>(IRequest<ErrorOr<TResponse>>? request, int statusCode = StatusCodes.Status200OK)
    {
        try
        {
            if (request is null)
                return MapErrorResponse(RequestError.Invalid);

            var response = await _mediator.Send(request);

            return response.Match(
                success => HandleSuccessResponse(success, statusCode),
                error => HandleErrorResponse(error));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error has occurred.");
            return MapErrorResponse(RequestError.Unexpected);
        }
    }

    private IActionResult HandleSuccessResponse<TResponse>(TResponse response, int statusCode) =>
        statusCode == StatusCodes.Status204NoContent ?
            StatusCode(statusCode) :
            StatusCode(statusCode, new ResponseBody(response));

    private ObjectResult HandleErrorResponse(List<Error> errors)
    {
        if (errors.Count == 0)
            return MapErrorResponse(errors, StatusCodes.Status400BadRequest);

        if (errors.All(e => e.Type == ErrorType.Validation))
            return MapErrorResponse(errors, StatusCodes.Status400BadRequest);

        if (errors.All(e => e.Type == ErrorType.NotFound))
            return MapErrorResponse(errors, StatusCodes.Status404NotFound);

        return MapErrorResponse(errors.First());
    }

    private ObjectResult MapErrorResponse(Error error)
    {
        var errorStatusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return MapErrorResponse([error], errorStatusCode);
    }

    private ObjectResult MapErrorResponse(List<Error> errors, int statusCode)
    {
        var response = new ResponseBody(Notifications: errors.ToDictionary(
                e => e.Code, e => _errorProvider.GetErrorMessage(e.Code, e.Description)));

        return StatusCode(statusCode, response);
    }
}