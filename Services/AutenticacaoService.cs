using TarimbaPresence.Database;
using TarimbaPresence.Models;

namespace TarimbaPresence.Services;

public class AutenticacaoService
{
    private readonly DatabaseService _db = new();

    public object? FazerLogin(string email, string senha)
    {
        // Verificar administrador
        if (email == "admin" && senha == "1234")
            return "ADMIN";

        // Verificar professor na base de dados
        var conta = _db.ObterContaPorEmail(email);

        if (conta == null || !conta.Ativo)
            return null;

        if (conta.PasswordHash != senha)
            return null;

        return conta;
    }
}