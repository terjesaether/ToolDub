// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.DubTool
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using Nordubb.DubTool.Keyboard;
using Nordubb.DubTool.Midi;
using Nordubb.DubTool.ScriptIO;
using Sanford.Multimedia.Midi;
using Timer = System.Threading.Timer;

namespace Nordubb.DubTool
{
    internal sealed class DubTool : Form
    {
        private static readonly Color LineColor = Color.Gray;
        private static readonly Color DoneColor = Color.Blue;
        private static readonly Color ActiveColor = Color.LightBlue;
        private static readonly Color ActiveDoneColor = Color.LightCyan;
        private static readonly double OffsetRatio = 0.1;
        private static readonly double WidthRatio = 1.0 - 2.0 * OffsetRatio;
        private KeyboardHook m_kh = new KeyboardHook();
        private ExcelScript m_xls = new ExcelScript();
        private List<ScriptLineBox> m_lineboxes = new List<ScriptLineBox>();
        private string m_longestcharname = string.Empty;
        private const int WM_SYSCOMMAND = 274;
        private const int SC_MAXIMIZE = 61488;
        private const Keys RecordKey = Keys.NumPad5;
        private const Keys PlayKey = Keys.NumPad8;
        private const Keys StopKey = Keys.NumPad2;
        private const Keys RewindKey = Keys.NumPad4;
        private const Keys FastForwardKey = Keys.NumPad6;
        private const Keys LineDoneToggleKey = Keys.NumPad1;
        private const Keys UpdateOffsetKey = Keys.NumPad7;
        private const Keys InsertLineKey = Keys.Insert;
        private const Keys DeleteLineKey = Keys.Delete;
        private const Keys PageDownKey = Keys.Next;
        private const Keys PageUpKey = Keys.Prior;
        private const Keys FullScreenToggleKey = Keys.F11;
        private IContainer components;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatus;
        private ToolStripStatusLabel lblEmpty;
        private ToolStripStatusLabel lblTime;
        private ToolStripDropDownButton ddCharacter;
        private ToolStripStatusLabel lblRecorded;
        private ToolStripSplitButton toolStripSplitButton1;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem saveFileToolStripMenuItem;
        private ToolStripMenuItem openFileToolStripMenuItem;
        private ToolStripMenuItem exitDubToolToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripStatusLabel lblEpisodeNo;
        private ILog m_logger;
        private int m_midiInDevice;
        private int m_midiOutDevice;
        private InputDevice m_midiIn;
        private OutputDevice m_midiOut;
        private ScriptLineState m_editstate;
        private ScriptLine m_editingline;
        private bool m_menuIsOpen;
        private bool m_isMaximized;
        private bool m_isPlaying;
        private bool m_isRecording;
        private bool m_recordingTriggered;
        private int m_expectedTime;
        private int m_hour;
        private int m_minute;
        private int m_second;
        private int m_frame;
        private int m_lsb;
        private MidiTime m_currentTime;
        private int m_gotosent;
        private int m_stopsent;
        private int m_playsent;
        private int m_recordstrobesent;
        private int m_lastreceived;
        private int m_curFps;
        private int m_curLeadSeconds;
        private string m_filename;
        private int m_episode;
        private int m_currentlyactive;
        private ScriptLine m_currentline;
        private ScriptLine m_topline;
        private ScriptLine m_bottomline;
        private int m_pageoffset;
        private ScriptLine m_startingline;
        private string m_currentchar;
        private Timer m_heartbeat;
        private bool m_haschanged;

