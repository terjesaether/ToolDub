// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.ScriptIO.ExcelScript
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using SpreadsheetGear;
using SpreadsheetGear.Drawing;

namespace Nordubb.DubTool.ScriptIO
{
  public class ExcelScript
  {
    private static readonly string ShowInfoCell = "A2";
    private static readonly string EpisodeNoRowStartCell = "E4";
    private static readonly string LinesTotalRowStartCell = "E5";
    private static readonly string LinesTotalLeftRowCell = "R5";
    private static readonly string OriginalColumnStart = "A7";
    private static readonly string TranslatedColumnStart = "B7";
    private static readonly string ActorColumnStart = "C7";
    private static readonly string CommentColumnStart = "D7";
    private static readonly string TotalCharacterTakenStart = "R7";
    private static readonly int LinesLeftMatrixColumn = 4;
    private static readonly string ActorEpisodeNoRowStartCell = "E43";
    private static readonly string ActorOverviewColumnStart = "C43";
    private static readonly string OriginalTitleCell = "B2";
    private static readonly string TranslatedTitleCell = "B3";
    private static readonly string EpisodeNoCell = "B5";
    private static readonly string OriginalEpisodeTitleCell = "B7";
    private static readonly string TranslatedEpisodeTitleCell = "B8";
    private static readonly string TranslatedByCell = "B10";
    private static readonly string TranslationDateCell = "B11";
    private static readonly string SummaryCell = "B13";
    private static readonly int ScriptStartRow = 24;
    private static readonly int CharacterColumn = 0;
    private static readonly int TimeColumn = 1;
    private static readonly int LineColumn = 2;
    private static readonly int CheckboxColumn = 3;
    private static readonly int LineSpacing = 2;
    private readonly Color DoneColor = Color.FromArgb(0, 176, 80);
    private readonly Color InProgressColor = Color.FromArgb(byte.MaxValue, 192, 0);
    private readonly Color NotStartedColor = Color.FromArgb(byte.MaxValue, 0, 0);
    private ScriptLineCollection m_lines;
    private int m_episodeNo;
    private string m_path;
    private int m_episodes;

    public string ShowTitleOriginal { get; set; }

    public string ShowTitleDubbed { get; set; }

    public string EpisodeNumber { get; set; }

    public string EpisodeTitleOriginal { get; set; }

    public string EpisodeTitleDubbed { get; set; }

    public string EpisodeTranslator { get; set; }

    public DateTime EpisodeTranslationDate { get; set; }

    public string Resume { get; set; }

    public ScriptLineCollection Lines
    {
      get
      {
        return m_lines;
      }
    }

    public string[] Characters
    {
      get
      {
        return m_lines.Characters;
      }
    }

    public ExcelScript()
    {
      m_lines = new ScriptLineCollection(null);
    }

