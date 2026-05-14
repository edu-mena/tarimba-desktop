namespace TarimbaPresence.Models;

public class Turma
{
    public string Id                { get; set; } = string.Empty;
    public int    Sala              { get; set; }
    public Turno  Turno             { get; set; }
    public int    Classe            { get; set; }
    public Curso? Curso             { get; set; }
    public int?   DirectorDeTurmaId { get; set; }

    public bool   PrecisaDeCurso => Classe >= 10;
    public string NomeClasse     => $"{Classe}ª Classe";

    public string Nome => Curso.HasValue
        ? $"Sala {Sala} — {Classe}ª {Curso.Value.Abreviatura()} ({Turno.DisplayText()})"
        : $"Sala {Sala} — {Classe}ª Classe ({Turno.DisplayText()})";

    // Short label for constrained UI (e.g. dashboard chart rows)
    public string NomeCurto => Curso.HasValue
        ? $"S{Sala} · {Curso.Value.Abreviatura()} ({Turno.DisplayText()[0]})"
        : $"S{Sala} · {Classe}ª ({Turno.DisplayText()[0]})";

    public override string ToString() => Nome;
}
