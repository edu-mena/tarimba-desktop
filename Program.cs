using TarimbaPresence.Forms;

namespace TarimbaPresence;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        // Mostra o login; se OK, abre o dashboard principal
        using var login = new LoginForm();
        if (login.ShowDialog() == DialogResult.OK)
        {
            Application.Run(new MainForm());
        }
    }
}
