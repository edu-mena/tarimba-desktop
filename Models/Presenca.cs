namespace TarimbaPresence.Models;

public enum StatusPresenca
{
    Presente,
    Falta,
    Justificada
}

public class Presenca
{
    public int    Id           { get; set; }
    public int    AlunoId      { get; set; }
    public string TurmaId      { get; set; } = string.Empty;
    public int    DisciplinaId { get; set; }
    public DateTime Data       { get; set; }
    public StatusPresenca Status { get; set; }
    public string? Observacao { get; set; }

    public string StatusTexto => Status switch
    {
        StatusPresenca.Presente    => "Presente",
        StatusPresenca.Falta       => "Falta",
        StatusPresenca.Justificada => "Justificada",
        _                          => "—"
    };
}
