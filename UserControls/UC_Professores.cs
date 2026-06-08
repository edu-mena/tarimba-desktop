using TarimbaPresence.Controls;
using TarimbaPresence.Helpers;
using TarimbaPresence.Models;
using TarimbaPresence.Database;

namespace TarimbaPresence.UserControls;

public class UC_Professores : UserControl
{
    private readonly DatabaseService _db;
    // Left panel
    private DataGridView dgvProfessores = null!;

    // Right panel — professor form
    private Label    lblFormTitle  = null!;
    private TextBox  txtNome       = null!;
    private TextBox  txtEmail      = null!;
    private TextBox  txtTelefone   = null!;
    private CheckBox chkAtivo      = null!;
    private Button   btnGuardar    = null!;
    private Button   btnNovo       = null!;

    // Right panel — assignment section
    private DataGridView dgvAtribuicoes  = null!;
    private ComboBox     cmbTurmaAtrib   = null!;
    private ComboBox     cmbDiscAtrib    = null!;
    private Panel        pnlAssignWrap   = null!;

    private Professor? _selectedProfessor;
    private bool       _isNewMode;

    public UC_Professores()
    {
        _db = new DatabaseService();

        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor  = ThemeHelper.ContentBg;
        AutoScroll = false;
        Padding    = new Padding(32, 28, 32, 28);
        Build();
    }

