// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.FileSelector
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CommonTools;
using Nordubb.DubTool.ScriptIO;
using FileInfo = Nordubb.DubTool.ScriptIO.FileInfo;

namespace Nordubb.DubTool
{
  internal sealed class FileSelector : Form
  {
    private IContainer components;
    private Button btnOpen;
    private Button btnCancel;
    private TreeListView tlvScripts;

    public string SelectedEpisodeFile { get; private set; }

    public int SelectedEpisodeNo { get; private set; }

    public int SelectedEpisodeIdx { get; private set; }

    public FileSelector()
    {
      InitializeComponent();
      FillFileList();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && components != null)
        components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      TreeListColumn treeListColumn1 = new TreeListColumn("fldName", "Filename");
      TreeListColumn treeListColumn2 = new TreeListColumn("fldTitle", "Title");
      TreeListColumn treeListColumn3 = new TreeListColumn("fldEpisode", "Episode");
      TreeListColumn treeListColumn4 = new TreeListColumn("fldLinesLeft", "Lines done");
      btnOpen = new Button();
      btnCancel = new Button();
      tlvScripts = new TreeListView();
      tlvScripts.BeginInit();
      SuspendLayout();
      btnOpen.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnOpen.DialogResult = DialogResult.OK;
      btnOpen.Enabled = false;
      btnOpen.Location = new Point(431, 257);
      btnOpen.Name = "btnOpen";
      btnOpen.Size = new Size(75, 23);
      btnOpen.TabIndex = 1;
      btnOpen.Text = "Open";
      btnOpen.UseVisualStyleBackColor = true;
      btnOpen.Click += btnOpen_Click;
      btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnCancel.DialogResult = DialogResult.Cancel;
      btnCancel.Location = new Point(350, 258);
      btnCancel.Name = "btnCancel";
      btnCancel.Size = new Size(75, 23);
      btnCancel.TabIndex = 2;
      btnCancel.Text = "Cancel";
      btnCancel.UseVisualStyleBackColor = true;
      btnCancel.Click += btnCancel_Click;
      tlvScripts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      treeListColumn1.AutoSize = true;
      treeListColumn1.AutoSizeMinSize = 0;
      treeListColumn1.Width = 50;
      treeListColumn2.AutoSize = true;
      treeListColumn2.AutoSizeMinSize = 0;
      treeListColumn2.Width = 50;
      treeListColumn3.AutoSizeMinSize = 0;
      treeListColumn3.HeaderFormat.TextAlignment = ContentAlignment.MiddleLeft;
      treeListColumn3.Width = 70;
      treeListColumn4.AutoSizeMinSize = 0;
      treeListColumn4.HeaderFormat.TextAlignment = ContentAlignment.MiddleLeft;
      treeListColumn4.Width = 70;
      tlvScripts.Columns.AddRange(new TreeListColumn[4]
      {
        treeListColumn1,
        treeListColumn2,
        treeListColumn3,
        treeListColumn4
      });
      tlvScripts.Cursor = Cursors.Arrow;
      tlvScripts.Images = null;
      tlvScripts.Location = new Point(13, 13);
      tlvScripts.Name = "tlvScripts";
      tlvScripts.RowOptions.ShowHeader = false;
      tlvScripts.Size = new Size(493, 238);
      tlvScripts.TabIndex = 3;
      tlvScripts.Text = "Scripts";
      tlvScripts.DoubleClick += tlvScripts_DoubleClick;
      tlvScripts.AfterSelect += tlvScripts_AfterSelect;
      tlvScripts.KeyPress += tlvScripts_KeyPress;
      AutoScaleDimensions = new SizeF(6f, 13f);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(518, 293);
      Controls.Add(tlvScripts);
      Controls.Add(btnCancel);
      Controls.Add(btnOpen);
      Name = "FileSelector";
      Text = "Select Script";
      tlvScripts.EndInit();
      ResumeLayout(false);
    }

