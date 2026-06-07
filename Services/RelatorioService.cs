using TarimbaPresence.Data;
using TarimbaPresence.Models;

namespace TarimbaPresence.Services;

public class RelatorioService
{
    // Gera relatório de um aluno específico
    public RelatorioAluno GerarRelatorioAluno(int alunoId, DateTime? de = null, DateTime? ate = null)
    {
        var aluno = MockDataStore.Alunos.FirstOrDefault(a => a.Id == alunoId);
        if (aluno == null) return new RelatorioAluno();

        var turma = MockDataStore.Turmas.FirstOrDefault(t => t.Id == aluno.TurmaId);

        var registos = MockDataStore.Presencas
            .Where(p => p.AlunoId == alunoId);

        if (de  != null) registos = registos.Where(p => p.Data.Date >= de.Value.Date);
        if (ate != null) registos = registos.Where(p => p.Data.Date <= ate.Value.Date);

        var lista = registos.ToList();

        return new RelatorioAluno
        {
            AlunoId        = aluno.Id,
            NomeAluno      = aluno.NomeCompleto,
            NumeroProcesso = aluno.NumeroProcesso,
            NomeTurma      = turma?.Nome ?? "—",
            Presentes      = lista.Count(p => p.Status == StatusPresenca.Presente),
            Faltas         = lista.Count(p => p.Status == StatusPresenca.Falta),
            Justificadas   = lista.Count(p => p.Status == StatusPresenca.Justificada)
        };
    }

    // Gera relatório de todos os alunos de uma turma
    public RelatorioPresenca GerarRelatorioTurma(string turmaId, DateTime? de = null, DateTime? ate = null)
    {
        var turma = MockDataStore.Turmas.FirstOrDefault(t => t.Id == turmaId);
        if (turma == null) return new RelatorioPresenca();

        var alunos = MockDataStore.GetAlunosDaTurma(turmaId);
        var relatoriosAlunos = alunos
            .Select(a => GerarRelatorioAluno(a.Id, de, ate))
            .ToList();

        return new RelatorioPresenca
        {
            NomeTurma      = turma.Nome,
            NomeDisciplina = "Todas as Disciplinas",
            DataInicio     = de  ?? DateTime.Today.AddDays(-30),
            DataFim        = ate ?? DateTime.Today,
            TotalPresentes = relatoriosAlunos.Sum(r => r.Presentes),
            TotalFaltas    = relatoriosAlunos.Sum(r => r.Faltas),
            TotalJustif    = relatoriosAlunos.Sum(r => r.Justificadas),
            Alunos         = relatoriosAlunos
        };
    }

    // Gera relatório de todos os alunos do sistema
    public List<RelatorioAluno> GerarRelatorioGeral(DateTime? de = null, DateTime? ate = null)
    {
        return MockDataStore.Alunos
            .Select(a => GerarRelatorioAluno(a.Id, de, ate))
            .OrderByDescending(r => r.Faltas)
            .ToList();
    }
}