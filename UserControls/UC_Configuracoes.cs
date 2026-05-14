using System.Drawing.Drawing2D;
using FontAwesome.Sharp;
using TarimbaPresence.Controls;
using TarimbaPresence.Helpers;

namespace TarimbaPresence.UserControls;

public class UC_Configuracoes : UserControl
{
    public UC_Configuracoes()
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
            Text = "Configurações", Font = ThemeHelper.FontTitle,
            ForeColor = ThemeHelper.DarkText, AutoSize = true, Left = 0, Top = 4,
            BackColor = Color.Transparent
        });
        pnlHeader.Controls.Add(new Label
        {
            Text = "Preferências do sistema, escola e conta de administrador",
            Font = ThemeHelper.FontSmall, ForeColor = ThemeHelper.MediumText,
            AutoSize = true, Left = 2, Top = 34, BackColor = Color.Transparent
        });

        // ── Sections ───────────────────────────────────────────────────────
        var pnlSchool   = BuildSection("Informações da Escola",  IconChar.School,      BuildSchoolFields());
        var pnlAccount  = BuildSection("Conta do Administrador", IconChar.UserShield,  BuildAccountFields());
        var pnlPrefs    = BuildSection("Preferências do Sistema",IconChar.SlidersH,    BuildPrefsFields());
        var pnlAbout    = BuildSection("Sobre o Sistema",        IconChar.CircleInfo,  BuildAboutContent());

        pnlSchool.Dock  = DockStyle.Top;
        pnlAccount.Dock = DockStyle.Top;
        pnlPrefs.Dock   = DockStyle.Top;
        pnlAbout.Dock   = DockStyle.Top;

        Controls.AddRange(new Control[] { pnlAbout, pnlPrefs, pnlAccount, pnlSchool, pnlHeader });

        ResumeLayout(true);
    }

    // ── Section wrapper ───────────────────────────────────────────────────
    private Panel BuildSection(string title, IconChar icon, Control content)
    {
        var pnl = new Panel
        {
            Height    = 0,
            BackColor = Color.Transparent,
            Padding   = new Padding(0, 0, 0, 20)
        };

        var card = new RoundedPanel
        {
            Dock         = DockStyle.Fill,
            BackColor    = Color.White,
            ShowShadow   = true,
            CornerRadius = 12,
            ShadowDepth  = 4
        };

        // Header strip inside card
        var pnlCardHeader = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 52,
            BackColor = Color.Transparent,
            Padding   = new Padding(20, 0, 20, 0)
        };
        pnlCardHeader.Paint += (_, e) =>
        {
            using var pen = new Pen(ThemeHelper.BorderColor, 1f);
            e.Graphics.DrawLine(pen, 0, pnlCardHeader.Height - 1,
                                pnlCardHeader.Width, pnlCardHeader.Height - 1);
        };

        // Icon + title
        pnlCardHeader.Controls.Add(new IconPictureBox
        {
            IconChar  = icon,
            IconColor = ThemeHelper.LightBlue,
            IconSize  = 18,
            BackColor = Color.Transparent,
            Left      = 20,
            Top       = 14,
            Width     = 24,
            Height    = 24
        });

        pnlCardHeader.Controls.Add(new Label
        {
            Text      = title,
            Font      = ThemeHelper.FontSubheading,
            ForeColor = ThemeHelper.DarkText,
            AutoSize  = true,
            Left      = 52,
            Top       = 15,
            BackColor = Color.Transparent
        });

        // Content wrapper
        var pnlContent = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding   = new Padding(24, 16, 24, 16)
        };
        content.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(content);

        card.Controls.AddRange(new Control[] { pnlContent, pnlCardHeader });

        // Auto-size outer panel based on content height after layout
        card.Layout += (_, _) =>
        {
            int h = pnlCardHeader.Height + content.Height + 32 + 28;
            if (pnl.Height != h) pnl.Height = h;
        };

        pnl.Controls.Add(card);
        return pnl;
    }

    // ── School fields ─────────────────────────────────────────────────────
    private Control BuildSchoolFields()
    {
        var pnl = new Panel { Height = 180, BackColor = Color.Transparent };

        AddLabeledField(pnl, "Nome da Escola",    "Complexo Escolar Tarimba III",  0,   320);
        AddLabeledField(pnl, "Localidade",        "Luanda, Angola",                 0,   200, 340);
        AddLabeledField(pnl, "Ano Lectivo",       "2025/2026",                      68,  160);
        AddLabeledField(pnl, "Nível de Ensino",   "Ensino Primário (1ª a 6ª Classe)",68, 320);
        AddLabeledField(pnl, "Director(a)",       "Dr. António Manuel Tarimba",     136, 320);

        var btnSave = UIHelper.MakePrimaryButton("Guardar Alterações", 180, 38);
        btnSave.Top  = 168;
        btnSave.Left = 0;
        btnSave.Click += (_, _) =>
            MessageBox.Show("Configurações guardadas com sucesso.", "Guardado",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

        pnl.Controls.Add(btnSave);
        return pnl;
    }

    // ── Account fields ────────────────────────────────────────────────────
    private Control BuildAccountFields()
    {
        var pnl = new Panel { Height = 180, BackColor = Color.Transparent };

        AddLabeledField(pnl, "Nome do Utilizador", "Administrador do Sistema", 0,   280);
        AddLabeledField(pnl, "Email",              "admin@tarimba3.ao",        0,   280, 300);
        AddLabeledField(pnl, "Perfil",             "Administrador",             68,  180);
        AddLabeledField(pnl, "Última Sessão",      DateTime.Now.AddHours(-2).ToString("dd/MM/yyyy HH:mm"), 68, 200, 200);

        // Password field
        var lblPwd = UIHelper.MakeLabel("Nova Palavra-passe", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblPwd.Left = 0; lblPwd.Top = 136;
        var txtPwd = UIHelper.MakeTextBox(280);
        txtPwd.UseSystemPasswordChar = true;
        txtPwd.Left = 0; txtPwd.Top = 154;
        pnl.Controls.AddRange(new Control[] { lblPwd, txtPwd });

        return pnl;
    }

    // ── Preferences fields ────────────────────────────────────────────────
    private Control BuildPrefsFields()
    {
        var pnl = new Panel { Height = 130, BackColor = Color.Transparent };

        // Theme toggle
        var lblTheme = UIHelper.MakeLabel("Tema da Interface:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblTheme.Left = 0; lblTheme.Top = 10;

        var cmbTheme = UIHelper.MakeComboBox(180);
        cmbTheme.Items.AddRange(new object[] { "Claro (padrão)", "Escuro (em breve)" });
        cmbTheme.SelectedIndex = 0;
        cmbTheme.Left = 0; cmbTheme.Top = 28;
        cmbTheme.SelectedIndexChanged += (_, _) =>
        {
            if (cmbTheme.SelectedIndex == 1)
            {
                cmbTheme.SelectedIndex = 0;
                MessageBox.Show("Tema escuro estará disponível em versão futura.",
                    "Em Breve", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };

        // Notifications toggle
        var lblNotif = UIHelper.MakeLabel("Notificações de Faltas Críticas:", ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lblNotif.Left = 0; lblNotif.Top = 68;

        var chkNotif = new CheckBox
        {
            Text      = "Alertar quando aluno superar 25% de faltas",
            Font      = ThemeHelper.FontBody,
            ForeColor = ThemeHelper.DarkText,
            Checked   = true,
            Left      = 0,
            Top       = 86,
            AutoSize  = true,
            BackColor = Color.Transparent
        };

        pnl.Controls.AddRange(new Control[] { lblTheme, cmbTheme, lblNotif, chkNotif });
        return pnl;
    }

    // ── About content ─────────────────────────────────────────────────────
    private Control BuildAboutContent()
    {
        var pnl = new Panel { Height = 110, BackColor = Color.Transparent };

        var info = new[]
        {
            ("Sistema:",       "Gestão de Presenças — Tarimba III"),
            ("Versão:",        "1.0.0  (2026)"),
            ("Plataforma:",    ".NET 8  •  Windows Forms"),
            ("Desenvolvido:",  "Departamento de TI  •  Complexo Escolar Tarimba III"),
        };

        int y = 4;
        foreach (var (label, value) in info)
        {
            var lbl = UIHelper.MakeLabel(label, ThemeHelper.FontSmall, ThemeHelper.MediumText);
            lbl.Left = 0; lbl.Top = y; lbl.Width = 130; lbl.AutoSize = false;

            var val = UIHelper.MakeLabel(value, ThemeHelper.FontBodyBold, ThemeHelper.DarkText);
            val.Left = 136; val.Top = y;

            pnl.Controls.AddRange(new Control[] { lbl, val });
            y += 24;
        }

        return pnl;
    }

    // ── Helper: labeled text field ────────────────────────────────────────
    private static void AddLabeledField(Panel parent, string label, string value,
                                         int top, int width, int left = 0)
    {
        var lbl = UIHelper.MakeLabel(label, ThemeHelper.FontSmall, ThemeHelper.MediumText);
        lbl.Left = left; lbl.Top = top;

        var txt = UIHelper.MakeTextBox(width);
        txt.Text   = value;
        txt.Left   = left;
        txt.Top    = top + 18;
        txt.Height = 36;

        parent.Controls.AddRange(new Control[] { lbl, txt });
    }
}

// ── Fluent helper extension ────────────────────────────────────────────────
internal static class PanelExt
{
    public static Panel Also(this Panel p, Action<Panel> action)
    {
        action(p);
        return p;
    }
}
