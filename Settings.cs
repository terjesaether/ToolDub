// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.Settings
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using System;
using Microsoft.Win32;

namespace Nordubb.DubTool
{
  internal static class Settings
  {
    private static readonly int DefaultFps = 32;
    private static readonly int DefaultLeadSeconds = 3;
    private static readonly string DefaultScriptFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    private static readonly string FpsKey = "Fps";
    private static readonly string InputDeviceKey = "InputDevice";
    private static readonly string LeadSecondsKey = "LeadSeconds";
    private static readonly string OutputDeviceKey = "OutputDevice";
    private static readonly string RegistryPath = "HKEY_CURRENT_USER\\Software\\Nordubb\\DubTool";
    private static readonly string ScriptFolderKey = "ScriptFolder";

    public static int Fps
    {
      get
      {
        return GetSetting(FpsKey, DefaultFps);
      }
      set
      {
        SetSetting(FpsKey, value);
      }
    }

    public static int LeadSeconds
    {
      get
      {
        return GetSetting(LeadSecondsKey, DefaultLeadSeconds);
      }
      set
      {
        SetSetting(LeadSecondsKey, value);
      }
    }

    public static int MidiIn
    {
      get
      {
        return GetSetting(InputDeviceKey, 0);
      }
      set
      {
        SetSetting(InputDeviceKey, value);
      }
    }

    public static int MidiOut
    {
      get
      {
        return GetSetting(OutputDeviceKey, 0);
      }
      set
      {
        SetSetting(OutputDeviceKey, value);
      }
    }

    public static string ScriptFolder
    {
      get
      {
        return GetSetting(ScriptFolderKey, DefaultScriptFolder);
      }
      set
      {
        SetSetting(ScriptFolderKey, value);
      }
    }

    private static void SetSetting<T>(string key, T value)
    {
      Registry.SetValue(RegistryPath, key, value);
    }

    private static T GetSetting<T>(string key, T defval)
    {
      object obj = Registry.GetValue(RegistryPath, key, defval);
      if (obj == null)
        return defval;
      return (T) obj;
    }
  }
}
