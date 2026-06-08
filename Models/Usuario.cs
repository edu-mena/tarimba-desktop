namespace TarimbaPresence.Models;

public abstract class Usuario
{
    public int    Id    { get; set; }
    public string Nome  { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UsuarioRole Role { get; set; }
}