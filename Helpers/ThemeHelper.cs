namespace TarimbaPresence.Helpers;

public static class ThemeHelper
{
    // ── Paleta Principal ───────────────────────────────────────────────────
    public static readonly Color NavyBlue     = Color.FromArgb(15,  23,  42);   // #0F172A
    public static readonly Color MediumBlue   = Color.FromArgb(30,  58,  138);  // #1E3A8A
    public static readonly Color LightBlue    = Color.FromArgb(59,  130, 246);  // #3B82F6
    public static readonly Color LightBlue10  = Color.FromArgb(25,  59,  130, 246);
    public static readonly Color White        = Color.White;
    public static readonly Color LightGray    = Color.FromArgb(241, 245, 249);  // #F1F5F9
    public static readonly Color DarkText     = Color.FromArgb(30,  41,  59);   // #1E293B
    public static readonly Color MediumText   = Color.FromArgb(100, 116, 139);  // #64748B
    public static readonly Color SubtleText   = Color.FromArgb(148, 163, 184);  // #94A3B8
    public static readonly Color BorderColor  = Color.FromArgb(226, 232, 240);  // #E2E8F0
    public static readonly Color ContentBg    = Color.FromArgb(248, 250, 252);  // #F8FAFC

    // ── Feedback / Status ─────────────────────────────────────────────────
    public static readonly Color Success      = Color.FromArgb(22,  163, 74);   // #16A34A
    public static readonly Color SuccessBg    = Color.FromArgb(220, 252, 231);  // #DCFCE7
    public static readonly Color SuccessText  = Color.FromArgb(20,  83,  45);
    public static readonly Color Warning      = Color.FromArgb(217, 119, 6);    // #D97706
    public static readonly Color WarningBg    = Color.FromArgb(254, 243, 199);  // #FEF3C7
    public static readonly Color WarningText  = Color.FromArgb(120, 53,  15);
    public static readonly Color Danger       = Color.FromArgb(220, 38,  38);   // #DC2626
    public static readonly Color DangerBg     = Color.FromArgb(254, 226, 226);  // #FEE2E2
    public static readonly Color DangerText   = Color.FromArgb(127, 29,  29);
    public static readonly Color Purple       = Color.FromArgb(124, 58,  237);  // #7C3AED
    public static readonly Color PurpleBg     = Color.FromArgb(237, 233, 254);  // #EDE9FE
    public static readonly Color PurpleText   = Color.FromArgb(76,  29,  149);

    // ── Sidebar ────────────────────────────────────────────────────────────
    public static readonly Color SidebarBg         = Color.FromArgb(15,  23,  42);
    public static readonly Color SidebarHeaderBg   = Color.FromArgb(9,   14,  27);
    public static readonly Color SidebarHover      = Color.FromArgb(30,  41,  60);
    public static readonly Color SidebarActive     = Color.FromArgb(30,  58,  138);
    public static readonly Color SidebarText       = Color.FromArgb(148, 163, 184);
    public static readonly Color SidebarTextActive = Color.White;
    public static readonly Color SidebarAccent     = Color.FromArgb(59,  130, 246);

    // ── Card ───────────────────────────────────────────────────────────────
    public static readonly Color CardBg      = Color.White;
    public static readonly Color CardShadow  = Color.FromArgb(20,  0,   0,   0);

    // ── Fonts (application-lifetime; não dispose) ─────────────────────────
    public static readonly Font FontDisplay    = new("Segoe UI",  26f,  FontStyle.Bold);
    public static readonly Font FontTitle      = new("Segoe UI",  18f,  FontStyle.Bold);
    public static readonly Font FontHeading    = new("Segoe UI",  14f,  FontStyle.Bold);
    public static readonly Font FontSubheading = new("Segoe UI",  12f,  FontStyle.Bold);
    public static readonly Font FontBody       = new("Segoe UI",  11f,  FontStyle.Regular);
    public static readonly Font FontBodyBold   = new("Segoe UI",  11f,  FontStyle.Bold);
    public static readonly Font FontSmall      = new("Segoe UI",  9.5f, FontStyle.Regular);
    public static readonly Font FontCaption    = new("Segoe UI",  9f,   FontStyle.Regular);
    public static readonly Font FontSidebar    = new("Segoe UI",  11.5f,FontStyle.Regular);
    public static readonly Font FontCardValue  = new("Segoe UI",  28f,  FontStyle.Bold);
    public static readonly Font FontCardLabel  = new("Segoe UI",  10.5f,FontStyle.Regular);
    public static readonly Font FontBadge      = new("Segoe UI",  9f,   FontStyle.Bold);
}