    private void AddFoldersAndFiles(TreeListViewNodes nodes, string path)
    {
      string[] directories;
      try
      {
        directories = Directory.GetDirectories(path);
      }
      catch (UnauthorizedAccessException ex)
      {
        return;
      }
      catch (DirectoryNotFoundException ex)
      {
        return;
      }
      foreach (string path1 in directories)
      {
        Node node = new Node(new object[4]
        {
          Path.GetFileName(path1),
          null,
          null,
          null
        });
        AddFoldersAndFiles(node, path1);
        nodes.Add(node);
      }
      string[] files;
      try
      {
        files = Directory.GetFiles(path, "*.xls");
      }
      catch (UnauthorizedAccessException ex)
      {
        return;
      }
      catch (DirectoryNotFoundException ex)
      {
        return;
      }
      foreach (string path1 in files)
      {
        try
        {
          FileInfo fileInfo = ExcelScript.GetFileInfo(path1);
          int num = 1;
          Node newnode = new Node(new object[4]
          {
            Path.GetFileName(path1),
            fileInfo.Title,
            string.Empty,
            fileInfo.LinesLeft
          });
          newnode.Tag = path1;
          foreach (EpisodeInfo episode in fileInfo.Episodes)
            newnode.Nodes.Add(new Node(new object[4]
            {
              string.Empty,
              episode.Title,
              episode.Number,
              episode.LinesLeft
            })
            {
              Tag = num++
            });
          nodes.Add(newnode);
        }
        catch (InvalidDataException ex)
        {
        }
        catch (IOException ex)
        {
        }
      }
    }

    private void AddFoldersAndFiles(Node n, string path)
    {
      string[] directories;
      try
      {
        directories = Directory.GetDirectories(path);
      }
      catch (UnauthorizedAccessException ex)
      {
        return;
      }
      catch (DirectoryNotFoundException ex)
      {
        return;
      }
      foreach (string path1 in directories)
      {
        Node node = new Node(new object[4]
        {
          Path.GetFileName(path1),
          null,
          null,
          null
        });
        AddFoldersAndFiles(node, path1);
        n.Nodes.Add(node);
      }
      string[] files;
      try
      {
        files = Directory.GetFiles(path, "*.xls");
      }
      catch (UnauthorizedAccessException ex)
      {
        return;
      }
      catch (DirectoryNotFoundException ex)
      {
        return;
      }
      foreach (string path1 in files)
      {
        try
        {
          FileInfo fileInfo = ExcelScript.GetFileInfo(path1);
          int num = 1;
          Node newnode = new Node(new object[4]
          {
            Path.GetFileName(path1),
            fileInfo.Title,
            string.Empty,
            fileInfo.LinesLeft
          });
          newnode.Tag = path1;
          foreach (EpisodeInfo episode in fileInfo.Episodes)
            newnode.Nodes.Add(new Node(new object[4]
            {
              string.Empty,
              episode.Title,
              episode.Number,
              episode.LinesLeft
            })
            {
              Tag = num++
            });
          n.Nodes.Add(newnode);
        }
        catch (InvalidDataException ex)
        {
        }
        catch (IOException ex)
        {
        }
      }
    }

    private void FillFileList()
    {
      string scriptFolder = Settings.ScriptFolder;
      tlvScripts.BeginUpdate();
      AddFoldersAndFiles(tlvScripts.Nodes, scriptFolder);
      tlvScripts.EndUpdate();
    }

    private void btnOpen_Click(object sender, EventArgs e)
    {
      tlvScripts.BeginUpdate();
      SelectedEpisodeFile = (string) tlvScripts.NodesSelection[0].Parent.Tag;
      SelectedEpisodeIdx = (int) tlvScripts.NodesSelection[0].Tag;
      SelectedEpisodeNo = (int) tlvScripts.NodesSelection[0][2];
      tlvScripts.EndUpdate();
      Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void tlvScripts_DoubleClick(object sender, EventArgs e)
    {
      btnOpen.PerformClick();
    }

    private void tlvScripts_AfterSelect(object sender, TreeViewEventArgs e)
    {
      if (tlvScripts.NodesSelection.Count > 1 || !string.IsNullOrEmpty((string) tlvScripts.NodesSelection[0][0]))
        btnOpen.Enabled = false;
      else
        btnOpen.Enabled = true;
    }

    private void tlvScripts_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == 13)
      {
        e.Handled = true;
        btnOpen.PerformClick();
      }
      else
      {
        if (e.KeyChar != 27)
          return;
        e.Handled = true;
        btnCancel.PerformClick();
      }
    }
  }
}
