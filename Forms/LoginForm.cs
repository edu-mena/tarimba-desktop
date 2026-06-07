using System.Drawing.Drawing2D;
using FontAwesome.Sharp;
using TarimbaPresence.Helpers;
using TarimbaPresence.Services;
using TarimbaPresence.Models;

namespace TarimbaPresence.Forms;

public class LoginForm : Form
{
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private Button  btnLogin    = null!;
    private Label   lblError    = null!;
    private Panel   pnlPwdWrap  = null!;
    private Panel   pnlUserWrap = null!;
    private bool    _pwdVisible = false;

    // Credenciais mock
    private const string MOCK_USER = "admin";
    private const string MOCK_PASS = "1234";

    public LoginForm()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BuildForm();
        BuildUI();
    }

    // ── Forma ─────────────────────────────────────────────────────────────
    private void BuildForm()
    {
        Text            = "Complexo Escolar Tarimba III — Acesso";
        Size            = new Size(960, 580);
        MinimumSize     = new Size(860, 520);
        MaximumSize     = new Size(1200, 700);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition   = FormStartPosition.CenterScreen;
        MaximizeBox     = false;
        BackColor       = ThemeHelper.NavyBlue;
        Font            = ThemeHelper.FontBody;
        AcceptButton    = null; // will assign after build
    }

    // ── Layout principal ──────────────────────────────────────────────────
    private void BuildUI()
    {
        SuspendLayout();

        // ── Painel esquerdo (branding) ─────────────────────────────────────
        var pnlLeft = new Panel
        {
            Width     = 420,
            Dock      = DockStyle.Left,
            BackColor = ThemeHelper.NavyBlue
        };
        pnlLeft.Paint += DrawLeftPanel;
        BuildLeftContent(pnlLeft);

        // ── Painel direito (formulário) ────────────────────────────────────
        var pnlRight = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = ThemeHelper.LightGray
        };
        BuildRightContent(pnlRight);

        // ── Linha divisória ────────────────────────────────────────────────
        var divider = new Panel
        {
            Width     = 1,
            Dock      = DockStyle.Left,
            BackColor = Color.FromArgb(30, 255, 255, 255)
        };

        Controls.Add(pnlRight);
        Controls.Add(divider);
        Controls.Add(pnlLeft);

        AcceptButton = btnLogin;
        ResumeLayout(true);

        ActiveControl = txtUsername;
    }

    // ── Gradient background do painel esquerdo ────────────────────────────
    private void DrawLeftPanel(object? sender, PaintEventArgs e)
    {
        var g   = e.Graphics;
        var pnl = (Panel)sender!;
        using var brush = new LinearGradientBrush(
            new Rectangle(0, 0, pnl.Width, pnl.Height),
            Color.FromArgb(9,  14,  27),
            Color.FromArgb(30, 58, 138),
            LinearGradientMode.ForwardDiagonal);
        g.FillRectangle(brush, 0, 0, pnl.Width, pnl.Height);

        // Subtle grid dots
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var dotBrush = new SolidBrush(Color.FromArgb(12, 255, 255, 255));
        for (int x = 20; x < pnl.Width; x += 40)
            for (int y = 20; y < pnl.Height; y += 40)
                g.FillEllipse(dotBrush, x - 1, y - 1, 2, 2);
    }

    // ── Conteúdo do painel esquerdo ───────────────────────────────────────
    private void BuildLeftContent(Panel parent)
    {
        // Wrapper centrado verticalmente
        var pnlCenter = new Panel
        {
            BackColor = Color.Transparent,
            Width     = 360,
            Height    = 400,
        };
        pnlCenter.Paint += (_, _) => { };

        // Posicionar centrado
        parent.SizeChanged += (_, _) =>
        {
            pnlCenter.Left = (parent.Width  - pnlCenter.Width)  / 2;
            pnlCenter.Top  = (parent.Height - pnlCenter.Height) / 2;
        };

        // Icon badge
        var pnlIconBadge = new Panel
        {
            Width     = 72,
            Height    = 72,
            Left      = 0,
            Top       = 0,
            BackColor = Color.Transparent
        };
        pnlIconBadge.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(0, 0, 71, 71);
            using var path  = UIHelper.RoundedRect(r, 20);
            using var brush = new SolidBrush(ThemeHelper.LightBlue);
            g.FillPath(brush, path);
            // Inner shine
            using var shine = new LinearGradientBrush(r,
                Color.FromArgb(80, 255, 255, 255), Color.Transparent,
                LinearGradientMode.ForwardDiagonal);
            g.FillPath(shine, path);
        };

        var iconPic = new IconPictureBox
        {
            IconChar  = IconChar.SchoolFlag,
            IconColor = Color.White,
            IconSize  = 36,
            BackColor = Color.Transparent,
            Width     = 72,
            Height    = 72
        };
        pnlIconBadge.Controls.Add(iconPic);

        // School name
        var lblSchool = new Label
        {
            Text      = "Complexo Escolar",
            Font      = new Font("Segoe UI", 13f, FontStyle.Regular),
            ForeColor = Color.FromArgb(148, 163, 184),
            AutoSize  = true,
            Left      = 0,
            Top       = 88,
            BackColor = Color.Transparent
        };
        var lblSchoolBold = new Label
        {
            Text      = "Tarimba III",
            Font      = new Font("Segoe UI", 28f, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize  = true,
            Left      = 0,
            Top       = 108,
            BackColor = Color.Transparent
        };

        // Separator line
        var pnlSep = new Panel
        {
            Left      = 0,
            Top       = 158,
            Width     = 48,
            Height    = 3,
            BackColor = ThemeHelper.LightBlue
        };

        // Tagline
        var lblTagline = new Label
        {
            Text      = "Sistema de Gestão de Presenças",
            Font      = new Font("Segoe UI", 11f, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 116, 139),
            AutoSize  = true,
            Left      = 0,
            Top       = 174,
            BackColor = Color.Transparent
        };

        // Feature pills
        var features = new[]
        {
            (IconChar.CheckCircle, "Registo de presenças em tempo real"),
            (IconChar.CheckCircle, "Relatórios por turma e classe"),
            (IconChar.CheckCircle, "Gestão completa de alunos"),
        };

        int featTop = 220;
        foreach (var (ic, txt) in features)
        {
            var pnlFeat = new Panel
            {
                Left      = 0,
                Top       = featTop,
                Width     = 360,
                Height    = 26,
                BackColor = Color.Transparent
            };
            var featIcon = new IconPictureBox
            {
                IconChar  = ic,
                IconColor = ThemeHelper.Success,
                IconSize  = 14,
                BackColor = Color.Transparent,
                Width     = 18,
                Height    = 18,
                Left      = 0,
                Top       = 4
            };
            var featLbl = new Label
            {
                Text      = txt,
                Font      = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize  = true,
                Left      = 24,
                Top       = 4,
                BackColor = Color.Transparent
            };
            pnlFeat.Controls.AddRange(new Control[] { featIcon, featLbl });
            pnlCenter.Controls.Add(pnlFeat);
            featTop += 30;
        }

        // Year badge
        var lblYear = new Label
        {
            Text      = "© 2026 — Complexo Escolar Tarimba III",
            Font      = new Font("Segoe UI", 8.5f),
            ForeColor = Color.FromArgb(51, 65, 85),
            AutoSize  = true,
            Left      = 0,
            Top       = 360,
            BackColor = Color.Transparent
        };

        pnlCenter.Controls.AddRange(new Control[]
        {
            pnlIconBadge, lblSchool, lblSchoolBold, pnlSep,
            lblTagline, lblYear
        });

        parent.Controls.Add(pnlCenter);

        // Trigger initial position
        parent.Width = 420;
        pnlCenter.Left = (420 - pnlCenter.Width)  / 2;
        pnlCenter.Top  = (580 - pnlCenter.Height) / 2;
    }

    // ── Conteúdo do painel direito (formulário) ───────────────────────────
    private void BuildRightContent(Panel parent)
    {
        // Card de login centrado
        var card = new Panel
        {
            Width     = 380,
            Height    = 440,
            BackColor = Color.White
        };
        card.Paint += DrawLoginCard;

        // Título do card
        var lblWelcome = new Label
        {
            Text      = "Bem-vindo de volta",
            Font      = new Font("Segoe UI", 20f, FontStyle.Bold),
            ForeColor = ThemeHelper.DarkText,
            AutoSize  = true,
            Left      = 36,
            Top       = 40,
            BackColor = Color.Transparent
        };
        var lblSub = new Label
        {
            Text      = "Introduza as suas credenciais para continuar",
            Font      = ThemeHelper.FontSmall,
            ForeColor = ThemeHelper.MediumText,
            AutoSize  = true,
            Left      = 36,
            Top       = 72,
            BackColor = Color.Transparent
        };

        // ── Campo utilizador ───────────────────────────────────────────────
        var lblUser = MakeFieldLabel("Utilizador", 36, 106);
        pnlUserWrap = MakeInputWrapper(36, 126, out txtUsername);
        txtUsername.PlaceholderText = "ex: admin ou  professor.tarimba3@gmail.com";
        txtUsername.Text = MOCK_USER;

        // ── Campo password ─────────────────────────────────────────────────
        var lblPass = MakeFieldLabel("Palavra-passe", 36, 196);
        pnlPwdWrap  = MakeInputWrapper(36, 216, out txtPassword, isPassword: true);
        txtPassword.PlaceholderText = "••••••••";
        txtPassword.Text = MOCK_PASS;

        // Olho (mostrar/ocultar senha)
        var btnEye = new IconButton
        {
            IconChar  = IconChar.Eye,
            IconColor = ThemeHelper.MediumText,
            IconSize  = 16,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            Width     = 36,
            Height    = 46,
            Left      = 308,
            Top       = 216,
            Cursor    = Cursors.Hand,
            TabStop   = false
        };
        btnEye.FlatAppearance.BorderSize = 0;
        btnEye.Click += (_, _) =>
        {
            _pwdVisible = !_pwdVisible;
            txtPassword.UseSystemPasswordChar = !_pwdVisible;
            btnEye.IconChar = _pwdVisible ? IconChar.EyeSlash : IconChar.Eye;
        };
        txtPassword.UseSystemPasswordChar = true;

        // ── Erro label ─────────────────────────────────────────────────────
        lblError = new Label
        {
            Text      = "",
            Font      = new Font("Segoe UI", 9.5f),
            ForeColor = ThemeHelper.Danger,
            AutoSize  = true,
            Left      = 36,
            Top       = 276,
            BackColor = Color.Transparent,
            Visible   = false
        };

        // ── Botão Entrar ───────────────────────────────────────────────────
        btnLogin = new Button
        {
            Text      = "Entrar  →",
            Font      = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = ThemeHelper.LightBlue,
            FlatStyle = FlatStyle.Flat,
            Width     = 308,
            Height    = 48,
            Left      = 36,
            Top       = 300,
            Cursor    = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleCenter
        };
        btnLogin.FlatAppearance.BorderSize         = 0;
        btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(37, 99, 235);
        btnLogin.FlatAppearance.MouseDownBackColor = Color.FromArgb(29, 78, 216);
        btnLogin.Click += BtnLogin_Click;

        // ── Dica de acesso ─────────────────────────────────────────────────
        var pnlHint = new Panel
        {
            Left      = 36,
            Top       = 366,
            Width     = 308,
            Height    = 48,
            BackColor = Color.FromArgb(241, 245, 249)
        };
        pnlHint.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var p = UIHelper.RoundedRect(new Rectangle(0, 0, pnlHint.Width - 1, pnlHint.Height - 1), 8);
            using var b = new SolidBrush(Color.FromArgb(241, 245, 249));
            g.FillPath(b, p);
            using var pen = new Pen(ThemeHelper.BorderColor);
            g.DrawPath(pen, p);
        };
        var lblHint = new Label
        {
            Text      = "Acesso demo:  utilizador  admin  •  senha  1234",
            Font      = new Font("Segoe UI", 9f),
            ForeColor = ThemeHelper.MediumText,
            AutoSize  = false,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent
        };
        pnlHint.Controls.Add(lblHint);

        card.Controls.AddRange(new Control[]
        {
            lblWelcome, lblSub,
            lblUser, pnlUserWrap,
            lblPass, pnlPwdWrap, btnEye,
            lblError, btnLogin, pnlHint
        });

        // Centrar o card no painel direito
        parent.SizeChanged += (_, _) =>
        {
            card.Left = (parent.Width  - card.Width)  / 2;
            card.Top  = (parent.Height - card.Height) / 2;
        };
        parent.Controls.Add(card);

        // Initial position
        card.Left = (parent.Width  - card.Width)  / 2;
        card.Top  = (parent.Height - card.Height) / 2;
    }

    // ── Desenho do card com sombra e cantos arredondados ──────────────────
    private void DrawLoginCard(object? sender, PaintEventArgs e)
    {
        var g   = e.Graphics;
        var pnl = (Panel)sender!;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Shadow layers
        for (int i = 8; i >= 1; i--)
        {
            var sr    = new Rectangle(i, i, pnl.Width - i - 1, pnl.Height - i - 1);
            int alpha = Math.Max(1, 20 / i);
            using var sp = UIHelper.RoundedRect(sr, 17);
            using var sb = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
            g.FillPath(sb, sp);
        }

        // White rounded card
        var cr = new Rectangle(0, 0, pnl.Width - 9, pnl.Height - 9);
        using var path  = UIHelper.RoundedRect(cr, 16);
        using var brush = new SolidBrush(Color.White);
        g.FillPath(brush, path);

        // Subtle border
        using var pen = new Pen(ThemeHelper.BorderColor, 0.8f);
        g.DrawPath(pen, path);

        // Top accent stripe
        var stripe = new Rectangle(0, 0, pnl.Width - 9, 4);
        using var stripePath  = UIHelper.RoundedRect(stripe, 2);
        using var stripeBrush = new LinearGradientBrush(stripe,
            ThemeHelper.LightBlue, ThemeHelper.MediumBlue,
            LinearGradientMode.Horizontal);
        g.FillPath(stripeBrush, stripePath);
    }

    // ── Helpers de input ──────────────────────────────────────────────────
    private static Label MakeFieldLabel(string text, int left, int top)
        => new()
        {
            Text      = text,
            Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = ThemeHelper.DarkText,
            AutoSize  = true,
            Left      = left,
            Top       = top,
            BackColor = Color.Transparent
        };

    private Panel MakeInputWrapper(int left, int top, out TextBox txt, bool isPassword = false)
    {
        var wrap = new Panel
        {
            Left      = left,
            Top       = top,
            Width     = 308,
            Height    = 46,
            BackColor = Color.White
        };
        wrap.Paint += DrawInputBorder;

        txt = new TextBox
        {
            BorderStyle           = BorderStyle.None,
            Font                  = ThemeHelper.FontBody,
            BackColor             = Color.White,
            ForeColor             = ThemeHelper.DarkText,
            Width                 = 260,
            Left                  = 14,
            Top                   = 13,
            UseSystemPasswordChar = isPassword
        };

        // Highlight border on focus
        txt.Enter += (_, _) => { wrap.Tag = "focused"; wrap.Invalidate(); };
        txt.Leave += (_, _) => { wrap.Tag = null;       wrap.Invalidate(); };

        wrap.Controls.Add(txt);
        return wrap;
    }

    private static void DrawInputBorder(object? sender, PaintEventArgs e)
    {
        var pnl = (Panel)sender!;
        var g   = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        bool focused  = pnl.Tag?.ToString() == "focused";
        Color borderC = focused ? ThemeHelper.LightBlue : ThemeHelper.BorderColor;
        float thick   = focused ? 2f : 1f;

        var r = new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1);
        using var path  = UIHelper.RoundedRect(r, 8);
        using var brush = new SolidBrush(Color.White);
        g.FillPath(brush, path);
        using var pen = new Pen(borderC, thick);
        g.DrawPath(pen, path);
    }

    // ── Login handler ─────────────────────────────────────────────────────
    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        lblError.Visible = false;
        pnlUserWrap.Tag  = null;
        pnlPwdWrap.Tag   = null;
        pnlUserWrap.Invalidate();
        pnlPwdWrap.Invalidate();

        string user = txtUsername.Text.Trim();
        string pass = txtPassword.Text;

        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
        {
            ShowError("Por favor preencha todos os campos.");
            return;
        }

        // Usar o serviço de autenticação
        var auth     = new AutenticacaoService();
        var resultado = auth.FazerLogin(user, pass);

        if (resultado == null)
        {
            ShowError("Utilizador ou palavra-passe incorrectos.");
            ShakeButton();
            return;
        }

        // Guardar quem fez login para o MainForm saber
        if (resultado is string admin && admin == "ADMIN")
        {
            // É o administrador
            Program.UtilizadorAtual = "ADMIN";
            Program.ContaProfessorAtual = null;
        }
        else if (resultado is ContaProfessor conta)
        {
            // É um professor
            Program.UtilizadorAtual = "PROFESSOR";
            Program.ContaProfessorAtual = conta;
        }

        // Login OK → abre o MainForm
        DialogResult = DialogResult.OK;
        Close();
    }
    private void ShowError(string msg)
    {
        lblError.Text    = "⚠  " + msg;
        lblError.Visible = true;

        // Red border on both fields
        pnlUserWrap.Tag = "error";
        pnlPwdWrap.Tag  = "error";

        // Override DrawInputBorder colour for error state
        pnlUserWrap.Paint -= DrawInputBorder;
        pnlPwdWrap.Paint  -= DrawInputBorder;

        pnlUserWrap.Paint += DrawErrorBorder;
        pnlPwdWrap.Paint  += DrawErrorBorder;

        pnlUserWrap.Invalidate();
        pnlPwdWrap.Invalidate();
    }

    private static void DrawErrorBorder(object? sender, PaintEventArgs e)
    {
        var pnl = (Panel)sender!;
        var g   = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var r = new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1);
        using var path  = UIHelper.RoundedRect(r, 8);
        using var brush = new SolidBrush(ThemeHelper.DangerBg);
        g.FillPath(brush, path);
        using var pen = new Pen(ThemeHelper.Danger, 1.5f);
        g.DrawPath(pen, path);
    }

    // Efeito de shake no botão quando credenciais erradas
    private async void ShakeButton()
    {
        int originalX = btnLogin.Left;
        int[] offsets = { -6, 6, -4, 4, -2, 2, 0 };
        foreach (int offset in offsets)
        {
            btnLogin.Left = originalX + offset;
            await Task.Delay(30);
        }
        btnLogin.Left = originalX;
    }
}
