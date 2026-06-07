namespace TarimbaPresence.Models;

public class RelatorioPresenca
{
    public string NomeTurma      { get; set; } = string.Empty;
    public string NomeDisciplina { get; set; } = string.Empty;
    public DateTime DataInicio   { get; set; }
    public DateTime DataFim      { get; set; }
    public int    TotalPresentes { get; set; }
    public int    TotalFaltas    { get; set; }
    public int    TotalJustif    { get; set; }
    public int    Total          => TotalPresentes + TotalFaltas + TotalJustif;
    public double TaxaPresenca   => Total > 0 ? (double)TotalPresentes / Total * 100.0 : 0;
    public List<RelatorioAluno> Alunos { get; set; } = new();
}