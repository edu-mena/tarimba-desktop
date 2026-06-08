using System.Drawing.Drawing2D;
using FontAwesome.Sharp;
using TarimbaPresence.Controls;
using TarimbaPresence.Database;
using TarimbaPresence.Helpers;

namespace TarimbaPresence.UserControls;

public class UC_Dashboard : UserControl
{
    private readonly DatabaseService _db = new();
    public UC_Dashboard()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor  = ThemeHelper.ContentBg;
        AutoScroll = true;
        Padding    = new Padding(32, 28, 32, 28);
        Build();
    }

    private void Build()
    {
        SuspendLayout();

        // ── Title row ──────────────────────────────────────────────────────
        var pnlTitle = new Panel
        {
            Height    = 56,
            Dock      = DockStyle.Top,
            BackColor = Color.Transparent
        };
        var lblTitle = new Label
        {
            Text      = "Visão Geral",
            Font      = ThemeHelper.FontTitle,
            ForeColor = ThemeHelper.DarkText,
            AutoSize  = true,
            Left      = 0,
            Top       = 4,
            BackColor = Color.Transparent
        };
        var lblSub = new Label
        {
            Text      = $"Dados do dia {DateTime.Today:dd/MM/yyyy}  —  Complexo Escolar Tarimba III",
            Font      = ThemeHelper.FontSmall,
            ForeColor = ThemeHelper.MediumText,
            AutoSize  = true,
            Left      = 2,
            Top       = 34,
            BackColor = Color.Transparent
        };
        pnlTitle.Controls.AddRange(new Control[] { lblTitle, lblSub });

        // ── Stat cards ─────────────────────────────────────────────────────
        var pnlCards = new Panel
        {
            Height    = 144,
            Dock      = DockStyle.Top,
            BackColor = Color.Transparent,
            Margin    = new Padding(0, 0, 0, 24)
        };

        var cards = new (IconChar Icon, string Title, string Value, string Sub, Color Accent, Color AccentBg)[]
        {
            (IconChar.UserGraduate, "Total de Alunos",   _db.ContarAlunos().ToString(),
             "matriculados activos",          ThemeHelper.LightBlue,  Color.FromArgb(219,234,254)),
            (IconChar.CircleCheck,  "Presenças Hoje",    _db.ContarPresencasHoje().ToString(),
             "alunos presentes",              ThemeHelper.Success,    ThemeHelper.SuccessBg),
            (IconChar.TriangleExclamation, "Faltas Críticas", _db.ContarFaltasCriticas().ToString(),
             "acima de 25% de faltas",        ThemeHelper.Danger,     ThemeHelper.DangerBg),
            (IconChar.LayerGroup,   "Turmas Activas",    _db.ContarTurmas().ToString(),
             "turmas no sistema",             ThemeHelper.Purple,     ThemeHelper.PurpleBg),
        };

        int cardX = 0;
        foreach (var c in cards)
        {
            var card = new StatCard(c.Icon, c.Title, c.Value, c.Sub, c.Accent, c.AccentBg)
            {
                Left = cardX,
                Top  = 10,
                Width  = 240,
                Height = 120
            };
            pnlCards.Controls.Add(card);
            cardX += 256;
        }

        // Resize cards evenly on width change
        pnlCards.SizeChanged += (_, _) =>
        {
            int available = pnlCards.Width - 3 * 16;
            int cardW     = Math.Max(200, available / 4);
            int x         = 0;
            foreach (Control cc in pnlCards.Controls)
            {
                if (cc is StatCard sc)
                {
                    sc.Left  = x;
                    sc.Width = cardW;
                    x       += cardW + 16;
                }
            }
        };

        // ── Row 2 title ────────────────────────────────────────────────────
        var pnlRow2Title = new Panel
        {
            Height    = 44,
            Dock      = DockStyle.Top,
            BackColor = Color.Transparent
        };
        var lblRow2 = new Label
        {
            Text      = "Frequência por Turma",
            Font      = ThemeHelper.FontSubheading,
            ForeColor = ThemeHelper.DarkText,
            AutoSize  = true,
            Top       = 10,
            BackColor = Color.Transparent
        };
        pnlRow2Title.Controls.Add(lblRow2);

        // ── Attendance chart (custom drawn progress bars) ──────────────────
        var pnlChart = BuildAttendanceChart();
        pnlChart.Dock = DockStyle.Top;

        // ── Row 3 title ────────────────────────────────────────────────────
        var pnlRow3Title = new Panel
        {
            Height    = 44,
            Dock      = DockStyle.Top,
            BackColor = Color.Transparent
        };
        var lblRow3 = new Label
        {
            Text      = "Actividade Recente",
            Font      = ThemeHelper.FontSubheading,
            ForeColor = ThemeHelper.DarkText,
            AutoSize  = true,
            Top       = 10,
            BackColor = Color.Transparent
        };
        pnlRow3Title.Controls.Add(lblRow3);

        // ── Recent activity table ─────────────────────────────────────────
        var pnlRecent = BuildRecentTable();
        pnlRecent.Dock = DockStyle.Top;

        // Stack controls (reverse order for DockStyle.Top)
        Controls.AddRange(new Control[]
        {
            pnlRecent,
            pnlRow3Title,
            pnlChart,
            pnlRow2Title,
            pnlCards,
            pnlTitle
        });

        ResumeLayout(true);
    }

    // ── Attendance chart panel ─────────────────────────────────────────────
    private Panel BuildAttendanceChart()
    {
        var pnl = new Panel
        {
            Height    = 280,
            BackColor = Color.Transparent,
            Padding   = new Padding(0, 0, 0, 16)
        };

        var card = new RoundedPanel
        {
            Dock        = DockStyle.Fill,
            BackColor   = Color.White,
            ShowShadow  = true,
            CornerRadius = 12,
            ShadowDepth  = 4,
            Padding      = new Padding(24, 20, 24, 20)
        };

        // Build per-turma rows
        var rowPanel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            AutoScroll = false
        };

        var turmas = _db.ObterTodasTurmas().Take(8).ToList();
        int rowH = 32;

        for (int i = 0; i < turmas.Count; i++)
        {
            var t       = turmas[i];
            var alunos  = _db.ObterAlunosDaTurma(t.Id);
            if (alunos.Count == 0) continue;

            int total     = alunos.Count;
            int presentes = _db.ObterPresencas(t.Id, DateTime.Today, DateTime.Today)
                .Where(p => p.Status == Models.StatusPresenca.Presente)
                .Select(p => p.AlunoId).Distinct().Count();

            double pct = total > 0 ? (double)presentes / total * 100.0 : 0;

            var row = new Panel
            {
                Height    = rowH,
                Top       = i * (rowH + 8),
                Left      = 0,
                BackColor = Color.Transparent
            };

            row.Paint += (sender, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var panel   = (Panel)sender!;
                int barLeft = 130;
                int barW    = panel.Width - barLeft - 80;
                int barH    = 10;
                int barY    = (rowH - barH) / 2;

                // Turma name
                using var nameBrush = new SolidBrush(ThemeHelper.DarkText);
                g.DrawString(t.NomeCurto, ThemeHelper.FontSmall,
                             nameBrush, 0, (rowH - 14) / 2f);

                // Background bar
                var bgRect = new Rectangle(barLeft, barY, barW, barH);
                using var bgPath  = UIHelper.RoundedRect(bgRect, barH / 2);
                using var bgBrush = new SolidBrush(ThemeHelper.BorderColor);
                g.FillPath(bgBrush, bgPath);

                // Fill bar
                int fillW = (int)(barW * pct / 100.0);
                if (fillW > 4)
                {
                    Color fillColor = pct >= 80
                        ? ThemeHelper.Success
                        : pct >= 60
                            ? ThemeHelper.Warning
                            : ThemeHelper.Danger;

                    var fillRect = new Rectangle(barLeft, barY, fillW, barH);
                    using var fillPath  = UIHelper.RoundedRect(fillRect, barH / 2);
                    using var fillBrush = new SolidBrush(fillColor);
                    g.FillPath(fillBrush, fillPath);
                }

                // Percentage text
                using var pctBrush = new SolidBrush(ThemeHelper.MediumText);
                g.DrawString($"{pct:0}%", ThemeHelper.FontSmall, pctBrush,
                             barLeft + barW + 8, (rowH - 14) / 2f);
            };

            rowPanel.Controls.Add(row);
        }

        card.Controls.Add(rowPanel);
        pnl.Controls.Add(card);
        return pnl;
    }

    // ── Recent activity DataGridView ───────────────────────────────────────
    private Panel BuildRecentTable()
    {
        var pnl = new Panel
        {
            Height    = 300,
            BackColor = Color.Transparent,
            Padding   = new Padding(0, 0, 0, 16)
        };

        var card = new RoundedPanel
        {
            Dock         = DockStyle.Fill,
            BackColor    = Color.White,
            ShowShadow   = true,
            CornerRadius = 12,
            ShadowDepth  = 4,
            Padding      = new Padding(0, 0, 0, 0)
        };

        var dgv = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGridView(dgv);

        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Processo", HeaderText = "Nº Processo", Width = 120, FillWeight = 15 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Nome",     HeaderText = "Nome do Aluno", FillWeight = 45 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Turma",    HeaderText = "Turma",          Width = 80,  FillWeight = 12 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Data",     HeaderText = "Data",           Width = 100, FillWeight = 15 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Status",   HeaderText = "Status",         Width = 110, FillWeight = 13 });

        // Last 15 records
        var recent = _db.ObterPresencas()
            .OrderByDescending(p => p.Data)
            .Take(15)
            .ToList();

        foreach (var p in recent)
        {
            var aluno = _db.ObterTodosAlunos().FirstOrDefault(a => a.Id == p.AlunoId);
            if (aluno == null) continue;
            var turma = _db.ObterTurma(p.TurmaId);
            dgv.Rows.Add(aluno.NumeroProcesso, aluno.NomeCompleto,
                         turma?.Nome ?? "—", p.Data.ToString("dd/MM/yyyy"),
                         p.StatusTexto);
        }

        // Colour-code the Status column
        dgv.CellFormatting += (_, e) =>
        {
            if (e.ColumnIndex != 4 || e.Value == null) return;
            var val = e.Value.ToString();
            e.CellStyle.ForeColor = val == "Presente"
                ? ThemeHelper.SuccessText
                : val == "Falta"
                    ? ThemeHelper.DangerText
                    : ThemeHelper.WarningText;
            e.CellStyle.Font = ThemeHelper.FontBodyBold;
        };

        card.Controls.Add(dgv);
        pnl.Controls.Add(card);
        return pnl;
    }
}