    public static FileInfo GetFileInfo(string path)
    {
      IWorkbook workbook;
      try
      {
        workbook = Factory.GetWorkbook(path);
      }
      catch (InvalidOperationException ex)
      {
        throw new InvalidDataException("Script not in supported format");
      }
      IWorksheet worksheet = workbook.Worksheets[0];
      FileInfo fileInfo = new FileInfo();
      fileInfo.Title = worksheet.Cells[ShowInfoCell].Formula;
      if (string.IsNullOrEmpty(fileInfo.Title))
        throw new InvalidDataException("Script title not found in expected position");
      int length = workbook.Worksheets.Count - 1;
      fileInfo.Episodes = new EpisodeInfo[length];
      for (int columnOffset = 0; columnOffset < length; ++columnOffset)
      {
        fileInfo.Episodes[columnOffset] = new EpisodeInfo();
        IRange range = worksheet.Cells[EpisodeNoRowStartCell].Offset(0, columnOffset);
        if (!int.TryParse(range.Formula, out fileInfo.Episodes[columnOffset].Number))
        {
          int num = (int) MessageBox.Show(string.Format("Invalid episode number format in {0}, sheet {1}, cell {2}", path, worksheet.Name, range.GetAddress(false, false, ReferenceStyle.A1, false, null)), "Error parsing episode number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          throw new InvalidDataException("Episode number format invalid");
        }
        fileInfo.Episodes[columnOffset].Title = workbook.Worksheets[columnOffset + 1].Cells[TranslatedTitleCell].Formula;
        if (string.IsNullOrEmpty(fileInfo.Episodes[columnOffset].Title))
          throw new InvalidDataException("Episode title not found in expected position");
        fileInfo.Episodes[columnOffset].FullNumber = workbook.Worksheets[columnOffset + 1].Cells[EpisodeNoCell].Formula;
        fileInfo.Episodes[columnOffset].LinesLeft = worksheet.Cells[LinesTotalRowStartCell].Offset(0, columnOffset).Formula;
      }
      fileInfo.LinesLeft = worksheet.Cells[LinesTotalLeftRowCell].Formula;
      return fileInfo;
    }

    public void Load(string path, int episode)
    {
      m_path = path;
      m_episodeNo = episode;
      IWorkbook workbook = Factory.GetWorkbook(path);
      m_episodes = workbook.Worksheets.Count - 1;
      IWorksheet worksheet = workbook.Worksheets[m_episodeNo];
      ShowTitleOriginal = worksheet.Cells[OriginalTitleCell].Formula;
      ShowTitleDubbed = worksheet.Cells[TranslatedEpisodeTitleCell].Formula;
      EpisodeNumber = worksheet.Cells[EpisodeNoCell].Formula;
      EpisodeTitleOriginal = worksheet.Cells[OriginalEpisodeTitleCell].Formula;
      EpisodeTitleDubbed = worksheet.Cells[TranslatedEpisodeTitleCell].Formula;
      EpisodeTranslator = worksheet.Cells[TranslatedByCell].Formula;
      try
      {
        EpisodeTranslationDate = !(worksheet.Cells[TranslationDateCell].Value is double) ? (!(worksheet.Cells[TranslationDateCell].Value is DateTime) ? DateTime.ParseExact(worksheet.Cells[TranslationDateCell].Formula, "MM.dd.yyyy", CultureInfo.InvariantCulture) : (DateTime) worksheet.Cells[TranslationDateCell].Value) : DateTime.FromOADate((double) worksheet.Cells[TranslationDateCell].Value);
      }
      catch (ArgumentException ex)
      {
        ShowDateParsingError(worksheet);
      }
      catch (FormatException ex)
      {
        ShowDateParsingError(worksheet);
      }
      Resume = worksheet.Cells[SummaryCell].Formula;
      int scriptStartRow = ScriptStartRow;
      ScriptLine list = null;
      ScriptLine scriptLine1 = null;
      Dictionary<string, ScriptLine> dictionary = new Dictionary<string, ScriptLine>();
      while (scriptStartRow <= worksheet.UsedRange.RowCount)
      {
        IRange cell1 = worksheet.Cells[scriptStartRow, CharacterColumn];
        IRange cell2 = worksheet.Cells[scriptStartRow, TimeColumn];
        IRange cell3 = worksheet.Cells[scriptStartRow, LineColumn];
        IRange cell4 = worksheet.Cells[scriptStartRow, CheckboxColumn];
        string key = cell1.MergeArea.Formula.Trim();
        string formula1 = cell2.MergeArea.Formula;
        string str = cell3.MergeArea.Formula.Trim();
        string formula2 = cell4.MergeArea.Formula;
        if (!string.IsNullOrEmpty(key))
        {
          ScriptLine scriptLine2 = new ScriptLine();
          scriptLine2.Character = key;
          try
          {
            scriptLine2.Offset = MidiTime.Parse(formula1);
          }
          catch (FormatException ex)
          {
            int num = (int) MessageBox.Show("Invalid time code formatting in sheet " + worksheet.Name + " cell " + cell2.GetAddress(false, false, ReferenceStyle.A1, false, null));
            scriptLine2.Offset = MidiTime.Zero;
          }
          scriptLine2.Line = str;
          scriptLine2.IsDone = !string.IsNullOrEmpty(formula2);
          if (list == null)
            list = scriptLine2;
          scriptLine2.PrevByTime = scriptLine1;
          if (scriptLine1 != null)
            scriptLine1.NextByTime = scriptLine2;
          if (dictionary.ContainsKey(key))
          {
            scriptLine2.PrevByCharacter = dictionary[key];
            dictionary[key].NextByCharacter = scriptLine2;
          }
          scriptLine1 = scriptLine2;
          dictionary[key] = scriptLine2;
        }
        scriptStartRow += cell1.MergeArea.RowCount;
      }
      m_lines = new ScriptLineCollection(list);
      m_lines.Sort();
      workbook.Close();
    }

    private static void ShowDateParsingError(IWorksheet ws)
    {
      int num = (int) MessageBox.Show("Error parsing translation date in sheet " + ws.Name + " cell " + TranslationDateCell);
    }

    public void Save(string path)
    {
      Color color = Color.FromArgb(204, byte.MaxValue, 204);
      IWorkbook workbook = Factory.GetWorkbook(path);
      UpdateTotals(workbook.Worksheets[0]);
      IWorksheet worksheet = workbook.Worksheets[m_episodeNo];
      int scriptStartRow = ScriptStartRow;
      worksheet.Cells[scriptStartRow, CharacterColumn, worksheet.UsedRange.RowCount - 1, CheckboxColumn].Clear();
      worksheet.Cells[scriptStartRow, CharacterColumn, worksheet.UsedRange.RowCount - 1, CheckboxColumn].Rows.AutoFit();
      for (ScriptLine scriptLine = m_lines.GetFirstLine(); scriptLine != null; scriptLine = scriptLine.NextByTime)
      {
        IRange cell1 = worksheet.Cells[scriptStartRow, CharacterColumn];
        IRange cell2 = worksheet.Cells[scriptStartRow, TimeColumn];
        IRange cell3 = worksheet.Cells[scriptStartRow, LineColumn];
        IRange cell4 = worksheet.Cells[scriptStartRow, CheckboxColumn];
        worksheet.Cells[scriptStartRow, CharacterColumn, scriptStartRow, CheckboxColumn].VerticalAlignment = VAlign.Top;
        cell1.MergeArea.MergeCells = false;
        cell2.MergeArea.MergeCells = false;
        cell3.MergeArea.MergeCells = false;
        cell4.MergeArea.MergeCells = false;
        cell3.WrapText = true;
        cell1.Value = scriptLine.Character;
        cell2.Value = scriptLine.Offset.ToString();
        cell3.Value = scriptLine.Line;
        IRange cell5 = worksheet.Cells[scriptStartRow, CharacterColumn, scriptStartRow, LineColumn];
        if (scriptLine.IsDone)
        {
          cell5.Interior.Color = color;
          cell4.Value = "ü";
          cell4.Font.Name = "Wingdings";
        }
        else
          cell5.Interior.ColorIndex = -2;
        scriptStartRow += LineSpacing;
      }
      workbook.Save();
      workbook.Close();
    }

    private Dictionary<string, ActorData> BuildActorsList(IWorksheet ws)
    {
      Dictionary<string, ActorData> dictionary = new Dictionary<string, ActorData>();
      for (IRange range = ws.Cells[ActorColumnStart]; !string.IsNullOrEmpty(range.Formula); range = range.Offset(1, 0))
      {
        string formula = range.Formula;
        if (!dictionary.ContainsKey(formula))
          dictionary[formula] = new ActorData
          {
            Name = formula,
            started = false,
            epleft = new int[m_episodes],
            epstarted = new bool[m_episodes]
          };
        ActorData actorData = dictionary[formula];
        if (Convert.ToInt32(ws.Cells[range.Row, LinesLeftMatrixColumn + m_episodes].Value) != 0)
        {
          for (int index = 0; index < m_episodes; ++index)
          {
            IRange cell = ws.Cells[range.Row, LinesLeftMatrixColumn + index];
            int int32 = Convert.ToInt32(cell.Value);
            actorData.epleft[index] += int32;
            actorData.left += int32;
            if (cell.Interior.Color == DoneColor || cell.Interior.Color == InProgressColor)
            {
              actorData.epstarted[index] = true;
              actorData.started = true;
            }
          }
        }
      }
      return dictionary;
    }

    private IRange FindCharacterCell(IWorksheet ws, string character)
    {
      IRange range = ws.Cells[TranslatedColumnStart];
      while (!string.IsNullOrEmpty(range.Formula) && !character.Equals(range.Formula, StringComparison.CurrentCultureIgnoreCase))
        range = range.Offset(1, 0);
      if (string.IsNullOrEmpty(range.Formula))
        range = null;
      return range;
    }

    private void GetLinesLeft(string character, out int left, out int total)
    {
      left = 0;
      total = 0;
      for (ScriptLine scriptLine = m_lines.GetFirstLine(character); scriptLine != null; scriptLine = scriptLine.NextByCharacter)
      {
        ++total;
        if (!scriptLine.IsDone)
          ++left;
      }
    }

    private void GetLinesLeft(string character, int episode, out int left, out int total)
    {
      ExcelScript excelScript = new ExcelScript();
      excelScript.Load(m_path, episode);
      left = 0;
      total = 0;
      for (ScriptLine scriptLine = excelScript.Lines.GetFirstLine(character); scriptLine != null; scriptLine = scriptLine.NextByCharacter)
      {
        ++total;
        if (!scriptLine.IsDone)
          ++left;
      }
    }

    private void UpdateActors(IWorksheet ws)
    {
      Dictionary<string, ActorData> dictionary = BuildActorsList(ws);
      for (IRange range = ws.Cells[ActorOverviewColumnStart]; !string.IsNullOrEmpty(range.Formula); range = range.Offset(1, 0))
      {
        ActorData actorData = dictionary[range.Formula];
        range.Interior.Color = actorData.left != 0 ? (!actorData.started ? NotStartedColor : InProgressColor) : DoneColor;
        for (int index = 0; index < m_episodes; ++index)
        {
          IRange cell = ws.Cells[range.Row, LinesLeftMatrixColumn + index];
          cell.Value = actorData.epleft[index];
          cell.Interior.Color = actorData.epleft[index] != 0 ? (!actorData.epstarted[index] ? NotStartedColor : InProgressColor) : DoneColor;
        }
      }
      for (IRange range = ws.Cells[ActorColumnStart]; !string.IsNullOrEmpty(range.Formula); range = range.Offset(1, 0))
      {
        ActorData actorData = dictionary[range.Formula];
        range.Interior.Color = actorData.left != 0 ? (!actorData.started ? NotStartedColor : InProgressColor) : DoneColor;
      }
    }

    private void UpdateCharacters(IWorksheet ws)
    {
      Dictionary<string, ActorData> dictionary = new Dictionary<string, ActorData>();
      for (IRange range = ws.Cells[TranslatedColumnStart]; !string.IsNullOrEmpty(range.Formula); range = range.Offset(1, 0))
      {
        if (Convert.ToInt32(ws.Cells[range.Row, LinesLeftMatrixColumn + m_episodes].Value) == 0)
        {
          range.Interior.Color = DoneColor;
        }
        else
        {
          bool flag = false;
          for (int index = 0; index < m_episodes; ++index)
          {
            IRange cell = ws.Cells[range.Row, LinesLeftMatrixColumn + index];
            if (cell.Interior.Color == DoneColor || cell.Interior.Color == InProgressColor)
            {
              flag = true;
              break;
            }
          }
          range.Interior.Color = !flag ? NotStartedColor : InProgressColor;
        }
      }
    }

    private void UpdateFrontPage(IWorksheet ws)
    {
      int length = GetFileInfo(m_path).Episodes.Length;
      Dictionary<string, CharacterStats> characters = BuildCharacterDictionary(length);
      UpdateFrontPageCharacters(ws, length, characters);
      UpdateFrontPageActors(ws, length, characters);
    }

    private void UpdateFrontPageCharacters(IWorksheet ws, int episodes, Dictionary<string, CharacterStats> characters)
    {
      List<int> intList1 = InitializeEpisodeList(episodes);
      List<int> intList2 = InitializeEpisodeList(episodes);
      foreach (string key in characters.Keys)
      {
        IRange characterCell = FindCharacterCell(ws, key);
        if (characterCell == null)
        {
          int num = (int) MessageBox.Show(string.Format("Warning: Character \"{0}\" not found in character list.", key), "Character warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        else
        {
          IRange cell = ws.Cells[characterCell.Row, ws.Cells[TotalCharacterTakenStart].Column];
          cell.NumberFormat = "@";
          int done;
          int total;
          characters[key].GetCharacterTotals(out done, out total);
          cell.Formula = string.Format("{0}/{1}", done, total);
          FillCellBackground(characterCell, done, total);
          FillCellBackground(cell, done, total);
        }
        for (int index1 = 0; index1 < episodes; ++index1)
        {
          int done = characters[key].Episodes[index1].done;
          int total = characters[key].Episodes[index1].total;
          List<int> intList3;
          int index2;
          (intList3 = intList1)[index2 = index1] = intList3[index2] + done;
          List<int> intList4;
          int index3;
          (intList4 = intList2)[index3 = index1] = intList4[index3] + total;
          if (characterCell != null)
          {
            IRange cell = ws.Cells[characterCell.Row, LinesLeftMatrixColumn + index1];
            cell.NumberFormat = "@";
            cell.Formula = total == 0 ? string.Empty : string.Format("{0}/{1}", done, total);
            FillCellBackground(cell, done, total);
          }
        }
      }
      int done1 = 0;
      int total1 = 0;
      for (int columnOffset = 0; columnOffset < episodes; ++columnOffset)
      {
        IRange cell = ws.Cells[LinesTotalRowStartCell].Offset(0, columnOffset);
        cell.NumberFormat = "@";
        cell.Formula = string.Format("{0}/{1}", intList1[columnOffset], intList2[columnOffset]);
        FillCellBackground(cell, intList1[columnOffset], intList2[columnOffset]);
        done1 += intList1[columnOffset];
        total1 += intList2[columnOffset];
      }
      IRange cell1 = ws.Cells[LinesTotalLeftRowCell];
      cell1.NumberFormat = "@";
      cell1.Formula = string.Format("{0}/{1}", done1, total1);
      FillCellBackground(cell1, done1, total1);
    }

    private void UpdateFrontPageActors(IWorksheet ws, int episodes, Dictionary<string, CharacterStats> characters)
    {
      Dictionary<string, List<string>> dictionary = BuildActorDictionary(ws, characters);
      foreach (string key in dictionary.Keys)
      {
        IRange actorCell = FindActorCell(ws, key);
        if (actorCell == null)
        {
          int num = (int) MessageBox.Show(string.Format("Warning: Actor \"{0}\" not found in actor overview list.", key), "Actor warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        else
        {
          int done1 = 0;
          int total1 = 0;
          for (int index1 = 0; index1 < episodes; ++index1)
          {
            int done2 = 0;
            int total2 = 0;
            foreach (string index2 in dictionary[key])
            {
              done2 += characters[index2].Episodes[index1].done;
              total2 += characters[index2].Episodes[index1].total;
            }
            IRange cell = ws.Cells[actorCell.Row, LinesLeftMatrixColumn].Offset(0, index1 + 1);
            cell.NumberFormat = "@";
            if (total2 != 0)
            {
              cell.Formula = string.Format("{0}/{1}", done2, total2);
              FillCellBackground(cell, done2, total2);
            }
            else
              cell.Formula = string.Empty;
            done1 += done2;
            total1 += total2;
          }
          IRange cell1 = ws.Cells[actorCell.Row, ws.Cells[TotalCharacterTakenStart].Column];
          cell1.NumberFormat = "@";
          cell1.Formula = string.Format("{0}/{1}", done1, total1);
          FillCellBackground(cell1, done1, total1);
          FillCellBackground(actorCell, done1, total1);
        }
      }
    }

    private IRange FindActorCell(IWorksheet ws, string actor)
    {
      IRange range = ws.Cells[ActorOverviewColumnStart];
      while (!string.IsNullOrEmpty(range.Formula) && !actor.Equals(range.Formula, StringComparison.CurrentCultureIgnoreCase))
        range = range.Offset(1, 0);
      if (string.IsNullOrEmpty(range.Formula))
        range = null;
      return range;
    }

    private Dictionary<string, List<string>> BuildActorDictionary(IWorksheet ws, Dictionary<string, CharacterStats> characters)
    {
      Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
      foreach (string key in characters.Keys)
      {
        IRange characterCell = FindCharacterCell(ws, key);
        if (characterCell != null)
        {
          string formula = ws.Cells[characterCell.Row, ws.Cells[ActorColumnStart].Column].Formula;
          if (!string.IsNullOrEmpty(formula))
          {
            if (!dictionary.ContainsKey(formula))
              dictionary.Add(formula, new List<string>());
            dictionary[formula].Add(key);
          }
        }
      }
      return dictionary;
    }

    private static List<int> InitializeEpisodeList(int episodes)
    {
      List<int> intList = new List<int>();
      for (int index = 0; index < episodes; ++index)
        intList.Add(0);
      return intList;
    }

    private Dictionary<string, CharacterStats> BuildCharacterDictionary(int episodes)
    {
      Dictionary<string, CharacterStats> dictionary = new Dictionary<string, CharacterStats>(StringComparer.CurrentCultureIgnoreCase);
      for (int index = 0; index < episodes; ++index)
      {
        ExcelScript excelScript = new ExcelScript();
        excelScript.Load(m_path, index + 1);
        foreach (string character in excelScript.Characters)
        {
          if (!dictionary.ContainsKey(character))
            dictionary.Add(character, new CharacterStats());
          int left;
          int total;
          excelScript.GetLinesLeft(character, out left, out total);
          while (dictionary[character].Episodes.Count < index + 1)
            dictionary[character].Episodes.Add(new EpisodeStats());
          dictionary[character].Episodes[index].done = total - left;
          dictionary[character].Episodes[index].total = total;
        }
      }
      foreach (CharacterStats characterStats in dictionary.Values)
      {
        while (characterStats.Episodes.Count < episodes + 1)
          characterStats.Episodes.Add(new EpisodeStats());
      }
      return dictionary;
    }

    private void FillCellBackground(IRange cell, int done, int total)
    {
      if (total == 0)
        cell.Interior.Color = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue);
      else if (done == total)
        cell.Interior.Color = DoneColor;
      else if (done > 0)
        cell.Interior.Color = InProgressColor;
      else
        cell.Interior.Color = NotStartedColor;
    }

    private void UpdateEpisode(IWorksheet ws)
    {
      int num1 = 0;
      foreach (string character in Characters)
      {
        IRange characterCell = FindCharacterCell(ws, character);
        if (characterCell != null)
        {
          IRange cell = ws.Cells[characterCell.Row, LinesLeftMatrixColumn + m_episodeNo - 1];
          int left;
          int total;
          GetLinesLeft(character, out left, out total);
          num1 += total;
          int num2 = total - left;
          cell.Value = num2;
          cell.NumberFormat = string.Format("0\"/{0}\"", total);
          cell.Interior.Color = left != 0 ? (left >= total ? NotStartedColor : InProgressColor) : DoneColor;
        }
      }
      ws.Cells[LinesTotalRowStartCell].Offset(0, m_episodeNo - 1).Value = num1;
    }

    private void UpdateTotals(IWorksheet ws)
    {
      UpdateFrontPage(ws);
    }

    private class CharacterStats
    {
      public List<EpisodeStats> Episodes = new List<EpisodeStats>();

      public void GetCharacterTotals(out int done, out int total)
      {
        done = 0;
        total = 0;
        foreach (EpisodeStats episode in Episodes)
        {
          done += episode.done;
          total += episode.total;
        }
      }
    }

    private class EpisodeStats
    {
      public int done;
      public int total;
    }

    private class CharacterMatrix
    {
      private Dictionary<string, int>[] m_done;
      private Dictionary<string, int>[] m_total;

      public void GetInfo(string character, int episode, out int done, out int total)
      {
        done = m_done[episode][character];
        total = m_total[episode][character];
      }

      public void GetEpisodeTotal(int episode, out int done, out int total)
      {
        done = 0;
        foreach (int num in m_done[episode].Values)
          done += num;
        total = 0;
        foreach (int num in m_total[episode].Values)
          total += num;
      }

      public void GetCharacterTotal(string character, out int done, out int total)
      {
        done = 0;
        total = 0;
        for (int index = 0; index < m_done.Length; ++index)
        {
          done += m_done[index][character];
          total += m_total[index][character];
        }
      }
    }

    private class ActorData
    {
      public string Name;
      public int left;
      public bool started;
      public int[] epleft;
      public bool[] epstarted;
    }
  }
}
