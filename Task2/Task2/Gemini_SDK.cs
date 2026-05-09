using DotNetEnv;
using Google.GenAI;

namespace Lesson_1_2_http
{
    public class Gemini_SDK
    {
        public static string[] GetModels()
        {
            return ["gemini-2.5-flash", "gemini-2.5-pro", "stub-other-provider-basic"];
        }

        public static async IAsyncEnumerable<string> CallStream(string model, string userMessage)
        {
            if (model.StartsWith("stub-", StringComparison.OrdinalIgnoreCase))
            {
                yield return "Stub model response";
                yield break;
            }

            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var geminiKey = Environment.GetEnvironmentVariable("GeminiAPIKey");
            var geminiModel = new Client(apiKey: geminiKey);

            await foreach (var response in geminiModel.Models.GenerateContentStreamAsync(model: model, contents: userMessage))
            {
                var text = response.Candidates?[0].Content?.Parts?[0].Text;
                if (!string.IsNullOrEmpty(text))
                {
                    yield return text;
                }
            }
        }
    }
}
