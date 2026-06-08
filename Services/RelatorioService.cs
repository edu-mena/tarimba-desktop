using TarimbaPresence.Database;
using TarimbaPresence.Models;

namespace TarimbaPresence.Services;

public class RelatorioService
{
    private readonly DatabaseService _db = new();

    // Gera relatório de um aluno específico
    public RelatorioAluno GerarRelatorioAluno(int alunoId, DateTime? de = null, DateTime? ate = null)
    {
        var aluno = _db.ObterTodosAlunos().FirstOrDefault(a => a.Id == alunoId);
        if (aluno == null) return new RelatorioAluno();

        var turma = _db.ObterTurma(aluno.TurmaId);

        var presencas = _db.ObterPresencasPorAluno(alunoId);

        if (de  != null) presencas = presencas.Where(p => p.Data.Date >= de.Value.Date).ToList();
        if (ate != null) presencas = presencas.Where(p => p.Data.Date <= ate.Value.Date).ToList();

        return new RelatorioAluno
        {
            AlunoId        = aluno.Id,
            NomeAluno      = aluno.NomeCompleto,
            NumeroProcesso = aluno.NumeroProcesso,
            NomeTurma      = turma?.Nome ?? "—",
            Presentes      = presencas.Count(p => p.Status == StatusPresenca.Presente),
            Faltas         = presencas.Count(p => p.Status == StatusPresenca.Falta),
            Justificadas   = presencas.Count(p => p.Status == StatusPresenca.Justificada)
        };
    }

    // Gera relatório de todos os alunos de uma turma
    public RelatorioPresenca GerarRelatorioTurma(string turmaId, DateTime? de = null, DateTime? ate = null)
    {
        var turma = _db.ObterTurma(turmaId);
        if (turma == null) return new RelatorioPresenca();

        var alunos = _db.ObterAlunosDaTurma(turmaId);
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
        return _db.ObterTodosAlunos()
            .Select(a => GerarRelatorioAluno(a.Id, de, ate))
            .OrderByDescending(r => r.Faltas)
            .ToList();
    }
}