namespace EnglishTutor.Infrastructure.Services.Gemini
{
    public interface IGeminiKeyManager
    {
        bool HasConfiguredKeys { get; }
        IReadOnlyList<string> GetModelsToTry();
        IReadOnlyList<string> GetOrderedApiKeys();
    }
}

