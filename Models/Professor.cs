namespace TarimbaPresence.Models;

public class Professor
{ 
    public int    Id           { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email        { get; set; } = string.Empty;
    public string Telefone     { get; set; } = string.Empty;
    public bool   Ativo        { get; set; } = true;
    public List<int>? TurmasIds { get; set; }
    public List<int>? DisciplinasIds { get; set; }

    public override string ToString() => NomeCompleto;
}
