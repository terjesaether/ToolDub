// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.ScriptLine
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

namespace Nordubb.DubTool
{
  public class ScriptLine
  {
    public string Character;
    public MidiTime Offset;
    public string Line;
    public bool IsDone;
    public ScriptLine NextByTime;
    public ScriptLine PrevByTime;
    public ScriptLine NextByCharacter;
    public ScriptLine PrevByCharacter;

    public void Delete()
    {
      PrevByTime.NextByTime = NextByTime;
      NextByTime.PrevByTime = PrevByTime;
      PrevByTime = null;
      NextByTime = null;
    }
  }
}
