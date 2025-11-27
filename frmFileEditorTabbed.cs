using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Win32;      // added for registry access

namespace TransferUniFLEX
{

    //  What differentiates this tabbed File Editor from the regulat non-tabbed File Editor is that
    //  this guy needs to behave like the non-tabbed File Editor until a second file is sent to it to
    //  be displayed and edited. When this happens this form must create a tab control and a tab page
    //  and place the existing file that is already open on that tab page and then create a new tab
    //  page and put the new file request in it.
    //
    //  This also means that the form should only be created once if it does not already exists. If
    //  the form has already been create, The main dialog must use it to send a new file to instead
    //  of openning a new instance of the form.
    //
    //  We also need to not allow changing from tabbed to non-tabbed or non-tabbed to tabbed when the
    //  Editor button is clicked if there is already an instance of the form open.
    //  

    public partial class frmFileEditorTabbed : Form
    {
        #region external dll declarations
        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        #endregion

        #region variables

        // added for column marking

        public class ColumnSelection
        {
            public int StartLine;
            public int EndLine;
            public int StartColumn;
            public int EndColumn;
        }

        private bool isColumnSelect = false;
        private Point columnStartPoint;
        private Point columnEndPoint;
        private ColumnSelection currentColumnSelection = null;

        // end of added for column marking

        string dialogConfigType = "";
        string szTargetFileName = "";
        string szFile = "";

        string fileContent = "";
        string fileSavedAs = "";

        string title = "Internal Editor";

        private const int EM_LINESCROLL          = 0x00B6;
        private const int EM_GETFIRSTVISIBLELINE = 0xCE;
        private const int EM_SETUNDOLIMIT        = 0x00C5;

        // for constructing the status bar

        private StatusStrip statusStrip = null;
        private ToolStripStatusLabel statusLabel = null;

        int heightDifference = 110;
        private ToolStripStatusLabel positionLabel;

        frmFindDialog findDialog;
        int lastFindIndex = 0;

        string lastSearchText = "";
        private Font font;

        [DllImport("user32.dll")]
        static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        const int SB_VERT = 1;
        const int WM_VSCROLL = 0x0115;
        const int SB_THUMBPOSITION = 4;

        private Point GetScrollPos(RichTextBox rtb)
        {
            return new Point(0, GetScrollPos(rtb.Handle, SB_VERT));
        }

        private void SetScrollPos(RichTextBox rtb, Point scrollPos)
        {
            SetScrollPos(rtb.Handle, SB_VERT, scrollPos.Y, true);
            PostMessage(rtb.Handle, WM_VSCROLL, (IntPtr)(SB_THUMBPOSITION + 0x10000 * scrollPos.Y), IntPtr.Zero);
        }

        #endregion

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref Point lParam);

        private const int EM_GETSCROLLPOS = 0x0400 + 221;
        private const int EM_SETSCROLLPOS = 0x0400 + 222;

        private Point GetScrollPos()
        {
            Point pt = new Point();
            SendMessage(richTextBox.Handle, EM_GETSCROLLPOS, IntPtr.Zero, ref pt);
            return pt;
        }

        private void SetScrollPos(Point pt)
        {
            SendMessage(richTextBox.Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref pt);
        }

        #region column marking code

        private int GetColumnIndex(RichTextBox rtb, int charIndex)
        {
            int line = rtb.GetLineFromCharIndex(charIndex);
            int firstChar = rtb.GetFirstCharIndexFromLine(line);
            return charIndex - firstChar;
        }

        private void HighlightColumnSelection()
        {
            if (currentColumnSelection == null) return;

            var sel = currentColumnSelection;
            richTextBox.SelectionLength = 0;

            for (int i = sel.StartLine; i <= sel.EndLine; i++)
            {
                if (i >= richTextBox.Lines.Length) break;

                int lineStart = richTextBox.GetFirstCharIndexFromLine(i);
                int colStart = Math.Min(sel.StartColumn, richTextBox.Lines[i].Length);
                int colLen = Math.Min(sel.EndColumn - sel.StartColumn, richTextBox.Lines[i].Length - colStart);

                if (colLen > 0)
                {
                    richTextBox.Select(lineStart + colStart, colLen);
                    richTextBox.SelectionBackColor = Color.LightBlue;
                }
            }

            richTextBox.Select(richTextBox.SelectionStart, 0); // Deselect after highlighting
        }
        
