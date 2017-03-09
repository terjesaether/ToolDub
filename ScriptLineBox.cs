// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.ScriptLineBox
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Nordubb.DubTool
{
    [DesignerCategory("")]
    internal class ScriptLineBox : UserControl
    {
        private CountdownTimer m_timer;
        private RichTextBox m_characterbox;
        private RichTextBox m_timebox;
        private RichTextBox m_linebox;
        private Color m_forecolor;
        private bool m_isfocused;
        private string m_longestchar;
        private bool m_isediting;

        public bool IsEditing => m_isediting;

        public bool IsActive { get; private set; }

        public ScriptLine LinkedLine { get; }

        public ScriptLineBox(ScriptLine sl)
        {
            LinkedLine = sl;
            InitializeControl();
            m_characterbox.Text = $"{LinkedLine.Character}:";
            m_linebox.Text = LinkedLine.Line;
            m_timebox.Text = LinkedLine.Offset.ToString().Substring(3, 5);
        }

        public ScriptLineBox(ScriptLine sl, float fontsize)
          : this(sl)
        {
            m_characterbox.Font = new Font("Franklin Gothic Medium", fontsize, FontStyle.Regular, GraphicsUnit.Point, 0);
            m_linebox.Font = m_characterbox.Font;
            m_timebox.Font = new Font("Franklin Gothic Medium", fontsize, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        public ScriptLineBox(ScriptLine sl, float fontsize, string longestcharname)
          : this(sl, fontsize)
        {
            m_longestchar = longestcharname;
        }

        public void MarkAsActive(bool active)
        {
            IsActive = active;
            BorderStyle = active ? BorderStyle.Fixed3D : BorderStyle.None;
        }

        public void SetProgress(double progress)
        {
            m_timer.SetTimer(progress);
        }

        public void ShowTimer(bool vis)
        {
            m_timer.Visible = vis;
        }

        public void RefreshOffset()
        {
            if (m_timebox.Focused)
                m_timebox.Text = LinkedLine.Offset.ToString().Substring(0, 11);
            else
                m_timebox.Text = LinkedLine.Offset.ToString().Substring(3, 5);
        }

        private void ChangeColor(object sender, EventArgs e)
        {
            m_forecolor = ForeColor;
            m_characterbox.ForeColor = ForeColor;
            m_timebox.ForeColor = ForeColor;
            m_linebox.ForeColor = ForeColor;
        }

        private void CharacterLeft(object sender, EventArgs e)
        {
            string str = m_characterbox.Text;
            if (str.EndsWith(":"))
                str = str.Substring(0, str.Length - 1);
            else
                m_characterbox.Text = string.Format("{0}:", m_characterbox.Text);
            if (!(LinkedLine.Character != str))
                return;
            SetSaveNeeded();
            LinkedLine.Character = str;
            if (str.Length > m_longestchar.Length)
                m_longestchar = str;
            SaveFocus();
            UpdateCharacterBox();
        }

        private void InitializeControl()
        {
            m_timebox = new RichTextBox();
            m_characterbox = new RichTextBox();
            m_linebox = new RichTextBox();
            m_timer = new CountdownTimer();
            SuspendLayout();
            m_timer.Location = new Point(205, 0);
            m_timer.TabIndex = 0;
            m_timer.BackColor = Color.Black;
            m_timer.ForeColor = Color.Gray;
            m_timer.Visible = false;
            m_timebox.Location = new Point(0, 0);
            m_timebox.Size = new Size(60, 20);
            m_timebox.TabIndex = 1;
            m_timebox.Multiline = false;
            m_timebox.BorderStyle = BorderStyle.None;
            m_timebox.BackColor = Color.Black;
            m_timebox.ForeColor = Color.Gray;
            m_timebox.ScrollBars = RichTextBoxScrollBars.None;
            m_timebox.ContentsResized += ResizeTime;
            m_timebox.Enter += TimeBoxEntered;
            m_timebox.Enter += EditableEntered;
            m_timebox.Leave += TimeBoxLeft;
            m_timebox.Leave += EditableLeft;
            m_timebox.KeyPress += EditableKeyPressed;
            m_timebox.MouseUp += EditableMouseUp;
            m_timebox.GotFocus += EditableGotFocus;
            m_characterbox.Location = new Point(0, 0);
            m_characterbox.Size = new Size(200, 20);
            m_characterbox.TabIndex = 0;
            m_characterbox.Multiline = false;
            m_characterbox.BorderStyle = BorderStyle.None;
            m_characterbox.Font = new Font("Franklin Gothic Medium", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            m_characterbox.BackColor = Color.Black;
            m_characterbox.ForeColor = Color.Gray;
            m_characterbox.ScrollBars = RichTextBoxScrollBars.None;
            m_characterbox.ContentsResized += ResizeCharacter;
            m_characterbox.Enter += EditableEntered;
            m_characterbox.Leave += CharacterLeft;
            m_characterbox.Leave += EditableLeft;
            m_characterbox.KeyPress += EditableKeyPressed;
            m_characterbox.MouseUp += EditableMouseUp;
            m_characterbox.GotFocus += EditableGotFocus;
            m_linebox.Location = new Point(275, 0);
            m_linebox.Size = new Size(400, 20);
            m_linebox.TabIndex = 2;
            m_linebox.Multiline = true;
            m_linebox.BorderStyle = BorderStyle.None;
            m_linebox.Font = m_characterbox.Font;
            m_linebox.BackColor = Color.Black;
            m_linebox.ForeColor = Color.Gray;
            m_linebox.ScrollBars = RichTextBoxScrollBars.None;
            m_linebox.ContentsResized += ResizeLine;
            m_linebox.TextChanged += LineChanged;
            m_linebox.Enter += EditableEntered;
            m_linebox.Leave += EditableLeft;
            m_linebox.KeyPress += EditableKeyPressed;
            m_linebox.MouseUp += EditableMouseUp;
            m_linebox.GotFocus += EditableGotFocus;
            ForeColorChanged += ChangeColor;
            Click += ScriptLineBox_Click;
            Controls.Add(m_timer);
            Controls.Add(m_characterbox);
            Controls.Add(m_linebox);
            Controls.Add(m_timebox);
            Size = new Size(715, 30);
            ResumeLayout(false);
            PerformLayout();
        }

        private void EditableGotFocus(object sender, EventArgs e)
        {
            if (!(sender is RichTextBox) || MouseButtons != MouseButtons.None)
                return;
            m_isfocused = true;
        }

        private void EditableMouseUp(object sender, MouseEventArgs e)
        {
            RichTextBox richTextBox = sender as RichTextBox;
            if (richTextBox == null || m_isfocused || richTextBox.SelectionLength != 0)
                return;
            m_isfocused = true;
        }

        protected override bool ProcessTabKey(bool forward)
        {
            PerformFocus(forward);
            return true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Prior:
                case Keys.Next:
                    ClearFocus();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void PerformFocus(bool forward)
        {
            if (m_characterbox.Focused && forward || m_linebox.Focused && !forward)
                m_timebox.Focus();
            else if (m_timebox.Focused && forward || m_characterbox.Focused && !forward)
            {
                m_linebox.Focus();
            }
            else
            {
                if ((!m_linebox.Focused || !forward) && (!m_timebox.Focused || forward))
                    return;
                m_characterbox.Focus();
            }
        }

        private void EditableKeyPressed(object sender, KeyPressEventArgs e)
        {
            if (!(sender is RichTextBox) || e.KeyChar != 27)
                return;
            ClearFocus();
            e.Handled = true;
        }

        private void ClearFocus()
        {
            ((DubTool)Parent).ClearFocus();
        }

        private void SetSaveNeeded()
        {
            ((DubTool)Parent).SetSaveNeeded();
        }

        public void Edit(DubTool.ScriptLineState state)
        {
            switch (state)
            {
                case DubTool.ScriptLineState.Character:
                    m_characterbox.Focus();
                    break;
                case DubTool.ScriptLineState.Time:
                    m_timebox.Focus();
                    break;
                default:
                    m_linebox.Focus();
                    break;
            }
        }

        private void SaveFocus()
        {
            DubTool.ScriptLineState state = DubTool.ScriptLineState.None;
            if (m_characterbox.Focused)
                state = DubTool.ScriptLineState.Character;
            else if (m_timebox.Focused)
                state = DubTool.ScriptLineState.Time;
            else if (m_linebox.Focused)
                state = DubTool.ScriptLineState.Line;
            ((DubTool)Parent).SetEditState(state, LinkedLine);
        }

        private void SortScript()
        {
            ((DubTool)Parent).SortScript();
        }

        private void UpdateCharacterBox()
        {
            ((DubTool)Parent).UpdateCharacterBox();
        }

        private void EditableEntered(object sender, EventArgs e)
        {
            RichTextBox richTextBox = sender as RichTextBox;
            if (richTextBox != null)
            {
                richTextBox.BackColor = Color.GhostWhite;
                richTextBox.ForeColor = Color.Black;
            }
            m_isediting = true;
        }

        private void TimeBoxEntered(object sender, EventArgs e)
        {
            m_timebox.Text = LinkedLine.Offset.ToString().Substring(0, 11);
            m_timebox.Width = (int)CreateGraphics().MeasureString(m_timebox.Text, m_timebox.Font, ParentForm.Width).Width + 5;
        }

        private void EditableLeft(object sender, EventArgs e)
        {
            RichTextBox richTextBox = sender as RichTextBox;
            if (!IsDisposed && richTextBox != null)
            {
                richTextBox.BackColor = Color.Black;
                richTextBox.ForeColor = m_forecolor;
                richTextBox.Select(0, 0);
            }
            m_isediting = false;
            m_isfocused = false;
        }

        private void TimeBoxLeft(object sender, EventArgs e)
        {
            if (ParentForm == null)
                return;
            MidiTime zero = MidiTime.Zero;
            bool flag = true;
            try
            {
                zero = MidiTime.Parse(m_timebox.Text);
            }
            catch (FormatException ex)
            {
                flag = false;
            }
            catch (OverflowException ex)
            {
                flag = false;
            }
            catch (IndexOutOfRangeException ex)
            {
                flag = false;
            }
            if (flag)
            {
                m_timebox.Width = (int)CreateGraphics().MeasureString(m_timebox.Text, m_timebox.Font, ParentForm.Width).Width + 5;
                if (!LinkedLine.Offset.Equals(zero))
                {
                    SetSaveNeeded();
                    LinkedLine.Offset = zero;
                    SaveFocus();
                    SortScript();
                }
                else
                    m_timebox.Text = LinkedLine.Offset.ToString().Substring(3, 5);
            }
            else
            {
                int num = (int)MessageBox.Show("Could not parse time offset. Please make sure that the offset is in the right format.", "Midi time parser problem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void LineChanged(object sender, EventArgs e)
        {
            if (!(LinkedLine.Line != m_linebox.Text))
                return;
            SetSaveNeeded();
            LinkedLine.Line = m_linebox.Text;
        }

        private void ResizeCharacter(object sender, ContentsResizedEventArgs e)
        {
            if (m_characterbox.Focused)
                return;
            int width = (int)CreateGraphics().MeasureString("00.00.00.00", m_timebox.Font, ParentForm.Width).Width;
            if (m_longestchar != null)
                m_characterbox.Width = (int)CreateGraphics().MeasureString(m_longestchar, m_characterbox.Font, ParentForm.Width).Width + 5;
            else
                m_characterbox.Width = (int)CreateGraphics().MeasureString(m_characterbox.Text, m_characterbox.Font, ParentForm.Width).Width + 5;
            m_characterbox.Height = e.NewRectangle.Height + 5;
            m_timer.Size = new Size(Height, Height);
            m_characterbox.SelectAll();
            m_characterbox.SelectionAlignment = HorizontalAlignment.Right;
            m_characterbox.Select(0, 0);
            m_timebox.Left = m_characterbox.Width + 25;
            m_linebox.Left = Math.Max(m_characterbox.Width + 25 + m_timer.Width + 25, m_timebox.Left + width);
            m_timer.Left = m_timebox.Left + m_timebox.Width + (m_linebox.Left - (m_timebox.Left + m_timebox.Width) - m_timer.Width) / 2;
            m_linebox.Width = Width - m_linebox.Left;
        }

        private void ResizeLine(object sender, ContentsResizedEventArgs e)
        {
            m_linebox.Height = e.NewRectangle.Height + 5;
            Height = m_linebox.Height;
        }

        private void ResizeTime(object sender, ContentsResizedEventArgs e)
        {
            if (m_timebox.Focused)
                return;
            m_timebox.Width = (int)CreateGraphics().MeasureString(m_timebox.Text, m_timebox.Font, ParentForm.Width).Width + 5;
            m_timebox.Height = e.NewRectangle.Height + 10;
        }

        private void ScriptLineBox_Click(object sender, EventArgs e)
        {
            ClearFocus();
        }
    }
}
