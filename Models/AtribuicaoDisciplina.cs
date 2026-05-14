namespace TarimbaPresence.Models;

public class AtribuicaoDisciplina
{
    public int    Id           { get; set; }
    public string TurmaId      { get; set; } = string.Empty;
    public int    DisciplinaId { get; set; }
    public int    ProfessorId  { get; set; }
}
