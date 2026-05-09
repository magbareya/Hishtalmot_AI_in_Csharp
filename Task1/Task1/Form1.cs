using Lesson_1_2_http;

namespace AIChat
{
    public partial class Form1 : Form
    {
        private static readonly object GeminiCallLock = new();

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Course step: take the user input, show it in the chat area,
        /// call Gemini through Gemini_SDK, and then show the model response.
        /// </summary>
        private async void sendButton_Click(object sender, EventArgs e)
        {
            var userMessage = userMessageTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return;
            }

            AppendMessage("You", userMessage);
            userMessageTextBox.Clear();

            sendButton.Enabled = false;
            userMessageTextBox.Enabled = false;

            try
            {
                var aiResponse = await GetAiResponseAsync(userMessage);
                AppendMessage("AI", aiResponse);
            }
            catch (Exception ex)
            {
                AppendMessage("AI", $"Error: {ex.Message}");
            }
            finally
            {
                sendButton.Enabled = true;
                userMessageTextBox.Enabled = true;
                userMessageTextBox.Focus();
            }
        }

        /// <summary>
        /// Course step: keep the conversation visible in a scrollable chat area.
        /// </summary>
        private void AppendMessage(string role, string message)
        {
            chatHistoryRichTextBox.AppendText($"{role}: {message}{Environment.NewLine}{Environment.NewLine}");
            chatHistoryRichTextBox.SelectionStart = chatHistoryRichTextBox.TextLength;
            chatHistoryRichTextBox.ScrollToCaret();
        }

        /// <summary>
        /// Course step: use Gemini_SDK as-is.
        /// Gemini_SDK reads Console input, so we provide the form message through Console.In.
        /// </summary>
        private static async Task<string> GetAiResponseAsync(string userMessage)
        {
            TextReader originalInput;
            lock (GeminiCallLock)
            {
                originalInput = Console.In;
                Console.SetIn(new StringReader(userMessage + Environment.NewLine));
            }

            try
            {
                return await Gemini_SDK.Call();
            }
            finally
            {
                lock (GeminiCallLock)
                {
                    Console.SetIn(originalInput);
                }
            }
        }
    }
}
