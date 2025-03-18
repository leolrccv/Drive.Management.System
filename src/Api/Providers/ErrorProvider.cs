using Api.Models;
using System.Text.Json;

namespace Api.Providers;

public class ErrorProvider : IErrorProvider
{
    private readonly ILogger _logger;
    private Dictionary<string, string> _errors = [];

    public ErrorProvider(ILogger<ErrorProvider> logger)
    {
        _logger = logger;
        Initialize();
    }

    private void Initialize()
    {
        try
        {
            var errorsJson = File.ReadAllText("errors.json");
            var errorMessages = JsonSerializer.Deserialize<ErrorFile>(errorsJson);
            _errors = errorMessages?.Errors ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to read errors.json file.");
        }
    }

    public string GetErrorMessage(string code, string description)
    {
        if (_errors.Count == 0) Initialize();

        return _errors.TryGetValue(code, out var message) ? message : description;
    }
}