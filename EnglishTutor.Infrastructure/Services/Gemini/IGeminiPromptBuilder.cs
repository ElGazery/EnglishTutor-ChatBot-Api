using EnglishTutor.Application.Interfaces.Services;
using EnglishTutor.Domain.Enums;
using EnglishTutor.Infrastructure.Services.Gemini.Models;

namespace EnglishTutor.Infrastructure.Services.Gemini
{
    public interface IGeminiPromptBuilder
    {
        GeminiRequest BuildRequest(string question, AnswerMode mode, IReadOnlyList<ChatMessageContext> context);
        string GenerateOfflineResponse(string question, AnswerMode mode);
    }
}

