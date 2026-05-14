using System.Drawing.Drawing2D;
using FontAwesome.Sharp;
using TarimbaPresence.Controls;
using TarimbaPresence.Data;
using TarimbaPresence.Helpers;
using TarimbaPresence.Models;

namespace TarimbaPresence.UserControls;

public class UC_Alunos : UserControl
{
    // ── Form fields ────────────────────────────────────────────────────────
    private TextBox     txtNome      = null!;
    private TextBox     txtProcesso  = null!;
    private ComboBox    cmbSexo      = null!;
    private DateTimePicker dtpNasc   = null!;
    private ComboBox    cmbTurma     = null!;
    private ComboBox    cmbClasse    = null!;
    private DataGridView dgv         = null!;
    private Panel       pnlAvatar    = null!;
    private Label       lblAvatarInitials = null!;
    private TextBox     txtSearchList = null!;

    private Aluno? _editingAluno;

    public UC_Alunos()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor  = ThemeHelper.ContentBg;
        Padding    = new Padding(32, 28, 32, 28);
        Build();
    }

    private void Build()
    {
        SuspendLayout();

        // ── Header ─────────────────────────────────────────────────────────
        var pnlHeader = new Panel
        {
            Height    = 56,
            Dock      = DockStyle.Top,
            BackColor = Color.Transparent
        };
        var lblTitle = new Label
        {
            Text      = "Gestão de Alunos",
            Font      = ThemeHelper.FontTitle,
            ForeColor = ThemeHelper.DarkText,
            AutoSize  = true,
            Left      = 0, Top = 4, BackColor = Color.Transparent
        };
        var lblSub = new Label
        {
            Text      = "Cadastre, edite e consulte os alunos matriculados",
            Font      = ThemeHelper.FontSmall,
            ForeColor = ThemeHelper.MediumText,
            AutoSize  = true,
            Left      = 2, Top = 34, BackColor = Color.Transparent
        };
        pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSub });

        // ── Main split (form left, table right) ───────────────────────────
        var pnlSplit = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent
        };

        var pnlForm  = BuildFormCard();
        var pnlTable = BuildTableCard();

        pnlForm.Dock  = DockStyle.Left;
        pnlForm.Width = 360;
        pnlTable.Dock = DockStyle.Fill;

        pnlSplit.Controls.Add(pnlTable);
        pnlSplit.Controls.Add(pnlForm);

        Controls.AddRange(new Control[] { pnlSplit, pnlHeader });
        ResumeLayout(true);
    }

    // ── Form card (left panel) ─────────────────────────────────────────────
    private Panel BuildFormCard()
    {
        var outer = new Panel
        {
            BackColor = Color.Transparent,
            Padding   = new Padding(0, 0, 16, 8)
        };

        var card = new RoundedPanel
        {
            Dock         = DockStyle.Fill,
            BackColor    = Color.White,
            ShowShadow   = true,
            CornerRadius = 12,
            ShadowDepth  = 4,
            Padding      = new Padding(24, 24, 24, 16)
        };

        // Avatar
        pnlAvatar = new Panel
        {
            Width     = 80,
            Height    = 80,
            Left      = (card.Width - 80) / 2,
            Top       = 20,
            BackColor = ThemeHelper.LightBlue
        };
        pnlAvatar.Anchor = AnchorStyles.Top;
        pnlAvatar.Paint += DrawFormAvatar;

        lblAvatarInitials = new Label
        {
            Text      = "?",
            Font      = new Font("Segoe UI", 26f, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize  = false,
            Width     = 80, Height = 80,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent
        };

        pnlAvatar.Controls.Add(lblAvatarInitials);

        var lblFormTitle = UIHelper.MakeLabel("Dados do Aluno", ThemeHelper.FontSubheading, ThemeHelper.DarkText);
        lblFormTitle.AutoSize = false;
        lblFormTitle.Dock     = DockStyle.Top;
        lblFormTitle.Height   = 40;
        lblFormTitle.Padding  = new Padding(0, 8, 0, 0);

        // Fields
        var fields = new Panel { Dock = DockStyle.Top, Height = 360, BackColor = Color.Transparent };

        txtProcesso = MakeField(fields, "Nº Processo", 0);
        txtNome     = MakeField(fields, "Nome Completo", 68);
        cmbSexo     = MakeComboField(fields, "Sexo", 136, new[] { "Masculino", "Feminino" });
        dtpNasc     = MakeDateField(fields, "Data de Nascimento", 204);
        cmbTurma    = MakeComboField(fields, "Turma", 272, Array.Empty<string>());
        foreach (var t in MockDataStore.Turmas) cmbTurma.Items.Add(t);
        if (cmbTurma.Items.Count > 0) cmbTurma.SelectedIndex = 0;

        cmbClasse   = MakeComboField(fields, "Classe", 340,
                          new[] { "1ª Classe","2ª Classe","3ª Classe","4ª Classe","5ª Classe","6ª Classe",
                                  "7ª Classe","8ª Classe","9ª Classe","10ª Classe","11ª Classe","12ª Classe","13ª Classe" });

        // Auto-fill class when turma is selected
        cmbTurma.SelectedIndexChanged += (_, _) =>
        {
            if (cmbTurma.SelectedItem is Turma st)
            {
                int ci = cmbClasse.Items.IndexOf(st.NomeClasse);
                if (ci >= 0) cmbClasse.SelectedIndex = ci;
            }
        };

        txtNome.TextChanged += (_, _) => UpdateAvatarInitials();

        // Buttons
        var pnlBtns = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 52,
            BackColor = Color.Transparent
        };

        var btnNovo   = UIHelper.MakeSecondaryButton("Novo",   100, 38);
        var btnSalvar = UIHelper.MakePrimaryButton  ("Salvar", 100, 38);
        var btnLimpar = UIHelper.MakeSecondaryButton("Limpar",  90, 38);

        btnNovo.Left   = 0;   btnNovo.Top   = 8;
        btnSalvar.Left = 106; btnSalvar.Top = 8;
        btnLimpar.Left = 212; btnLimpar.Top = 8;

        btnNovo.Click   += (_, _) => ClearForm();
        btnSalvar.Click += (_, _) => SaveAluno();
        btnLimpar.Click += (_, _) => ClearForm();

        pnlBtns.Controls.AddRange(new Control[] { btnNovo, btnSalvar, btnLimpar });

        card.Controls.AddRange(new Control[]
        {
            pnlBtns,
            fields,
            lblFormTitle,
            pnlAvatar
        });

        outer.Controls.Add(card);
        return outer;
    }

    private TextBox MakeField(Panel parent, string label, int top)
    {
        var lbl = UIHelper.MakeLabel(label, ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lbl.Left = 0; lbl.Top = top;

        var txt = UIHelper.MakeTextBox(290);
        txt.Left = 0; txt.Top = top + 18;
        txt.Height = 36;
        parent.Controls.AddRange(new Control[] { lbl, txt });
        return txt;
    }

    private ComboBox MakeComboField(Panel parent, string label, int top, string[] items)
    {
        var lbl = UIHelper.MakeLabel(label, ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lbl.Left = 0; lbl.Top = top;

        var cmb = UIHelper.MakeComboBox(290);
        cmb.Left = 0; cmb.Top = top + 18;
        cmb.Height = 36;
        foreach (var i in items) cmb.Items.Add(i);
        if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;

        parent.Controls.AddRange(new Control[] { lbl, cmb });
        return cmb;
    }

    private DateTimePicker MakeDateField(Panel parent, string label, int top)
    {
        var lbl = UIHelper.MakeLabel(label, ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lbl.Left = 0; lbl.Top = top;

        var dtp = new DateTimePicker
        {
            Format  = DateTimePickerFormat.Short,
            Font    = ThemeHelper.FontBody,
            Width   = 290,
            Left    = 0,
            Top     = top + 18,
            Height  = 36,
            Value   = DateTime.Today.AddYears(-10)
        };
        parent.Controls.AddRange(new Control[] { lbl, dtp });
        return dtp;
    }

    private void DrawFormAvatar(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path  = UIHelper.RoundedRect(new Rectangle(0, 0, 79, 79), 40);
        using var brush = new SolidBrush(ThemeHelper.LightBlue);
        g.FillPath(brush, path);
    }

    private void UpdateAvatarInitials()
    {
        var parts = txtNome.Text.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        lblAvatarInitials.Text = parts.Length >= 2
            ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
            : parts.Length == 1 && parts[0].Length > 0
                ? parts[0][0].ToString().ToUpper()
                : "?";
    }

    // ── Table card (right panel) ───────────────────────────────────────────
    private Panel BuildTableCard()
    {
        var outer = new Panel
        {
            BackColor = Color.Transparent,
            Padding   = new Padding(0, 0, 0, 8)
        };

        var card = new RoundedPanel
        {
            Dock         = DockStyle.Fill,
            BackColor    = Color.White,
            ShowShadow   = true,
            CornerRadius = 12,
            ShadowDepth  = 4,
            Padding      = new Padding(20, 16, 20, 16)
        };

        // Search bar inside card
        var pnlSearch = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 52,
            BackColor = Color.Transparent
        };
        var lblTotal = UIHelper.MakeLabel("Lista de Alunos", ThemeHelper.FontSubheading, ThemeHelper.DarkText);
        lblTotal.Left = 0; lblTotal.Top = 14;

        txtSearchList = UIHelper.MakeTextBox(220);
        txtSearchList.PlaceholderText = "Pesquisar aluno...";
        txtSearchList.Top  = 10;
        txtSearchList.Height = 34;
        txtSearchList.TextChanged += (_, _) => FilterList();
        pnlSearch.SizeChanged += (_, _) =>
        {
            txtSearchList.Left = pnlSearch.Width - txtSearchList.Width;
        };
        pnlSearch.Controls.AddRange(new Control[] { lblTotal, txtSearchList });

        // Grid
        dgv = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGridView(dgv);

        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Processo", HeaderText = "Nº Processo", Width = 120 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Nome",     HeaderText = "Nome Completo", FillWeight = 50 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Sexo",     HeaderText = "Sexo", Width = 64 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Turma",    HeaderText = "Turma", Width = 80 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Classe",   HeaderText = "Classe", Width = 100 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Idade",    HeaderText = "Idade", Width = 64 });

        PopulateGrid(MockDataStore.Alunos);

        dgv.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0) EditFromGrid(e.RowIndex);
        };

        card.Controls.AddRange(new Control[] { dgv, pnlSearch });
        outer.Controls.Add(card);
        return outer;
    }

    private void PopulateGrid(IEnumerable<Aluno> source)
    {
        dgv.Rows.Clear();
        foreach (var a in source)
        {
            var turma = MockDataStore.Turmas.FirstOrDefault(t => t.Id == a.TurmaId);
            dgv.Rows.Add(a.NumeroProcesso, a.NomeCompleto, a.Sexo,
                         turma?.Nome ?? "—", a.Classe, a.Idade.ToString());
        }
    }

    private void FilterList()
    {
        var q = txtSearchList.Text.Trim().ToLower();
        if (string.IsNullOrEmpty(q))
        {
            PopulateGrid(MockDataStore.Alunos);
            return;
        }
        PopulateGrid(MockDataStore.Alunos.Where(a =>
            a.NomeCompleto.ToLower().Contains(q) ||
            a.NumeroProcesso.ToLower().Contains(q) ||
            a.Classe.ToLower().Contains(q)));
    }

    private void EditFromGrid(int rowIndex)
    {
        var processo = dgv.Rows[rowIndex].Cells["Processo"].Value?.ToString();
        var aluno    = MockDataStore.Alunos.FirstOrDefault(a => a.NumeroProcesso == processo);
        if (aluno == null) return;

        _editingAluno     = aluno;
        txtProcesso.Text  = aluno.NumeroProcesso;
        txtNome.Text      = aluno.NomeCompleto;
        cmbSexo.SelectedIndex = aluno.Sexo == "M" ? 0 : 1;
        dtpNasc.Value     = aluno.DataNascimento;

        cmbTurma.SelectedIndex = -1;
        for (int i = 0; i < cmbTurma.Items.Count; i++)
            if (cmbTurma.Items[i] is Turma ti && ti.Id == aluno.TurmaId) { cmbTurma.SelectedIndex = i; break; }

        int classeIdx = cmbClasse.Items.IndexOf(aluno.Classe);
        cmbClasse.SelectedIndex = classeIdx >= 0 ? classeIdx : 0;
        UpdateAvatarInitials();
    }

    private void SaveAluno()
    {
        if (string.IsNullOrWhiteSpace(txtNome.Text))
        {
            MessageBox.Show("O nome do aluno é obrigatório.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var turma = cmbTurma.SelectedItem as Turma;

        if (_editingAluno != null)
        {
            // Update existing
            _editingAluno.NomeCompleto    = txtNome.Text.Trim();
            _editingAluno.NumeroProcesso  = txtProcesso.Text.Trim();
            _editingAluno.Sexo            = cmbSexo.Text.StartsWith("M") ? "M" : "F";
            _editingAluno.DataNascimento  = dtpNasc.Value;
            _editingAluno.TurmaId         = turma?.Id ?? "";
            _editingAluno.Classe          = cmbClasse.Text;
        }
        else
        {
            // New aluno
            var novo = new Aluno
            {
                Id              = MockDataStore.Alunos.Count == 0 ? 1 : MockDataStore.Alunos.Max(a => a.Id) + 1,
                NumeroProcesso  = txtProcesso.Text.Trim(),
                NomeCompleto    = txtNome.Text.Trim(),
                Sexo            = cmbSexo.Text.StartsWith("M") ? "M" : "F",
                DataNascimento  = dtpNasc.Value,
                TurmaId         = turma?.Id ?? "",
                Classe          = cmbClasse.Text,
                Ativo           = true
            };
            MockDataStore.Alunos.Add(novo);
        }

        PopulateGrid(MockDataStore.Alunos);
        ClearForm();

        MessageBox.Show("Aluno guardado com sucesso.", "Sucesso",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ClearForm()
    {
        _editingAluno      = null;
        txtProcesso.Text   = string.Empty;
        txtNome.Text       = string.Empty;
        if (cmbSexo.Items.Count > 0)     cmbSexo.SelectedIndex  = 0;
        if (cmbTurma.Items.Count > 0)    cmbTurma.SelectedIndex  = 0;
        if (cmbClasse.Items.Count > 0)   cmbClasse.SelectedIndex = 0;
        dtpNasc.Value      = DateTime.Today.AddYears(-10);
        lblAvatarInitials.Text = "?";
    }
}
