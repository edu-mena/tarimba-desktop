using TarimbaPresence.Models;

namespace TarimbaPresence.Services;

public class ExportacaoService
{
    // Exporta lista de relatórios de alunos para CSV
    public void ExportarRelatorioAlunos(List<RelatorioAluno> relatorios, string caminhoFicheiro)
    {
        var linhas = new List<string>();

        // Cabeçalho
        linhas.Add("Nº Processo;Nome;Turma;Presenças;Faltas;Justificadas;Total;Taxa Presença;Nível Risco");

        // Uma linha por aluno
        foreach (var r in relatorios)
        {
            linhas.Add(string.Join(";", new[]
            {
                r.NumeroProcesso,
                r.NomeAluno,
                r.NomeTurma,
                r.Presentes.ToString(),
                r.Faltas.ToString(),
                r.Justificadas.ToString(),
                r.Total.ToString(),
                $"{r.TaxaPresenca:0.0}%",
                r.NivelRisco
            }));
        }

        File.WriteAllLines(caminhoFicheiro, linhas, System.Text.Encoding.UTF8);
    }

    // Exporta relatório de uma turma
    public void ExportarRelatorioTurma(RelatorioPresenca relatorio, string caminhoFicheiro)
    {
        ExportarRelatorioAlunos(relatorio.Alunos, caminhoFicheiro);
    }
}