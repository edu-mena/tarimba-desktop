using System.ComponentModel;
using System.Drawing.Drawing2D;
using TarimbaPresence.Helpers;

namespace TarimbaPresence.Controls;

public class RoundedPanel : Panel
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CornerRadius { get; set; } = 12;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowShadow { get; set; } = true;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ShadowDepth { get; set; } = 4;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowLeftAccent { get; set; } = false;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color AccentColor { get; set; } = ThemeHelper.LightBlue;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int AccentWidth { get; set; } = 4;

    public RoundedPanel()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw          |
                 ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.White;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Shadow
        if (ShowShadow)
        {
            for (int i = ShadowDepth; i >= 1; i--)
            {
                var sr    = new Rectangle(i, i, Width - i - 1, Height - i - 1);
                int alpha = Math.Max(1, 14 / i);
                using var sp = UIHelper.RoundedRect(sr, CornerRadius + 1);
                using var sb = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
                g.FillPath(sb, sp);
            }
        }

        // Background
        var bg = new Rectangle(0, 0, Width - ShadowDepth - 1, Height - ShadowDepth - 1);
        using var bgPath  = UIHelper.RoundedRect(bg, CornerRadius);
        using var bgBrush = new SolidBrush(BackColor);
        g.FillPath(bgBrush, bgPath);

        // Left accent bar
        if (ShowLeftAccent)
        {
            var accentRect = new Rectangle(0, CornerRadius / 2, AccentWidth, Height - ShadowDepth - CornerRadius);
            using var ab = new SolidBrush(AccentColor);
            g.FillRectangle(ab, accentRect);
        }

        // Subtle border
        using var borderPath = UIHelper.RoundedRect(bg, CornerRadius);
        using var borderPen  = new Pen(ThemeHelper.BorderColor, 0.8f);
        g.DrawPath(borderPen, borderPath);
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        var bg = new Rectangle(0, 0, Width - ShadowDepth, Height - ShadowDepth);
        using var path = UIHelper.RoundedRect(bg, CornerRadius);
        Region = new Region(path);
    }
}
