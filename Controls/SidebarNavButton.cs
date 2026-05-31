using System.ComponentModel;
using System.Drawing.Drawing2D;
using FontAwesome.Sharp;
using TarimbaPresence.Helpers;

namespace TarimbaPresence.Controls;

/// <summary>
/// Botão de navegação da sidebar — estende IconButton com estado activo e accent bar.
/// </summary>
public class SidebarNavButton : IconButton
{
    private bool _isActive;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new string? Text
    {
        get => base.Text;
        set => base.Text = "        " + value; // ~10px gap between icon and label
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive  = value;
            BackColor  = value ? ThemeHelper.SidebarActive : ThemeHelper.SidebarBg;
            ForeColor  = value ? Color.White               : ThemeHelper.SidebarText;
            IconColor  = value ? Color.White               : ThemeHelper.SidebarText;
            Invalidate();
        }
    }

    public event EventHandler? NavClick;

    public SidebarNavButton()
    {
        FlatStyle  = FlatStyle.Flat;
        FlatAppearance.BorderSize           = 0;
        FlatAppearance.MouseOverBackColor   = ThemeHelper.SidebarHover;
        FlatAppearance.MouseDownBackColor   = ThemeHelper.SidebarActive;
        BackColor  = ThemeHelper.SidebarBg;
        ForeColor  = ThemeHelper.SidebarText;
        IconColor  = ThemeHelper.SidebarText;
        IconSize   = 20;
        ImageAlign = ContentAlignment.MiddleLeft;
        TextAlign  = ContentAlignment.MiddleLeft;
        Padding    = new Padding(16, 0, 0, 0);
        Height     = 50;
        Dock       = DockStyle.Top;
        Font       = ThemeHelper.FontSidebar;
        Cursor     = Cursors.Hand;
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        NavClick?.Invoke(this, e);
    }

    // Accent bar on left side when active
    protected override void OnPaint(PaintEventArgs pevent)
    {
        base.OnPaint(pevent);
        if (_isActive)
        {
            using var brush = new SolidBrush(ThemeHelper.SidebarAccent);
            pevent.Graphics.FillRectangle(brush, 0, 8, 4, Height - 16);
        }
    }
}
