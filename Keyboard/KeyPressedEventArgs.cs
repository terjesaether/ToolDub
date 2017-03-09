// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.Keyboard.KeyPressedEventArgs
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using System;
using System.Windows.Forms;

namespace Nordubb.DubTool.Keyboard
{
  internal class KeyPressedEventArgs : EventArgs
  {
    private ModifierKeys _modifier;
    private Keys _key;

    public ModifierKeys Modifier
    {
      get
      {
        return _modifier;
      }
    }

    public Keys Key
    {
      get
      {
        return _key;
      }
    }

    internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
    {
      _modifier = modifier;
      _key = key;
    }
  }
}
