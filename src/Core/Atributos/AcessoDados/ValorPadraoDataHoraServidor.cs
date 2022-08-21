using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoDataHoraServidorAttribute : SomenteLeituraAttribute
    {
        public bool DataHoraUTC { get; set; }

        public bool PermitirAtualizacao { get; set; }

        public ValorPadraoDataHoraServidorAttribute(bool dataHoraUTC = true, bool permitirAtualizacao = false)
        {
            this.DataHoraUTC = dataHoraUTC;
            this.PermitirAtualizacao = permitirAtualizacao;
        }

        public object RetornarValorPadrao()
        {
            return null;
        }
    }
}
