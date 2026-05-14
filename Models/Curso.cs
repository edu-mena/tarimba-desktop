namespace TarimbaPresence.Models;

public enum Curso
{
    ContabilidadeEGestao,
    InformaticaDeGestao,
    CienciasEconomicasEJuridicas,
    CienciasFisicasEBiologicas
}

public static class CursoExt
{
    public static string DisplayText(this Curso c) => c switch
    {
        Curso.ContabilidadeEGestao         => "Contabilidade e Gestão",
        Curso.InformaticaDeGestao          => "Informática de Gestão",
        Curso.CienciasEconomicasEJuridicas => "Ciências Económicas e Jurídicas",
        Curso.CienciasFisicasEBiologicas   => "Ciências Físicas e Biológicas",
        _                                  => ""
    };

    public static string Abreviatura(this Curso c) => c switch
    {
        Curso.ContabilidadeEGestao         => "CG",
        Curso.InformaticaDeGestao          => "IG",
        Curso.CienciasEconomicasEJuridicas => "CEJ",
        Curso.CienciasFisicasEBiologicas   => "CFB",
        _                                  => ""
    };

    public static bool SuportaDecimaTeirceira(this Curso c) =>
        c == Curso.ContabilidadeEGestao || c == Curso.InformaticaDeGestao;
}
