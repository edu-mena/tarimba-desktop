namespace TarimbaPresence.Models;

public class Aluno
{
    public int Id { get; set; }
    public string NumeroProcesso { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string Sexo { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string TurmaId { get; set; } = string.Empty;
    public string Classe { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public int Idade
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DataNascimento.Year;
            if (DataNascimento.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
