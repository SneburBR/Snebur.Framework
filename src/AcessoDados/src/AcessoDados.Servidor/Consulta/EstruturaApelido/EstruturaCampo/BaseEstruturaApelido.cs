using Snebur.AcessoDados.Estrutura;
using System;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class EstruturaCampoApelido : BaseEstruturaApelido
    {

        internal string CaminhoPropriedade { get; set; }

        internal EstruturaCampo EstruturaCampo { get; set; }

        internal EstruturaEntidadeApelido EstruturaEntidadeApelido { get; set; }
        public string ChavePropriedadeAdicional => this.CaminhoPropriedade.Replace("_", ".");

        //public string ChavePropriedade { get; internal set; }

        internal EstruturaCampoApelido(BaseMapeamentoConsulta mapeamentoConsulta,
                                       string caminhoPropriedade,
                                       EstruturaCampo estruturaCampo,
                                       EstruturaEntidadeApelido estruturaEntidadeApelido,
                                       string apelido,
                                       string caminhoBanco) : base(mapeamentoConsulta, apelido, caminhoBanco)
        {
            this.CaminhoPropriedade = caminhoPropriedade;
            this.EstruturaCampo = estruturaCampo;
            this.EstruturaEntidadeApelido = estruturaEntidadeApelido;
             
        }

        private string RetornarChavePropriedade()
        {
            var posicaoUltimoPonto = this.CaminhoPropriedade.LastIndexOf(".");
            var inicio = this.CaminhoPropriedade.Substring(0, posicaoUltimoPonto - 1);
            var fim = this.CaminhoPropriedade.Substring(posicaoUltimoPonto);
            return $"{inicio}_{fim}";
        }

        internal string RetornarCaminhoBancoApelido()
        {
            return String.Format("{0} As {1} ", this.CaminhoBanco, this.Apelido);
        }

        public override string ToString()
        {
            return String.Format("{0} - {1} As {2} ", this.CaminhoPropriedade, this.CaminhoBanco, this.Apelido);
        }
    }
}