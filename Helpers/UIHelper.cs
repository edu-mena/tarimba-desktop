using System.Drawing.Drawing2D;
using TarimbaPresence.Models;

namespace TarimbaPresence.Helpers;

public static class UIHelper
{
    // ── DataGridView ───────────────────────────────────────────────────────
    public static void StyleDataGridView(DataGridView dgv)
    {
        dgv.BackgroundColor                      = Color.White;
        dgv.BorderStyle                          = BorderStyle.None;
        dgv.CellBorderStyle                      = DataGridViewCellBorderStyle.SingleHorizontal;
        dgv.GridColor                            = ThemeHelper.BorderColor;
        dgv.SelectionMode                        = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect                          = false;
        dgv.ReadOnly                             = true;
        dgv.AllowUserToAddRows                   = false;
        dgv.AllowUserToDeleteRows                = false;
        dgv.AllowUserToResizeRows                = false;
        dgv.RowHeadersVisible                    = false;
        dgv.AutoSizeColumnsMode                  = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.EnableHeadersVisualStyles            = false;
        dgv.ScrollBars                           = ScrollBars.Vertical;
        dgv.ColumnHeadersHeightSizeMode          = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgv.RowTemplate.Height                   = 52;
        dgv.ColumnHeadersHeight                  = 48;
        dgv.Cursor                               = Cursors.Hand;

        // Default cells
        dgv.DefaultCellStyle.BackColor           = Color.White;
        dgv.DefaultCellStyle.ForeColor           = ThemeHelper.DarkText;
        dgv.DefaultCellStyle.Font                = ThemeHelper.FontBody;
        dgv.DefaultCellStyle.Padding             = new Padding(10, 0, 10, 0);
        dgv.DefaultCellStyle.SelectionBackColor  = Color.FromArgb(219, 234, 254);
        dgv.DefaultCellStyle.SelectionForeColor  = ThemeHelper.DarkText;

        // Alternating rows
        dgv.AlternatingRowsDefaultCellStyle.BackColor          = ThemeHelper.ContentBg;
        dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(214, 230, 253);

        // Column headers
        var hStyle = dgv.ColumnHeadersDefaultCellStyle;
        hStyle.BackColor  = ThemeHelper.LightGray;
        hStyle.ForeColor  = ThemeHelper.MediumText;
        hStyle.Font       = new Font("Segoe UI", 10f, FontStyle.Bold);
        hStyle.Padding    = new Padding(10, 0, 10, 0);
        hStyle.SelectionBackColor = ThemeHelper.LightGray;
        hStyle.SelectionForeColor = ThemeHelper.MediumText;
    }

    // ── Labels ─────────────────────────────────────────────────────────────
    public static Label MakeLabel(string text, Font font, Color color, ContentAlignment align = ContentAlignment.MiddleLeft)
        => new()
        {
            Text      = text,
            Font      = font,
            ForeColor = color,
            TextAlign = align,
            AutoSize  = true,
            BackColor = Color.Transparent
        };

    public static Label MakeSectionTitle(string text)
        => new()
        {
            Text      = text,
            Font      = ThemeHelper.FontHeading,
            ForeColor = ThemeHelper.DarkText,
            AutoSize  = true,
            BackColor = Color.Transparent
        };

    // ── TextBox moderno ───────────────────────────────────────────────────
    public static TextBox MakeTextBox(int width = 240)
        => new()
        {
            Font        = ThemeHelper.FontBody,
            ForeColor   = ThemeHelper.DarkText,
            BackColor   = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Height      = 38,
            Width       = width
        };

    // ── ComboBox moderno ──────────────────────────────────────────────────
    public static ComboBox MakeComboBox(int width = 200)
        => new()
        {
            Font        = ThemeHelper.FontBody,
            ForeColor   = ThemeHelper.DarkText,
            BackColor   = Color.White,
            FlatStyle   = FlatStyle.Flat,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width       = width,
            Height      = 38
        };

