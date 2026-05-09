using System.Text;

namespace AIChat
{
    public partial class Form1 : Form
    {
        private const int MaxMemoryMessages = 20;
        private readonly List<(string Role, string Content)> conversationMemory = new();
        private readonly string[] availableModels;

        public Form1()
        {
            InitializeComponent();
            availableModels = Gemini_SDK.GetModels();
            modelComboBox.Items.AddRange(availableModels);
            modelComboBox.SelectedIndex = 0;
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            var userMessage = userMessageTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return;
            }

            AppendMessage("You", userMessage);
            conversationMemory.Add(("You", userMessage));
            userMessageTextBox.Clear();

            SetUiEnabled(false);

            try
            {
                var aiResponse = await StreamAiResponseAsync();
                conversationMemory.Add(("AI", aiResponse));
                chatHistoryRichTextBox.AppendText(Environment.NewLine + Environment.NewLine);
                ScrollChatToEnd();
            }
            catch (Exception ex)
            {
                AppendMessage("AI", $"Error: {ex.Message}");
            }
            finally
            {
                SetUiEnabled(true);
                userMessageTextBox.Focus();
            }
        }

        private async Task<string> StreamAiResponseAsync()
        {
            var model = modelComboBox.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(model))
            {
                model = availableModels[0];
            }

            chatHistoryRichTextBox.AppendText("AI: ");
            var responseBuilder = new StringBuilder();

            await foreach (var chunk in Gemini_SDK.CallStream(model, BuildPromptFromMemory()))
            {
                if (string.IsNullOrEmpty(chunk))
                {
                    continue;
                }

                responseBuilder.Append(chunk);
                chatHistoryRichTextBox.AppendText(chunk);
                ScrollChatToEnd();
            }

            return responseBuilder.ToString();
        }

        private string BuildPromptFromMemory()
        {
            return string.Join(Environment.NewLine, conversationMemory.TakeLast(MaxMemoryMessages).Select(message => $"{message.Role}: {message.Content}"));
        }

        private void clearMemoryButton_Click(object sender, EventArgs e)
        {
            conversationMemory.Clear();
            chatHistoryRichTextBox.Clear();
            userMessageTextBox.Focus();
        }

        private void userMessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;
            if (sendButton.Enabled)
            {
                sendButton.PerformClick();
            }
        }

        private void AppendMessage(string role, string message)
        {
            chatHistoryRichTextBox.AppendText($"{role}: {message}{Environment.NewLine}{Environment.NewLine}");
            ScrollChatToEnd();
        }

        private void ScrollChatToEnd()
        {
            chatHistoryRichTextBox.SelectionStart = chatHistoryRichTextBox.TextLength;
            chatHistoryRichTextBox.ScrollToCaret();
        }

        private void SetUiEnabled(bool enabled)
        {
            sendButton.Enabled = enabled;
            userMessageTextBox.Enabled = enabled;
            modelComboBox.Enabled = enabled;
            clearMemoryButton.Enabled = enabled;
        }
    }
}
