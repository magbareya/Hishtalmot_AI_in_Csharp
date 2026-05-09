using DotNetEnv;
using Google.GenAI;
using Google.GenAI.Types;

namespace AIChat
{
    public class Gemini_SDK
    {
        private const int MaxToolSteps = 5;
        private const int StreamChunkSize = 24;

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

            var geminiKey = System.Environment.GetEnvironmentVariable("GeminiAPIKey");
            if (string.IsNullOrWhiteSpace(geminiKey))
            {
                Env.Load();
                geminiKey = System.Environment.GetEnvironmentVariable("GeminiAPIKey");
            }

            var geminiModel = new Client(apiKey: geminiKey);
            var dateTimeTools = new DateTimeTools();
            var utilityTools = new UtilityTools();
            var noParamsSchema = new Schema();

            var getDate = new FunctionDeclaration
            {
                Name = "GetDate",
                Description = "Get today's date",
                Parameters = noParamsSchema
            };
            var getTime = new FunctionDeclaration
            {
                Name = "GetTime",
                Description = "Get the current time",
                Parameters = noParamsSchema
            };
            var getRandomNumber = new FunctionDeclaration
            {
                Name = "GetRandomNumber",
                Description = "Get a random number between 1 and 100",
                Parameters = noParamsSchema
            };
            var getGuid = new FunctionDeclaration
            {
                Name = "GetGuid",
                Description = "Get a unique identifier",
                Parameters = noParamsSchema
            };

            var dateTimeTool = new Tool
            {
                FunctionDeclarations = [getDate, getTime]
            };
            var utilityTool = new Tool
            {
                FunctionDeclarations = [getRandomNumber, getGuid]
            };

            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts =
                    [
                        new Part
                        {
                            Text = """
                                   You may call tools when needed.
                                   Tool 1: Date/Time tool with GetDate and GetTime.
                                   Tool 2: Utility tool with GetRandomNumber and GetGuid.
                                   Prefer concise final answers.
                                   """
                        }
                    ]
                },
                Tools = [dateTimeTool, utilityTool]
            };

            var history = new List<Content>
            {
                new()
                {
                    Role = "user",
                    Parts = [new Part { Text = userMessage }]
                }
            };

            for (var step = 0; step < MaxToolSteps; step++)
            {
                var response = await geminiModel.Models.GenerateContentAsync(model: model, contents: history, config: config);
                var content = response.Candidates?[0].Content;
                if (content is null)
                {
                    yield break;
                }

                history.Add(content);

                var toolCalls = content.Parts?.Where(part => part.FunctionCall is not null).ToList() ?? [];
                if (toolCalls.Count == 0)
                {
                    var text = string.Concat(content.Parts?.Select(part => part.Text).Where(textPart => !string.IsNullOrEmpty(textPart)) ?? []);
                    foreach (var chunk in StreamText(text))
                    {
                        yield return chunk;
                    }
                    yield break;
                }

                var toolOutput = new Content
                {
                    Role = "user",
                    Parts = new List<Part>()
                };

                foreach (var part in toolCalls)
                {
                    var call = part.FunctionCall!;
                    var toolResult = call.Name switch
                    {
                        "GetDate" => dateTimeTools.GetDate(),
                        "GetTime" => dateTimeTools.GetTime(),
                        "GetRandomNumber" => utilityTools.GetRandomNumber(),
                        "GetGuid" => utilityTools.GetGuid(),
                        _ => "Unknown tool: " + call.Name
                    };

                    toolOutput.Parts.Add(new Part
                    {
                        FunctionResponse = new FunctionResponse
                        {
                            Name = call.Name,
                            Response = new Dictionary<string, object> { { "result", toolResult } }
                        }
                    });
                }

                if (step == MaxToolSteps - 1)
                {
                    toolOutput.Parts.Add(new Part
                    {
                        Text = "Max tool steps reached. No more tool calls are allowed. Reply normally with your best final answer using the information you already have."
                    });
                }

                history.Add(toolOutput);
            }

            yield return "Max iterations reached.";
        }

        private static IEnumerable<string> StreamText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                yield break;
            }

            for (var i = 0; i < text.Length; i += StreamChunkSize)
            {
                var length = Math.Min(StreamChunkSize, text.Length - i);
                yield return text.Substring(i, length);
            }
        }
    }
}
