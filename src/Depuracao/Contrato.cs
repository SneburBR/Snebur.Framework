using System;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;

namespace Snebur.Depuracao
{
    public class Contrato : BaseDominio
    {

        #region Campos Privados


        #endregion

        public Mensagem Mensagem { get; set; }

        [IgnorarConstrutorTSAttribute]
        public Contrato()
        {
             
        }

        public Contrato(Mensagem mensagem)
        {
            this.Mensagem = mensagem;
        }
    }
}