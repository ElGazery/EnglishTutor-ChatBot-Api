using EnglishTutor.Application.Common.Settings;
using Microsoft.Extensions.Options;

namespace EnglishTutor.Infrastructure.Services.Gemini
{
    public class GeminiKeyManager : IGeminiKeyManager
    {
        private readonly GeminiSettings _settings;
        private int _keyIndexCounter;

        public GeminiKeyManager(IOptions<GeminiSettings> options)
        {
            _settings = options.Value;
        }

        public bool HasConfiguredKeys => _settings.GetAllApiKeys().Count > 0;

        public IReadOnlyList<string> GetModelsToTry()
        {
            var models = new List<string>();

            if (!string.IsNullOrWhiteSpace(_settings.Model))
            {
                models.Add(_settings.Model.Trim());
            }

            if (_settings.FallbackModels != null)
            {
                foreach (var model in _settings.FallbackModels.Where(m => !string.IsNullOrWhiteSpace(m)))
                {
                    if (!models.Contains(model.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        models.Add(model.Trim());
                    }
                }
            }

            return models.Count > 0 ? models : new[] { "gemini-2.0-flash" };
        }

        public IReadOnlyList<string> GetOrderedApiKeys()
        {
            var apiKeys = _settings.GetAllApiKeys();
            if (apiKeys.Count <= 1)
            {
                return apiKeys;
            }

            int startIndex = (int)((uint)Interlocked.Increment(ref _keyIndexCounter) % (uint)apiKeys.Count);
            var ordered = new List<string>(apiKeys.Count);

            for (int i = 0; i < apiKeys.Count; i++)
            {
                int index = (startIndex + i) % apiKeys.Count;
                ordered.Add(apiKeys[index]);
            }

            return ordered;
        }
    }
}

