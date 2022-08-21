using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class MaximoRegistroPorConsultaAttribute : Attribute
    {
        public int MaximoRegistroPorConsulta { get; set; }

        public MaximoRegistroPorConsultaAttribute(int maximoRegistroPorConsulta)
        {
            this.MaximoRegistroPorConsulta = maximoRegistroPorConsulta;
        }
    }
}