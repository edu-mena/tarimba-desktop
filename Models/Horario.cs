namespace TarimbaPresence.Models;

public class Horario
{
    public int    Id           { get; set; }
    public string TurmaId      { get; set; } = string.Empty;
    public int    DisciplinaId { get; set; }
    public int    DiaDaSemana  { get; set; } // 1=Seg, 2=Ter, 3=Qua, 4=Qui, 5=Sex
    public int    Aula         { get; set; } // 1..6
}
