namespace TarimbaPresence.Models;

public class Disciplina
{
    public int    Id           { get; set; }
    public string Nome         { get; set; } = string.Empty;
    public int    ClasseMinima { get; set; } = 1;
    public int    ClasseMaxima { get; set; } = 13;
    public Curso? Curso        { get; set; }  // null = sem restrição de curso

    public override string ToString() => Nome;
}
