using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoIDUsuarioLogadoAttribute : Attribute
    {
        public bool IsPermitirUsuarioAnonimo { get; }

        public bool IsFiltrarUsuario { get; set; } = false;

        public bool IsFiltrarUsuarioAdministrador { get; set; } = false;

        public ValorPadraoIDUsuarioLogadoAttribute(bool isPermitirUsuarioAnonimo = false)
        {
            this.IsPermitirUsuarioAnonimo = isPermitirUsuarioAnonimo;
        }
    }
}