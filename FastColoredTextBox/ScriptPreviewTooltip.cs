using System;
using System.Drawing;
using System.Windows.Forms;

namespace FastColoredTextBoxNS
{
    /// <summary>
    /// Custom tooltip that displays syntax-highlighted script preview
    /// </summary>
    internal class ScriptPreviewTooltip : Form
    {
        private FastColoredTextBox previewBox;
        private SyntaxHighlighter syntaxHighlighter;
        private readonly Timer hideTimer;
        private bool isShowing;

        public ScriptPreviewTooltip()
        {
            // Form setup
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            BackColor = Color.FromArgb(43, 43, 43); // Dark background
            Padding = new Padding(2);
            
            // Create the preview text box
            previewBox = new FastColoredTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                ShowLineNumbers = true,
                BackColor = Color.FromArgb(30, 30, 30), // Dark editor background
                ForeColor = Color.White,
                LineNumberColor = Color.Gray,
                IndentBackColor = Color.FromArgb(43, 43, 43),
                SelectionColor = Color.Transparent,
                CurrentLineColor = Color.Transparent,
                CaretVisible = false,
                Font = new Font("Consolas", 9),
                WordWrap = false,
                ShowScrollBars = false,
                Cursor = Cursors.Default,
                Language = Language.Razor
            };
            
            // Initialize syntax highlighter
            syntaxHighlighter = new SyntaxHighlighter(previewBox);
            syntaxHighlighter.InitStyleSchema(Language.Razor);
            
            // Disable interaction
            previewBox.TabStop = false;
            
            Controls.Add(previewBox);
            
            // Auto-hide timer
            hideTimer = new Timer { Interval = 5000 };
            hideTimer.Tick += (s, e) => HideTooltip();
        }

        /// <summary>
        /// Show the tooltip with syntax-highlighted script content
        /// </summary>
        public void ShowTooltip(string scriptContent, Point location, int maxWidth = 500, int maxHeight = 400)
        {
            if (string.IsNullOrEmpty(scriptContent))
                return;

            isShowing = true;
            
            // Set the script text
            previewBox.Text = scriptContent;
            previewBox.ClearUndo();
            
            // Apply syntax highlighting using SyntaxHighlighter
            syntaxHighlighter.RazorSyntaxHighlight(previewBox.Range);
            
            // Calculate size based on content
            int lines = previewBox.LinesCount;
            int contentHeight = Math.Min(lines * previewBox.CharHeight + 10, maxHeight);
            int contentWidth = Math.Min(maxWidth, maxWidth);
            
            Size = new Size(contentWidth, contentHeight);
            Location = location;
            
            // Show the tooltip
            Show();
            hideTimer.Start();
        }

        /// <summary>
        /// Hide the tooltip
        /// </summary>
        public void HideTooltip()
        {
            if (!isShowing)
                return;
                
            isShowing = false;
            hideTimer.Stop();
            Hide();
            previewBox.Text = string.Empty;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                hideTimer?.Dispose();
                syntaxHighlighter?.Dispose();
                previewBox?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000008; // WS_EX_TOPMOST
                cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW
                return cp;
            }
        }
    }
}
