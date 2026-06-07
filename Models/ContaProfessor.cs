namespace TarimbaPresence.Models
{
    public class ContaProfessor
    {
        public int Id { get; set; }
        public int ProfessorId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public bool PrimeiroLogin { get; set; }
    }
}