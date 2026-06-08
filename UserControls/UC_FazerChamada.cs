using FontAwesome.Sharp;
using TarimbaPresence.Controls;
using TarimbaPresence.Database;
using TarimbaPresence.Helpers;
using TarimbaPresence.Models;

namespace TarimbaPresence.UserControls;

public class UC_FazerChamada : UserControl
{
    private ComboBox       cmbTurma      = null!;
    private ComboBox       cmbDisciplina = null!;
    private DateTimePicker dtpData       = null!;
    private DataGridView   dgv           = null!;
    private Label          lblInfo       = null!;

    private bool _loadingDisciplinas;
    private readonly List<(Aluno Aluno, StatusPresenca Status)> _registos = new();
    private readonly DatabaseService _db = new();
    public UC_FazerChamada()
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

        // ── Page header ────────────────────────────────────────────────────
        var pnlHeader = new Panel { Height = 56, Dock = DockStyle.Top, BackColor = Color.Transparent };
        pnlHeader.Controls.AddRange(new Control[]
        {
            new Label
            {
                Text      = "Registar Presenças",
                Font      = ThemeHelper.FontTitle,
                ForeColor = ThemeHelper.DarkText,
                AutoSize  = true, Left = 0, Top = 4,
                BackColor = Color.Transparent
            },
            new Label
            {
                Text      = "Seleccione a turma e disciplina, depois marque a presença de cada aluno",
                Font      = ThemeHelper.FontSmall,
                ForeColor = ThemeHelper.MediumText,
                AutoSize  = true, Left = 2, Top = 34,
                BackColor = Color.Transparent
            }
        });

        var pnlFilter = BuildFilterBar();
        pnlFilter.Dock = DockStyle.Top;

        var pnlTable = BuildTableCard();
        pnlTable.Dock = DockStyle.Fill;

        // Add to Controls BEFORE wiring events so dgv is non-null when events fire
        Controls.AddRange(new Control[] { pnlTable, pnlFilter, pnlHeader });

        // Wire events after all controls exist
        cmbTurma.SelectedIndexChanged      += (_, _) => LoadDisciplinas();
        cmbDisciplina.SelectedIndexChanged  += OnDisciplinaChanged;
        dtpData.ValueChanged               += (_, _) => LoadChamada();

        // Populate and trigger initial cascade: turma → disciplinas → chamada
        var turmasParaMostrar = Program.UtilizadorAtual == "PROFESSOR" && Program.ContaProfessorAtual != null
            ? _db.ObterTurmasDoProfessor(Program.ContaProfessorAtual.ProfessorId).AsEnumerable()
            : _db.ObterTodasTurmas().AsEnumerable();

        foreach (var t in turmasParaMostrar)
            cmbTurma.Items.Add(t);

        if (cmbTurma.Items.Count > 0)
            cmbTurma.SelectedIndex = 0;

