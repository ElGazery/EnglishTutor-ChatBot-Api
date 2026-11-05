using EnglishTutor.Application.Interfaces.Services;
using EnglishTutor.Domain.Enums;
using EnglishTutor.Domain.Exceptions;
using EnglishTutor.Infrastructure.Services.Gemini;
using Microsoft.Extensions.Logging;

namespace EnglishTutor.Infrastructure.Services
{
    public class GeminiAiTutorService : IAiTutorService
    {
        private readonly IGeminiPromptBuilder _promptBuilder;
        private readonly IGeminiKeyManager _keyManager;
        private readonly IGeminiApiClient _apiClient;
        private readonly ILogger<GeminiAiTutorService> _logger;

        public GeminiAiTutorService(
            IGeminiPromptBuilder promptBuilder,
            IGeminiKeyManager keyManager,
            IGeminiApiClient apiClient,
            ILogger<GeminiAiTutorService> logger)
        {
            _promptBuilder = promptBuilder;
            _keyManager = keyManager;
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<string> GetResponseAsync(
            string question,
            AnswerMode mode,
            IReadOnlyList<ChatMessageContext> context,
            CancellationToken cancellationToken = default)
        {
            if (!_keyManager.HasConfiguredKeys)
            {
                _logger.LogWarning("No Gemini API keys found. Returning offline demo response.");
                return _promptBuilder.GenerateOfflineResponse(question, mode);
            }

            var request = _promptBuilder.BuildRequest(question, mode, context);
            var models = _keyManager.GetModelsToTry();
            var keys = _keyManager.GetOrderedApiKeys();

            int lastStatusCode = 0;
            string? lastError = null;

            // Iterate through configured models and API keys as a resilience strategy against 429/quota limits
            foreach (var model in models)
            {
                foreach (var key in keys)
                {
                    var result = await _apiClient.GenerateContentAsync(model, key, request, cancellationToken);

                    if (result.IsSuccess && !string.IsNullOrWhiteSpace(result.Text))
                    {
                        return result.Text;
                    }

                    lastStatusCode = result.StatusCode;
                    lastError = result.Error;
                }
            }

            _logger.LogError("All Gemini model and key attempts failed. Last status: {StatusCode}, Error: {Error}",
                lastStatusCode, lastError);

            if (lastStatusCode == 429)
            {
                throw new BadRequestCustomException("The AI tutor is currently at high capacity. Please try again shortly.");
            }

            throw new BadRequestCustomException("Unable to generate AI tutor response at this moment. Please try again.");
        }
    }
}
