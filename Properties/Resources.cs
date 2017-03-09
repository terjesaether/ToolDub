// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.Properties.Resources
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Nordubb.DubTool.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ReferenceEquals(resourceMan, null))
          resourceMan = new ResourceManager("Nordubb.DubTool.Properties.Resources", typeof (Resources).Assembly);
        return resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return resourceCulture;
      }
      set
      {
        resourceCulture = value;
      }
    }

    internal static Bitmap openfolderHS
    {
      get
      {
        return (Bitmap) ResourceManager.GetObject("openfolderHS", resourceCulture);
      }
    }
  }
}
