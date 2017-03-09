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
        private RichTextBox _mCharacterbox;
        private RichTextBox _mTimebox;
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
            _mCharacterbox.Text = $"{LinkedLine.Character}:";
            m_linebox.Text = LinkedLine.Line;
            _mTimebox.Text = LinkedLine.Offset.ToString().Substring(3, 5);
        }

        public ScriptLineBox(ScriptLine sl, float fontsize)
          : this(sl)
        {
            _mCharacterbox.Font = new Font("Franklin Gothic Medium", fontsize, FontStyle.Regular, GraphicsUnit.Point, 0);
            m_linebox.Font = _mCharacterbox.Font;
            _mTimebox.Font = new Font("Franklin Gothic Medium", fontsize, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            if (_mTimebox.Focused)
                _mTimebox.Text = LinkedLine.Offset.ToString().Substring(0, 11);
            else
                _mTimebox.Text = LinkedLine.Offset.ToString().Substring(3, 5);
        }

        private void ChangeColor(object sender, EventArgs e)
        {
            m_forecolor = ForeColor;
            _mCharacterbox.ForeColor = ForeColor;
            _mTimebox.ForeColor = ForeColor;
            m_linebox.ForeColor = ForeColor;
        }

        private void CharacterLeft(object sender, EventArgs e)
        {
            string str = _mCharacterbox.Text;
            if (str.EndsWith(":"))
                str = str.Substring(0, str.Length - 1);
            else
                _mCharacterbox.Text = string.Format("{0}:", _mCharacterbox.Text);
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
            _mTimebox = new RichTextBox();
            _mCharacterbox = new RichTextBox();
            m_linebox = new RichTextBox();
            m_timer = new CountdownTimer();
            SuspendLayout();
            m_timer.Location = new Point(205, 0);
            m_timer.TabIndex = 0;
            m_timer.BackColor = Color.Black;
            m_timer.ForeColor = Color.Gray;
            m_timer.Visible = false;
            _mTimebox.Location = new Point(0, 0);
            _mTimebox.Size = new Size(60, 20);
            _mTimebox.TabIndex = 1;
            _mTimebox.Multiline = false;
            _mTimebox.BorderStyle = BorderStyle.None;
            _mTimebox.BackColor = Color.Black;
            _mTimebox.ForeColor = Color.Gray;
            _mTimebox.ScrollBars = RichTextBoxScrollBars.None;
            _mTimebox.ContentsResized += ResizeTime;
            _mTimebox.Enter += TimeBoxEntered;
            _mTimebox.Enter += EditableEntered;
            _mTimebox.Leave += TimeBoxLeft;
            _mTimebox.Leave += EditableLeft;
            _mTimebox.KeyPress += EditableKeyPressed;
            _mTimebox.MouseUp += EditableMouseUp;
            _mTimebox.GotFocus += EditableGotFocus;
            _mCharacterbox.Location = new Point(0, 0);
            _mCharacterbox.Size = new Size(200, 20);
            _mCharacterbox.TabIndex = 0;
            _mCharacterbox.Multiline = false;
            _mCharacterbox.BorderStyle = BorderStyle.None;
            _mCharacterbox.Font = new Font("Franklin Gothic Medium", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            _mCharacterbox.BackColor = Color.Black;
            _mCharacterbox.ForeColor = Color.Gray;
            _mCharacterbox.ScrollBars = RichTextBoxScrollBars.None;
            _mCharacterbox.ContentsResized += ResizeCharacter;
            _mCharacterbox.Enter += EditableEntered;
            _mCharacterbox.Leave += CharacterLeft;
            _mCharacterbox.Leave += EditableLeft;
            _mCharacterbox.KeyPress += EditableKeyPressed;
            _mCharacterbox.MouseUp += EditableMouseUp;
            _mCharacterbox.GotFocus += EditableGotFocus;
            m_linebox.Location = new Point(275, 0);
            m_linebox.Size = new Size(400, 20);
            m_linebox.TabIndex = 2;
            m_linebox.Multiline = true;
            m_linebox.BorderStyle = BorderStyle.None;
            m_linebox.Font = _mCharacterbox.Font;
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
            Controls.Add(_mCharacterbox);
            Controls.Add(m_linebox);
            Controls.Add(_mTimebox);
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
            if (_mCharacterbox.Focused && forward || m_linebox.Focused && !forward)
                _mTimebox.Focus();
            else if (_mTimebox.Focused && forward || _mCharacterbox.Focused && !forward)
            {
                m_linebox.Focus();
            }
            else
            {
                if ((!m_linebox.Focused || !forward) && (!_mTimebox.Focused || forward))
                    return;
                _mCharacterbox.Focus();
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
                    _mCharacterbox.Focus();
                    break;
                case DubTool.ScriptLineState.Time:
                    _mTimebox.Focus();
                    break;
                default:
                    m_linebox.Focus();
                    break;
            }
        }

        private void SaveFocus()
        {
            DubTool.ScriptLineState state = DubTool.ScriptLineState.None;
            if (_mCharacterbox.Focused)
                state = DubTool.ScriptLineState.Character;
            else if (_mTimebox.Focused)
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
            _mTimebox.Text = LinkedLine.Offset.ToString().Substring(0, 11);
            _mTimebox.Width = (int)CreateGraphics().MeasureString(_mTimebox.Text, _mTimebox.Font, ParentForm.Width).Width + 5;
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
                zero = MidiTime.Parse(_mTimebox.Text);
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
                _mTimebox.Width = (int)CreateGraphics().MeasureString(_mTimebox.Text, _mTimebox.Font, ParentForm.Width).Width + 5;
                if (!LinkedLine.Offset.Equals(zero))
                {
                    SetSaveNeeded();
                    LinkedLine.Offset = zero;
                    SaveFocus();
                    SortScript();
                }
                else
                    _mTimebox.Text = LinkedLine.Offset.ToString().Substring(3, 5);
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
            if (_mCharacterbox.Focused)
                return;
            int width = (int)CreateGraphics().MeasureString("00.00.00.00", _mTimebox.Font, ParentForm.Width).Width;
            if (m_longestchar != null)
                _mCharacterbox.Width = (int)CreateGraphics().MeasureString(m_longestchar, _mCharacterbox.Font, ParentForm.Width).Width + 5;
            else
                _mCharacterbox.Width = (int)CreateGraphics().MeasureString(_mCharacterbox.Text, _mCharacterbox.Font, ParentForm.Width).Width + 5;
            _mCharacterbox.Height = e.NewRectangle.Height + 5;
            m_timer.Size = new Size(Height, Height);
            _mCharacterbox.SelectAll();
            _mCharacterbox.SelectionAlignment = HorizontalAlignment.Right;
            _mCharacterbox.Select(0, 0);
            _mTimebox.Left = _mCharacterbox.Width + 25;
            m_linebox.Left = Math.Max(_mCharacterbox.Width + 25 + m_timer.Width + 25, _mTimebox.Left + width);
            m_timer.Left = _mTimebox.Left + _mTimebox.Width + (m_linebox.Left - (_mTimebox.Left + _mTimebox.Width) - m_timer.Width) / 2;
            m_linebox.Width = Width - m_linebox.Left;
        }

        private void ResizeLine(object sender, ContentsResizedEventArgs e)
        {
            m_linebox.Height = e.NewRectangle.Height + 5;
            Height = m_linebox.Height;
        }

        private void ResizeTime(object sender, ContentsResizedEventArgs e)
        {
            if (_mTimebox.Focused)
                return;
            _mTimebox.Width = (int)CreateGraphics().MeasureString(_mTimebox.Text, _mTimebox.Font, ParentForm.Width).Width + 5;
            _mTimebox.Height = e.NewRectangle.Height + 10;
        }

        private void ScriptLineBox_Click(object sender, EventArgs e)
        {
            ClearFocus();
        }
    }
}