    private void Build()
    {
        SuspendLayout();

        var pnlHeader = new Panel { Height = 56, Dock = DockStyle.Top, BackColor = Color.Transparent };
        pnlHeader.Controls.AddRange(new Control[]
        {
            new Label { Text = "Professores e Disciplinas", Font = ThemeHelper.FontTitle, ForeColor = ThemeHelper.DarkText, AutoSize = true, Left = 0, Top = 4, BackColor = Color.Transparent },
            new Label { Text = "Cadastro de professores e atribuição de disciplinas por turma", Font = ThemeHelper.FontSmall, ForeColor = ThemeHelper.MediumText, AutoSize = true, Left = 2, Top = 34, BackColor = Color.Transparent }
        });

        var tlp = new TableLayoutPanel
        {
            Dock            = DockStyle.Fill,
            ColumnCount     = 2,
            RowCount        = 1,
            BackColor       = Color.Transparent,
            Padding         = Padding.Empty,
            Margin          = Padding.Empty,
            CellBorderStyle = TableLayoutPanelCellBorderStyle.None
        };
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 370f));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,  100f));
        tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var pnlLeft  = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        var pnlRight = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(12, 0, 0, 0) };

        BuildLeftPanel(pnlLeft);
        BuildRightPanel(pnlRight);

        tlp.Controls.Add(pnlLeft,  0, 0);
        tlp.Controls.Add(pnlRight, 1, 0);

        Controls.AddRange(new Control[] { tlp, pnlHeader });

        ResumeLayout(true);
        LoadProfessores();
    }

    // ── LEFT PANEL ────────────────────────────────────────────────────────
    private void BuildLeftPanel(Panel parent)
    {
        var card = new RoundedPanel
        {
            Dock = DockStyle.Fill, BackColor = Color.White,
            ShowShadow = true, CornerRadius = 12, ShadowDepth = 4
        };

        // Top button row
        var pnlTop = new Panel { Height = 56, Dock = DockStyle.Top, BackColor = Color.Transparent, Padding = new Padding(12, 10, 12, 0) };

        btnNovo = UIHelper.MakePrimaryButton("+ Novo Professor", 180, 36);
        btnNovo.Left = 12; btnNovo.Top = 10;
        btnNovo.Click += OnNovoProfessor;
        pnlTop.Controls.Add(btnNovo);

        // Professor list DGV
        dgvProfessores = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGridView(dgvProfessores);
        dgvProfessores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvProfessores.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProfId", Visible = false });
        dgvProfessores.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome",    HeaderText = "Professor",     FillWeight = 70 });
        dgvProfessores.Columns.Add(new DataGridViewTextBoxColumn { Name = "Atrib",   HeaderText = "Disciplinas",   Width = 90  });
        dgvProfessores.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado",  HeaderText = "Estado",        Width = 72  });

        dgvProfessores.CellFormatting += (_, e) =>
        {
            if (e.ColumnIndex == 3 && e.Value?.ToString() == "Inativo")
            {
                e.CellStyle.ForeColor  = ThemeHelper.DangerText;
                e.CellStyle.Font       = ThemeHelper.FontBodyBold;
            }
        };
        dgvProfessores.SelectionChanged += OnProfessorSelected;

        card.Controls.AddRange(new Control[] { dgvProfessores, pnlTop });
        parent.Controls.Add(card);
    }

    // ── RIGHT PANEL ───────────────────────────────────────────────────────
    private void BuildRightPanel(Panel parent)
    {
        var pnl = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

        // ── Professor form card ──────────────────────────────────────────
        var pnlFormOuter = new Panel { Height = 200, Dock = DockStyle.Top, BackColor = Color.Transparent };

        var formCard = new RoundedPanel
        {
            Dock = DockStyle.Fill, BackColor = Color.White,
            ShowShadow = true, CornerRadius = 12, ShadowDepth = 4,
            Padding = new Padding(20, 14, 20, 14)
        };

        lblFormTitle = UIHelper.MakeLabel("Seleccione um professor", ThemeHelper.FontSubheading, ThemeHelper.MediumText);
        lblFormTitle.Dock = DockStyle.Top; lblFormTitle.Height = 32;

        // Form fields (absolute layout inside formCard)
        var pnlFields = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

        var lblN = UIHelper.MakeLabel("Nome:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblN.Left = 0; lblN.Top = 10; lblN.AutoSize = true;

        txtNome = UIHelper.MakeTextBox(280);
        txtNome.Left = 90; txtNome.Top = 8;

        var lblE = UIHelper.MakeLabel("Email:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblE.Left = 0; lblE.Top = 54; lblE.AutoSize = true;

        txtEmail = UIHelper.MakeTextBox(280);
        txtEmail.Left = 90; txtEmail.Top = 52;

        var lblT = UIHelper.MakeLabel("Telefone:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblT.Left = 0; lblT.Top = 98; lblT.AutoSize = true;

        txtTelefone = UIHelper.MakeTextBox(170);
        txtTelefone.Left = 90; txtTelefone.Top = 96;

        chkAtivo = new CheckBox
        {
            Text = "Ativo", Font = ThemeHelper.FontBody, Left = 274, Top = 100,
            AutoSize = true, BackColor = Color.Transparent, Checked = true
        };

        btnGuardar = UIHelper.MakePrimaryButton("Guardar", 130, 36);
        btnGuardar.Left = 90; btnGuardar.Top = 138;
        btnGuardar.Click += OnGuardar;

        var btnRedefinir = UIHelper.MakeSecondaryButton("🔑 Redefinir Senha", 150, 36);
        btnRedefinir.Left = 230; btnRedefinir.Top = 138;
        btnRedefinir.Click += OnRedefinirSenha;

        pnlFields.Controls.AddRange(new Control[]
            { lblN, txtNome, lblE, txtEmail, lblT, txtTelefone, chkAtivo, btnGuardar, btnRedefinir });
        formCard.Controls.AddRange(new Control[] { pnlFields, lblFormTitle });
        pnlFormOuter.Controls.Add(formCard);

        // ── Assignment section header ────────────────────────────────────
        var pnlAssignHeader = new Panel { Height = 44, Dock = DockStyle.Top, BackColor = Color.Transparent };
        var lblAssign = new Label
        {
            Text = "Atribuição de Disciplinas", Font = ThemeHelper.FontSubheading,
            ForeColor = ThemeHelper.DarkText, AutoSize = true, Left = 0, Top = 12,
            BackColor = Color.Transparent
        };
        pnlAssignHeader.Controls.Add(lblAssign);

        // ── Assignment form bar ──────────────────────────────────────────
        var pnlAssignForm = new Panel { Height = 80, Dock = DockStyle.Top, BackColor = Color.Transparent };

        var lblTurma = UIHelper.MakeLabel("Turma:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblTurma.Left = 0; lblTurma.Top = 10;

        cmbTurmaAtrib = UIHelper.MakeComboBox(185);
        cmbTurmaAtrib.Left = 0; cmbTurmaAtrib.Top = 30;

        var lblDisc = UIHelper.MakeLabel("Disciplina:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblDisc.Left = 203; lblDisc.Top = 10;

        cmbDiscAtrib = UIHelper.MakeComboBox(185);
        cmbDiscAtrib.Left = 203; cmbDiscAtrib.Top = 30;

        var btnAtribuir = UIHelper.MakePrimaryButton("+ Atribuir", 120, 36);
        btnAtribuir.Left = 400; btnAtribuir.Top = 30;
        btnAtribuir.Click += OnAddAtribuicao;

        // Wire turma combo AFTER disc combo exists
        cmbTurmaAtrib.SelectedIndexChanged += (_, _) => LoadDisciplinasParaTurma();
        foreach (var t in _db.ObterTodasTurmas()) cmbTurmaAtrib.Items.Add(t);
        if (cmbTurmaAtrib.Items.Count > 0) cmbTurmaAtrib.SelectedIndex = 0;

        pnlAssignForm.Controls.AddRange(new Control[]
            { lblTurma, cmbTurmaAtrib, lblDisc, cmbDiscAtrib, btnAtribuir });

        // ── Assignments DGV ──────────────────────────────────────────────
        pnlAssignWrap = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(0, 4, 0, 0) };

        var assignCard = new RoundedPanel
        {
            Dock = DockStyle.Fill, BackColor = Color.White,
            ShowShadow = true, CornerRadius = 12, ShadowDepth = 4
        };

        var pnlRemove = new Panel { Height = 52, Dock = DockStyle.Bottom, BackColor = Color.Transparent };
        var btnRemove = UIHelper.MakeSecondaryButton("Remover Seleccionado", 190, 36);
        btnRemove.Top = 8;
        btnRemove.BackColor = ThemeHelper.DangerBg;
        btnRemove.ForeColor = ThemeHelper.DangerText;
        btnRemove.Click += OnRemoveAtribuicao;
        pnlRemove.SizeChanged += (_, _) => btnRemove.Left = pnlRemove.Width - btnRemove.Width;
        pnlRemove.Controls.Add(btnRemove);

        dgvAtribuicoes = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGridView(dgvAtribuicoes);
        dgvAtribuicoes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvAtribuicoes.Columns.Add(new DataGridViewTextBoxColumn { Name = "AtribId",    Visible = false });
        dgvAtribuicoes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Turma",      HeaderText = "Turma",      FillWeight = 35 });
        dgvAtribuicoes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Disciplina", HeaderText = "Disciplina", FillWeight = 40 });
        dgvAtribuicoes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Classe",     HeaderText = "Classe",     Width = 90 });
        dgvAtribuicoes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Turno",      HeaderText = "Turno",      Width = 76 });

        assignCard.Controls.AddRange(new Control[] { dgvAtribuicoes, pnlRemove });
        pnlAssignWrap.Controls.Add(assignCard);

        // Add to right panel (Fill first, Tops in reverse display order)
        pnl.Controls.AddRange(new Control[]
            { pnlAssignWrap, pnlAssignForm, pnlAssignHeader, pnlFormOuter });

        parent.Controls.Add(pnl);
    }

    // ── Data loading ──────────────────────────────────────────────────────
    private void LoadProfessores()
    {
        dgvProfessores.Rows.Clear();

        foreach (var p in _db.ObterTodosProfessores())
        {
            dgvProfessores.Rows.Add(
                p.Id,
                p.NomeCompleto,
                0,
                p.Ativo ? "Ativo" : "Inativo");
        }
    }

    private void LoadAtribuicoes()
    {
        dgvAtribuicoes.Rows.Clear();
        if (_selectedProfessor == null) return;

        foreach (var a in _db.ObterAtribuicoesPorProfessor(_selectedProfessor.Id))
        {
            var turma = _db.ObterTurma(a.TurmaId);
            var disc  = _db.ObterDisciplinaPorId(a.DisciplinaId);
            if (turma == null || disc == null) continue;
            dgvAtribuicoes.Rows.Add(a.Id, turma.Nome, disc.Nome, turma.NomeClasse, turma.Turno.DisplayText());
        }
    }

    private void LoadDisciplinasParaTurma()
    {
        if (cmbDiscAtrib == null) return;
        cmbDiscAtrib.Items.Clear();
        if (cmbTurmaAtrib.SelectedItem is not Turma turma) return;
        foreach (var d in _db.ObterDisciplinasDaTurma(turma))
            cmbDiscAtrib.Items.Add(d);
        if (cmbDiscAtrib.Items.Count > 0) cmbDiscAtrib.SelectedIndex = 0;
    }

    private void FillForm(Professor? prof)
    {
        if (prof == null)
        {
            txtNome.Text       = "";
            txtEmail.Text      = "";
            txtTelefone.Text   = "";
            chkAtivo.Checked   = true;
            lblFormTitle.Text  = "Novo Professor";
            lblFormTitle.ForeColor = ThemeHelper.LightBlue;
        }
        else
        {
            txtNome.Text       = prof.NomeCompleto;
            txtEmail.Text      = prof.Email;
            txtTelefone.Text   = prof.Telefone;
            chkAtivo.Checked   = prof.Ativo;
            lblFormTitle.Text  = prof.NomeCompleto;
            lblFormTitle.ForeColor = ThemeHelper.DarkText;
        }
    }

    // ── Event handlers ────────────────────────────────────────────────────
    private void OnNovoProfessor(object? sender, EventArgs e)
    {
        _selectedProfessor = null;
        _isNewMode         = true;
        dgvProfessores.ClearSelection();
        FillForm(null);
        dgvAtribuicoes.Rows.Clear();
        txtNome.Focus();
    }

    private void OnProfessorSelected(object? sender, EventArgs e)
    {
        if (dgvProfessores.SelectedRows.Count == 0) return;
        var profId = Convert.ToInt32(dgvProfessores.SelectedRows[0].Cells["ProfId"].Value);
        _selectedProfessor = _db.ObterProfessorPorId(profId);
        _isNewMode         = false;
        FillForm(_selectedProfessor);
        LoadAtribuicoes();
    }

    private void OnGuardar(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNome.Text))
        {
            MessageBox.Show("O nome do professor é obrigatório.", "Validação",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtNome.Focus();
            return;
        }

        if (_isNewMode)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("O email é obrigatório para criar a conta de acesso.",
                    "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            var novo = new Professor
            {
                NomeCompleto = txtNome.Text.Trim(),
                Email        = txtEmail.Text.Trim(),
                Telefone     = txtTelefone.Text.Trim(),
                Ativo        = chkAtivo.Checked
            };
            novo.Id = _db.CriarProfessor(novo);

            // Gerar senha automática: primeiro nome + 4 dígitos aleatórios
            string primeiroNome = novo.NomeCompleto.Split(' ')[0].ToLower();
            string senha = primeiroNome + new Random().Next(1000, 9999).ToString();

            // Criar conta de login na base de dados
            var conta = new ContaProfessor
            {
                ProfessorId   = novo.Id,
                Email         = novo.Email,
                PasswordHash  = senha,
                Ativo         = true,
                PrimeiroLogin = true
            };
            _db.CriarContaProfessor(conta);

            _selectedProfessor = novo;
            _isNewMode = false;

            LoadProfessores();
            SelectProfessorInGrid(novo.Id);

            // Mostrar a senha ao administrador
            MessageBox.Show(
                $"✓  Professor criado com sucesso!\n\n" +
                $"Nome:   {novo.NomeCompleto}\n" +
                $"Email:  {novo.Email}\n" +
                $"Senha:  {senha}\n\n" +
                $"Guarde esta senha e entregue ao professor.\n" +
                $"Ela não será mostrada novamente.",
                "Conta Criada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        else if (_selectedProfessor != null)
        {
            _selectedProfessor.NomeCompleto = txtNome.Text.Trim();
            _selectedProfessor.Email        = txtEmail.Text.Trim();
            _selectedProfessor.Telefone     = txtTelefone.Text.Trim();
            _selectedProfessor.Ativo        = chkAtivo.Checked;
            _db.AtualizarProfessor(_selectedProfessor);

            LoadProfessores();
            SelectProfessorInGrid(_selectedProfessor.Id);
        }

        lblFormTitle.Text      = _selectedProfessor?.NomeCompleto ?? "";
        lblFormTitle.ForeColor = ThemeHelper.DarkText;
    }

    private void OnAddAtribuicao(object? sender, EventArgs e)
    {
        if (_selectedProfessor == null || _isNewMode)
        {
            MessageBox.Show("Guarde o professor primeiro.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cmbTurmaAtrib.SelectedItem is not Turma     turma ||
            cmbDiscAtrib.SelectedItem   is not Disciplina disc)
        {
            MessageBox.Show("Seleccione uma turma e uma disciplina.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // One professor per (turma, discipline) slot
        _db.RemoverAtribuicaoPorTurmaDisciplina(turma.Id, disc.Id);
        _db.CriarAtribuicao(new AtribuicaoDisciplina
        {
            TurmaId      = turma.Id,
            DisciplinaId = disc.Id,
            ProfessorId  = _selectedProfessor.Id
        });

        RefreshCountAndAssignments();
    }

    private void OnRemoveAtribuicao(object? sender, EventArgs e)
    {
        if (dgvAtribuicoes.SelectedRows.Count == 0) return;
        var atribId = Convert.ToInt32(dgvAtribuicoes.SelectedRows[0].Cells["AtribId"].Value);
        _db.RemoverAtribuicaoPorId(atribId);
        RefreshCountAndAssignments();
    }

    // ── Helpers ───────────────────────────────────────────────────────────
    private void RefreshCountAndAssignments()
    {
        int? profId = _selectedProfessor?.Id;
        LoadProfessores();
        if (profId.HasValue) SelectProfessorInGrid(profId.Value);
        LoadAtribuicoes();
    }

    private void SelectProfessorInGrid(int profId)
    {
        foreach (DataGridViewRow row in dgvProfessores.Rows)
        {
            if (Convert.ToInt32(row.Cells["ProfId"].Value) == profId)
            {
                row.Selected = true;
                return;
            }
        }
    }

    private void OnRedefinirSenha(object? sender, EventArgs e)
    {
        if (_selectedProfessor == null || _isNewMode)
        {
            MessageBox.Show("Seleccione um professor primeiro.",
                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Confirmar a ação
        var confirmar = MessageBox.Show(
            $"Deseja redefinir a senha de {_selectedProfessor.NomeCompleto}?\n\n" +
            $"Uma nova senha será gerada automaticamente.",
            "Confirmar Redefinição",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmar != DialogResult.Yes) return;

        // Buscar a conta do professor
        var conta = _db.ObterContaPorEmail(_selectedProfessor.Email);
        if (conta == null)
        {
            MessageBox.Show(
                "Este professor não tem conta de acesso criada.\n" +
                "Guarde o professor primeiro para criar a conta.",
                "Conta não encontrada",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Gerar nova senha
        string primeiroNome = _selectedProfessor.NomeCompleto.Split(' ')[0].ToLower();
        string novaSenha    = primeiroNome + new Random().Next(1000, 9999).ToString();

        // Atualizar na base de dados
        _db.AtualizarSenha(conta.Id, novaSenha);

        // Mostrar ao administrador
        MessageBox.Show(
            $"✓  Senha redefinida com sucesso!\n\n" +
            $"Professor:  {_selectedProfessor.NomeCompleto}\n" +
            $"Email:      {_selectedProfessor.Email}\n" +
            $"Nova Senha: {novaSenha}\n\n" +
            $"Entregue esta senha ao professor.\n" +
            $"Ela não será mostrada novamente.",
            "Senha Redefinida",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
