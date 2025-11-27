using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace TransferUniFLEX
{ 
    public class RichTextBoxWithTab : RichTextBox
    {
        public int TabSize { get; set; } = 4;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref Point lParam);

        private const int EM_GETSCROLLPOS = 0x0400 + 221;
        private const int EM_SETSCROLLPOS = 0x0400 + 222;

        public Point _scrollBeforeUndo;
        public int _caretBeforeUndo = 0;

        private const int SB_VERT = 1;
        private const int WM_VSCROLL = 0x0115;
        private const int SB_THUMBPOSITION = 4;

        private void RestoreAfterUndo(object sender, EventArgs e)
        {
            Application.Idle -= RestoreAfterUndo;

            // Step 1: Restore scroll and caret
            SetScrollPos(_scrollBeforeUndo);
            this.Select(_caretBeforeUndo, 0);
            this.ScrollToCaret();

            // Step 2: Forcefully clear any selection after undo
            this.BeginInvoke(new Action(() =>
            {
                int caret = this.SelectionStart;
                this.Select(caret, 0);
                this.ScrollToCaret();
            }));
        }

        private void SetScrollPos(Point pt)
        {
            SendMessage(this.Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref pt);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x0100;

            if (msg.Msg == WM_KEYDOWN && keyData == Keys.Tab)
            {
                int selectionStart = this.SelectionStart;
                int column = GetCurrentColumn(selectionStart);
                int spacesToInsert = TabSize - (column % TabSize);
                this.SelectedText = new string(' ', spacesToInsert);
                return true; // suppress default tab behavior
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private int GetCurrentColumn(int selectionStart)
        {
            int lineIndex = this.GetLineFromCharIndex(selectionStart);
            int lineStart = this.GetFirstCharIndexFromLine(lineIndex);
            return selectionStart - lineStart;
        }
    }
}
