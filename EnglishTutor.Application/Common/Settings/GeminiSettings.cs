namespace EnglishTutor.Application.Common.Settings
{
    public class GeminiSettings
    {
        public const string SectionName = "GeminiSettings";

        public string Model { get; set; } = "gemini-1.5-flash";
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com";
        public int MaxContextMessages { get; set; } = 6;
        public int TimeoutSeconds { get; set; } = 30;
        public List<string> FallbackModels { get; set; } = new();
        public List<string> ApiKeys { get; set; } = new();

        public IReadOnlyList<string> GetAllApiKeys()
        {
            var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(ApiKey))
            {
                keys.Add(ApiKey.Trim());
            }

            if (ApiKeys != null)
            {
                foreach (var key in ApiKeys.Where(k => !string.IsNullOrWhiteSpace(k)))
                {
                    keys.Add(key.Trim());
                }
            }

            return keys.ToList();
        }
    }
}
