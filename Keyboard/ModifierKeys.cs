// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.Keyboard.ModifierKeys
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using System;

namespace Nordubb.DubTool.Keyboard
{
  [Flags]
  internal enum ModifierKeys : uint
  {
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Win = 8
  }
}
