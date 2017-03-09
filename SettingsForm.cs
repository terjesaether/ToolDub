// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.SettingsForm
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

namespace Nordubb.DubTool
{
  internal sealed class SettingsForm : Form
  {
    private IContainer components;
    private Label lblMidiOutCountLabel;
    private Label lblMidiOutLabel;
    private ComboBox cmbMidiOut;
    private GroupBox grpSettings;
    private Label lblMidiInCountLabel;
    private Label lblMidiOutCount;
    private Label lblMidiInLabel;
    private ComboBox cmbMidiIn;
    private Label lblMidiInCount;
    private Label lblFps;
    private ComboBox cmbFps;
    private Label lblFolder;
    private TextBox txtLead;
    private Label lblLead;
    private TextBox txtFolder;
    private Button btnBrowseForFolder;
    private ImageList imglistIcons;
    private int m_midiInCount;
    private int m_midiOutCount;
    private Dictionary<int, string> m_fps;
    private Dictionary<int, string> m_midiIns;
    private Dictionary<int, string> m_midiOuts;

    public SettingsForm()
    {
      InitializeComponent();
      GetMidiDeviceInfo();
      InitializeFps();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && components != null)
        components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      components = new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (SettingsForm));
      lblMidiOutCountLabel = new Label();
      lblMidiOutLabel = new Label();
      cmbMidiOut = new ComboBox();
      grpSettings = new GroupBox();
      btnBrowseForFolder = new Button();
      imglistIcons = new ImageList(components);
      txtLead = new TextBox();
      lblLead = new Label();
      txtFolder = new TextBox();
      lblFolder = new Label();
      lblFps = new Label();
      cmbFps = new ComboBox();
      lblMidiInCountLabel = new Label();
      lblMidiOutCount = new Label();
      lblMidiInLabel = new Label();
      cmbMidiIn = new ComboBox();
      lblMidiInCount = new Label();
      grpSettings.SuspendLayout();
      SuspendLayout();
      lblMidiOutCountLabel.AutoSize = true;
      lblMidiOutCountLabel.Location = new Point(6, 16);
      lblMidiOutCountLabel.Name = "lblMidiOutCountLabel";
      lblMidiOutCountLabel.Size = new Size(95, 13);
      lblMidiOutCountLabel.TabIndex = 0;
      lblMidiOutCountLabel.Text = "# of Midi out devs:";
      lblMidiOutLabel.AutoSize = true;
      lblMidiOutLabel.Location = new Point(6, 55);
      lblMidiOutLabel.Name = "lblMidiOutLabel";
      lblMidiOutLabel.Size = new Size(47, 13);
      lblMidiOutLabel.TabIndex = 5;
      lblMidiOutLabel.Text = "Midi out:";
      cmbMidiOut.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbMidiOut.FormattingEnabled = true;
      cmbMidiOut.Location = new Point(59, 52);
      cmbMidiOut.Name = "cmbMidiOut";
      cmbMidiOut.Size = new Size(222, 21);
      cmbMidiOut.TabIndex = 12;
      grpSettings.Controls.Add(btnBrowseForFolder);
      grpSettings.Controls.Add(txtLead);
      grpSettings.Controls.Add(lblLead);
      grpSettings.Controls.Add(txtFolder);
      grpSettings.Controls.Add(lblFolder);
      grpSettings.Controls.Add(lblFps);
      grpSettings.Controls.Add(cmbFps);
      grpSettings.Controls.Add(lblMidiOutCountLabel);
      grpSettings.Controls.Add(lblMidiInCountLabel);
      grpSettings.Controls.Add(lblMidiOutCount);
      grpSettings.Controls.Add(lblMidiOutLabel);
      grpSettings.Controls.Add(lblMidiInLabel);
      grpSettings.Controls.Add(cmbMidiOut);
      grpSettings.Controls.Add(cmbMidiIn);
      grpSettings.Controls.Add(lblMidiInCount);
      grpSettings.Location = new Point(3, 0);
      grpSettings.Name = "grpSettings";
      grpSettings.Size = new Size(287, 166);
      grpSettings.TabIndex = 24;
      grpSettings.TabStop = false;
      grpSettings.Text = "Settings";
      btnBrowseForFolder.ImageIndex = 0;
      btnBrowseForFolder.ImageList = imglistIcons;
      btnBrowseForFolder.Location = new Point(258, 131);
      btnBrowseForFolder.Name = "btnBrowseForFolder";
      btnBrowseForFolder.Size = new Size(23, 23);
      btnBrowseForFolder.TabIndex = 21;
      btnBrowseForFolder.UseVisualStyleBackColor = true;
      btnBrowseForFolder.Click += btnBrowseForFolder_Click;
      imglistIcons.ImageStream = (ImageListStreamer) componentResourceManager.GetObject("imglistIcons.ImageStream");
      imglistIcons.TransparentColor = Color.Transparent;
      imglistIcons.Images.SetKeyName(0, "openfolderHS.png");
      txtLead.Location = new Point(204, 106);
      txtLead.Name = "txtLead";
      txtLead.Size = new Size(77, 20);
      txtLead.TabIndex = 20;
      lblLead.AutoSize = true;
      lblLead.Location = new Point(121, 109);
      lblLead.Name = "lblLead";
      lblLead.Size = new Size(77, 13);
      lblLead.TabIndex = 19;
      lblLead.Text = "Lead seconds:";
      txtFolder.Location = new Point(59, 133);
      txtFolder.Name = "txtFolder";
      txtFolder.Size = new Size(191, 20);
      txtFolder.TabIndex = 18;
      lblFolder.AutoSize = true;
      lblFolder.Location = new Point(6, 136);
      lblFolder.Name = "lblFolder";
      lblFolder.Size = new Size(39, 13);
      lblFolder.TabIndex = 17;
      lblFolder.Text = "Folder:";
      lblFps.AutoSize = true;
      lblFps.Location = new Point(6, 109);
      lblFps.Name = "lblFps";
      lblFps.Size = new Size(27, 13);
      lblFps.TabIndex = 16;
      lblFps.Text = "Fps:";
      cmbFps.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbFps.FormattingEnabled = true;
      cmbFps.Location = new Point(59, 106);
      cmbFps.Name = "cmbFps";
      cmbFps.Size = new Size(56, 21);
      cmbFps.TabIndex = 15;
      lblMidiInCountLabel.AutoSize = true;
      lblMidiInCountLabel.Location = new Point(6, 34);
      lblMidiInCountLabel.Name = "lblMidiInCountLabel";
      lblMidiInCountLabel.Size = new Size(88, 13);
      lblMidiInCountLabel.TabIndex = 1;
      lblMidiInCountLabel.Text = "# of Midi in devs:";
      lblMidiOutCount.Location = new Point(109, 16);
      lblMidiOutCount.Name = "lblMidiOutCount";
      lblMidiOutCount.Size = new Size(100, 13);
      lblMidiOutCount.TabIndex = 2;
      lblMidiInLabel.AutoSize = true;
      lblMidiInLabel.Location = new Point(6, 82);
      lblMidiInLabel.Name = "lblMidiInLabel";
      lblMidiInLabel.Size = new Size(40, 13);
      lblMidiInLabel.TabIndex = 6;
      lblMidiInLabel.Text = "Midi in:";
      cmbMidiIn.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbMidiIn.FormattingEnabled = true;
      cmbMidiIn.Location = new Point(59, 79);
      cmbMidiIn.Name = "cmbMidiIn";
      cmbMidiIn.Size = new Size(222, 21);
      cmbMidiIn.TabIndex = 13;
      lblMidiInCount.Location = new Point(109, 34);
      lblMidiInCount.Name = "lblMidiInCount";
      lblMidiInCount.Size = new Size(97, 13);
      lblMidiInCount.TabIndex = 3;
      AutoScaleDimensions = new SizeF(6f, 13f);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(292, 170);
      Controls.Add(grpSettings);
      Name = "SettingsForm";
      Text = "Settings";
      FormClosing += SettingsForm_FormClosing;
      grpSettings.ResumeLayout(false);
      grpSettings.PerformLayout();
      ResumeLayout(false);
    }

    private void GetMidiDeviceInfo()
    {
      m_midiOutCount = OutputDeviceBase.DeviceCount;
      m_midiInCount = InputDevice.DeviceCount;
      lblMidiOutCount.Text = m_midiOutCount.ToString();
      lblMidiInCount.Text = m_midiInCount.ToString();
      m_midiOuts = new Dictionary<int, string>();
      for (int index = 0; index < m_midiOutCount; ++index)
      {
        MidiOutCaps deviceCapabilities = OutputDeviceBase.GetDeviceCapabilities(index);
        m_midiOuts.Add(index, deviceCapabilities.name);
      }
      m_midiIns = new Dictionary<int, string>();
      for (int index = 0; index < m_midiInCount; ++index)
      {
        MidiInCaps deviceCapabilities = InputDevice.GetDeviceCapabilities(index);
        m_midiIns.Add(index, deviceCapabilities.name);
      }
      cmbMidiIn.DataSource = new BindingSource(m_midiIns, null);
      cmbMidiIn.DisplayMember = "Value";
      cmbMidiIn.ValueMember = "Key";
      cmbMidiIn.SelectedValue = Settings.MidiIn;
      cmbMidiOut.DataSource = new BindingSource(m_midiOuts, null);
      cmbMidiOut.DisplayMember = "Value";
      cmbMidiOut.ValueMember = "Key";
      cmbMidiOut.SelectedValue = Settings.MidiOut;
    }

    private void InitializeFps()
    {
      m_fps = new Dictionary<int, string>();
      m_fps.Add(0, "24");
      m_fps.Add(32, "25");
      m_fps.Add(96, "29.97");
      m_fps.Add(64, "30");
      cmbFps.DisplayMember = "Value";
      cmbFps.ValueMember = "Key";
      cmbFps.DataSource = new BindingSource(m_fps, null);
      cmbFps.SelectedValue = Settings.Fps;
      txtLead.Text = Settings.LeadSeconds.ToString();
      txtFolder.Text = Settings.ScriptFolder;
    }

    private void SaveSettings()
    {
      Settings.Fps = (int) cmbFps.SelectedValue;
      Settings.MidiIn = (int) cmbMidiIn.SelectedValue;
      Settings.MidiOut = (int) cmbMidiOut.SelectedValue;
      SetLead();
      Settings.ScriptFolder = txtFolder.Text;
    }

    private void SetLead()
    {
      try
      {
        Settings.LeadSeconds = int.Parse(txtLead.Text);
      }
      catch (ArgumentNullException ex)
      {
      }
      catch (FormatException ex)
      {
      }
      catch (OverflowException ex)
      {
      }
    }

    private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      SaveSettings();
    }

    private void btnBrowseForFolder_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
      if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
        return;
      txtFolder.Text = folderBrowserDialog.SelectedPath;
    }
  }
}
