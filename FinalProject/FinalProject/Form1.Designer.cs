namespace AIChat
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            chatHistoryRichTextBox = new RichTextBox();
            userMessageTextBox = new TextBox();
            sendButton = new Button();
            modelComboBox = new ComboBox();
            clearMemoryButton = new Button();
            toolsLabel = new Label();
            SuspendLayout();
            chatHistoryRichTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chatHistoryRichTextBox.Location = new Point(12, 58);
            chatHistoryRichTextBox.Name = "chatHistoryRichTextBox";
            chatHistoryRichTextBox.ReadOnly = true;
            chatHistoryRichTextBox.Size = new Size(760, 326);
            chatHistoryRichTextBox.TabIndex = 0;
            chatHistoryRichTextBox.Text = "";
            userMessageTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            userMessageTextBox.Location = new Point(12, 401);
            userMessageTextBox.Name = "userMessageTextBox";
            userMessageTextBox.Size = new Size(649, 23);
            userMessageTextBox.TabIndex = 3;
            userMessageTextBox.KeyDown += userMessageTextBox_KeyDown;
            sendButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            sendButton.Location = new Point(667, 400);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(105, 24);
            sendButton.TabIndex = 4;
            sendButton.Text = "Send";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += sendButton_Click;
            modelComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            modelComboBox.FormattingEnabled = true;
            modelComboBox.Location = new Point(12, 12);
            modelComboBox.Name = "modelComboBox";
            modelComboBox.Size = new Size(262, 23);
            modelComboBox.TabIndex = 1;
            clearMemoryButton.Location = new Point(280, 11);
            clearMemoryButton.Name = "clearMemoryButton";
            clearMemoryButton.Size = new Size(133, 24);
            clearMemoryButton.TabIndex = 2;
            clearMemoryButton.Text = "Clear Chat";
            clearMemoryButton.UseVisualStyleBackColor = true;
            clearMemoryButton.Click += clearMemoryButton_Click;
            toolsLabel.AutoSize = true;
            toolsLabel.Location = new Point(12, 39);
            toolsLabel.Name = "toolsLabel";
            toolsLabel.Size = new Size(308, 15);
            toolsLabel.TabIndex = 5;
            toolsLabel.Text = "Built-in tools: Calculator and Unit Converter (length, weight, temperature)";
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 441);
            Controls.Add(toolsLabel);
            Controls.Add(clearMemoryButton);
            Controls.Add(modelComboBox);
            Controls.Add(sendButton);
            Controls.Add(userMessageTextBox);
            Controls.Add(chatHistoryRichTextBox);
            MinimumSize = new Size(500, 300);
            Name = "Form1";
            Text = "Final Project AI Chat";
            ResumeLayout(false);
            PerformLayout();
        }

        private RichTextBox chatHistoryRichTextBox;
        private TextBox userMessageTextBox;
        private Button sendButton;
        private ComboBox modelComboBox;
        private Button clearMemoryButton;
        private Label toolsLabel;
    }
}
