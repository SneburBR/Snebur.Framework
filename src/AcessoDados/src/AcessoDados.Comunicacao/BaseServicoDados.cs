using Snebur.AcessoDados.Servidor.Salvar;
using Snebur.Dominio;
using System.Collections.Generic;

namespace Snebur.AcessoDados.Comunicacao
{
    public abstract class BaseServicoContextoDados<TContextoDados> : BaseServicoComunicacaoDados<TContextoDados>, IServicoDados where TContextoDados : BaseContextoDados
    {

        public BaseServicoContextoDados()
        {

        }

        #region IServicoDados

        public ResultadoConsulta RetornarResultadoConsulta(EstruturaConsulta estruturaConsulta)
        {
            return this.ContextoDados.RetornarResultadoConsulta(estruturaConsulta);

        }

        public object RetornarValorScalar(EstruturaConsulta estruturaConsulta)
        {
            return this.ContextoDados.RetornarValorScalar(estruturaConsulta);
        }

        public ResultadoSalvar Salvar(IEnumerable<IEntidade> entidades)
        {
            return this.ContextoDados.Salvar(entidades, true);
        }

        public ResultadoDeletar Deletar(IEnumerable<IEntidade> entidades, string relacoesEmCascata)
        {
            return this.ContextoDados.Deletar(entidades, relacoesEmCascata);
        }

        #endregion


    }
}