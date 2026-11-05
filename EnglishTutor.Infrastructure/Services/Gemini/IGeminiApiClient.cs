using EnglishTutor.Infrastructure.Services.Gemini.Models;

namespace EnglishTutor.Infrastructure.Services.Gemini
{
    public record GeminiCallResult(bool IsSuccess, string? Text, int StatusCode, string? Error);

    public interface IGeminiApiClient
    {
        Task<GeminiCallResult> GenerateContentAsync(
            string model,
            string apiKey,
            GeminiRequest request,
            CancellationToken cancellationToken = default);
    }
}

