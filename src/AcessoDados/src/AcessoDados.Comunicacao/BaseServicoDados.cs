using System.Collections.Generic;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Comunicacao
{
    public abstract class BaseServicoContextoDados<TContextoDados> : BaseServicoComunicacaoDados<TContextoDados>, IServicoDados
                    where TContextoDados : BaseContextoDados
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

        public ResultadoSalvar Salvar(List<Entidade> entidades)
        {
            return this.ContextoDados.Salvar(entidades, true);
        }

        public ResultadoExcluir Excluir(List<Entidade> entidades, string relacoesEmCascata)
        {
            return this.ContextoDados.Excluir(entidades, relacoesEmCascata, true);
        }

        #endregion


    }
}