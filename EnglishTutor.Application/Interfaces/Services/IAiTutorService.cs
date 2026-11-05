using EnglishTutor.Domain.Enums;

namespace EnglishTutor.Application.Interfaces.Services
{
    public interface IAiTutorService
    {
        Task<string> GetResponseAsync( string question, AnswerMode mode, IReadOnlyList<ChatMessageContext> context, CancellationToken cancellationToken = default);
    }

    public record ChatMessageContext(string Role, string Content);
}
