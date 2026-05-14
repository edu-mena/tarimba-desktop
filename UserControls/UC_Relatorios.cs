using System.Drawing.Drawing2D;
using FontAwesome.Sharp;
using TarimbaPresence.Controls;
using TarimbaPresence.Data;
using TarimbaPresence.Helpers;
using TarimbaPresence.Models;

namespace TarimbaPresence.UserControls;

public class UC_Relatorios : UserControl
{
    private ComboBox    cmbTurmaFiltro    = null!;
    private ComboBox    cmbDisciplinaFiltro = null!;
    private ComboBox    cmbPeriodo        = null!;
    private DataGridView dgv              = null!;

    public UC_Relatorios()
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

        // ── Header ─────────────────────────────────────────────────────────
        var pnlHeader = new Panel
        {
            Height = 56, Dock = DockStyle.Top, BackColor = Color.Transparent
        };
        pnlHeader.Controls.Add(new Label
        {
            Text = "Relatórios de Presença", Font = ThemeHelper.FontTitle,
            ForeColor = ThemeHelper.DarkText, AutoSize = true, Left = 0, Top = 4,
            BackColor = Color.Transparent
        });
        pnlHeader.Controls.Add(new Label
        {
            Text = "Análise estatística de presenças e faltas por turma",
            Font = ThemeHelper.FontSmall, ForeColor = ThemeHelper.MediumText,
            AutoSize = true, Left = 2, Top = 34, BackColor = Color.Transparent
        });

        // ── Summary cards ──────────────────────────────────────────────────
        var pnlSummary = BuildSummaryCards();
        pnlSummary.Dock = DockStyle.Top;

        // ── Filters + table ────────────────────────────────────────────────
        var pnlFilters = BuildFilters();
        pnlFilters.Dock = DockStyle.Top;

        var pnlTable = BuildTable();
        pnlTable.Dock = DockStyle.Top;

        // ── Top alunos em risco ────────────────────────────────────────────
        var pnlRisk = BuildRiskPanel();
        pnlRisk.Dock = DockStyle.Top;

        Controls.AddRange(new Control[]
        {
            pnlRisk,
            pnlTable,
            pnlFilters,
            pnlSummary,
            pnlHeader
        });

