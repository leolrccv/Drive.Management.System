using Application.Models;
using ErrorOr;

namespace Application.Contracts;
public interface IGeminiClient
{
    Task<ErrorOr<GeminiResponse>> PostAsync(GeminiModel request, CancellationToken cancellationToken);
}