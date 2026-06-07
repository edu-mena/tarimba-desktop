namespace TarimbaPresence.Models;

public class RelatorioAluno
{
    public int    AlunoId        { get; set; }
    public string NomeAluno      { get; set; } = string.Empty;
    public string NumeroProcesso { get; set; } = string.Empty;
    public string NomeTurma      { get; set; } = string.Empty;
    public int    Presentes      { get; set; }
    public int    Faltas         { get; set; }
    public int    Justificadas   { get; set; }
    public int    Total          => Presentes + Faltas + Justificadas;
    public double TaxaPresenca   => Total > 0 ? (double)Presentes / Total * 100.0 : 0;
    public string NivelRisco     => TaxaPresenca < 60 ? "Alto Risco"
                                  : TaxaPresenca < 75 ? "Risco Moderado"
                                  : "Normal";
}