namespace AIChat
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support. Do not modify this method manually.
        /// </summary>
        private void InitializeComponent()
        {
            chatHistoryRichTextBox = new RichTextBox();
            userMessageTextBox = new TextBox();
            sendButton = new Button();
            SuspendLayout();
            // 
            // chatHistoryRichTextBox
            // 
            chatHistoryRichTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chatHistoryRichTextBox.Location = new Point(12, 12);
            chatHistoryRichTextBox.Name = "chatHistoryRichTextBox";
            chatHistoryRichTextBox.ReadOnly = true;
            chatHistoryRichTextBox.Size = new Size(760, 372);
            chatHistoryRichTextBox.TabIndex = 0;
            chatHistoryRichTextBox.Text = "";
            // 
            // userMessageTextBox
            // 
            userMessageTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            userMessageTextBox.Location = new Point(12, 399);
            userMessageTextBox.Name = "userMessageTextBox";
            userMessageTextBox.Size = new Size(649, 23);
            userMessageTextBox.TabIndex = 1;
            // 
            // sendButton
            // 
            sendButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            sendButton.Location = new Point(667, 398);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(105, 24);
            sendButton.TabIndex = 2;
            sendButton.Text = "Send";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += sendButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 441);
            Controls.Add(sendButton);
            Controls.Add(userMessageTextBox);
            Controls.Add(chatHistoryRichTextBox);
            MinimumSize = new Size(500, 300);
            Name = "Form1";
            Text = "AIChat";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox chatHistoryRichTextBox;
        private TextBox userMessageTextBox;
        private Button sendButton;
    }
}
