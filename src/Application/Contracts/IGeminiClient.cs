using Application.Models;

namespace Application.Contracts;
public interface IGeminiClient
{
    Task<GeminiResponse> PostAsync(GeminiModel request, CancellationToken cancellationToken);
}