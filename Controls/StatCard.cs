using System.Drawing.Drawing2D;
using FontAwesome.Sharp;
using TarimbaPresence.Helpers;

namespace TarimbaPresence.Controls;

/// <summary>
/// Card de estatística para o Dashboard — usa controles filhos para ícone, valor e rótulo.
/// </summary>
public class StatCard : Panel
{
    public StatCard(IconChar icon, string title, string value, string subtitle,
                    Color accentColor, Color accentBg)
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw          |
                 ControlStyles.SupportsTransparentBackColor, true);

        Width     = 240;
        Height    = 120;
        BackColor = Color.White;
        Margin    = new Padding(0, 0, 16, 0);

        // ── Icon picture box ───────────────────────────────────────────────
        var pnlIcon = new Panel
        {
            Width     = 48,
            Height    = 48,
            BackColor = accentBg,
            Top       = 36,
            Left      = 170  // positioned right — will adjust on resize
        };
        pnlIcon.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var p = UIHelper.RoundedRect(new Rectangle(0, 0, 47, 47), 24);
            using var b = new SolidBrush(accentBg);
            g.FillPath(b, p);
        };

        var iconBox = new IconPictureBox
        {
            IconChar  = icon,
            IconColor = accentColor,
            IconSize  = 22,
            BackColor = accentBg,
            Width     = 48,
            Height    = 48
        };
        pnlIcon.Controls.Add(iconBox);

        // ── Text labels ────────────────────────────────────────────────────
        var lblTitle = new Label
        {
            Text      = title,
            Font      = ThemeHelper.FontCardLabel,
            ForeColor = ThemeHelper.MediumText,
            AutoSize  = true,
            Left      = 16,
            Top       = 16,
            BackColor = Color.Transparent
        };

        var lblValue = new Label
        {
            Text      = value,
            Font      = ThemeHelper.FontCardValue,
            ForeColor = ThemeHelper.DarkText,
            AutoSize  = true,
            Left      = 14,
            Top       = 36,
            BackColor = Color.Transparent
        };

        var lblSub = new Label
        {
            Text      = subtitle,
            Font      = ThemeHelper.FontCaption,
            ForeColor = ThemeHelper.SubtleText,
            AutoSize  = true,
            Left      = 16,
            Top       = 90,
            BackColor = Color.Transparent
        };

        Controls.AddRange(new Control[] { pnlIcon, lblTitle, lblValue, lblSub });

        // Reposition icon circle when width changes
        SizeChanged += (_, _) =>
        {
            pnlIcon.Left = Width - pnlIcon.Width - 16;
        };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Shadow
        for (int i = 4; i >= 1; i--)
        {
            var sr    = new Rectangle(i, i, Width - i - 2, Height - i - 2);
            int alpha = Math.Max(1, 12 / i);
            using var sp = UIHelper.RoundedRect(sr, 13);
            using var sb = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
            g.FillPath(sb, sp);
        }

        var cardRect = new Rectangle(0, 0, Width - 5, Height - 5);
        using var bgPath  = UIHelper.RoundedRect(cardRect, 12);
        using var bgBrush = new SolidBrush(Color.White);
        g.FillPath(bgBrush, bgPath);

        using var borderPen = new Pen(ThemeHelper.BorderColor, 1f);
        g.DrawPath(borderPen, bgPath);
    }
}
