using TarimbaPresence.Data;
using TarimbaPresence.Helpers;
using TarimbaPresence.Models;

namespace TarimbaPresence.UserControls;

public class UC_DashboardProfessor : UserControl
{
    public UC_DashboardProfessor()
    {
        BackColor  = ThemeHelper.ContentBg;
        Padding    = new Padding(32, 28, 32, 28);
        AutoScroll = true;
        Build();
    }

    private void Build()
    {
        SuspendLayout();

        // Cabeçalho
        var lblTitulo = new Label
        {
            Text      = "O Meu Painel",
            Font      = ThemeHelper.FontTitle,
            ForeColor = ThemeHelper.DarkText,
            AutoSize  = true,
            Left      = 0,
            Top       = 0,
            BackColor = Color.Transparent
        };

        // Buscar dados do professor logado
        var conta = Program.ContaProfessorAtual;
        var professor = conta != null
            ? MockDataStore.Professores.FirstOrDefault(p => p.Id == conta.ProfessorId)
            : null;

        // Turmas do professor
        var turmasIds = MockDataStore.AtribuicaoDisciplinas
            .Where(a => a.ProfessorId == professor?.Id)
            .Select(a => a.TurmaId)
            .Distinct()
            .ToList();

        var turmas = MockDataStore.Turmas
            .Where(t => turmasIds.Contains(t.Id))
            .ToList();

        // Info do professor
        var lblNome = new Label
        {
            Text      = $"Professor(a): {professor?.NomeCompleto ?? "Desconhecido"}",
            Font      = ThemeHelper.FontHeading,
            ForeColor = ThemeHelper.DarkText,
            AutoSize  = true,
            Left      = 0,
            Top       = 48,
            BackColor = Color.Transparent
        };

        var lblTurmas = new Label
        {
            Text      = $"Turmas atribuídas: {turmas.Count}",
            Font      = ThemeHelper.FontBody,
            ForeColor = ThemeHelper.MediumText,
            AutoSize  = true,
            Left      = 0,
            Top       = 80,
            BackColor = Color.Transparent
        };

        // Lista de turmas
        int topAtual = 120;
        var listaTurmas = new List<Control> { lblTitulo, lblNome, lblTurmas };

        foreach (var turma in turmas)
        {
            var lblTurma = new Label
            {
                Text      = $"▸  {turma.Nome}  ({turma.Turno.DisplayText()})",
                Font      = ThemeHelper.FontBody,
                ForeColor = ThemeHelper.DarkText,
                AutoSize  = true,
                Left      = 16,
                Top       = topAtual,
                BackColor = Color.Transparent
            };
            listaTurmas.Add(lblTurma);
            topAtual += 28;
        }

        Controls.AddRange(listaTurmas.ToArray());
        ResumeLayout(true);
    }
}