        private void PerformColumnSelection(Point pt1, Point pt2)
        {
            int startIndex = richTextBox.GetCharIndexFromPosition(pt1);
            int endIndex = richTextBox.GetCharIndexFromPosition(pt2);

            int line1 = richTextBox.GetLineFromCharIndex(startIndex);
            int line2 = richTextBox.GetLineFromCharIndex(endIndex);
            int lineStart = Math.Min(line1, line2);
            int lineEnd = Math.Max(line1, line2);

            int col1 = GetColumnIndex(richTextBox, startIndex);
            int col2 = GetColumnIndex(richTextBox, endIndex);
            int colStart = Math.Min(col1, col2);
            int colEnd = Math.Max(col1, col2);

            currentColumnSelection = new ColumnSelection
            {
                StartLine = lineStart,
                EndLine = lineEnd,
                StartColumn = colStart,
                EndColumn = colEnd
            };

            // Optional visual simulation
            HighlightColumnSelection();
        }

        private void richTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isColumnSelect && e.Button == MouseButtons.Left)
            {
                columnEndPoint = e.Location;
                PerformColumnSelection(columnStartPoint, columnEndPoint);
            }
        }

        private void richTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Alt))
            {
                isColumnSelect = true;
                columnStartPoint = e.Location;
            }
        }

        private void richTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            UpdateCaretPosition(sender, e);
            isColumnSelect = false;
        }

        public void CopyColumnSelectionToClipboard()
        {
            if (currentColumnSelection == null)
                return;

            var sel = currentColumnSelection;
            StringBuilder sb = new StringBuilder();

            for (int i = sel.StartLine; i <= sel.EndLine; i++)
            {
                if (i >= richTextBox.Lines.Length)
                    break;

                string line = richTextBox.Lines[i];
                int start = Math.Min(sel.StartColumn, line.Length);
                int len = Math.Min(sel.EndColumn - sel.StartColumn, line.Length - start);

                if (len > 0)
                    sb.AppendLine(line.Substring(start, len));
                else
                    sb.AppendLine();
            }

            Clipboard.SetText(sb.ToString().TrimEnd('\r', '\n')); // avoids trailing blank line
        }

        private void DeleteColumnSelection()
        {
            if (currentColumnSelection == null)
                return;

            // this defines the block on the screen
            int startLine = currentColumnSelection.StartLine;
            int endLine = currentColumnSelection.EndLine;
            int startCol = currentColumnSelection.StartColumn;
            int endCol = currentColumnSelection.EndColumn;

            // if the selection was made from bottom right instead of top left
            if (startLine > endLine)
                (startLine, endLine) = (endLine, startLine);
            if (startCol > endCol)
                (startCol, endCol) = (endCol, startCol);

            // Store the original text before making changes
            string originalText = richTextBox.Text;

            // get the pointer in the text buffer to the first character to delete
            int originalSelectionStart = richTextBox.SelectionStart;
            int caretTarget = richTextBox.GetFirstCharIndexFromLine(startLine) + startCol;

            // Suspend updates
            richTextBox.SuspendLayout();

            // Split the entire text into lines
            var lines = richTextBox.Lines.ToList();

            for (int i = startLine; i <= endLine; i++)
            {
                // make sure we do not go past the end of the list
                if (i >= lines.Count)
                    break;

                // get the current line in the list pointed to by i
                string line = lines[i];
                if (startCol < line.Length)
                {
                    // get the number of characters to delete on this line and remove them
                    int deleteLength = Math.Min(endCol, line.Length) - startCol;
                    lines[i] = line.Remove(startCol, deleteLength);
                }
            }

            // Undo trick: replace text, then insert the modified version to make the entire op a single undo
            richTextBox.Undo(); // Clear previous change
            richTextBox.Text = originalText; // Restore original
            richTextBox.SelectionStart = 0;
            richTextBox.SelectionLength = richTextBox.TextLength;
            richTextBox.SelectedText = string.Join(Environment.NewLine, lines); // Replace with modified version

            // Restore caret manually
            richTextBox.SelectionStart = Math.Min(caretTarget, richTextBox.TextLength);
            richTextBox.SelectionLength = 0;

            CenterLineInRichTextBox(richTextBox, startLine);

            // Resume layout
            richTextBox.ResumeLayout();
        }

        public void ClearColumnHighlight()
        {
            if (currentColumnSelection == null)
                return;

            var sel = currentColumnSelection;
            currentColumnSelection = null;

            // Save current selection
            int originalSelectionStart = richTextBox.SelectionStart;
            int originalSelectionLength = richTextBox.SelectionLength;
            Color defaultBackColor = richTextBox.BackColor;

            // Clear background color line by line in the marked column area
            for (int i = sel.StartLine; i <= sel.EndLine; i++)
            {
                if (i >= richTextBox.Lines.Length)
                    break;

                string line = richTextBox.Lines[i];
                int startCol = Math.Min(sel.StartColumn, line.Length);
                int endCol = Math.Min(sel.EndColumn, line.Length);

                int lineStartIndex = richTextBox.GetFirstCharIndexFromLine(i);
                int charIndex = lineStartIndex + startCol;
                int length = Math.Max(0, endCol - startCol);

                if (length > 0 && charIndex + length <= richTextBox.TextLength)
                {
                    richTextBox.Select(charIndex, length);
                    richTextBox.SelectionBackColor = defaultBackColor;
                }
            }

            // Restore previous selection
            richTextBox.Select(originalSelectionStart, originalSelectionLength);
        }
        #endregion

        private void SetStatusBarVisibility(object sender, EventArgs e)
        {
            //// will be null on Form Load.
            //if (statusStrip != null)
            //{
            //    bool showStatusBar = (int)Program.preferencesKey.GetValue("show status bar", (object)false) == 0 ? false : true;

            //    if ((bool)showStatusBar)
            //    {
            //        // make room for it
            //        panelLineNumbers.Height = this.Height - heightDifference - statusStrip.Size.Height;
            //        richTextBox.Height = panelLineNumbers.Height;
            //        statusStrip.Visible = true;
            //        statusBarToolStripMenuItem.Checked = true;
            //    }
            //    else
            //    {
            //        // make the line number panel and richedit text box fill to the bottom of the form
            //        panelLineNumbers.Height = this.Height - heightDifference;
            //        richTextBox.Height = panelLineNumbers.Height;
            //        statusStrip.Visible = false;
            //        statusBarToolStripMenuItem.Checked = false;
            //    }
            //}
        }

        private void UpdateCaretPosition(object sender, EventArgs e)
        {
            try
            {
                int index = richTextBox.SelectionStart;
                int line = richTextBox.GetLineFromCharIndex(index);
                int firstCharIndex = richTextBox.GetFirstCharIndexFromLine(line);
                int column = index - firstCharIndex;

                // Calculate byte position using UTF-8 encoding (you can change this)
                byte[] bytes = Encoding.UTF8.GetBytes(richTextBox.Text.Substring(0, index));
                int bytePos = bytes.Length;

                // the positionLabel will be null on form load
                if (positionLabel != null)
                    positionLabel.Text = $"Ln {line + 1}, Col {column + 1}, Byte {bytePos}";
            }
            catch
            {

            }
        }

        private void richTextBox_SelectionChanged(object sender, EventArgs e)
        {
            UpdateCaretPosition(sender, e);
        }

        private void richTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                int caretBeforeUndo = richTextBox._caretBeforeUndo;
                Point scrollBeforeUndo = richTextBox._scrollBeforeUndo;

                richTextBox.BeginInvoke(new Action(() =>
                {
                    richTextBox.Select(caretBeforeUndo, 0); // Restore caret
                    SetScrollPos(richTextBox, scrollBeforeUndo); // Restore view (optional)
                    richTextBox.ScrollToCaret(); // Ensure caret stays visible
                }));
            }

            UpdateCaretPosition(sender, e);
        }

        private void EditorForm_KeyDown(object sender, KeyEventArgs e)
        {
            // shift-Insert to paste is shandled by the edit control and never gets here
            // control-shift-V to paste is shandled by the edit control and never gets here

            if (currentColumnSelection != null)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    ClearColumnHighlight();
                    currentColumnSelection = null;
                }
                else if (e.Control && e.KeyCode == Keys.Insert)              // control-Insert to copy
                {
                    // this gets hit if there is columnar text selected - not when normal selection i smade.
                    CopyColumnSelectionToClipboard();
                    ClearColumnHighlight();
                    currentColumnSelection = null;
                    e.Handled = true;
                }
                else if (e.Shift && e.KeyCode == Keys.Delete)
                {
                    CopyColumnSelectionToClipboard();
                    DeleteColumnSelection();
                    richTextBox.Select(richTextBox.SelectionStart, 0);
                    currentColumnSelection = null;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    DeleteColumnSelection();
                    richTextBox.Select(richTextBox.SelectionStart, 0);
                    currentColumnSelection = null;
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.Z)
                {
                    richTextBox._scrollBeforeUndo = GetScrollPos();
                    richTextBox._caretBeforeUndo = richTextBox.SelectionStart;

                    //((RichTextBoxWithTab)sender).PrepareForUndo();
                }
            }
            else
            {
                if (e.KeyCode == Keys.Escape)
                {
                    int caretIndex = richTextBox.SelectionStart;

                    int line = richTextBox.GetLineFromCharIndex(caretIndex);
                    int col = caretIndex - richTextBox.GetFirstCharIndexFromLine(line);

                    int firstVisibleLine = (int)SendMessage(richTextBox.Handle, EM_GETFIRSTVISIBLELINE, IntPtr.Zero, IntPtr.Zero);

                    // select everything and set the back color to white - then deselect everything

                    richTextBox.Select(0, richTextBox.TextLength);
                    richTextBox.BackColor = Color.White;
                    richTextBox.SelectionLength = 0;

                    SendMessage(richTextBox.Handle, EM_LINESCROLL, IntPtr.Zero, (IntPtr)firstVisibleLine);
                }
                else if (e.Control && e.KeyCode == Keys.Z)
                {
                    richTextBox._scrollBeforeUndo = GetScrollPos();
                    richTextBox._caretBeforeUndo = richTextBox.SelectionStart;

                    //((RichTextBoxWithTab)sender).PrepareForUndo();
                }
            }
        }

        private void frmFileEditorTabbed_Load(object sender, EventArgs e)
        {
            try
            {
                // to implement this we need to save the file locally and then start a transfer back to the remote.
                //      not yet implemented

                saveToImageToolStripMenuItem.Enabled = false;       // do not use this in TransferUniFLEX yet.

                SetControls();

                // load up the control - we can do this because the file we are loading has already
                // been checked for space compression and it has the spaces instead of the tabs.

                fileContent = File.ReadAllText(szTargetFileName);
                richTextBox.Text = fileContent;

                title = string.Format("Internal Editor - {0}", szTargetFileName);
                this.Text = title;

                // now set the font.
                int selectionStart = richTextBox.SelectionStart;
                int selectionLength = richTextBox.SelectionLength;

                // Change the font family name to the one set in selectedFontFamilyName from calling the dialog.

                richTextBox.SelectAll();

                //font = new Font(Program.selectedFontFamily, Program.selectedFontSize, richTextBox.SelectionFont.Style);
                //richTextBox.SelectionFont = font;
                //richTextBox.Font = font;
                //panelLineNumbers.Font = richTextBox.SelectionFont;

                //// Restore the selection start and length

                //richTextBox.Select(selectionStart, selectionLength);
                //labelSelectedFont.Text = font.FontFamily.Name;
                //labelSelectedFontSize.Text = font.Size.ToString();

                //object showLineNumbers = Program.preferencesKey.GetValue("show line numbers", 0);
                //checkBoxLineNumbers.Checked = (int)showLineNumbers == 1 ? true : false;

                //// set this one time for resizeing when the status bar is made visible and invisible.
                //heightDifference = this.Height - panelLineNumbers.Height;

                //// get the registry value for he key show status bar. If it has not been set yet = set it to false
                //bool showStatusBar = (int)Program.preferencesKey.GetValue("show status bar", 0) == 0 ? false : true;

                //// now - in case it dis not exist - make it exist - as false since it did not exist
                //if (!showStatusBar)
                //    Program.preferencesKey.SetValue("show status bar", 0, RegistryValueKind.DWord);

                //// create the status strip
                //statusStrip = new StatusStrip();
                //// Create a status label
                //statusLabel = new ToolStripStatusLabel();
                //statusLabel.Text = "Ready";

                //// Add the label to the status strip
                //statusStrip.Items.Add(statusLabel);

                //// Dock the status strip to the bottom
                //statusStrip.Dock = DockStyle.Bottom;

                //// Create the "spring" label for pushing the position label to the far right
                //ToolStripStatusLabel spacer = new ToolStripStatusLabel();
                //spacer.Spring = true; // This takes up all extra space

                //positionLabel = new ToolStripStatusLabel("Ln 1, Col 1, Byte 0");
                //statusStrip.Items.Add(spacer);        // Left side filler
                //statusStrip.Items.Add(positionLabel);

                //// Add the status strip to the form's controls
                //this.Controls.Add(statusStrip);

                //// start out with it not visible
                //statusStrip.Visible = false;

                //SetStatusBarVisibility(sender, e);

                //int x = (int)Program.preferencesKey.GetValue("editor position left", Program.mainForm.Left);
                //int y = (int)Program.preferencesKey.GetValue("editor position top", Program.mainForm.Top);
                //int width = (int)Program.preferencesKey.GetValue("editor position width", Program.mainForm.Width);
                //int height = (int)Program.preferencesKey.GetValue("editor position height", Program.mainForm.Height);

                //this.StartPosition = FormStartPosition.Manual;
                //this.SetBounds(x, y, width, height);
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                MessageBox.Show(message);
            }

            this.KeyPreview = true;
            this.KeyDown += EditorForm_KeyDown;
        }

        private int GetCurrentColumn(RichTextBox rtb, int selectionStart)
        {
            int lineIndex = rtb.GetLineFromCharIndex(selectionStart);
            int firstCharIndex = rtb.GetFirstCharIndexFromLine(lineIndex);
            return selectionStart - firstCharIndex;
        }

        private void richTextBoxSource_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab && !e.Shift)
            {
                const int tabSize = 4;

                var rtb = sender as RichTextBoxWithTab;
                int selectionStart = rtb.SelectionStart;

                // Calculate how many spaces until the next tab stop
                int column = GetCurrentColumn(rtb, selectionStart);
                int spacesToInsert = tabSize - (column % tabSize);

                rtb.SelectedText = new string(' ', spacesToInsert);
                e.Handled = true;
            }
        }

        private void CenterToCaret ()
        {
            int caretPosition = richTextBox.SelectionStart;
            int lineNumber = richTextBox.GetLineFromCharIndex(caretPosition);
            CenterLineInRichTextBox(richTextBox, lineNumber);
        }

        private void buttonSelectFont_Click(object sender, EventArgs e)
        {
            SelectFont(sender, e);
        }

        private void SelectFont(object sender, EventArgs e)
        {
            frmDialogSelectFont dlg = new frmDialogSelectFont(Program.selectedFontFamily, Program.selectedFontSize);
            DialogResult result = dlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (dlg.selectedFontFamilyName == "<None Specified>")
                {
                    Program.preferencesKey.DeleteValue("Source Font Family");
                    Program.selectedFontFamily = Program.defaultFontFamilyName;
                    Program.selectedFontSize = Program.defaultFontSize;
                }
                else
                {
                    Program.selectedFontFamily = dlg.selectedFontFamilyName;
                    Program.preferencesKey.SetValue("Source Font Family", dlg.selectedFontFamilyName);

                    // change the font on all of the text in all of the edit boxes

                    // save any existing selection

                    int rtbSelectionStart = richTextBox.SelectionStart;
                    int rtbSelectionLength = richTextBox.SelectionLength;

                    // Change the font family name to the one set in selectedFontFamilyName from calling the dialog.

                    richTextBox.SelectAll();

                    font = new Font(Program.selectedFontFamily, Program.selectedFontSize, richTextBox.SelectionFont.Style);
                    richTextBox.SelectionFont = font;
                    richTextBox.Font = font;
                    panelLineNumbers.Font = richTextBox.SelectionFont;

                    // Restore the selection start and length

                    richTextBox.Select(rtbSelectionStart, rtbSelectionLength);
                }

                labelSelectedFont.Text = font.FontFamily.Name;
                labelSelectedFontSize.Text = font.Size.ToString();
            }
        }

        private int GetFirstVisibleLineIndex()
        {
            return SendMessage(richTextBox.Handle, EM_GETFIRSTVISIBLELINE, IntPtr.Zero, IntPtr.Zero);
        }

        private void panelLineNumbers_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(panelLineNumbers.BackColor);

            using (Font font = richTextBox.SelectionFont)
            {
                //int lineHeight = richTextBox.SelectionFont.Height;
                int lineHeight = TextRenderer.MeasureText("A", font).Height;
                int startLine = GetFirstVisibleLineIndex();
                int visibleLines = panelLineNumbers.Height / lineHeight;
                int totalLines = richTextBox.Lines.Length;

                if (richTextBox.Text.EndsWith("\n"))
                    totalLines++;

                using (Brush brush = new SolidBrush(Color.Gray))
                {
                    for (int i = 0; i <= visibleLines; i++)
                    {
                        int lineNumber = startLine + i;
                        if (lineNumber >= totalLines)
                            break;

                        string lineNumText = (lineNumber + 1).ToString();
                        SizeF size = e.Graphics.MeasureString(lineNumText, font);
                        float x = panelLineNumbers.Width - size.Width - 4;
                        float y = i * lineHeight;

                        e.Graphics.DrawString(lineNumText, font, brush, x, y);
                    }
                }
            }
        }

        private void SetControls ()
        {
            if (checkBoxLineNumbers.Checked)
            {
                // make the line numebrs panel visible and schutch the rishTextBox over a bit by making it smaller
                // by the width of the line number panel. But only if the line number panel is not currently visible.

                if (!panelLineNumbers.Visible)
                {
                    panelLineNumbers.Visible = true;
                    int currentWidth = richTextBox.Width;
                    int newWidth = currentWidth - 40;

                    richTextBox.Width = newWidth;
                    richTextBox.Left = richTextBox.Left + 40;
                }
            }
            else
            {
                // make the lineNumberPanel not visible and make the richtextbox bigger and move it to the left,
                // but only if the line number panel is being shown

                if (panelLineNumbers.Visible)
                {
                    panelLineNumbers.Visible = false;
                    int currentWidth = richTextBox.Width;
                    int newWidth = currentWidth + 40;

                    richTextBox.Left = richTextBox.Left - 40;
                    richTextBox.Width = newWidth;
                }
            }
        }

        private void EnableLineNumbers (object sender)
        {
            SetControls();

            // let's make the checkbox state selection sticky - remember the last selection in the registry
            //Program.preferencesKey.SetValue("show line numbers", checkBoxLineNumbers.Checked ? 1 : 0, RegistryValueKind.DWord);
        }

        private void checkBoxLineNumbers_CheckedChanged(object sender, EventArgs e)
        {
            EnableLineNumbers(sender);
        }

        private void richTextBox_FontChanged(object sender, EventArgs e)
        {
            panelLineNumbers.Font = richTextBox.SelectionFont;
        }

        private void CenterLineInRichTextBox(RichTextBox rtb, int targetLine)
        {
            if (targetLine < 0 || targetLine >= rtb.Lines.Length)
                return;

            int visibleLines = rtb.Height / rtb.Font.Height;
            int topLine = Math.Max(0, targetLine - (visibleLines / 2));

            // Move the caret to the topLine and scroll manually to that line
            int topCharIndex = rtb.GetFirstCharIndexFromLine(topLine);
            rtb.SelectionStart = topCharIndex;
            rtb.SelectionLength = 0;

            // This scrolls the topLine into view
            rtb.ScrollToCaret();

            // Now move the caret to the actual target line,
            // but DO NOT call ScrollToCaret again — just set it
            int targetCharIndex = rtb.GetFirstCharIndexFromLine(targetLine);
            rtb.SelectionStart = targetCharIndex;
            rtb.SelectionLength = 0;
        }

        private void frmFileEditorTabbed_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Program.mainForm.Focus();
            //Program.fileEditorTabbedDialog = null;
        }
    }
}