        public DubTool()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DubTool));
            statusStrip1 = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            lblEmpty = new ToolStripStatusLabel();
            ddCharacter = new ToolStripDropDownButton();
            lblEpisodeNo = new ToolStripStatusLabel();
            lblRecorded = new ToolStripStatusLabel();
            toolStripSplitButton1 = new ToolStripSplitButton();
            openFileToolStripMenuItem = new ToolStripMenuItem();
            saveFileToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitDubToolToolStripMenuItem = new ToolStripMenuItem();
            lblTime = new ToolStripStatusLabel();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            //statusStrip1.BackColor = SystemColors.Control;
            statusStrip1.BackColor = System.Drawing.Color.DarkGreen;
            statusStrip1.Dock = DockStyle.Top;
            statusStrip1.Items.AddRange(new ToolStripItem[7]
            {
        lblStatus,
        lblEmpty,
        ddCharacter,
        lblEpisodeNo,
        lblRecorded,
        toolStripSplitButton1,
        lblTime
            });
            statusStrip1.Location = new Point(0, 458);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(640, 32);
            statusStrip1.TabIndex = 22;
            statusStrip1.Text = "statusStrip1";
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 17);
            lblEmpty.Name = "lblEmpty";
            lblEmpty.Size = new Size(0, 17);
            ddCharacter.AutoSize = false;
            ddCharacter.DisplayStyle = ToolStripItemDisplayStyle.Text;
            ddCharacter.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            ddCharacter.ImageTransparentColor = Color.Magenta;
            ddCharacter.Name = "ddCharacter";
            ddCharacter.Size = new Size(250, 20);
            ddCharacter.Text = "All characters";
            ddCharacter.TextAlign = ContentAlignment.MiddleLeft;
            ddCharacter.DropDownItemClicked += ddCharacter_DropDownItemClicked;
            lblEpisodeNo.AutoSize = false;
            lblEpisodeNo.Name = "lblEpisodeNo";
            lblEpisodeNo.Size = new Size(90, 17);
            lblEpisodeNo.TextAlign = ContentAlignment.MiddleLeft;
            lblRecorded.DisplayStyle = ToolStripItemDisplayStyle.Text;
            lblRecorded.Name = "lblRecorded";
            lblRecorded.Size = new Size(121, 17);
            lblRecorded.Spring = true;
            lblRecorded.TextAlign = ContentAlignment.MiddleLeft;
            toolStripSplitButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripSplitButton1.DropDownItems.AddRange(new ToolStripItem[5]
            {
        openFileToolStripMenuItem,
        saveFileToolStripMenuItem,
        settingsToolStripMenuItem,
        toolStripSeparator1,
        exitDubToolToolStripMenuItem
            });
            toolStripSplitButton1.ImageTransparentColor = Color.Magenta;
            toolStripSplitButton1.Name = "toolStripSplitButton1";
            toolStripSplitButton1.Size = new Size(54, 20);
            toolStripSplitButton1.Text = "Menu";
            toolStripSplitButton1.DropDownOpened += toolStripSplitButton1_DropDownOpened;
            toolStripSplitButton1.ButtonClick += toolStripSplitButton1_ButtonClick;
            toolStripSplitButton1.DropDownClosed += toolStripSplitButton1_DropDownClosed;
            openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            openFileToolStripMenuItem.ShortcutKeys = Keys.O | Keys.Control;
            openFileToolStripMenuItem.Size = new Size(186, 22);
            openFileToolStripMenuItem.Text = "Open file ...";
            openFileToolStripMenuItem.Click += openToolStripMenuItem_Click;
            saveFileToolStripMenuItem.Name = "saveFileToolStripMenuItem";
            saveFileToolStripMenuItem.ShortcutKeys = Keys.S | Keys.Control;
            saveFileToolStripMenuItem.Size = new Size(186, 22);
            saveFileToolStripMenuItem.Text = "Save file";
            saveFileToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(186, 22);
            settingsToolStripMenuItem.Text = "Settings";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(183, 6);
            exitDubToolToolStripMenuItem.Name = "exitDubToolToolStripMenuItem";
            exitDubToolToolStripMenuItem.ShortcutKeys = Keys.W | Keys.Control;
            exitDubToolToolStripMenuItem.Size = new Size(186, 22);
            exitDubToolToolStripMenuItem.Text = "Exit DubTool";
            exitDubToolToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(79, 17);
            lblTime.Text = "00:00:00.00.00";
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(640, 480);
            Controls.Add(statusStrip1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            Name = "DubTool";
            Text = "DubTool ";
            Load += DubTool_Load;
            Click += DubTool_Click;
            FormClosing += DubTool_FormClosing;
            Resize += DubTool_Resize;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        public void SetEditState(ScriptLineState state, ScriptLine line)
        {
            m_editstate = state;
            m_editingline = line;
        }

        [DllImport("user32")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32")]
        public static extern int UpdateWindow(IntPtr hwnd);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 274 && (int)m.WParam == 61488)
                ToggleMaximized();
            else
                base.WndProc(ref m);
        }

        private void KeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteLine();
            }
            else
            {
                if (e.KeyCode != Keys.Insert)
                    return;
                InsertLine();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            KeyUpHandler(this, e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool flag = true;
            switch (keyData)
            {
                case Keys.Prior:
                    ShowPreviousPage();
                    break;
                case Keys.Next:
                    ShowNextPage();
                    break;
                case Keys.F11:
                    ToggleMaximized();
                    break;
                default:
                    flag = false;
                    break;
            }
            if (flag)
                return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static string FormatMidiMessage(byte[] msg)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte num in msg)
                stringBuilder.AppendFormat("{0:x} ", num);
            stringBuilder.Append("\n");
            return stringBuilder.ToString();
        }

        private void DubTool_FormClosing(object sender, FormClosingEventArgs e)
        {
            var dialogResult = DialogResult.No;
            if (m_haschanged && m_filename != null)
                dialogResult = MessageBox.Show("Save script before exiting?", "Exit", MessageBoxButtons.YesNoCancel);
            if (dialogResult == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                if (dialogResult == DialogResult.Yes)
                    SaveActiveEpisode();
                Shutdown();
            }
        }

        private void DubTool_Resize(object sender, EventArgs e)
        {
            DisplayLine(m_currentline, true);
        }

        private void ddCharacter_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            m_currentchar = !((string)e.ClickedItem.Tag == "ALL") ? (string)e.ClickedItem.Tag : null;
            ChangeCharacter();
        }

        private void ChangeCharacter()
        {
            bool flag = false;
            foreach (ToolStripItem dropDownItem in ddCharacter.DropDownItems)
            {
                if ((string)dropDownItem.Tag == m_currentchar)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
                m_currentchar = null;
            if (string.IsNullOrEmpty(m_currentchar))
            {
                m_logger.Info("Selecting all characters");
                m_currentline = m_xls.Lines.GetFirstUnrecordedLine();
            }
            else
            {
                m_logger.Info("Selecting character " + m_currentchar);
                m_currentline = m_xls.Lines.GetFirstUnrecordedLine(m_currentchar);
            }
            if (m_currentchar != null)
                ddCharacter.Text = m_currentchar;
            else
                ddCharacter.Text = "All characters";
            GotoLineWithLead(m_currentline, true);
            UpdateRecordedStatus();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSettings();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowOpenDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveActiveEpisode();
        }

        private void SaveActiveEpisode()
        {
            m_xls.Save(m_filename);
            m_haschanged = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripSplitButton1_DropDownOpened(object sender, EventArgs e)
        {
            m_menuIsOpen = true;
        }

        private void toolStripSplitButton1_DropDownClosed(object sender, EventArgs e)
        {
            m_menuIsOpen = false;
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            if (m_menuIsOpen)
                toolStripSplitButton1.HideDropDown();
            else
                toolStripSplitButton1.ShowDropDown();
        }

        private void CommonMessageHandler(object sender, SysCommonMessageEventArgs e)
        {
            m_lastreceived = Environment.TickCount;
            byte[] bytes = e.Message.GetBytes();
            int num1 = bytes[1];
            if (bytes[0] != 241 || num1 >> 4 != m_expectedTime)
                return;
            int num2 = num1 & 15;
            switch (m_expectedTime)
            {
                case 0:
                case 2:
                case 4:
                case 6:
                    m_lsb = num2;
                    break;
                case 1:
                    m_frame = num2 << 4 | m_lsb;
                    break;
                case 3:
                    m_second = num2 << 4 | m_lsb;
                    break;
                case 5:
                    m_minute = num2 << 4 | m_lsb;
                    break;
                case 7:
                    m_hour = num2 << 4 | m_lsb;
                    if (Environment.TickCount < m_lastreceived + 20)
                    {
                        SetDisplayTime(m_hour, m_minute, m_second, m_frame, 0);
                        if (m_currentline != null && m_lineboxes != null)
                        {
                            UpdateScriptLines();
                        }
                    }
                    break;
            }
            m_expectedTime = (m_expectedTime + 1) % 8;
        }

        private void MidiMessageHandler(object sender, SysExMessageEventArgs e)
        {
            switch (SysExCommands.CheckType(e.Message.GetBytes()))
            {
                case SysExCommands.CommandType.Play:
                case SysExCommands.CommandType.DeferredPlay:
                    if (!WeDidntSendIt(m_playsent))
                        break;
                    m_logger.Info("MMC Play received");
                    MarkAsPlaying();
                    break;
                case SysExCommands.CommandType.RecordStrobe:
                    if (!WeDidntSendIt(m_recordstrobesent))
                        break;
                    m_logger.Info("MMC Record received");
                    MarkAsRecording();
                    break;
                case SysExCommands.CommandType.Stop:
                    if (!WeDidntSendIt(m_stopsent))
                        break;
                    m_logger.Info("MMC Stop received");
                    MarkAsStopped();
                    break;
                case SysExCommands.CommandType.Goto:
                    if (!WeDidntSendIt(m_gotosent))
                        break;
                    byte[] bytes1 = e.Message.GetBytes();
                    if (bytes1.Length >= 12)
                    {
                        m_logger.Info("MMC Goto received");
                        Goto(bytes1[7], bytes1[8], bytes1[9], bytes1[10], bytes1[11]);
                        break;
                    }
                    m_logger.Error("Invalid MMC Goto received" + FormatMidiMessage(bytes1));
                    break;
                case SysExCommands.CommandType.MtcGoto:
                    if (!WeDidntSendIt(m_gotosent))
                        break;
                    byte[] bytes2 = e.Message.GetBytes();
                    if (bytes2.Length >= 10)
                    {
                        m_logger.Info("MTC Goto received");
                        Goto(bytes2[5], bytes2[6], bytes2[7], bytes2[8], 0);
                        break;
                    }
                    m_logger.Error("Invalid MTC Goto received: " + FormatMidiMessage(bytes2));
                    break;
                default:
                    m_logger.Debug("Unknown MIDI SysEx received: " + FormatMidiMessage(e.Message.GetBytes()));
                    break;
            }
        }

        private void UpdateOffset()
        {
            if (m_isRecording || m_isPlaying)
                return;
            m_currentline.Offset = m_currentTime;
            SetSaveNeeded();
            if (!IsEditing())
            {
                DisplayLine(m_currentline, true);
            }
            else
            {
                foreach (ScriptLineBox linebox in m_lineboxes)
                {
                    if (linebox.IsEditing)
                        linebox.RefreshOffset();
                }
            }
        }

        private void KeyPressedHandler(object sender, KeyPressedEventArgs e)
        {
            if (e.Key == Keys.NumPad7)
            {
                UpdateOffset();
            }
            else
            {
                if (IsEditing())
                    return;
                switch (e.Key)
                {
                    case Keys.NumPad1:
                        ToggleLineDone();
                        break;
                    case Keys.NumPad2:
                        Stop();
                        break;
                    case Keys.NumPad4:
                        Rewind();
                        break;
                    case Keys.NumPad5:
                        Record();
                        break;
                    case Keys.NumPad6:
                        FastForward();
                        break;
                    case Keys.NumPad8:
                        Play();
                        break;
                }
            }
        }

        private MidiTime BuildMidiTime()
        {
            return new MidiTime
            {
                Hour = m_hour & 31,
                Minute = m_minute,
                Second = m_second,
                Frame = m_frame
            };
        }

        private MidiTime BuildMidiTime(int fpshour, int minute, int second, int frame, int subframe)
        {
            return new MidiTime
            {
                Hour = fpshour & 31,
                Minute = minute,
                Second = second,
                Frame = frame,
                Subframe = subframe
            };
        }

        private ScriptLineBox AddScriptLineBox(ScriptLine sl, int LeftOffset, int BoxWidth, float FontSize)
        {
            ScriptLineBox scriptLineBox = new ScriptLineBox(sl, FontSize, m_longestcharname);
            scriptLineBox.Left = LeftOffset;
            scriptLineBox.Width = BoxWidth;
            if (m_currentchar != null && sl.Character != null && sl.Character.Equals(m_currentchar, StringComparison.CurrentCultureIgnoreCase))
                scriptLineBox.ForeColor = sl.IsDone ? ActiveDoneColor : ActiveColor;
            else
                scriptLineBox.ForeColor = sl.IsDone ? DoneColor : LineColor;
            m_lineboxes.Add(scriptLineBox);
            Controls.Add(scriptLineBox);
            return scriptLineBox;
        }

        private void ClearActiveLineBox()
        {
            foreach (ScriptLineBox linebox in m_lineboxes)
            {
                if (linebox.IsActive)
                    linebox.MarkAsActive(false);
            }
        }

        private void CloseMidiIn()
        {
            m_logger.Info("Closing Midi In");
            m_midiIn.StopRecording();
            m_midiIn.Dispose();
        }

        private void CloseMidiOut()
        {
            m_logger.Info("Closing Midi Out");
            m_midiOut.Reset();
            m_midiOut.Dispose();
        }

        private void Heartbeat(object o)
        {
            if (!m_isPlaying)
                return;
            if (Environment.TickCount - m_lastreceived >= 500)
            {
                MarkAsStopped();
                m_currentline = GotoLineWithLead(m_currentline, false);
                Timer timer = new Timer(DelayedGotoCurrent, null, 200, -1);
            }
            if (!m_isPlaying)
                return;
            TriggerHeartbeat();
        }

        private void TriggerHeartbeat()
        {
            m_heartbeat = new Timer(Heartbeat, null, 5000, -1);
        }

        private void DelayedGotoCurrent(object o)
        {
            GotoLineWithLead(m_currentline, true);
        }

        private void DelayedPlay(object o)
        {
            Play();
        }

        private void DelayedRecord(object o)
        {
            Record();
        }

        private void ShowPreviousPage()
        {
            if (m_topline == null || m_topline.PrevByTime == null)
                return;
            if (m_pageoffset == 1)
            {
                DisplayLine(m_currentline, true);
            }
            else
            {
                int LeftOffset = (int)(OffsetRatio * Width);
                int BoxWidth = (int)(WidthRatio * Width);
                float FontSize = Width / 64f;
                RemoveLineBoxes();
                int num = Height - 12;
                ScriptLine sl = m_topline;
                m_bottomline = sl;
                ScriptLineBox scriptLineBox;
                for (; num > 0 && sl != null; num = scriptLineBox.Top)
                {
                    scriptLineBox = AddScriptLineBox(sl, LeftOffset, BoxWidth, FontSize);
                    scriptLineBox.Top = num - scriptLineBox.Height - 20;
                    ScriptLine scriptLine = sl;
                    sl = sl.PrevByTime;
                    if (scriptLineBox.Top < 0)
                    {
                        m_lineboxes.Remove(scriptLineBox);
                        Controls.Remove(scriptLineBox);
                    }
                    else
                        m_topline = scriptLine;
                }
                --m_pageoffset;
            }
        }

        private void ShowNextPage()
        {
            if (m_bottomline == null || m_bottomline.NextByTime == null)
                return;
            if (m_pageoffset == -1)
            {
                DisplayLine(m_currentline, true);
            }
            else
            {
                int LeftOffset = (int)(OffsetRatio * Width);
                int BoxWidth = (int)(WidthRatio * Width);
                float FontSize = Width / 64f;
                RemoveLineBoxes();
                int num = 0;
                ScriptLine sl = m_bottomline;
                m_topline = sl;
                ScriptLineBox scriptLineBox;
                for (; num < Height - 22 && sl != null; num = scriptLineBox.Bottom)
                {
                    scriptLineBox = AddScriptLineBox(sl, LeftOffset, BoxWidth, FontSize);
                    scriptLineBox.Top = num + 20;
                    ScriptLine scriptLine = sl;
                    sl = sl.NextByTime;
                    if (scriptLineBox.Bottom > Height - 22)
                    {
                        m_lineboxes.Remove(scriptLineBox);
                        Controls.Remove(scriptLineBox);
                    }
                    else
                        m_bottomline = scriptLine;
                }
                ++m_pageoffset;
            }
        }

        private ScriptLine DisplayLine(ScriptLine sl, bool scroll)
        {
            m_pageoffset = 0;
            int LeftOffset = (int)(OffsetRatio * Width);
            int BoxWidth = (int)(WidthRatio * Width);
            float FontSize = Width / 64f;
            if (!scroll)
            {
                bool flag = false;
                foreach (ScriptLineBox linebox in m_lineboxes)
                {
                    linebox.ShowTimer(false);
                    if (linebox.LinkedLine == sl)
                    {
                        linebox.MarkAsActive(true);
                        flag = true;
                    }
                    else
                        linebox.MarkAsActive(false);
                }
                if (!flag)
                {
                    ScriptLineBox linebox = m_lineboxes[m_lineboxes.Count - 1];
                    linebox.MarkAsActive(true);
                    return linebox.LinkedLine;
                }
            }
            else
            {
                RemoveLineBoxes();
                if (sl != null)
                {
                    ScriptLineBox scriptLineBox1 = AddScriptLineBox(sl, LeftOffset, BoxWidth, FontSize);
                    scriptLineBox1.MarkAsActive(true);
                    int num = scriptLineBox1.Top = (Height - 154 - scriptLineBox1.Height) / 3;
                    int bottom = scriptLineBox1.Bottom;
                    ScriptLine prevByTime = sl.PrevByTime;
                    m_topline = prevByTime;
                    m_bottomline = prevByTime;
                    ScriptLineBox scriptLineBox2;
                    for (; num > 0 && prevByTime != null; num = scriptLineBox2.Top)
                    {
                        scriptLineBox2 = AddScriptLineBox(prevByTime, LeftOffset, BoxWidth, FontSize);
                        scriptLineBox2.Top = num - scriptLineBox2.Height - 20;
                        ScriptLine scriptLine = prevByTime;
                        prevByTime = prevByTime.PrevByTime;
                        if (scriptLineBox2.Top < 0)
                        {
                            m_lineboxes.Remove(scriptLineBox2);
                            Controls.Remove(scriptLineBox2);
                        }
                        else
                            m_topline = scriptLine;
                    }
                    ScriptLineBox scriptLineBox3;
                    for (ScriptLine nextByTime = sl.NextByTime; bottom < Height - 22 && nextByTime != null; bottom = scriptLineBox3.Bottom)
                    {
                        scriptLineBox3 = AddScriptLineBox(nextByTime, LeftOffset, BoxWidth, FontSize);
                        scriptLineBox3.Top = bottom + 20;
                        ScriptLine scriptLine = nextByTime;
                        nextByTime = nextByTime.NextByTime;
                        if (scriptLineBox3.Bottom > Height - 22)
                        {
                            m_lineboxes.Remove(scriptLineBox3);
                            Controls.Remove(scriptLineBox3);
                        }
                        else
                            m_bottomline = scriptLine;
                    }
                }
            }
            return sl;
        }

        private void FastForward()
        {
            if (m_isPlaying)
                return;
            m_logger.Info("Skipping one line forward");
            if (m_currentchar != null && m_currentline != null)
            {
                if (m_currentline.NextByCharacter != null)
                    m_currentline = m_currentline.NextByCharacter;
            }
            else if (m_currentline != null && m_currentline.NextByTime != null)
                m_currentline = m_currentline.NextByTime;
            GotoLineWithLead(m_currentline, true);
        }

        private void FillCharacterBox()
        {
            ddCharacter.DropDown.Items.Clear();
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("All characters");
            toolStripMenuItem.Tag = "ALL";
            ddCharacter.DropDown.Items.Add(toolStripMenuItem);
            foreach (string character in m_xls.Characters)
            {
                ddCharacter.DropDown.Items.Add(character).Tag = character;
                if (character.Length > m_longestcharname.Length)
                    m_longestcharname = character;
            }
        }

        private void GrabFocus()
        {
            IntPtr handle = Handle;
            ShowWindow(handle, 9);
            UpdateWindow(handle);
        }

        private void Goto(int fpshours, int minutes, int seconds, int frames, int subframes)
        {
            SetDisplayTime(fpshours, minutes, seconds, frames, subframes);
            ClearActiveLineBox();
            MidiTime b = new MidiTime();
            b.Hour = fpshours & 31;
            b.Minute = minutes;
            b.Second = seconds;
            b.Frame = frames;
            b.Subframe = subframes;
            bool flag = false;
            foreach (ScriptLineBox linebox in m_lineboxes)
            {
                linebox.ShowTimer(false);
                ScriptLine linkedLine = linebox.LinkedLine;
                if (IsCurrentChar(linkedLine.Character))
                {
                    int num = MidiTime.SecondsAway(linkedLine.Offset, b);
                    if (!flag && num > 0 && num <= m_curLeadSeconds)
                    {
                        m_currentline = linkedLine;
                        linebox.MarkAsActive(true);
                        flag = true;
                    }
                }
            }
        }

        private ScriptLine GotoLineWithLead(ScriptLine sl, bool scroll)
        {
            if (InvokeRequired)
                return (ScriptLine)Invoke(new GotoLineCallback(GotoLineWithLead), (object)sl, (object)scroll);
            m_logger.Info("Jumping to new script line");
            if (sl != null)
                SendGotoCmd(sl.Offset.SubtractSeconds(m_curLeadSeconds));
            return DisplayLine(sl, scroll);
        }

        private void InitLog4Net()
        {
            FileAppender fileAppender = new FileAppender();
            fileAppender.File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "DubTool.log");
            fileAppender.Encoding = Encoding.UTF8;
            fileAppender.Layout = new PatternLayout("%date [%thread] %-5level %message%newline");
            fileAppender.ActivateOptions();
            BasicConfigurator.Configure(fileAppender);
            m_logger = LogManager.GetLogger("DubTool");
        }

        private bool IsCurrentChar(string character)
        {
            if (m_currentchar != null)
                return character.Equals(m_currentchar, StringComparison.CurrentCultureIgnoreCase);
            return true;
        }

        private void MarkAsPlaying()
        {
            SetPlayingState(true);
        }

        private void MarkAsRecording()
        {
            SetPlayingState(true);
            SetRecordingState(true);
        }

        private void MarkAsStopped()
        {
            m_currentlyactive = 0;
            SetPlayingState(false);
            SetRecordingState(false);
        }

        private void OpenMidiIn()
        {
            m_midiInDevice = Settings.MidiIn;
            m_logger.Info("Opening Midi In, using device #" + m_midiInDevice);
            m_midiIn = new InputDevice(m_midiInDevice);
            m_midiIn.StartRecording();
            Thread.Sleep(200);
            m_midiIn.SysExMessageReceived += MidiMessageHandler;
            m_midiIn.SysCommonMessageReceived += CommonMessageHandler;
        }

        private void OpenMidiOut()
        {
            m_midiOutDevice = Settings.MidiOut;
            m_logger.Info("Opening Midi Out, using device #" + m_midiOutDevice);
            m_midiOut = new OutputDevice(m_midiOutDevice);
        }

        private void Play()
        {
            m_expectedTime = 0;
            m_logger.Info("Sending MMC Play");
            m_playsent = Environment.TickCount;
            m_midiOut.Send(SysExCommands.Play);
            m_startingline = m_currentline;
            MarkAsPlaying();
            TriggerHeartbeat();
        }

        private void Record()
        {
            m_recordingTriggered = false;
            m_startingline = m_currentline;
            m_expectedTime = 0;
            m_logger.Info("Sending MMC Play");
            m_playsent = Environment.TickCount;
            m_midiOut.Send(SysExCommands.Play);
            MarkAsRecording();
            TriggerHeartbeat();
        }

        private void RegisterHotKeys()
        {
            m_kh.KeyPressed += KeyPressedHandler;
            m_kh.RegisterHotKey(Keyboard.ModifierKeys.None, Keys.NumPad6);
            m_kh.RegisterHotKey(Keyboard.ModifierKeys.None, Keys.NumPad4);
            m_kh.RegisterHotKey(Keyboard.ModifierKeys.None, Keys.NumPad2);
            m_kh.RegisterHotKey(Keyboard.ModifierKeys.None, Keys.NumPad8);
            m_kh.RegisterHotKey(Keyboard.ModifierKeys.None, Keys.NumPad5);
            m_kh.RegisterHotKey(Keyboard.ModifierKeys.None, Keys.NumPad1);
            m_kh.RegisterHotKey(Keyboard.ModifierKeys.None, Keys.NumPad7);
        }

        private void RemoveLineBoxes()
        {
            foreach (ScriptLineBox linebox in m_lineboxes)
            {
                Controls.Remove(linebox);
                linebox.Dispose();
            }
            m_lineboxes.Clear();
        }

        private void ResumeIfNeeded(bool wasPlaying, bool wasRecording)
        {
            if (!wasPlaying)
                return;
            Timer timer = new Timer(!wasRecording ? DelayedPlay : new TimerCallback(DelayedRecord), null, 1200, -1);
        }

        private void Rewind()
        {
            if (m_isPlaying)
                return;
            m_logger.Info("Skipping one line back");
            if (m_currentchar != null && m_currentline != null)
            {
                if (m_currentline.PrevByCharacter != null)
                    m_currentline = m_currentline.PrevByCharacter;
            }
            else if (m_currentline != null && m_currentline.PrevByTime != null)
                m_currentline = m_currentline.PrevByTime;
            GotoLineWithLead(m_currentline, true);
        }

        private void SetDisplayTime(int fpshours, int minutes, int seconds, int frames, int subframes)
        {
            if (InvokeRequired)
            {
                Invoke(new SetTimeCallback(SetDisplayTime), (object)fpshours, (object)minutes, (object)seconds, (object)frames, (object)subframes);
            }
            else
            {
                m_currentTime = BuildMidiTime(fpshours, minutes, seconds, frames, subframes);
                int num = fpshours & 96;
                if (m_curFps != num)
                {
                    m_curFps = num;
                    lblStatus.Text = string.Format("{0} fps", num);
                }
                lblTime.Text = string.Format("{0:00}:{1:00}:{2:00}.{3:00}.{4:00}", (object)(fpshours & 31), (object)minutes, (object)seconds, (object)frames, (object)subframes);
            }
        }

        private bool IsEditing()
        {
            foreach (ScriptLineBox linebox in m_lineboxes)
            {
                if (linebox.IsEditing)
                    return true;
            }
            return false;
        }

        private void SetPlayingState(bool playing)
        {
            m_logger.Debug("Playing state set to " + playing);
            m_isPlaying = playing;
        }

        private void SetRecordingState(bool recording)
        {
            m_logger.Debug("Recording state set to " + recording);
            m_isRecording = recording;
        }

        private void SetStatusBarInformation(string status)
        {
            if (InvokeRequired)
                Invoke(new SetStatusCallback(SetStatusBarInformation), (object)status);
            else
                lblRecorded.Text = status;
        }

        private void SendGotoCmd(MidiTime mt)
        {
            m_logger.Info("Sending MMC Goto");
            m_gotosent = Environment.TickCount;
            m_midiOut.Send(SysExCommands.Goto(m_curFps | mt.Hour, mt.Minute, mt.Second, mt.Frame, mt.Subframe));
            SetDisplayTime(m_curFps | mt.Hour, mt.Minute, mt.Second, mt.Frame, mt.Subframe);
        }

        private void SendStopCmd()
        {
            m_logger.Info("Sending MMC Stop");
            m_stopsent = Environment.TickCount;
            m_midiOut.Send(SysExCommands.Stop);
            MarkAsStopped();
        }

        private void ShowOpenDialog()
        {
            DialogResult dialogResult = DialogResult.No;
            if (m_haschanged && m_filename != null)
                dialogResult = MessageBox.Show("Save script before opening new file?", "Open", MessageBoxButtons.YesNoCancel);
            if (dialogResult == DialogResult.Yes)
                SaveActiveEpisode();
            if (dialogResult == DialogResult.Cancel)
                return;
            FileSelector fileSelector = new FileSelector();
            int num = (int)fileSelector.ShowDialog();
            if (fileSelector.DialogResult != DialogResult.OK)
                return;
            m_filename = fileSelector.SelectedEpisodeFile;
            m_episode = fileSelector.SelectedEpisodeIdx;
            lblEpisodeNo.Text = "Episode " + fileSelector.SelectedEpisodeNo;
            m_xls = new ExcelScript();
            m_xls.Load(fileSelector.SelectedEpisodeFile, m_episode);
            m_xls.Lines.Sort();
            m_haschanged = false;
            FillCharacterBox();
            ChangeCharacter();
            GotoLineWithLead(m_currentline, true);
            UpdateRecordedStatus();
        }

        private void ShowSettings()
        {
            int num = (int)new SettingsForm().ShowDialog();
            if (Settings.MidiIn != m_midiInDevice)
            {
                CloseMidiIn();
                OpenMidiIn();
            }
            if (Settings.MidiOut != m_midiOutDevice)
            {
                CloseMidiOut();
                OpenMidiOut();
            }
            m_curFps = Settings.Fps;
            m_curLeadSeconds = Settings.LeadSeconds;
        }

        private void Shutdown()
        {
            m_logger.Info("Shutting down");
            CloseMidiOut();
            CloseMidiIn();
        }

        private void Stop()
        {
            bool flag = false;
            if (m_isRecording || m_isPlaying)
                flag = true;
            SendStopCmd();
            m_currentline = GotoLineWithLead(m_currentline, false);
            if (!flag)
                return;
            Timer timer = new Timer(DelayedGotoCurrent, null, 200, -1);
        }

        private void StopIfNeeded(out bool wasPlaying, out bool wasRecording)
        {
            wasPlaying = false;
            wasRecording = false;
            if (!m_isPlaying)
                return;
            wasPlaying = m_isPlaying;
            wasRecording = m_isRecording;
            SendStopCmd();
        }

        private void ToggleMaximized()
        {
            FormBorderStyle = m_isMaximized ? FormBorderStyle.Sizable : FormBorderStyle.None;
            WindowState = m_isMaximized ? FormWindowState.Normal : FormWindowState.Maximized;
            m_isMaximized = !m_isMaximized;
        }

        private void ToggleLineDone()
        {
            if (m_isRecording || m_isPlaying)
                return;
            m_currentline.IsDone = !m_currentline.IsDone;
            SetSaveNeeded();
            DisplayLine(m_currentline, true);
            UpdateRecordedStatus();
        }

        private void InsertLine()
        {
            if (m_isRecording || m_isPlaying || m_currentline == null)
                return;
            m_currentline = m_xls.Lines.Insert(m_currentchar, m_currentTime);
            SetSaveNeeded();
            DisplayLine(m_currentline, true);
            EditCurrentLine();
        }

        private void EditLine(ScriptLine line)
        {
            foreach (ScriptLineBox linebox in m_lineboxes)
            {
                if (linebox.LinkedLine == line)
                    linebox.Edit(m_editstate);
            }
        }

        private void EditCurrentLine()
        {
            EditLine(m_currentline);
        }

        private void DeleteLine()
        {
            if (m_isRecording || m_isPlaying || m_currentline == null)
                return;
            ScriptLine scriptLine = m_currentline.NextByTime;
            if (m_currentchar != null)
                scriptLine = m_currentline.NextByCharacter;
            m_currentline.Delete();
            m_currentline = scriptLine;
            SetSaveNeeded();
            DisplayLine(m_currentline, true);
        }

        private void UpdateScriptLines()
        {
            MidiTime b = BuildMidiTime();
            foreach (ScriptLineBox linebox in m_lineboxes)
            {
                ScriptLine linkedLine = linebox.LinkedLine;
                if ((m_isPlaying || m_isRecording) && IsCurrentChar(linkedLine.Character))
                {
                    int num = MidiTime.SecondsAway(linkedLine.Offset, b);
                    if (num > 0 && num <= m_curLeadSeconds && (linkedLine.Offset > m_startingline.Offset || linkedLine == m_startingline))
                    {
                        m_currentline = linkedLine;
                        if (!linebox.IsActive)
                        {
                            linebox.MarkAsActive(true);
                            ++m_currentlyactive;
                        }
                        if (m_isRecording)
                        {
                            double progress = (m_curLeadSeconds - num) / (double)m_curLeadSeconds;
                            linebox.ShowTimer(true);
                            linebox.SetProgress(progress);
                            if (!m_recordingTriggered && num <= 1)
                            {
                                m_recordingTriggered = true;
                                m_recordstrobesent = Environment.TickCount;
                                m_logger.Info("MMC RecordStrobe Sent");
                                m_midiOut.Send(SysExCommands.RecordStrobe);
                            }
                        }
                    }
                    if (num <= 0 && m_isRecording && (linkedLine.Offset > m_startingline.Offset || linkedLine == m_startingline))
                    {
                        if (!m_recordingTriggered)
                        {
                            m_recordingTriggered = true;
                            m_recordstrobesent = Environment.TickCount;
                            m_logger.Info("MMC RecordStrobe Sent");
                            m_midiOut.Send(SysExCommands.RecordStrobe);
                        }
                        linkedLine.IsDone = true;
                        SetSaveNeeded();
                        UpdateRecordedStatus();
                        linebox.ForeColor = m_currentchar != null ? ActiveDoneColor : DoneColor;
                        linebox.ShowTimer(false);
                        if (linebox.IsActive && m_currentlyactive > 0)
                        {
                            linebox.MarkAsActive(false);
                            --m_currentlyactive;
                        }
                    }
                }
            }
        }

        private void UpdateRecordedStatus()
        {
            int LinesDone;
            int LinesLeft;
            if (m_currentchar != null)
            {
                m_xls.Lines.LineCount(m_currentchar, out LinesDone, out LinesLeft);
                if (LinesLeft == 0)
                    SetCharacterDone(m_currentchar, true);
                else
                    SetCharacterDone(m_currentchar, false);
            }
            else
                m_xls.Lines.LineCount(out LinesDone, out LinesLeft);
            SetStatusBarInformation(string.Format("{0} recorded, {1} left", LinesDone, LinesLeft));
        }

        private void SetCharacterDone(string name, bool done)
        {
            foreach (ToolStripItem dropDownItem in ddCharacter.DropDownItems)
            {
                if (string.Equals((string)dropDownItem.Tag, name))
                {
                    string str = done ? name + " - OK" : name;
                    if (m_currentchar == name)
                        ddCharacter.Text = str;
                    dropDownItem.Text = str;
                }
            }
        }

        private static bool WeDidntSendIt(int lastsent)
        {
            return Environment.TickCount > lastsent + 100;
        }

        private void DubTool_Load(object sender, EventArgs e)
        {
            InitLog4Net();
            m_logger.Info("Starting DubTool");
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            m_curFps = Settings.Fps;
            m_curLeadSeconds = Settings.LeadSeconds;
            OpenMidiIn();
            OpenMidiOut();
            RegisterHotKeys();
            GrabFocus();
            ToggleMaximized();
            statusStrip1.KeyUp += KeyUpHandler;
        }

        private void DubTool_Click(object sender, EventArgs e)
        {
            ClearFocus();
        }

        public void ClearFocus()
        {
            if (InvokeRequired)
                Invoke(new ClearFocusCallback(ClearFocus));
            else
                statusStrip1.Focus();
        }

        public void SetSaveNeeded()
        {
            m_haschanged = true;
        }

        public void SortScript()
        {
            m_xls.Lines.Sort();
            DisplayLine(m_currentline, true);
            ResumeEditing();
        }

        public void UpdateCharacterBox()
        {
            m_xls.Lines.Sort();
            FillCharacterBox();
            DisplayLine(m_currentline, true);
            ResumeEditing();
        }

        private void ResumeEditing()
        {
            if (m_editstate == ScriptLineState.None)
                return;
            EditLine(m_editingline);
        }

        public enum ScriptLineState
        {
            None,
            Character,
            Time,
            Line
        }

        private delegate void ClearFocusCallback();

        private delegate void SetStatusCallback(string status);

        private delegate void SetTimeCallback(int fpshours, int minutes, int seconds, int frames, int subframes);

        private delegate void SetCountdownValueCallback(int value);

        private delegate ScriptLine GotoLineCallback(ScriptLine sl, bool scroll);
    }
}
