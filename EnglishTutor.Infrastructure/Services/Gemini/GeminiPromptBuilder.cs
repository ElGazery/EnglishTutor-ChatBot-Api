using EnglishTutor.Application.Interfaces.Services;
using EnglishTutor.Domain.Enums;
using EnglishTutor.Infrastructure.Services.Gemini.Models;

namespace EnglishTutor.Infrastructure.Services.Gemini
{
    public class GeminiPromptBuilder : IGeminiPromptBuilder
    {
        public GeminiRequest BuildRequest(string question, AnswerMode mode, IReadOnlyList<ChatMessageContext> context)
        {
            return new GeminiRequest
            {
                SystemInstruction = new GeminiContent
                {
                    Parts = new List<GeminiPart> { new() { Text = BuildSystemInstruction(mode) } }
                },
                Contents = BuildContents(question, context),
                GenerationConfig = new GeminiGenerationConfig
                {
                    Temperature = mode == AnswerMode.Quick ? 0.1 : 0.4,
                    MaxOutputTokens = mode == AnswerMode.Quick ? 300 : 8192
                }
            };
        }

        public string GenerateOfflineResponse(string question, AnswerMode mode)
        {
            if (mode == AnswerMode.Quick)
            {
                return $"Answer: {question.Trim()} (Offline mode — configure a Gemini API key in appsettings.json for live responses).";
            }

            return $@"### شرح توضيحي (وضع تجريبي بدون مفتاح API)

**سؤالك:** {question}

> **ملاحظة:** يعمل النظام حالياً بالوضع غير المتصل لعدم ضبط مفتاح API في الإعدادات. يرجى تفعيل مفتاح Gemini API للحصول على شرح ذكي وتفاعلي متكامل.

#### أمثلة توضيحية عامة:
- **Present Simple:** *She plays tennis every Friday.*
- **Past Simple:** *They went to school yesterday.*";
        }

        private static string BuildSystemInstruction(AnswerMode mode)
        {
            const string basePrompt = @"You are a friendly, encouraging, and expert AI English Tutor dedicated to preparatory-school students (middle school / ages 12 to 15).
You specialize in English grammar, vocabulary, tenses, prepositions, translation between English and Arabic, sentence correction, and spelling.";

            if (mode == AnswerMode.Quick)
            {
                return basePrompt + @"

MODE: QUICK MODE.
- Return ONLY the shortest direct correct answer.
- Give just one or two words or the single corrected word/phrase whenever appropriate (e.g., 'went', 'have eaten', 'at').
- DO NOT add full sentences, greetings, filler text, or explanations. Keep it as brief and instant as possible.";
            }

            return basePrompt + @"

MODE: DETAILED MODE.
- Explain the answer clearly, gently, and step-by-step for a preparatory-school student.
- Provide simple explanations in Arabic (شرح مبسط وواضح باللغة العربية) to help them understand the rule.
- Provide clear English example sentences with simple everyday vocabulary.
- Highlight the key rule (قاعدة الدرس) and common mistakes to avoid (أخطاء شائعة تجنبها).
- Use clean Markdown with bullet points and bold text for readability.
- IMPORTANT: Always conclude and complete your full explanation. Never stop or truncate mid-sentence. Ensure all sections, examples, and practice questions are completely finished.";
        }

        private static List<GeminiContent> BuildContents(string question, IReadOnlyList<ChatMessageContext> context)
        {
            var contents = new List<GeminiContent>();

            foreach (var item in context)
            {
                var isUser = item.Role.Equals("Student", StringComparison.OrdinalIgnoreCase)
                    || item.Role.Equals("user", StringComparison.OrdinalIgnoreCase);

                contents.Add(new GeminiContent
                {
                    Role = isUser ? "user" : "model",
                    Parts = new List<GeminiPart> { new() { Text = item.Content } }
                });
            }

            contents.Add(new GeminiContent
            {
                Role = "user",
                Parts = new List<GeminiPart> { new() { Text = question } }
            });

            return contents;
        }
    }
}

