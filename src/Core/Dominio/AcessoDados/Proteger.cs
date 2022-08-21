using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Dominio
{
    [IgnorarAtributoTS]
    public class NaoAbrirPropriedade : Attribute
    {

    }

    [IgnorarAtributoTS]
    public class ProtegerAttribute : Attribute
    {
        public bool MostrarConsultaPelaChavePrimaria { get; set; }

        public string MascaraProtecao { get; set; } = "###*##";

        public int MaximoExibicao { get; set; } = 50;

        public TimeSpan EspacoTempo { get; set; } = TimeSpan.FromHours(1);
    }

    public interface IPropriedadesDesbloqueada
    {
        string NomeEntidade { get; set; }

        string NomeProprieade { get; set; }

        EnumTipoDesbloqueio EnumTipoDesbloqueio { get; set; }
    }

    public enum EnumTipoDesbloqueio
    {
        Tudo = 1,
        UmPorVez = 2,
    }
}
