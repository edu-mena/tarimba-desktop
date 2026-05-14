namespace TarimbaPresence.Models;

public enum Turno { Manha, Tarde }

public static class TurnoExt
{
    public static string DisplayText(this Turno t) => t == Turno.Manha ? "Manhã" : "Tarde";
}
