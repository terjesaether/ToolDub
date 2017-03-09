// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.Midi.SysExCommands
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using Sanford.Multimedia.Midi;

namespace Nordubb.DubTool.Midi
{
  internal static class SysExCommands
  {
    private static readonly byte[] FastForwardSysex = new byte[6]
    {
      240,
      127,
      127,
      6,
      4,
      247
    };
    private static readonly byte[] PauseSysEx = new byte[6]
    {
      240,
      127,
      127,
      6,
      9,
      247
    };
    private static readonly byte[] PlaySysEx = new byte[6]
    {
      240,
      127,
      127,
      6,
      2,
      247
    };
    private static readonly byte[] DeferredPlaySysEx = new byte[6]
    {
      240,
      127,
      127,
      6,
      2,
      247
    };
    private static readonly byte[] RecordStrobeSysEx = new byte[6]
    {
      240,
      127,
      127,
      6,
      6,
      247
    };
    private static readonly byte[] RewindSysEx = new byte[6]
    {
      240,
      127,
      127,
      6,
      5,
      247
    };
    private static readonly byte[] StopSysEx = new byte[6]
    {
      240,
      127,
      127,
      6,
      1,
      247
    };
    private static readonly byte[] GotoFirstPart = new byte[7]
    {
      240,
      127,
      127,
      6,
      68,
      6,
      1
    };
    private static readonly byte[] MtcGotoFirstPart = new byte[5]
    {
      240,
      127,
      127,
      1,
      1
    };
    public static readonly SysExMessage FastForward = new SysExMessage(FastForwardSysex);
    public static readonly SysExMessage Pause = new SysExMessage(PauseSysEx);
    public static readonly SysExMessage Play = new SysExMessage(PlaySysEx);
    public static readonly SysExMessage RecordStrobe = new SysExMessage(RecordStrobeSysEx);
    public static readonly SysExMessage Rewind = new SysExMessage(RewindSysEx);
    public static readonly SysExMessage Stop = new SysExMessage(StopSysEx);

    private static byte[] GotoSysEx(byte hours, byte minutes, byte seconds, byte frames, byte subframes)
    {
      return new byte[13]
      {
        240,
        127,
        127,
        6,
        68,
        6,
        1,
        hours,
        minutes,
        seconds,
        frames,
        subframes,
        247
      };
    }

    internal static CommandType CheckType(byte[] msg)
    {
      if (ArraysMatch(msg, FastForwardSysex))
        return CommandType.FastForward;
      if (ArraysMatch(msg, PauseSysEx))
        return CommandType.Pause;
      if (ArraysMatch(msg, PlaySysEx))
        return CommandType.Play;
      if (ArraysMatch(msg, DeferredPlaySysEx))
        return CommandType.DeferredPlay;
      if (ArraysMatch(msg, RecordStrobeSysEx))
        return CommandType.RecordStrobe;
      if (ArraysMatch(msg, RewindSysEx))
        return CommandType.Rewind;
      if (ArraysMatch(msg, StopSysEx))
        return CommandType.Stop;
      if (ArraysMatch(msg, GotoFirstPart, GotoFirstPart.Length))
        return CommandType.Goto;
      return ArraysMatch(msg, MtcGotoFirstPart, MtcGotoFirstPart.Length) ? CommandType.MtcGoto : CommandType.Unknown;
    }

    private static bool ArraysMatch(byte[] a1, byte[] a2, int length)
    {
      if (a1.Length < length || a2.Length < length)
        return false;
      for (int index = 0; index < length; ++index)
      {
        if (a1[index] != a2[index])
          return false;
      }
      return true;
    }

    private static bool ArraysMatch(byte[] a1, byte[] a2)
    {
      return ArraysMatch(a1, a2, a1.Length);
    }

    public static SysExMessage Goto(int fpshours, int minutes, int seconds, int frames, int subframes)
    {
      return new SysExMessage(GotoSysEx((byte) fpshours, (byte) minutes, (byte) seconds, (byte) frames, (byte) subframes));
    }

    public enum CommandType
    {
      Unknown,
      FastForward,
      Pause,
      Play,
      RecordStrobe,
      Rewind,
      Stop,
      Goto,
      MtcGoto,
      DeferredPlay
    }
  }
}
