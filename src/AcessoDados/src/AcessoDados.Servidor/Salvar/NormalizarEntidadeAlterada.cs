using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal partial class NormalizarEntidadeAlterada : IDisposable
    {
        internal List<EntidadeAlterada> EntidadesAlterada { get; set; }

        internal BaseContextoDados Contexto { get; set; }

        internal HashSet<Guid> IdentificadorUnicoEntidadesAnalisadas { get; set; }

        internal Dictionary<string, EntidadeAlterada> EntidadesAlteradasNormalizadas { get; set; }

        private NormalizarEntidadeAlterada(BaseContextoDados contexto, 
                                           List<EntidadeAlterada> entidadesAlteradas)
        {
            this.EntidadesAlteradasNormalizadas = new Dictionary<string, EntidadeAlterada>();
            this.Contexto = contexto;
            this.EntidadesAlterada = entidadesAlteradas;
        }

        internal List<EntidadeAlterada> RetornarEntidadesAlteradaNormalizada()
        {
            this.Normalizar();

            foreach (var entidadeAlterada in this.EntidadesAlteradasNormalizadas.Values)
            {
                entidadeAlterada.AtualizarEntidadesDepedentes();
            }
            return this.EntidadesAlteradasNormalizadas.Values.ToList();
        }

        internal void Normalizar()
        {
            foreach (var entidade in this.EntidadesAlterada)
            {
                this.Normalizar(entidade);
            }
        }

        private void Normalizar(EntidadeAlterada entidadeAlterada)
        {
            if (!this.EntidadesAlteradasNormalizadas.ContainsKey(entidadeAlterada.IdentificadorEntidade))
            {
                this.EntidadesAlteradasNormalizadas.Add(entidadeAlterada.IdentificadorEntidade, entidadeAlterada);

                var estruturaEntidade = this.Contexto.EstruturaBancoDados.EstruturasEntidade[entidadeAlterada.Entidade.GetType().Name];

                this.NormalizarRelacoesFilho(entidadeAlterada.Entidade, estruturaEntidade);
                this.NormalizarRelacoesNn(entidadeAlterada.Entidade, estruturaEntidade);
            }
        }

        private void NormalizarRelacoesFilho(Entidade entidade, EstruturaEntidade estruturaEntidade)
        {
            var estruturasRelacoesFilhos = estruturaEntidade.TodasRelacoesFilhos;
            foreach (var estruturaRelacaoFilhos in estruturasRelacoesFilhos)
            {
                var entidadesFilho = (IListaEntidades)estruturaRelacaoFilhos.Propriedade.GetValue(entidade);
                if (entidadesFilho.EntidadesRemovida.Count > 0)
                {
                    //TODO: Remover relacoes filhos desabilitar
                    //throw new ErroNaoImplementado();
                }
            }
        }

        private void NormalizarRelacoesNn(Entidade entidade, EstruturaEntidade estruturaEntidade)
        {
            var estruturasRelacoesNn = estruturaEntidade.TodasRelacoesNn();
            foreach (var estruturaRelacaoNn in estruturasRelacoesNn)
            {
                var entidadesFilhosNn = (IListaEntidades)estruturaRelacaoNn.Propriedade.GetValue(entidade);
                foreach (Entidade entidadeFilhoNn in entidadesFilhosNn)
                {
                    var consultaEntidadeRelacaoNn = this.Contexto.RetornarConsulta<Entidade>(estruturaRelacaoNn.EstruturaEntidadeRelacaoNn.TipoEntidade);

                    consultaEntidadeRelacaoNn.AdicionarFiltroPropriedade(estruturaRelacaoNn.EstruturaCampoChaveEstrangeiraPai.Propriedade, EnumOperadorFiltro.Igual, entidade.Id);
                    consultaEntidadeRelacaoNn.AdicionarFiltroPropriedade(estruturaRelacaoNn.EstruturaCampoChaveEstrangeiraFilho.Propriedade, EnumOperadorFiltro.Igual, entidadeFilhoNn.Id);

                    var entidadeRelacaoNn = consultaEntidadeRelacaoNn.SingleOrDefault();
                    if (entidadeRelacaoNn == null)
                    {
                        entidadeRelacaoNn = (Entidade)Activator.CreateInstance(estruturaRelacaoNn.EstruturaEntidadeRelacaoNn.TipoEntidade);

                        //Pai
                        if (!entidade.__IsNewEntity)
                        {
                            estruturaRelacaoNn.EstruturaCampoChaveEstrangeiraPai.Propriedade.SetValue(entidadeRelacaoNn, entidade.Id);
                        }
                        else
                        {
                            estruturaRelacaoNn.EstruturaRelacaoPaiEntidadePai.Propriedade.SetValue(entidadeRelacaoNn, entidade);
                        }
                        //Filho
                        if (!entidadeFilhoNn.__IsNewEntity)
                        {
                            estruturaRelacaoNn.EstruturaCampoChaveEstrangeiraFilho.Propriedade.SetValue(entidadeRelacaoNn, entidadeFilhoNn.Id);
                        }
                        else
                        {
                            estruturaRelacaoNn.EstruturaRelacaoPaiEntidadeFilho.Propriedade.SetValue(entidadeRelacaoNn, entidadeFilhoNn);
                        }
                        var novaEntidadeAlterada = new EntidadeAlterada(this.Contexto, entidadeRelacaoNn, estruturaRelacaoNn.EstruturaEntidadeRelacaoNn, EnumOpcaoSalvar.Salvar);
                        this.EntidadesAlteradasNormalizadas.Add(novaEntidadeAlterada.IdentificadorEntidade, novaEntidadeAlterada);
                    }
                }
                if (entidadesFilhosNn.EntidadesRemovida.Count > 0)
                {
                    foreach (Entidade entidadeFilhoNnRemovida in entidadesFilhosNn.EntidadesRemovida)
                    {
                        var consultaEntidadeRelacaoNn = this.Contexto.RetornarConsulta<Entidade>(estruturaRelacaoNn.EstruturaEntidadeRelacaoNn.TipoEntidade);

                        consultaEntidadeRelacaoNn.AdicionarFiltroPropriedade(estruturaRelacaoNn.EstruturaCampoChaveEstrangeiraPai.Propriedade, EnumOperadorFiltro.Igual, entidade.Id);
                        consultaEntidadeRelacaoNn.AdicionarFiltroPropriedade(estruturaRelacaoNn.EstruturaCampoChaveEstrangeiraFilho.Propriedade, EnumOperadorFiltro.Igual, entidadeFilhoNnRemovida.Id);
                        var entidadeRelacaoNn = consultaEntidadeRelacaoNn.SingleOrDefault();
                        if (entidadeRelacaoNn != null)
                        {
                            var entidadeDeletadas = new EntidadeAlterada(this.Contexto, entidadeRelacaoNn, estruturaRelacaoNn.EstruturaEntidadeRelacaoNn, EnumOpcaoSalvar.Deletar);
                            this.EntidadesAlteradasNormalizadas.Add(entidadeDeletadas.IdentificadorEntidade, entidadeDeletadas);
                        }
                    }
                }
            }
        }
        #region IDisposable

        public void Dispose()
        {
        }
        #endregion
    }
}