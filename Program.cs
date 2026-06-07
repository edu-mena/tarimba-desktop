using TarimbaPresence.Forms;
using TarimbaPresence.Models;

namespace TarimbaPresence;

internal static class Program
{
    // Guarda quem está logado ("ADMIN" ou "PROFESSOR")
    public static string UtilizadorAtual { get; set; } = "";

    // Se for professor, guarda os dados da conta dele
    public static ContaProfessor? ContaProfessorAtual { get; set; } = null;

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        // Verificar se pode abrir dashboard principal (se já tiver alguém logado)
        using var login = new LoginForm();
        if (login.ShowDialog() == DialogResult.OK)
        {
            Application.Run(new MainForm());
        }
    }
}