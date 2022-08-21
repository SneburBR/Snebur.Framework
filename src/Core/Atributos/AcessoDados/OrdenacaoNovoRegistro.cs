using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class OrdenacaoNovoRegistroAttribute : Attribute
    {
        public EnumOrdenacaoNovoRegistro OrdenacaoNovoRegistro { get; set; }

        public OrdenacaoNovoRegistroAttribute(EnumOrdenacaoNovoRegistro ordenacaoNovoRegistro)
        {
            this.OrdenacaoNovoRegistro = ordenacaoNovoRegistro;
        }
    }
    [IgnorarGlobalizacao]
    public enum EnumOrdenacaoNovoRegistro
    {
        Inicio = 1,
        Fim = 2,
        Aleatorio = 3
    }
}