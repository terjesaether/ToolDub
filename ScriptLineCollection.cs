// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.ScriptLineCollection
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using System;
using System.Collections.Generic;

namespace Nordubb.DubTool
{
  public class ScriptLineCollection
  {
    private ScriptLine m_list;
    private string[] m_characters;

    public string[] Characters
    {
      get
      {
        return m_characters;
      }
    }

    public ScriptLineCollection(ScriptLine list)
    {
      m_list = list;
    }

    public ScriptLine Insert(string character, MidiTime offset)
    {
      ScriptLine scriptLine1 = new ScriptLine();
      scriptLine1.Character = character;
      scriptLine1.Offset = offset;
      ScriptLine scriptLine2 = GetFirstLine();
      while (scriptLine2.NextByTime != null && scriptLine2.Offset < offset)
        scriptLine2 = scriptLine2.NextByTime;
      if (scriptLine2.Offset >= offset)
      {
        scriptLine1.NextByTime = scriptLine2;
        scriptLine1.PrevByTime = scriptLine2.PrevByTime;
        if (scriptLine1.PrevByTime != null)
          scriptLine1.PrevByTime.NextByTime = scriptLine1;
        else
          m_list = scriptLine1;
        scriptLine2.PrevByTime = scriptLine1;
      }
      else
      {
        scriptLine1.PrevByTime = scriptLine2;
        scriptLine2.NextByTime = scriptLine1;
      }
      UpdateCharacterLinks();
      return scriptLine1;
    }

    private void MoveLine(ScriptLine target, ScriptLine source)
    {
      if (source.PrevByTime != null)
        source.PrevByTime.NextByTime = source.NextByTime;
      if (source.NextByTime != null)
        source.NextByTime.PrevByTime = source.PrevByTime;
      if (target.PrevByTime != null)
        target.PrevByTime.NextByTime = source;
      source.PrevByTime = target.PrevByTime;
      source.NextByTime = target;
      target.PrevByTime = source;
    }

    public void Sort()
    {
      ScriptLine scriptLine = GetFirstLine();
      ScriptLine source = scriptLine.NextByTime;
      while (source != null)
      {
        ScriptLine nextByTime = source.NextByTime;
        ScriptLine target = null;
        for (; scriptLine != null && scriptLine.Offset > source.Offset; scriptLine = scriptLine.PrevByTime)
          target = scriptLine;
        if (target != null)
        {
          MoveLine(target, source);
          if (source.PrevByTime == null)
            m_list = source;
        }
        source = nextByTime;
        if (source != null)
          scriptLine = source.PrevByTime;
      }
      UpdateCharacterLinks();
    }

    private void UpdateCharacterLinks()
    {
      Dictionary<string, ScriptLine> dictionary = new Dictionary<string, ScriptLine>(StringComparer.CurrentCultureIgnoreCase);
      for (ScriptLine scriptLine = GetFirstLine(); scriptLine != null; scriptLine = scriptLine.NextByTime)
      {
        if (scriptLine.Character != null)
        {
          if (dictionary.ContainsKey(scriptLine.Character))
          {
            dictionary[scriptLine.Character].NextByCharacter = scriptLine;
            scriptLine.PrevByCharacter = dictionary[scriptLine.Character];
            dictionary[scriptLine.Character] = scriptLine;
          }
          else
          {
            scriptLine.PrevByCharacter = null;
            dictionary.Add(scriptLine.Character, scriptLine);
          }
        }
      }
      foreach (ScriptLine scriptLine in dictionary.Values)
        scriptLine.NextByCharacter = null;
      m_characters = new string[dictionary.Count];
      dictionary.Keys.CopyTo(m_characters, 0);
      Array.Sort(m_characters);
    }

    public ScriptLine GetFirstLine()
    {
      return m_list;
    }

    public ScriptLine GetFirstLine(string character)
    {
      ScriptLine scriptLine = m_list;
      while (scriptLine != null && scriptLine.Character != character)
        scriptLine = scriptLine.NextByTime;
      return scriptLine;
    }

    public ScriptLine GetFirstUnrecordedLine()
    {
      ScriptLine scriptLine = m_list;
      while (scriptLine != null && scriptLine.IsDone)
        scriptLine = scriptLine.NextByTime;
      if (scriptLine == null)
        scriptLine = m_list;
      return scriptLine;
    }

    public ScriptLine GetFirstUnrecordedLine(string character)
    {
      ScriptLine scriptLine = GetFirstLine(character);
      while (scriptLine != null && scriptLine.IsDone)
        scriptLine = scriptLine.NextByCharacter;
      if (scriptLine == null)
        scriptLine = GetFirstLine(character);
      return scriptLine;
    }

    public ScriptLine GetNearestLine(MidiTime time)
    {
      ScriptLine scriptLine = m_list;
      for (ScriptLine nextByTime = m_list.NextByTime; nextByTime != null && scriptLine.Offset < time; nextByTime = nextByTime.NextByTime)
      {
        if (Math.Abs(time.Difference(nextByTime.Offset)) < Math.Abs(time.Difference(scriptLine.Offset)))
          scriptLine = nextByTime;
      }
      return scriptLine;
    }

    public ScriptLine GetNearestLine(string character, MidiTime time)
    {
      ScriptLine scriptLine1 = m_list;
      while (scriptLine1 != null && scriptLine1.Character != character)
        scriptLine1 = scriptLine1.NextByTime;
      ScriptLine scriptLine2 = scriptLine1;
      for (ScriptLine nextByCharacter = scriptLine1.NextByCharacter; nextByCharacter != null && scriptLine2.Offset < time; nextByCharacter = nextByCharacter.NextByCharacter)
      {
        time.Difference(nextByCharacter.Offset);
        time.Difference(scriptLine2.Offset);
        if (Math.Abs(time.Difference(nextByCharacter.Offset)) < Math.Abs(time.Difference(scriptLine2.Offset)))
          scriptLine2 = nextByCharacter;
      }
      return scriptLine2;
    }

    public ScriptLine GetNearestLineAfter(MidiTime time)
    {
      ScriptLine scriptLine = m_list;
      while (scriptLine != null && scriptLine.Offset < time)
        scriptLine = scriptLine.NextByTime;
      return scriptLine;
    }

    public ScriptLine GetNearestLineAfter(string character, MidiTime time)
    {
      ScriptLine scriptLine = m_list;
      while (scriptLine != null && scriptLine.Character != character)
        scriptLine = scriptLine.NextByTime;
      while (scriptLine != null && scriptLine.Offset < time)
        scriptLine = scriptLine.NextByCharacter;
      return scriptLine;
    }

    public void LineCount(out int LinesDone, out int LinesLeft)
    {
      LinesDone = 0;
      LinesLeft = 0;
      for (ScriptLine scriptLine = GetFirstLine(); scriptLine != null; scriptLine = scriptLine.NextByTime)
      {
        if (scriptLine.IsDone)
          ++LinesDone;
        else
          ++LinesLeft;
      }
    }

    public void LineCount(string charname, out int LinesDone, out int LinesLeft)
    {
      LinesDone = 0;
      LinesLeft = 0;
      for (ScriptLine scriptLine = GetFirstLine(charname); scriptLine != null; scriptLine = scriptLine.NextByCharacter)
      {
        if (scriptLine.IsDone)
          ++LinesDone;
        else
          ++LinesLeft;
      }
    }
  }
}
