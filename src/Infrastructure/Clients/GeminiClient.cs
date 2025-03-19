using Application.Contracts;
using Application.Models;
using Infrastructure.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Infrastructure.Clients;

public class GeminiClient(HttpClient _httpClient, IOptions<ClientSettings> _options) : IGeminiClient
{
    public async Task<GeminiResponse> PostAsync(GeminiModel request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync(
            $"v1beta/models/gemini-2.0-flash:generateContent?key={_options.Value.GeminiApi.ApiKey}",
            JsonContent.Create(request), cancellationToken);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken);

        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new Exception(errorContent);
    }
}