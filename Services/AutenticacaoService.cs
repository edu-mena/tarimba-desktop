using TarimbaPresence.Data;
using TarimbaPresence.Models;

namespace TarimbaPresence.Services
{
    public class AutenticacaoService
    {
        // Este método recebe email e senha e devolve quem é o utilizador
        public object? FazerLogin(string email, string senha)
        {
            // Verificar se é o administrador
            if (email == "admin" && senha == "1234")
                return "ADMIN";

            // Verificar se é um professor
            var conta = MockDataStore.ContasProfessor
                .FirstOrDefault(x =>
                    x.Email == email &&
                    x.PasswordHash == senha &&
                    x.Ativo);

            // Se encontrou uma conta de professor, devolve essa conta
            if (conta != null)
                return conta;

            // Se não encontrou nada, devolve null (ninguém)
            return null;
        }
    }
}