        ResumeLayout(true);
    }

    // ── Filter bar ─────────────────────────────────────────────────────────
    private Panel BuildFilterBar()
    {
        var pnl = new Panel { Height = 80, BackColor = Color.Transparent };

        // Turma
        var lblTurma = UIHelper.MakeLabel("Turma:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblTurma.Left = 0; lblTurma.Top = 10;
        cmbTurma = UIHelper.MakeComboBox(160);
        cmbTurma.Left = 0; cmbTurma.Top = 30;

        // Disciplina
        var lblDisc = UIHelper.MakeLabel("Disciplina:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblDisc.Left = 178; lblDisc.Top = 10;
        cmbDisciplina = UIHelper.MakeComboBox(175);
        cmbDisciplina.Left = 178; cmbDisciplina.Top = 30;

        // Data
        var lblData = UIHelper.MakeLabel("Data:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblData.Left = 368; lblData.Top = 10;
        dtpData = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Font   = ThemeHelper.FontBody,
            Width  = 140, Left = 368, Top = 30,
            Value  = DateTime.Today
        };

        // Info label
        lblInfo = UIHelper.MakeLabel("", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblInfo.AutoSize = true;
        lblInfo.Left = 524; lblInfo.Top = 38;

        // Buttons (right-aligned)
        var btnAll = UIHelper.MakePrimaryButton("✓  Todos Presentes", 178, 38);
        btnAll.Top = 29;
        btnAll.Click += (_, _) => MarkAllPresent();

        var btnSave = UIHelper.MakePrimaryButton("💾  Guardar Registos", 178, 38);
        btnSave.BackColor = ThemeHelper.Success;
        btnSave.FlatAppearance.MouseOverBackColor = ThemeHelper.SuccessText;
        btnSave.Top = 29;
        btnSave.Click += (_, _) => SaveRecords();

        pnl.SizeChanged += (_, _) =>
        {
            btnSave.Left = pnl.Width - btnSave.Width;
            btnAll.Left  = pnl.Width - btnAll.Width - btnSave.Width - 12;
        };

        pnl.Controls.AddRange(new Control[]
        {
            lblTurma, cmbTurma, lblDisc, cmbDisciplina,
            lblData, dtpData, lblInfo, btnAll, btnSave
        });

        return pnl;
    }

    // ── Table card ──────────────────────────────────────────────────────────
    private Panel BuildTableCard()
    {
        var pnl = new Panel { BackColor = Color.Transparent, Padding = new Padding(0, 8, 0, 8) };

        var card = new RoundedPanel
        {
            Dock = DockStyle.Fill, BackColor = Color.White,
            ShowShadow = true, CornerRadius = 12, ShadowDepth = 4
        };

        dgv = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGridView(dgv);
        dgv.ReadOnly = false;

        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Processo", HeaderText = "Nº Processo", ReadOnly = true, Width = 120 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Nome", HeaderText = "Nome Completo", ReadOnly = true, FillWeight = 50 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Sexo", HeaderText = "Sexo", ReadOnly = true, Width = 64 });
        dgv.Columns.Add(new DataGridViewComboBoxColumn
        {
            Name = "Status", HeaderText = "Presença", Width = 148,
            DataSource = new[] { "Presente", "Falta", "Justificada" },
            DefaultCellStyle = { Font = ThemeHelper.FontBodyBold },
            FlatStyle = FlatStyle.Flat
        });

        dgv.CellFormatting        += Dgv_CellFormatting;
        dgv.CellValueChanged      += Dgv_CellValueChanged;
        dgv.CurrentCellDirtyStateChanged += (_, _) =>
        {
            if (dgv.IsCurrentCellDirty) dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };

        card.Controls.Add(dgv);
        pnl.Controls.Add(card);
        return pnl;
    }

    // ── Discipline cascade ─────────────────────────────────────────────────
    private void LoadDisciplinas()
    {
        if (cmbTurma.SelectedItem is not Turma turma) return;

        _loadingDisciplinas = true;
        cmbDisciplina.Items.Clear();
        foreach (var d in _db.ObterDisciplinasDaTurma(turma))
            cmbDisciplina.Items.Add(d);
        _loadingDisciplinas = false;

        if (cmbDisciplina.Items.Count > 0)
            cmbDisciplina.SelectedIndex = 0;  // → fires OnDisciplinaChanged → LoadChamada()
        else
            LoadChamada();
    }

    private void OnDisciplinaChanged(object? sender, EventArgs e)
    {
        if (!_loadingDisciplinas) LoadChamada();
    }

    // ── Load chamada ───────────────────────────────────────────────────────
    private void LoadChamada()
    {
        if (cmbTurma.SelectedItem      is not Turma      turma      ||
            cmbDisciplina.SelectedItem  is not Disciplina disciplina)
        {
            dgv.Rows.Clear();
            lblInfo.Text = "";
            return;
        }

        _registos.Clear();
        var alunos = _db.ObterAlunosDaTurma(turma.Id);

        // Buscar presenças já registadas para este dia/turma/disciplina
        var presencasHoje = _db.ObterPresencas(
            turmaId: turma.Id,
            de:  dtpData.Value.Date,
            ate: dtpData.Value.Date);

        foreach (var a in alunos)
        {
            var existing = presencasHoje.FirstOrDefault(p =>
                p.AlunoId      == a.Id         &&
                p.DisciplinaId == disciplina.Id);
            _registos.Add((a, existing?.Status ?? StatusPresenca.Presente));
        }

        lblInfo.Text = $"{alunos.Count} alunos  |  {turma.Turno.DisplayText()}";
        PopulateGrid(_registos);
    }

    private void PopulateGrid(IEnumerable<(Aluno Aluno, StatusPresenca Status)> source)
    {
        dgv.Rows.Clear();
        foreach (var (a, s) in source)
            dgv.Rows.Add(a.NumeroProcesso, a.NomeCompleto, a.Sexo, s.StatusTexto());
    }

    private void MarkAllPresent()
    {
        for (int i = 0; i < _registos.Count; i++)
            _registos[i] = (_registos[i].Aluno, StatusPresenca.Presente);
        PopulateGrid(_registos);
    }

    private void SaveRecords()
    {
        if (cmbTurma.SelectedItem      is not Turma      turma      ||
            cmbDisciplina.SelectedItem  is not Disciplina disciplina) return;

        SyncGridToRegistos();

        // Preparar lista de presenças para guardar na base de dados
        var paraGuardar = _registos.Select(r => new Presenca
        {
            AlunoId      = r.Aluno.Id,
            TurmaId      = turma.Id,
            DisciplinaId = disciplina.Id,
            Data         = dtpData.Value.Date,
            Status       = r.Status
        }).ToList();

        _db.GuardarPresencas(paraGuardar);

        MessageBox.Show(
            $"✓  {_registos.Count} registos guardados com sucesso.\n{turma.Nome}  |  {disciplina.Nome}",
            "Registos Guardados",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void SyncGridToRegistos()
    {
        for (int i = 0; i < dgv.Rows.Count && i < _registos.Count; i++)
        {
            var cellVal = dgv.Rows[i].Cells["Status"].Value?.ToString();
            _registos[i] = (_registos[i].Aluno, cellVal switch
            {
                "Falta"       => StatusPresenca.Falta,
                "Justificada" => StatusPresenca.Justificada,
                _             => StatusPresenca.Presente
            });
        }
    }

    // ── Cell formatting (colour status column) ─────────────────────────────
    private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.ColumnIndex != 3 || e.Value == null) return;
        switch (e.Value.ToString())
        {
            case "Presente":
                e.CellStyle.ForeColor = ThemeHelper.SuccessText;
                e.CellStyle.BackColor = ThemeHelper.SuccessBg;
                break;
            case "Falta":
                e.CellStyle.ForeColor = ThemeHelper.DangerText;
                e.CellStyle.BackColor = ThemeHelper.DangerBg;
                break;
            case "Justificada":
                e.CellStyle.ForeColor = ThemeHelper.WarningText;
                e.CellStyle.BackColor = ThemeHelper.WarningBg;
                break;
        }
        e.CellStyle.Font = ThemeHelper.FontBodyBold;
    }

    private void Dgv_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex == 3) dgv.InvalidateRow(e.RowIndex);
    }
}

// ── Extension for StatusPresenca ──────────────────────────────────────────
internal static class StatusExt
{
    public static string StatusTexto(this StatusPresenca s) => s switch
    {
        StatusPresenca.Presente    => "Presente",
        StatusPresenca.Falta       => "Falta",
        StatusPresenca.Justificada => "Justificada",
        _                          => "Presente"
    };
}
