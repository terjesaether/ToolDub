// Decompiled with JetBrains decompiler
// Type: Nordubb.DubTool.CountdownTimer
// Assembly: DubTool, Version=2.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: D99BE9DF-C71C-4275-A311-38ECE5DD2F4E
// Assembly location: C:\Users\terje\Documents\Visual Studio 2015\Projects\Nordubb\DubToolApp\DubTool.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Nordubb.DubTool
{
  [DesignerCategory("")]
  internal class CountdownTimer : Control
  {
    private double m_progress;

    public CountdownTimer()
    {
      InitializeControl();
    }

    public void SetTimer(double progress)
    {
      if (m_progress == progress)
        return;
      m_progress = progress;
      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      if (e.ClipRectangle.IsEmpty)
        return;
      DrawTimer(e.Graphics);
    }

    private static void AddTopRightCorner(GraphicsPath p, int radius, int cx, int cy)
    {
      Point point = new Point(cx + radius, cy - radius);
      p.AddLine(p.GetLastPoint(), point);
    }

    private static void AddBottomRightCorner(GraphicsPath p, int radius, int cx, int cy)
    {
      Point point = new Point(cx + radius, cy + radius);
      p.AddLine(p.GetLastPoint(), point);
    }

    private static void AddBottomLeftCorner(GraphicsPath p, int radius, int cx, int cy)
    {
      Point point = new Point(cx - radius, cy + radius);
      p.AddLine(p.GetLastPoint(), point);
    }

    private Region CalculateFirstQuarterRegion(double completeFactor, int radius, int cx, int cy)
    {
      GraphicsPath startingPath = GetStartingPath(cx, cy, radius);
      Point circlePoint = GetCirclePoint(completeFactor, cx, cy, radius);
      Point pt1 = new Point(circlePoint.X, cy - radius);
      startingPath.AddLine(startingPath.GetLastPoint(), pt1);
      startingPath.AddLine(pt1, circlePoint);
      startingPath.CloseFigure();
      return new Region(startingPath);
    }

    private Region CalculateSecondQuarterRegion(double completeFactor, int radius, int cx, int cy)
    {
      Point circlePoint = GetCirclePoint(completeFactor, cx, cy, radius);
      Point pt1 = new Point(cx + radius, circlePoint.Y);
      GraphicsPath startingPath = GetStartingPath(cx, cy, radius);
      AddTopRightCorner(startingPath, radius, cx, cy);
      startingPath.AddLine(startingPath.GetLastPoint(), pt1);
      startingPath.AddLine(pt1, circlePoint);
      startingPath.CloseFigure();
      return new Region(startingPath);
    }

    private Region CalculateThirdQuarterRegion(double completefactor, int radius, int cx, int cy)
    {
      Point circlePoint = GetCirclePoint(completefactor, cx, cy, radius);
      Point pt1 = new Point(circlePoint.X, cy + radius);
      GraphicsPath startingPath = GetStartingPath(cx, cy, radius);
      AddTopRightCorner(startingPath, radius, cx, cy);
      AddBottomRightCorner(startingPath, radius, cx, cy);
      startingPath.AddLine(startingPath.GetLastPoint(), pt1);
      startingPath.AddLine(pt1, circlePoint);
      startingPath.CloseFigure();
      return new Region(startingPath);
    }

    private Region CalculateFourthQuarterRegion(double completeFactor, int radius, int cx, int cy)
    {
      Point circlePoint = GetCirclePoint(completeFactor, cx, cy, radius);
      Point pt1 = new Point(cx - radius, circlePoint.Y);
      GraphicsPath startingPath = GetStartingPath(cx, cy, radius);
      AddTopRightCorner(startingPath, radius, cx, cy);
      AddBottomRightCorner(startingPath, radius, cx, cy);
      AddBottomLeftCorner(startingPath, radius, cx, cy);
      startingPath.AddLine(startingPath.GetLastPoint(), pt1);
      startingPath.AddLine(pt1, circlePoint);
      startingPath.CloseFigure();
      return new Region(startingPath);
    }

    private void DrawCircle(Graphics g, Brush b, int r)
    {
      int num = 2 * r;
      Point center = FindCenter();
      Rectangle rect = new Rectangle(new Point(center.X - r, center.Y - r), new Size(num, num));
      g.FillEllipse(b, rect);
    }

    private void DrawTimer(Graphics g)
    {
      SolidBrush solidBrush1 = new SolidBrush(Color.Gray);
      int r = Width / 2;
      DrawCircle(g, solidBrush1, r);
      int num = (int) (r * 0.95);
      SolidBrush solidBrush2 = new SolidBrush(Color.Green);
      DrawCircle(g, solidBrush2, num);
      g.Clip = FindClippingRegion(m_progress, num);
      SolidBrush solidBrush3 = new SolidBrush(Color.Black);
      DrawCircle(g, solidBrush3, num);
    }

    private Point FindCenter()
    {
      return new Point(Width / 2, Height / 2);
    }

    private Region FindClippingRegion(double completeFactor, int radius)
    {
      int cx = Width / 2;
      int cy = Height / 2;
      if (completeFactor == 0.0)
        return new Region(Rectangle.Empty);
      if (completeFactor == 1.0)
        return new Region(new Rectangle(0, 0, Width, Height));
      if (completeFactor <= 0.25)
        return CalculateFirstQuarterRegion(completeFactor, radius, cx, cy);
      if (completeFactor <= 0.5)
        return CalculateSecondQuarterRegion(completeFactor, radius, cx, cy);
      if (completeFactor <= 0.75)
        return CalculateThirdQuarterRegion(completeFactor, radius, cx, cy);
      return CalculateFourthQuarterRegion(completeFactor, radius, cx, cy);
    }

    private GraphicsPath GetStartingPath(int cx, int cy, int r)
    {
      Point pt1 = new Point(cx, cy);
      Point pt2 = new Point(cx, cy - r);
      GraphicsPath graphicsPath = new GraphicsPath();
      graphicsPath.AddLine(pt1, pt2);
      return graphicsPath;
    }

    private Point GetCirclePoint(double completeFactor, int cx, int cy, int r)
    {
      double num1 = -completeFactor * 2.0 * Math.PI + Math.PI / 2.0;
      double num2 = r;
      double num3 = Math.Cos(num1) * num2;
      double num4 = Math.Sin(num1) * num2;
      return new Point((int) (cx + num3), (int) (cy - num4));
    }

    private void InitializeControl()
    {
      SuspendLayout();
      ResumeLayout(false);
      PerformLayout();
    }
  }
}
