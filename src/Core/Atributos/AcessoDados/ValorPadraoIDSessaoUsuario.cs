using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoIDSessaoUsuarioAttribute : Attribute
    {
        public bool IsSomenteCadastro { get; }

        public ValorPadraoIDSessaoUsuarioAttribute(bool isSomenteCadastro)
        {
            this.IsSomenteCadastro = isSomenteCadastro;
        }
    }
}