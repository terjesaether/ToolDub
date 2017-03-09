// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.MidiTime
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using System;

namespace Nordubb.DubTool
{
  public struct MidiTime : IComparable<MidiTime>
  {
    private static readonly char[] Separator = new char[1]
    {
      '.'
    };
    private static readonly double mspf_24 = 125.0 / 3.0;
    private static readonly double mspf_25 = 40.0;
    private static readonly double mspf_2997 = 33.3667000333667;
    private static readonly double mspf_30 = 100.0 / 3.0;
    public static readonly MidiTime Zero = new MidiTime();
    public int Hour;
    public int Minute;
    public int Second;
    public int Frame;
    public int Subframe;

    public static bool operator <(MidiTime mt1, MidiTime mt2)
    {
      return mt1.CompareTo(mt2) < 0;
    }

    public static bool operator >(MidiTime mt1, MidiTime mt2)
    {
      return mt1.CompareTo(mt2) > 0;
    }

    public static bool operator ==(MidiTime mt1, MidiTime mt2)
    {
      return mt1.CompareTo(mt2) == 0;
    }

    public static bool operator !=(MidiTime mt1, MidiTime mt2)
    {
      return !(mt1 == mt2);
    }

    public static bool operator >=(MidiTime mt1, MidiTime mt2)
    {
      return mt1.CompareTo(mt2) >= 0;
    }

    public static bool operator <=(MidiTime mt1, MidiTime mt2)
    {
      return mt1.CompareTo(mt2) <= 0;
    }

    public static MidiTime FromDateTime(DateTime dt, int fps)
    {
      MidiTime midiTime = new MidiTime();
      midiTime.Hour = dt.Hour;
      midiTime.Minute = dt.Minute;
      midiTime.Second = dt.Second;
      switch (fps)
      {
        case 64:
          midiTime.Frame = (int) (dt.Millisecond / mspf_30);
          break;
        case 96:
          midiTime.Frame = (int) (dt.Millisecond / mspf_2997);
          break;
        case 0:
          midiTime.Frame = (int) (dt.Millisecond / mspf_24);
          break;
        case 32:
          midiTime.Frame = (int) (dt.Millisecond / mspf_25);
          break;
      }
      return midiTime;
    }

    public static MidiTime Parse(string s)
    {
      MidiTime midiTime = new MidiTime();
      string[] strArray = s.Split(Separator);
      midiTime.Hour = int.Parse(strArray[0]);
      midiTime.Minute = int.Parse(strArray[1]);
      midiTime.Second = int.Parse(strArray[2]);
      if (strArray.Length > 3)
        midiTime.Frame = int.Parse(strArray[3]);
      if (strArray.Length > 4)
        midiTime.Subframe = int.Parse(strArray[4]);
      return midiTime;
    }

    public static int SecondsAway(MidiTime a, MidiTime b)
    {
      int num = ((a.Hour - b.Hour) * 60 + (a.Minute - b.Minute)) * 60 + (a.Second - b.Second);
      if (a.Frame - b.Frame > 0 || a.Subframe - b.Subframe > 0)
        ++num;
      return num;
    }

    public int Difference(MidiTime other)
    {
      return ((Hour - other.Hour) * 3600 + (Minute - other.Minute) * 60 + (Second - other.Second)) * 100 + (Frame - other.Frame);
    }

    public override bool Equals(object obj)
    {
      if (obj == null || !(obj is MidiTime))
        return false;
      return this == (MidiTime) obj;
    }

    public override int GetHashCode()
    {
      return ((((0 + Hour) * 60 + Minute) * 60 + Second) * 60 + Frame) * 60 + Subframe;
    }

    public MidiTime SubtractSeconds(int seconds)
    {
      MidiTime midiTime = new MidiTime();
      midiTime.Subframe = Subframe;
      midiTime.Frame = Frame;
      if ((midiTime.Second = Second - seconds) < 0)
      {
        midiTime.Second += 60;
        if ((midiTime.Minute = Minute - 1) < 0)
        {
          midiTime.Minute += 60;
          midiTime.Hour = Math.Min(Hour - 1, 0);
        }
        else
          midiTime.Hour = Hour;
      }
      else
      {
        midiTime.Minute = Minute;
        midiTime.Hour = Hour;
      }
      return midiTime;
    }

    public override string ToString()
    {
      return string.Format("{0:00}{5}{1:00}{5}{2:00}{5}{3:00}{5}{4:00}", (object) Hour, (object) Minute, (object) Second, (object) Frame, (object) Subframe, (object) Separator[0]);
    }

    public int CompareTo(MidiTime other)
    {
      int num1;
      if ((num1 = Hour - other.Hour) != 0)
        return num1;
      int num2;
      if ((num2 = Minute - other.Minute) != 0)
        return num2;
      int num3;
      if ((num3 = Second - other.Second) != 0)
        return num3;
      int num4;
      if ((num4 = Frame - other.Frame) != 0)
        return num4;
      return Subframe - other.Subframe;
    }
  }
}
