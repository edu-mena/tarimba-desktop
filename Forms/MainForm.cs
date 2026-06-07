using System.Drawing.Drawing2D;
using FontAwesome.Sharp;
using TarimbaPresence.Controls;
using TarimbaPresence.Helpers;
using TarimbaPresence.UserControls;
using TarimbaPresence.Models;

namespace TarimbaPresence.Forms;

public class MainForm : Form
{
    // ── Painéis estruturais ────────────────────────────────────────────────
    private Panel pnlSidebar    = null!;
    private Panel pnlMain       = null!;
    private Panel pnlHeader     = null!;
    private Panel pnlContent    = null!;

    // ── Sidebar ────────────────────────────────────────────────────────────
    private Panel pnlLogo = null!;

    private SidebarNavButton btnDashboard    = null!;
    private SidebarNavButton btnAlunos       = null!;
    private SidebarNavButton btnProfessores  = null!;
    private SidebarNavButton btnHorarios     = null!;
    private SidebarNavButton btnRelatorios   = null!;
    private SidebarNavButton btnConfig       = null!;

    private SidebarNavButton? _activeNavBtn;

    // ── Header ─────────────────────────────────────────────────────────────
    private Label lblGreeting   = null!;
    private Label lblDate       = null!;
    private Panel pnlAvatar     = null!;

    // ── UserControls (lazy) ────────────────────────────────────────────────
    private UC_Dashboard?         _ucDashboard;
    private UC_Alunos?            _ucAlunos;
    private UC_Professores?       _ucProfessores;
    private UC_Horarios?          _ucHorarios;
    private UC_Relatorios?        _ucRelatorios;
    private UC_Configuracoes?     _ucConfig;
    
    private UC_DashboardProfessor? _ucDashboardProf;
    private UserControl?          _currentUC;

    // ══════════════════════════════════════════════════════════════════════
    public MainForm()
    {
        BuildForm();
        BuildSidebar();
        BuildMainArea();
        AjustarMenuParaPerfil();

        // Se for professor, abre o dashboard do professor
        if (Program.UtilizadorAtual == "PROFESSOR")
            Navigate(btnHorarios);
        else
            Navigate(btnDashboard);
    }

    // ── Forma ──────────────────────────────────────────────────────────────
    private void BuildForm()
    {
        Text            = "Complexo Escolar Tarimba III — Sistema de Presenças";
        Size            = new Size(1380, 840);
        MinimumSize     = new Size(1100, 700);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = ThemeHelper.ContentBg;
        Font            = ThemeHelper.FontBody;
        // Custom window chrome (borderless + title bar)
        FormBorderStyle = FormBorderStyle.Sizable;
    }

