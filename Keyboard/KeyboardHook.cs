// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.Keyboard.KeyboardHook
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Nordubb.DubTool.Keyboard
{
  internal sealed class KeyboardHook : IDisposable
  {
    private Window _window = new Window();
    private int _currentId;

    public event EventHandler<KeyPressedEventArgs> KeyPressed;

    public KeyboardHook()
    {
      _window.KeyPressed += (EventHandler<KeyPressedEventArgs>) ((sender, args) =>
      {
        if (KeyPressed == null)
          return;
        KeyPressed(this, args);
      });
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public void RegisterHotKey(ModifierKeys modifier, Keys key)
    {
      _currentId = _currentId + 1;
      if (!RegisterHotKey(_window.Handle, _currentId, (uint) modifier, (uint) key))
        throw new InvalidOperationException("Couldn’t register the hot key.");
    }

    public void Dispose()
    {
      for (int currentId = _currentId; currentId > 0; --currentId)
        UnregisterHotKey(_window.Handle, currentId);
      _window.Dispose();
    }

    private class Window : NativeWindow, IDisposable
    {
      private static int WM_HOTKEY = 786;

      public event EventHandler<KeyPressedEventArgs> KeyPressed;

      public Window()
      {
        CreateHandle(new CreateParams());
      }

      protected override void WndProc(ref Message m)
      {
        base.WndProc(ref m);
        if (m.Msg != WM_HOTKEY)
          return;
        Keys key = (Keys) ((int) m.LParam >> 16 & ushort.MaxValue);
        ModifierKeys modifier = (ModifierKeys) ((int) m.LParam & ushort.MaxValue);
        if (KeyPressed == null)
          return;
        KeyPressed(this, new KeyPressedEventArgs(modifier, key));
      }

      public void Dispose()
      {
        DestroyHandle();
      }
    }
  }
}
