using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoDataHoraServidorAttribute : SomenteLeituraAttribute
    {
        public bool DataHoraUTC { get; set; }

        public bool PermitirAtualizacao { get; set; }

        public override OpcoesSomenteLeitura OpcoesSomenteLeitura => new OpcoesSomenteLeitura(!this.PermitirAtualizacao, this.IsNotificarSeguranca);

        public ValorPadraoDataHoraServidorAttribute(bool dataHoraUTC = true, bool permitirAtualizacao = false)
        {
            this.DataHoraUTC = dataHoraUTC;
            this.PermitirAtualizacao = permitirAtualizacao;
            this.IsNotificarSeguranca = false;
        }

        public object RetornarValorPadrao()
        {
            return null;
        }
    }
}