        ResumeLayout(true);
        RefreshTable();
    }

    // ── Summary cards row ─────────────────────────────────────────────────
    private Panel BuildSummaryCards()
    {
        var pnl = new Panel { Height = 148, BackColor = Color.Transparent };

        var stats = new[]
        {
            ("Taxa Global de Presença",
             $"{CalcGlobalPresenca():0.0}%",
             "nos últimos 30 dias",
             ThemeHelper.LightBlue,   Color.FromArgb(219,234,254)),
            ("Total de Registos",
             MockDataStore.Presencas.Count.ToString(),
             "registos no sistema",
             ThemeHelper.Purple,      ThemeHelper.PurpleBg),
            ("Dias com Chamada",
             CountDaysWithChamada().ToString(),
             "dias lectivos registados",
             ThemeHelper.Success,     ThemeHelper.SuccessBg),
            ("Alunos em Risco",
             MockDataStore.FaltasCriticas.ToString(),
             "mais de 25% de faltas",
             ThemeHelper.Danger,      ThemeHelper.DangerBg),
        };

        int x = 0;
        foreach (var (title, value, sub, accent, accentBg) in stats)
        {
            var card = new RoundedPanel
            {
                Left         = x,
                Top          = 10,
                Width        = 240,
                Height       = 120,
                BackColor    = Color.White,
                ShowShadow   = true,
                CornerRadius = 12,
                ShadowDepth  = 4
            };

            // Icon
            var iconPic = new IconPictureBox
            {
                IconChar  = IconChar.ChartPie,
                IconColor = accent,
                IconSize  = 22,
                BackColor = accentBg,
                Width = 44, Height = 44,
                Top   = (120 - 44) / 2
            };
            card.Controls.Add(iconPic);
            card.SizeChanged += (_, _) => iconPic.Left = card.Width - iconPic.Width - 16;

            // Labels
            card.Controls.Add(new Label { Text = title, Font = ThemeHelper.FontCardLabel, ForeColor = ThemeHelper.MediumText, AutoSize = true, Left = 18, Top = 16, BackColor = Color.Transparent });
            card.Controls.Add(new Label { Text = value, Font = ThemeHelper.FontCardValue, ForeColor = ThemeHelper.DarkText,   AutoSize = true, Left = 16, Top = 36, BackColor = Color.Transparent });
            card.Controls.Add(new Label { Text = sub,   Font = ThemeHelper.FontCaption,   ForeColor = ThemeHelper.SubtleText, AutoSize = true, Left = 18, Top = 90, BackColor = Color.Transparent });

            pnl.Controls.Add(card);
            x += 256;
        }

        pnl.SizeChanged += (_, _) =>
        {
            int available = pnl.Width - 3 * 16;
            int cw = Math.Max(180, available / 4);
            int cx = 0;
            foreach (Control c in pnl.Controls)
            {
                c.Left  = cx;
                c.Width = cw;
                cx     += cw + 16;
            }
        };

        return pnl;
    }

    // ── Filters ───────────────────────────────────────────────────────────
    private Panel BuildFilters()
    {
        var pnl = new Panel { Height = 72, BackColor = Color.Transparent };

        var lblTurma = UIHelper.MakeLabel("Filtrar por Turma:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblTurma.Left = 0; lblTurma.Top = 10;

        cmbTurmaFiltro = UIHelper.MakeComboBox(160);
        cmbTurmaFiltro.Items.Add("Todas as Turmas");
        foreach (var t in MockDataStore.Turmas) cmbTurmaFiltro.Items.Add(t.Nome);
        cmbTurmaFiltro.SelectedIndex = 0;
        cmbTurmaFiltro.Left = 0; cmbTurmaFiltro.Top = 28;
        cmbTurmaFiltro.SelectedIndexChanged += (_, _) => RefreshTable();

        var lblDisc = UIHelper.MakeLabel("Disciplina:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblDisc.Left = 178; lblDisc.Top = 10;

        cmbDisciplinaFiltro = UIHelper.MakeComboBox(180);
        cmbDisciplinaFiltro.Items.Add("Todas as Disciplinas");
        foreach (var d in MockDataStore.Disciplinas) cmbDisciplinaFiltro.Items.Add(d);
        cmbDisciplinaFiltro.SelectedIndex = 0;
        cmbDisciplinaFiltro.Left = 178; cmbDisciplinaFiltro.Top = 28;
        cmbDisciplinaFiltro.SelectedIndexChanged += (_, _) => RefreshTable();

        var lblPer = UIHelper.MakeLabel("Período:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblPer.Left = 366; lblPer.Top = 10;

        cmbPeriodo = UIHelper.MakeComboBox(160);
        cmbPeriodo.Items.AddRange(new object[]
            { "Últimos 7 dias", "Últimos 30 dias", "Últimos 60 dias", "Todo o período" });
        cmbPeriodo.SelectedIndex = 1;
        cmbPeriodo.Left = 366; cmbPeriodo.Top = 28;
        cmbPeriodo.SelectedIndexChanged += (_, _) => RefreshTable();

        var btnExport = UIHelper.MakeSecondaryButton("↓  Exportar CSV", 150, 36);
        btnExport.Top  = 28;
        btnExport.Click += (_, _) =>
            MessageBox.Show("Funcionalidade de exportação disponível em versão futura.",
                            "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);

        pnl.SizeChanged += (_, _) => { btnExport.Left = pnl.Width - btnExport.Width; };
        pnl.Controls.AddRange(new Control[]
            { lblTurma, cmbTurmaFiltro, lblDisc, cmbDisciplinaFiltro, lblPer, cmbPeriodo, btnExport });

        return pnl;
    }

    // ── Main table ────────────────────────────────────────────────────────
    private Panel BuildTable()
    {
        var pnl = new Panel { Height = 280, BackColor = Color.Transparent, Padding = new Padding(0, 0, 0, 16) };

        var card = new RoundedPanel
        {
            Dock = DockStyle.Fill, BackColor = Color.White,
            ShowShadow = true, CornerRadius = 12, ShadowDepth = 4
        };

        dgv = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGridView(dgv);

        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Turma",     HeaderText = "Turma",           Width = 80  });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Classe",    HeaderText = "Classe",          Width = 100 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Alunos",    HeaderText = "Total Alunos",    Width = 100 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Presentes", HeaderText = "Presenças",       Width = 90  });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Faltas",    HeaderText = "Faltas",          Width = 80  });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Justif",    HeaderText = "Justificadas",    Width = 110 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Taxa",      HeaderText = "Taxa Presença",   FillWeight = 20 });

        dgv.CellFormatting += (_, e) =>
        {
            if (e.ColumnIndex == 6 && e.Value != null)
            {
                var val = e.Value.ToString()?.TrimEnd('%');
                if (double.TryParse(val, out double pct))
                {
                    e.CellStyle.ForeColor = pct >= 80
                        ? ThemeHelper.SuccessText
                        : pct >= 60
                            ? ThemeHelper.WarningText
                            : ThemeHelper.DangerText;
                    e.CellStyle.Font = ThemeHelper.FontBodyBold;
                }
            }
        };

        card.Controls.Add(dgv);
        pnl.Controls.Add(card);
        return pnl;
    }

    // ── Risk panel ────────────────────────────────────────────────────────
    private Panel BuildRiskPanel()
    {
        var pnl = new Panel { Height = 260, BackColor = Color.Transparent, Padding = new Padding(0, 0, 0, 16) };

        var card = new RoundedPanel
        {
            Dock = DockStyle.Fill, BackColor = Color.White,
            ShowShadow = true, CornerRadius = 12, ShadowDepth = 4,
            Padding = new Padding(20, 16, 20, 16)
        };

        var lblRisk = UIHelper.MakeLabel("Alunos em Situação de Risco (> 25% faltas)",
                                          ThemeHelper.FontSubheading, ThemeHelper.DarkText);
        lblRisk.Dock = DockStyle.Top; lblRisk.Height = 36;

        var dgvRisk = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGridView(dgvRisk);

        dgvRisk.Columns.Add(new DataGridViewTextBoxColumn { Name = "Processo", HeaderText = "Nº Processo", Width = 120 });
        dgvRisk.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome",     HeaderText = "Aluno",       FillWeight = 50 });
        dgvRisk.Columns.Add(new DataGridViewTextBoxColumn { Name = "Turma",    HeaderText = "Turma",       Width = 80 });
        dgvRisk.Columns.Add(new DataGridViewTextBoxColumn { Name = "PctFaltas",HeaderText = "% Faltas",    Width = 100 });
        dgvRisk.Columns.Add(new DataGridViewTextBoxColumn { Name = "Risco",    HeaderText = "Nível de Risco", Width = 120 });

        dgvRisk.CellFormatting += (_, e) =>
        {
            if (e.ColumnIndex == 4 && e.Value != null)
            {
                e.CellStyle.ForeColor = e.Value.ToString() == "Alto Risco"
                    ? ThemeHelper.DangerText
                    : ThemeHelper.WarningText;
                e.CellStyle.Font = ThemeHelper.FontBodyBold;
            }
        };

        // Populate risk table
        var riskAlunos = MockDataStore.Alunos
            .Select(a => new { Aluno = a, Pct = MockDataStore.GetPercentualFaltas(a.Id) })
            .Where(x => x.Pct > 25)
            .OrderByDescending(x => x.Pct)
            .ToList();

        foreach (var r in riskAlunos)
        {
            var t = MockDataStore.Turmas.FirstOrDefault(t => t.Id == r.Aluno.TurmaId);
            dgvRisk.Rows.Add(
                r.Aluno.NumeroProcesso,
                r.Aluno.NomeCompleto,
                t?.Nome ?? "—",
                $"{r.Pct:0.0}%",
                r.Pct > 40 ? "Alto Risco" : "Risco Moderado");
        }

        card.Controls.AddRange(new Control[] { dgvRisk, lblRisk });
        pnl.Controls.Add(card);
        return pnl;
    }

    // ── Data helpers ──────────────────────────────────────────────────────
    private void RefreshTable()
    {
        dgv.Rows.Clear();

        DateTime cutoff = cmbPeriodo.SelectedIndex switch
        {
            0 => DateTime.Today.AddDays(-7),
            1 => DateTime.Today.AddDays(-30),
            2 => DateTime.Today.AddDays(-60),
            _ => DateTime.MinValue
        };

        string turmaFiltro = cmbTurmaFiltro.SelectedIndex == 0 ? "" : cmbTurmaFiltro.Text;
        var discFiltro = cmbDisciplinaFiltro.SelectedIndex == 0
            ? null
            : cmbDisciplinaFiltro.SelectedItem as Disciplina;

        var turmas = string.IsNullOrEmpty(turmaFiltro)
            ? MockDataStore.Turmas
            : MockDataStore.Turmas.Where(t => t.Nome == turmaFiltro).ToList();

        foreach (var t in turmas)
        {
            var alunos   = MockDataStore.GetAlunosDaTurma(t.Id);
            var registos = MockDataStore.Presencas
                .Where(p => p.TurmaId == t.Id && p.Data.Date >= cutoff);

            if (discFiltro != null)
                registos = registos.Where(p => p.DisciplinaId == discFiltro.Id);

            // Deduplicate to one record per (student, date): Presente beats Justificada beats Falta
            var deduped = registos
                .GroupBy(p => (p.AlunoId, p.Data.Date))
                .Select(g => g.OrderBy(p => (int)p.Status).First())
                .ToList();

            int pres  = deduped.Count(p => p.Status == StatusPresenca.Presente);
            int falts = deduped.Count(p => p.Status == StatusPresenca.Falta);
            int just  = deduped.Count(p => p.Status == StatusPresenca.Justificada);
            int total = deduped.Count;
            double taxa = total > 0 ? (double)pres / total * 100.0 : 0;

            dgv.Rows.Add(t.Nome, t.NomeClasse, alunos.Count, pres, falts, just, $"{taxa:0.0}%");
        }
    }

    private static double CalcGlobalPresenca()
    {
        var last30 = MockDataStore.Presencas.Where(p => p.Data.Date >= DateTime.Today.AddDays(-30)).ToList();
        if (last30.Count == 0) return 0;
        return (double)last30.Count(p => p.Status == StatusPresenca.Presente) / last30.Count * 100.0;
    }

    private static int CountDaysWithChamada()
    {
        return MockDataStore.Presencas
            .Select(p => p.Data.Date)
            .Distinct()
            .Count();
    }
}
