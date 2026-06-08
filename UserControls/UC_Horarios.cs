using TarimbaPresence.Controls;
using TarimbaPresence.Database;
using TarimbaPresence.Helpers;
using TarimbaPresence.Models;

namespace TarimbaPresence.UserControls;

public class UC_Horarios : UserControl
{
    private readonly DatabaseService _db = new();
    private ComboBox     cmbTurma    = null!;
    private DataGridView dgv         = null!;
    private ComboBox     cmbEditDia  = null!;
    private ComboBox     cmbEditAula = null!;
    private ComboBox     cmbEditDisc = null!;
    private Label        lblStatus   = null!;

    // Period start times (minutes from base hour), Manhã=07h, Tarde=13h
    private static readonly int[] PeriodOffsets = { 0, 55, 110, 175, 230, 285 };

    public UC_Horarios()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor  = ThemeHelper.ContentBg;
        AutoScroll = false;
        Padding    = new Padding(32, 28, 32, 28);
        Build();
    }

    private void Build()
    {
        SuspendLayout();

        // ── Header ──────────────────────────────────────────────────────────
        var pnlHeader = new Panel { Height = 56, Dock = DockStyle.Top, BackColor = Color.Transparent };
        pnlHeader.Controls.AddRange(new Control[]
        {
            new Label { Text = "Horários", Font = ThemeHelper.FontTitle, ForeColor = ThemeHelper.DarkText, AutoSize = true, Left = 0, Top = 4, BackColor = Color.Transparent },
            new Label { Text = "Definição e gestão do horário semanal por turma", Font = ThemeHelper.FontSmall, ForeColor = ThemeHelper.MediumText, AutoSize = true, Left = 2, Top = 34, BackColor = Color.Transparent }
        });

        // ── Filter bar ───────────────────────────────────────────────────────
        var pnlFilter = new Panel { Height = 72, Dock = DockStyle.Top, BackColor = Color.Transparent };

        var lblTurma = UIHelper.MakeLabel("Turma:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblTurma.Left = 0; lblTurma.Top = 10;

        cmbTurma = UIHelper.MakeComboBox(230);
        cmbTurma.Left = 0; cmbTurma.Top = 30;
        cmbTurma.SelectedIndexChanged += (_, _) => { LoadDiscipinas(); LoadHorario(); };

        lblStatus = UIHelper.MakeLabel("", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblStatus.AutoSize = true; lblStatus.Left = 248; lblStatus.Top = 38;

        pnlFilter.Controls.AddRange(new Control[] { lblTurma, cmbTurma, lblStatus });

        // ── Main timetable card ──────────────────────────────────────────────
        var pnlContent = new Panel { BackColor = Color.Transparent, Padding = new Padding(0, 4, 0, 0) };
        pnlContent.Dock = DockStyle.Fill;

        var card = new RoundedPanel
        {
            Dock = DockStyle.Fill, BackColor = Color.White,
            ShowShadow = true, CornerRadius = 12, ShadowDepth = 4
        };

        var pnlEdit = BuildEditBar();
        pnlEdit.Dock = DockStyle.Bottom;

        dgv = BuildTimetableDgv();
        dgv.Dock = DockStyle.Fill;

        card.Controls.AddRange(new Control[] { dgv, pnlEdit });
        pnlContent.Controls.Add(card);

        Controls.AddRange(new Control[] { pnlContent, pnlFilter, pnlHeader });

        // Populate turma combo after events wired
        foreach (var t in _db.ObterTodasTurmas()) cmbTurma.Items.Add(t);
        if (cmbTurma.Items.Count > 0) cmbTurma.SelectedIndex = 0;

        ResumeLayout(true);
    }

    // ── Timetable DataGridView ────────────────────────────────────────────
    private DataGridView BuildTimetableDgv()
    {
        var dg = new DataGridView();
        UIHelper.StyleDataGridView(dg);
        dg.ReadOnly      = true;
        dg.SelectionMode = DataGridViewSelectionMode.CellSelect;
        dg.DefaultCellStyle.WrapMode  = DataGridViewTriState.True;
        dg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dg.DefaultCellStyle.Font      = ThemeHelper.FontSmall;
        dg.RowTemplate.Height         = 64;
        dg.ColumnHeadersHeight        = 40;
        dg.ScrollBars                 = ScrollBars.None;
        dg.AllowUserToResizeColumns   = false;

        dg.Columns.Add(new DataGridViewTextBoxColumn { Name = "Hora",     HeaderText = "Hora",    Width = 90,  Resizable = DataGridViewTriState.False });
        dg.Columns.Add(new DataGridViewTextBoxColumn { Name = "Segunda",  HeaderText = "Segunda", FillWeight = 20 });
        dg.Columns.Add(new DataGridViewTextBoxColumn { Name = "Terca",    HeaderText = "Terça",   FillWeight = 20 });
        dg.Columns.Add(new DataGridViewTextBoxColumn { Name = "Quarta",   HeaderText = "Quarta",  FillWeight = 20 });
        dg.Columns.Add(new DataGridViewTextBoxColumn { Name = "Quinta",   HeaderText = "Quinta",  FillWeight = 20 });
        dg.Columns.Add(new DataGridViewTextBoxColumn { Name = "Sexta",    HeaderText = "Sexta",   FillWeight = 20 });

        dg.CellFormatting += Dgv_CellFormatting;
        dg.CellClick      += Dgv_CellClick;

        return dg;
    }

    // ── Edit bar ──────────────────────────────────────────────────────────
    private Panel BuildEditBar()
    {
        var pnl = new Panel { Height = 72, BackColor = ThemeHelper.ContentBg };

        var lblDia = UIHelper.MakeLabel("Dia:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblDia.Left = 16; lblDia.Top = 10;

        cmbEditDia = UIHelper.MakeComboBox(120);
        cmbEditDia.Left = 16; cmbEditDia.Top = 30;
        cmbEditDia.Items.AddRange(new object[] { "Segunda", "Terça", "Quarta", "Quinta", "Sexta" });
        cmbEditDia.SelectedIndex = 0;

        var lblAula = UIHelper.MakeLabel("Aula:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblAula.Left = 152; lblAula.Top = 10;

        cmbEditAula = UIHelper.MakeComboBox(110);
        cmbEditAula.Left = 152; cmbEditAula.Top = 30;
        cmbEditAula.Items.AddRange(new object[] { "1ª Aula", "2ª Aula", "3ª Aula", "4ª Aula", "5ª Aula", "6ª Aula" });
        cmbEditAula.SelectedIndex = 0;

        var lblDisc = UIHelper.MakeLabel("Disciplina:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblDisc.Left = 278; lblDisc.Top = 10;

        cmbEditDisc = UIHelper.MakeComboBox(220);
        cmbEditDisc.Left = 278; cmbEditDisc.Top = 30;

        var btnAtribuir = UIHelper.MakePrimaryButton("Atribuir", 110, 36);
        btnAtribuir.Left = 514; btnAtribuir.Top = 30;
        btnAtribuir.Click += OnAtribuir;

        var btnLimpar = UIHelper.MakeSecondaryButton("Limpar Slot", 110, 36);
        btnLimpar.Left = 634; btnLimpar.Top = 30;
        btnLimpar.Click += OnLimpar;

        pnl.Controls.AddRange(new Control[]
            { lblDia, cmbEditDia, lblAula, cmbEditAula, lblDisc, cmbEditDisc, btnAtribuir, btnLimpar });

        return pnl;
    }

    // ── Load logic ────────────────────────────────────────────────────────
    private void LoadDiscipinas()
    {
        if (cmbEditDisc == null) return;
        cmbEditDisc.Items.Clear();
        if (cmbTurma.SelectedItem is not Turma turma) return;
        foreach (var d in _db.ObterDisciplinasDaTurma(turma))
            cmbEditDisc.Items.Add(d);
        if (cmbEditDisc.Items.Count > 0) cmbEditDisc.SelectedIndex = 0;
    }

    private void LoadHorario()
    {
        if (cmbTurma.SelectedItem is not Turma turma)
        {
            dgv.Rows.Clear();
            lblStatus.Text = "";
            return;
        }

        string[] times = GetTimesForTurno(turma.Turno);
        int discCount  = _db.ObterDisciplinasDaTurma(turma).Count;
        lblStatus.Text = $"{turma.Turno.DisplayText()}  |  {discCount} disciplinas";

        dgv.Rows.Clear();
        for (int aula = 1; aula <= 6; aula++)
        {
            var row = new DataGridViewRow();
            row.CreateCells(dgv);
            row.Height = 64;

            row.Cells[0].Value = times[aula - 1];
            row.Cells[0].Style.Font      = ThemeHelper.FontSmall;
            row.Cells[0].Style.ForeColor = ThemeHelper.MediumText;
            row.Cells[0].Style.BackColor = ThemeHelper.LightGray;

            for (int dia = 1; dia <= 5; dia++)
            {
                var h = _db.ObterHorario(turma.Id, dia, aula);
                if (h != null)
                {
                    var disc = _db.ObterDisciplinaPorId(h.DisciplinaId);
                    var prof = _db.ObterProfessorDaDisciplina(turma.Id, h.DisciplinaId);
                    string profShort = prof != null ? ShortName(prof.NomeCompleto) : "—";
                    row.Cells[dia].Value = $"{disc?.Nome ?? "—"}\n{profShort}";
                }
                else
                {
                    row.Cells[dia].Value = "—";
                }
            }
            dgv.Rows.Add(row);
        }
    }

    // ── Event handlers ────────────────────────────────────────────────────
    private void Dgv_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex < 1 || e.RowIndex < 0) return;

        cmbEditDia.SelectedIndex  = e.ColumnIndex - 1;
        cmbEditAula.SelectedIndex = e.RowIndex;

        if (cmbTurma.SelectedItem is not Turma turma) return;
        var h = _db.ObterHorario(turma.Id, e.ColumnIndex, e.RowIndex + 1);
        if (h == null) return;

        for (int i = 0; i < cmbEditDisc.Items.Count; i++)
        {
            if (cmbEditDisc.Items[i] is Disciplina d && d.Id == h.DisciplinaId)
            {
                cmbEditDisc.SelectedIndex = i;
                break;
            }
        }
    }

    private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.ColumnIndex == 0 || e.RowIndex < 0 ||
            cmbTurma.SelectedItem is not Turma turma) return;

        int dia  = e.ColumnIndex;
        int aula = e.RowIndex + 1;

        var h = _db.ObterHorario(turma.Id, dia, aula);
        if (h == null) { e.CellStyle.BackColor = Color.White; return; }

        var disc = _db.ObterDisciplinaPorId(h.DisciplinaId);
        e.CellStyle.BackColor = disc?.Curso switch
        {
            null                                           => Color.FromArgb(219, 234, 254), // azul (transversal)
            Curso.CienciasEconomicasEJuridicas             => Color.FromArgb(209, 250, 229), // verde (CEJ)
            Curso.CienciasFisicasEBiologicas               => Color.FromArgb(237, 233, 254), // lilás (CFB)
            Curso.InformaticaDeGestao                      => Color.FromArgb(254, 237, 213), // laranja (IG)
            Curso.ContabilidadeEGestao                     => Color.FromArgb(204, 251, 241), // teal (CG)
            _                                              => Color.White
        };
        e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
        e.CellStyle.SelectionForeColor = ThemeHelper.DarkText;
        e.CellStyle.Font = ThemeHelper.FontSmall;
    }

    private void OnAtribuir(object? sender, EventArgs e)
    {
        if (cmbTurma.SelectedItem    is not Turma      turma ||
            cmbEditDisc.SelectedItem  is not Disciplina disc)
        {
            MessageBox.Show("Seleccione uma turma e uma disciplina.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int dia  = cmbEditDia.SelectedIndex  + 1;
        int aula = cmbEditAula.SelectedIndex + 1;

        _db.RemoverHorario(turma.Id, dia, aula);
        _db.CriarHorario(new Horario
        {
            TurmaId      = turma.Id,
            DisciplinaId = disc.Id,
            DiaDaSemana  = dia,
            Aula         = aula
        });

        LoadHorario();
    }

    private void OnLimpar(object? sender, EventArgs e)
    {
        if (cmbTurma.SelectedItem is not Turma turma) return;

        int dia  = cmbEditDia.SelectedIndex  + 1;
        int aula = cmbEditAula.SelectedIndex + 1;

        _db.RemoverHorario(turma.Id, dia, aula);
        LoadHorario();
    }

    // ── Helpers ───────────────────────────────────────────────────────────
    private static string[] GetTimesForTurno(Turno turno)
    {
        int baseH = turno == Turno.Manha ? 7 : 13;
        return PeriodOffsets.Select(off =>
        {
            int totalMin = baseH * 60 + off;
            string start = $"{totalMin / 60:D2}:{totalMin % 60:D2}";
            int endMin = totalMin + 50;
            string end = $"{endMin / 60:D2}:{endMin % 60:D2}";
            return $"{start}\n{end}";
        }).ToArray();
    }

    private static string ShortName(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2 ? $"{parts[0]} {parts[^1]}" : fullName;
    }
}
