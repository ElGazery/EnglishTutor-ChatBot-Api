using EnglishTutor.Infrastructure.Services.Gemini.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace EnglishTutor.Infrastructure.Services.Gemini
{
    public class GeminiApiClient : IGeminiApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiApiClient> _logger;

        public GeminiApiClient(HttpClient httpClient, ILogger<GeminiApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<GeminiCallResult> GenerateContentAsync(
            string model,
            string apiKey,
            GeminiRequest request,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"/v1beta/models/{model}:generateContent?key={apiKey}";

            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
                var statusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: cancellationToken);
                    var text = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        return new GeminiCallResult(true, text.Trim(), statusCode, null);
                    }

                    _logger.LogWarning("Model [{Model}] returned an empty response body.", model);
                    return new GeminiCallResult(false, null, statusCode, "Empty candidate content in response.");
                }

                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Model [{Model}] failed with status {StatusCode}: {Error}", model, statusCode, errorBody);

                return new GeminiCallResult(false, null, statusCode, errorBody);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "HTTP call failed for model [{Model}].", model);
                return new GeminiCallResult(false, null, 0, ex.Message);
            }
        }
    }
}