    private void AjustarMenuParaPerfil()
    {
        bool isAdmin = Program.UtilizadorAtual == "ADMIN";

        // Menus que só o admin vê
        btnAlunos.Visible      = isAdmin;
        btnProfessores.Visible = isAdmin;
        btnConfig.Visible      = isAdmin;
    }
    // ══════════════════════════════════════════════════════════════════════
    // SIDEBAR
    // ══════════════════════════════════════════════════════════════════════
    private void BuildSidebar()
    {
        pnlSidebar = new Panel
        {
            Width     = 248,
            Dock      = DockStyle.Left,
            BackColor = ThemeHelper.SidebarBg,
        };

        // Logo
        pnlLogo = new Panel
        {
            Height    = 76,
            Dock      = DockStyle.Top,
            BackColor = ThemeHelper.SidebarHeaderBg,
            Padding   = new Padding(20, 0, 12, 0)
        };

        var pnlLogoIcon = new Panel
        {
            Width     = 36,
            Height    = 36,
            BackColor = ThemeHelper.LightBlue,
            Left      = 20,
            Top       = 20,
        };
        var iconPicLogo = new IconPictureBox
        {
            IconChar  = IconChar.School,
            IconColor = Color.White,
            IconSize  = 20,
            BackColor = ThemeHelper.LightBlue,
            Width     = 36,
            Height    = 36,
            Left      = 0,
            Top       = 0,
            Padding   = new Padding(0)
        };
        pnlLogoIcon.Controls.Add(iconPicLogo);

        var lblSchool = new Label
        {
            Text      = "Tarimba III",
            Font      = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize  = false,
            Width     = 150,
            Height    = 18,
            Left      = 64,
            Top        = 18,
            BackColor = Color.Transparent
        };
        var lblSchoolSub = new Label
        {
            Text      = "Gestão de Presenças",
            Font      = ThemeHelper.FontCaption,
            ForeColor = ThemeHelper.SubtleText,
            AutoSize  = false,
            Width     = 150,
            Height    = 15,
            Left      = 64,
            Top        = 40,
            BackColor = Color.Transparent
        };

        pnlLogo.Controls.AddRange(new Control[] { pnlLogoIcon, lblSchool, lblSchoolSub });

        // ── Navigation section label ───────────────────────────────────────
        var lblNav = new Label
        {
            Text      = "MENU PRINCIPAL",
            Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(71, 85, 105),
            AutoSize  = false,
            Dock      = DockStyle.Top,
            Height    = 36,
            TextAlign = ContentAlignment.BottomLeft,
            Padding   = new Padding(20, 0, 0, 4),
            BackColor = Color.Transparent
        };

        // ── Nav buttons ────────────────────────────────────────────────────
        btnDashboard   = MakeNavBtn(IconChar.House,         "Início");
        btnAlunos      = MakeNavBtn(IconChar.UserGraduate,  "Alunos");
        btnProfessores = MakeNavBtn(IconChar.UserTie,       "Professores");
        btnHorarios    = MakeNavBtn(IconChar.CalendarWeek,  "Horários");
        btnRelatorios  = MakeNavBtn(IconChar.ChartSimple,   "Relatórios");
        btnConfig      = MakeNavBtn(IconChar.Gear,          "Configurações");

        // Spacer
        var spacer = new Panel { Height = 8, Dock = DockStyle.Top, BackColor = Color.Transparent };

        // Version at bottom
        var pnlBottom = new Panel
        {
            Height    = 50,
            Dock      = DockStyle.Bottom,
            BackColor = ThemeHelper.SidebarHeaderBg,
            Padding   = new Padding(20, 14, 0, 0)
        };
        var lblVer = new Label
        {
            Text      = "v1.0.0  •  2026",
            Font      = ThemeHelper.FontCaption,
            ForeColor = Color.FromArgb(71, 85, 105),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        pnlBottom.Controls.Add(lblVer);

        pnlSidebar.Controls.AddRange(new Control[]
        {
            pnlBottom,
            btnConfig,
            btnRelatorios,
            btnHorarios,
            btnProfessores,
            btnAlunos,
            btnDashboard,
            spacer,
            lblNav,
            pnlLogo
        });

        // NÃO adicionar ao form aqui — será adicionado ao TableLayoutPanel em BuildMainArea()
    }

    private SidebarNavButton MakeNavBtn(IconChar icon, string text)
    {
        var btn = new SidebarNavButton { IconChar = icon, Text = text };
        btn.NavClick += SidebarBtn_NavClick;
        return btn;
    }

    private void SidebarBtn_NavClick(object? sender, EventArgs e)
    {
        if (sender is SidebarNavButton btn)
            Navigate(btn);
    }

    private void Navigate(SidebarNavButton target)
    {
        if (_activeNavBtn != null) _activeNavBtn.IsActive = false;
        target.IsActive = true;
        _activeNavBtn   = target;

        UserControl uc;
        string pageTitle;

        if (target == btnDashboard)
        {
            _ucDashboard ??= new UC_Dashboard();
            uc = _ucDashboard;
            pageTitle = "Início";
        }
        else if (target == btnAlunos)
        {
            _ucAlunos ??= new UC_Alunos();
            uc = _ucAlunos;
            pageTitle = "Gestão de Alunos";
        }
        else if (target == btnProfessores)
        {
            _ucProfessores ??= new UC_Professores();
            uc = _ucProfessores;
            pageTitle = "Professores e Disciplinas";
        }
        else if (target == btnHorarios)
        {
            _ucHorarios ??= new UC_Horarios();
            uc = _ucHorarios;
            pageTitle = "Horários";
        }
        else if (target == btnRelatorios)
        {
            _ucRelatorios ??= new UC_Relatorios();
            uc = _ucRelatorios;
            pageTitle = "Relatórios";
        }
        else
        {
            _ucConfig ??= new UC_Configuracoes();
            uc = _ucConfig;
            pageTitle = "Configurações";
        }

        SwapContent(uc, pageTitle);
    }

    private void SwapContent(UserControl uc, string title)
    {
        pnlContent.SuspendLayout();

        if (_currentUC != null)
        {
            _currentUC.Visible = false;
            pnlContent.Controls.Remove(_currentUC);
        }

        uc.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(uc);
        uc.Visible = true;
        uc.BringToFront();
        _currentUC = uc;

        // Update header breadcrumb
        if (pnlHeader.Controls.Find("lblPageTitle", true).FirstOrDefault() is Label lbl)
            lbl.Text = title;

        pnlContent.ResumeLayout(true);
    }

    // ══════════════════════════════════════════════════════════════════════
    // MAIN AREA (header + content)
    // ══════════════════════════════════════════════════════════════════════
    private void BuildMainArea()
    {
        pnlMain = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = ThemeHelper.ContentBg,
            Padding   = new Padding(0)
        };

        BuildHeader();
        BuildContent();

        // Fill processado ÚLTIMO (adicionar primeiro), Top processado PRIMEIRO (adicionar depois)
        pnlMain.Controls.Add(pnlContent);  // DockStyle.Fill  → Controls[0] → processado por último
        pnlMain.Controls.Add(pnlHeader);   // DockStyle.Top   → Controls[1] → processado primeiro

        // ── TableLayoutPanel garante separação sidebar/conteúdo sem sobreposição ──
        // DockStyle.Left + DockStyle.Fill dependem da ordem de adição e causam overlap.
        // TableLayoutPanel é determinístico: coluna 0 = sidebar, coluna 1 = conteúdo.
        var tlp = new TableLayoutPanel
        {
            Dock            = DockStyle.Fill,
            ColumnCount     = 2,
            RowCount        = 1,
            Padding         = Padding.Empty,
            Margin          = Padding.Empty,
            CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
            BackColor       = Color.Transparent
        };
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 248f));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,  100f));
        tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        // pnlSidebar foi construído em BuildSidebar() mas NÃO adicionado ao form.
        // Agora entra na célula (0,0); pnlMain fica na célula (1,0).
        pnlSidebar.Dock = DockStyle.Fill;   // preenche a célula da tabela
        pnlMain.Dock    = DockStyle.Fill;

        tlp.Controls.Add(pnlSidebar, 0, 0);
        tlp.Controls.Add(pnlMain,    1, 0);

        Controls.Add(tlp);
    }

    private void BuildHeader()
    {
        pnlHeader = new Panel
        {
            Height    = 68,
            Dock      = DockStyle.Top,
            BackColor = Color.White,
            Padding   = new Padding(28, 0, 24, 0)
        };

        // Bottom border of header
        pnlHeader.Paint += (_, e) =>
        {
            using var pen = new Pen(ThemeHelper.BorderColor, 1f);
            e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1,
                                pnlHeader.Width, pnlHeader.Height - 1);
        };

        // Page title (breadcrumb)
        var lblPageTitle = new Label
        {
            Name      = "lblPageTitle",
            Text      = "Início",
            Font      = ThemeHelper.FontHeading,
            ForeColor = ThemeHelper.DarkText,
            AutoSize  = true,
            Left      = 28,
            BackColor = Color.Transparent
        };
        lblPageTitle.Top = (68 - lblPageTitle.PreferredHeight) / 2;

        // Date
        lblDate = new Label
        {
            Text      = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy"),
            Font      = ThemeHelper.FontSmall,
            ForeColor = ThemeHelper.MediumText,
            AutoSize  = true,
            BackColor = Color.Transparent
        };

        // Greeting
        lblGreeting = new Label
        {
            Text      = GetGreeting(),
            Font      = ThemeHelper.FontSmall,
            ForeColor = ThemeHelper.MediumText,
            AutoSize  = true,
            BackColor = Color.Transparent
        };

        // Avatar panel
        pnlAvatar = new Panel
        {
            Width     = 38,
            Height    = 38,
            BackColor = ThemeHelper.LightBlue,
            Cursor    = Cursors.Hand
        };
        pnlAvatar.Paint += DrawAvatar;

        // Right side: greeting + date stacked
        pnlHeader.SizeChanged += (_, _) =>
        {
            int rightX = pnlHeader.Width - pnlAvatar.Width - 28;
            pnlAvatar.Left   = rightX;
            pnlAvatar.Top    = (68 - pnlAvatar.Height) / 2;

            lblGreeting.Left = rightX - lblGreeting.Width - 12;
            lblGreeting.Top  = 16;
            lblDate.Left     = rightX - Math.Max(lblDate.Width, lblGreeting.Width) - 12;
            lblDate.Top      = 36;
        };

        pnlHeader.Controls.AddRange(new Control[]
        {
            lblPageTitle, lblDate, lblGreeting, pnlAvatar
        });
    }

    private void DrawAvatar(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path  = UIHelper.RoundedRect(new Rectangle(0, 0, 37, 37), 19);
        using var brush = new SolidBrush(ThemeHelper.LightBlue);
        g.FillPath(brush, path);
        using var f  = new Font("Segoe UI", 14f, FontStyle.Bold);
        using var fb = new SolidBrush(Color.White);
        using var sf = new StringFormat
        {
            Alignment     = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        g.DrawString("A", f, fb, new RectangleF(0, 0, 38, 38), sf);
    }

    private void BuildContent()
    {
        pnlContent = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = ThemeHelper.ContentBg,
            Padding   = new Padding(0)
        };
    }

    private static string GetGreeting()
    {
        int h    = DateTime.Now.Hour;
        string nome;

        if (Program.UtilizadorAtual == "PROFESSOR" && Program.ContaProfessorAtual != null)
        {
            // Buscar o nome do professor pelo Id
            var prof = TarimbaPresence.Data.MockDataStore.Professores
                .FirstOrDefault(p => p.Id == Program.ContaProfessorAtual.ProfessorId);
            nome = prof?.NomeCompleto.Split(' ')[0] ?? "Professor";
        }
        else
        {
            nome = "Admin";
        }

        return h < 12 ? $"Bom dia, {nome}"
            : h < 18 ? $"Boa tarde, {nome}"
                    : $"Boa noite, {nome}";
    }
}
