using System.Globalization;
using System.Text.Json;
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
                yield return "Stub model response. Configure GEMINI_API_KEY to use the full AI chat with tools.";
                yield break;
            }

            var geminiKey = GetGeminiApiKey();
            if (string.IsNullOrWhiteSpace(geminiKey))
            {
                throw new InvalidOperationException("Set GEMINI_API_KEY or GeminiAPIKey in your environment or in a .env file.");
            }

            var geminiModel = new Client(apiKey: geminiKey);
            var calculatorTools = new CalculatorTools();
            var unitConverterTools = new UnitConverterTools();
            var calculateExpression = new FunctionDeclaration
            {
                Name = "CalculateExpression",
                Description = "Evaluate an arithmetic expression for the user.",
                Parameters = new Schema
                {
                    Type = Google.GenAI.Types.Type.OBJECT,
                    Properties = new Dictionary<string, Schema>
                    {
                        ["expression"] = new()
                        {
                            Type = Google.GenAI.Types.Type.STRING,
                            Description = "An arithmetic expression such as (12.5*3)+8/2"
                        }
                    },
                    Required = ["expression"]
                }
            };
            var convertUnits = new FunctionDeclaration
            {
                Name = "ConvertUnits",
                Description = "Convert a numeric value from one unit to another.",
                Parameters = new Schema
                {
                    Type = Google.GenAI.Types.Type.OBJECT,
                    Properties = new Dictionary<string, Schema>
                    {
                        ["value"] = new()
                        {
                            Type = Google.GenAI.Types.Type.NUMBER,
                            Description = "The numeric value to convert"
                        },
                        ["fromUnit"] = new()
                        {
                            Type = Google.GenAI.Types.Type.STRING,
                            Description = "The source unit, for example km, m, cm, mm, mi, ft, in, kg, g, lb, oz, celsius, fahrenheit, kelvin"
                        },
                        ["toUnit"] = new()
                        {
                            Type = Google.GenAI.Types.Type.STRING,
                            Description = "The target unit, for example km, m, cm, mm, mi, ft, in, kg, g, lb, oz, celsius, fahrenheit, kelvin"
                        }
                    },
                    Required = ["value", "fromUnit", "toUnit"]
                }
            };
            var tool = new Tool
            {
                FunctionDeclarations = [calculateExpression, convertUnits]
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
                                   You are a helpful assistant in a WinForms chat application.
                                   Use CalculateExpression whenever the user asks for a calculation or arithmetic result.
                                   Use ConvertUnits whenever the user asks to convert a value between supported units.
                                   Supported unit groups are length, weight and temperature.
                                   If no tool is needed, answer normally.
                                   """
                        }
                    ]
                },
                Tools = [tool]
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
                    foreach (var chunk in StreamText(string.IsNullOrWhiteSpace(text) ? "The model returned an empty response." : text))
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
                        "CalculateExpression" => calculatorTools.Calculate(GetStringArgument(call, "expression")),
                        "ConvertUnits" => unitConverterTools.ConvertUnits(GetDoubleArgument(call, "value"), GetStringArgument(call, "fromUnit"), GetStringArgument(call, "toUnit")),
                        _ => "Unknown tool: " + call.Name
                    };

                    toolOutput.Parts.Add(new Part
                    {
                        FunctionResponse = new FunctionResponse
                        {
                            Id = call.Id,
                            Name = call.Name,
                            Response = new Dictionary<string, object> { ["result"] = toolResult }
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

        private static string? GetGeminiApiKey()
        {
            var geminiKey = System.Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (!string.IsNullOrWhiteSpace(geminiKey))
            {
                return geminiKey;
            }

            geminiKey = System.Environment.GetEnvironmentVariable("GeminiAPIKey");
            if (!string.IsNullOrWhiteSpace(geminiKey))
            {
                return geminiKey;
            }

            try
            {
                Env.TraversePath().Load();
            }
            catch
            {
                Env.Load();
            }

            geminiKey = System.Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            return string.IsNullOrWhiteSpace(geminiKey) ? System.Environment.GetEnvironmentVariable("GeminiAPIKey") : geminiKey;
        }

        private static string GetStringArgument(FunctionCall call, string argumentName)
        {
            if (call.Args is null || !call.Args.TryGetValue(argumentName, out var value) || value is null)
            {
                return string.Empty;
            }

            return value switch
            {
                string stringValue => stringValue,
                JsonElement jsonElement when jsonElement.ValueKind == JsonValueKind.String => jsonElement.GetString() ?? string.Empty,
                JsonElement jsonElement => jsonElement.GetRawText(),
                _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
            };
        }

        private static double GetDoubleArgument(FunctionCall call, string argumentName)
        {
            if (call.Args is null || !call.Args.TryGetValue(argumentName, out var value) || value is null)
            {
                return 0;
            }

            return value switch
            {
                double doubleValue => doubleValue,
                float floatValue => floatValue,
                int intValue => intValue,
                long longValue => longValue,
                decimal decimalValue => (double)decimalValue,
                JsonElement jsonElement when jsonElement.ValueKind == JsonValueKind.Number => jsonElement.GetDouble(),
                JsonElement jsonElement when jsonElement.ValueKind == JsonValueKind.String && double.TryParse(jsonElement.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedFromJson) => parsedFromJson,
                string stringValue when double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedFromString) => parsedFromString,
                _ => 0
            };
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