    // ── Botão primário ────────────────────────────────────────────────────
    public static Button MakePrimaryButton(string text, int width = 160, int height = 40)
    {
        var btn = new Button
        {
            Text      = text,
            Font      = ThemeHelper.FontBodyBold,
            ForeColor = Color.White,
            BackColor = ThemeHelper.LightBlue,
            FlatStyle = FlatStyle.Flat,
            Width     = width,
            Height    = height,
            Cursor    = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleCenter
        };
        btn.FlatAppearance.BorderSize          = 0;
        btn.FlatAppearance.MouseOverBackColor  = Color.FromArgb(37, 99, 235);
        btn.FlatAppearance.MouseDownBackColor  = Color.FromArgb(29, 78, 216);
        return btn;
    }

    // ── Botão secundário ──────────────────────────────────────────────────
    public static Button MakeSecondaryButton(string text, int width = 160, int height = 40)
    {
        var btn = new Button
        {
            Text      = text,
            Font      = ThemeHelper.FontBody,
            ForeColor = ThemeHelper.DarkText,
            BackColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Width     = width,
            Height    = height,
            Cursor    = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleCenter
        };
        btn.FlatAppearance.BorderSize         = 1;
        btn.FlatAppearance.BorderColor        = ThemeHelper.BorderColor;
        btn.FlatAppearance.MouseOverBackColor = ThemeHelper.LightGray;
        return btn;
    }

    // ── Separador horizontal ──────────────────────────────────────────────
    public static Panel MakeSeparator(int leftMargin = 0, int topMargin = 8)
        => new()
        {
            Height    = 1,
            Dock      = DockStyle.Top,
            BackColor = ThemeHelper.BorderColor,
            Margin    = new Padding(leftMargin, topMargin, 0, topMargin)
        };

    // ── Badge de status ───────────────────────────────────────────────────
    public static void DrawStatusBadge(Graphics g, Rectangle cellBounds, StatusPresenca status)
    {
        string text;
        Color bg, fg;

        switch (status)
        {
            case StatusPresenca.Presente:
                text = "  Presente  ";
                bg   = ThemeHelper.SuccessBg;
                fg   = ThemeHelper.SuccessText;
                break;
            case StatusPresenca.Falta:
                text = "    Falta    ";
                bg   = ThemeHelper.DangerBg;
                fg   = ThemeHelper.DangerText;
                break;
            default:
                text = " Justificada ";
                bg   = ThemeHelper.WarningBg;
                fg   = ThemeHelper.WarningText;
                break;
        }

        g.SmoothingMode = SmoothingMode.AntiAlias;

        var sz     = g.MeasureString(text, ThemeHelper.FontBadge);
        int badgeW = (int)sz.Width + 8;
        int badgeH = 22;
        int bx     = cellBounds.X + (cellBounds.Width  - badgeW) / 2;
        int by     = cellBounds.Y + (cellBounds.Height - badgeH) / 2;

        var rect = new Rectangle(bx, by, badgeW, badgeH);
        using var path = RoundedRect(rect, 11);
        using var brush = new SolidBrush(bg);
        g.FillPath(brush, path);

        using var sf = new StringFormat
        {
            Alignment     = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        using var textBrush = new SolidBrush(fg);
        g.DrawString(text, ThemeHelper.FontBadge, textBrush, rect, sf);
    }

    // ── Rounded GraphicsPath ──────────────────────────────────────────────
    public static GraphicsPath RoundedRect(Rectangle r, int radius)
    {
        var path     = new GraphicsPath();
        int diameter = radius * 2;
        path.AddArc(r.X,              r.Y,               diameter, diameter,  180, 90);
        path.AddArc(r.Right - diameter, r.Y,             diameter, diameter,  270, 90);
        path.AddArc(r.Right - diameter, r.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(r.X,              r.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    // ── Draw card shadow (simulated, multi-layer) ─────────────────────────
    public static void DrawCardShadow(Graphics g, Rectangle bounds, int radius, int depth = 4)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        for (int i = depth; i >= 1; i--)
        {
            var r    = new Rectangle(bounds.X + i, bounds.Y + i, bounds.Width - i, bounds.Height - i);
            int alpha = (int)(15.0 / i);
            using var path  = RoundedRect(r, radius + 1);
            using var brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
            g.FillPath(brush, path);
        }
    }
}
