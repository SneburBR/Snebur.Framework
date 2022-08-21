using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.Collections;

namespace Snebur.AcessoDados.Mapeamento
{
    internal partial class MapeamentoConsulta
    {

        private void AbrirRelacoes()
        {
            if (this.Entidades.Count > 0)
            {
                foreach (var mapeamento in this.MapeamentosRelacaoAberta.Values)
                {
                    if (mapeamento is MapeamentoConsultaRelacaoAbertaNn)
                    {
                        this.AbrirRelacaoNn((MapeamentoConsultaRelacaoAbertaNn)mapeamento);
                    }
                    else
                    {
                        
                        var filtroMapeamento = this.RetornarFiltroMapeamento(mapeamento);
                        
                        var entidades = mapeamento.RetornarEntidades(filtroMapeamento);
                        this.MapearRelacaoAberta(entidades, mapeamento);
                    }
                }
            }
        }
        #region RetornarFiltroMapeamento

        private BaseFiltroMapeamento RetornarFiltroMapeamento(MapeamentoConsultaRelacaoAberta mapeamento)
        {
            if (mapeamento is MapeamentoConsultaRelacaoAbertaPai)
            {
            }
            switch (mapeamento)
            {
                case MapeamentoConsultaRelacaoAbertaPai mapeamentoConsutlaRelacaoAbertaPai:

                    return this.RetornarFiltroMapeamentoRelacaoAbertaPai(mapeamentoConsutlaRelacaoAbertaPai);

                case MapeamentoConsultaRelacaoAbertaUmUm mapeamentoConsultaRelacaoAbertaUmUm:

                    return this.RetornarFiltroMapeamentoRelacaoAbertaUmUm(mapeamentoConsultaRelacaoAbertaUmUm);

                case MapeamentoConsultaRelacaoAbertaUmUmReversa mapeamentoConsultaRelacaoAbertaUmUmReversa:

                    return this.RetornarFiltroMapeamentoRelacaoAbertaUmUmReversa(mapeamentoConsultaRelacaoAbertaUmUmReversa);

                case MapeamentoConsultaRelacaoAbertaFilhos mapeamentoConsultaRelacaoAbertaFilhos:

                    return this.RetornarFiltroMapeamentoFilhos(mapeamentoConsultaRelacaoAbertaFilhos);

                default:

                    throw new ErroNaoSuportado($"O mapeamento {mapeamento.GetType().Name}  não é suprotado");
            }
        }

        private BaseFiltroMapeamento RetornarFiltroMapeamentoRelacaoAbertaPai(MapeamentoConsultaRelacaoAbertaPai mapeamentoPai)
        {
            var ids = new SortedSet<long>();
            var propriedadeChaveEstrangeira = mapeamentoPai.EstruturaRelacaoPai.EstruturaCampoChaveEstrangeira.Propriedade;
            foreach (var entidade in this.Entidades.Values)
            {
                if (entidade != null && ReflexaoUtil.TipoIgualOuHerda(entidade.GetType(), propriedadeChaveEstrangeira.DeclaringType))
                {
                    var idChaveEstrangeira = Convert.ToInt64(propriedadeChaveEstrangeira.GetValue(entidade));
                    if (idChaveEstrangeira > 0)
                    {
                        if (!ids.Contains(idChaveEstrangeira))
                        {
                            ids.Add(idChaveEstrangeira);
                        }
                    }
                }
            }
            if (ids.Count == 0)
            {
                //gambiarra
                ids.Add(0);
            }
            return new FiltroMapeamentoIds(ids);
        }

        private BaseFiltroMapeamento RetornarFiltroMapeamentoRelacaoAbertaUmUm(MapeamentoConsultaRelacaoAbertaUmUm mapeamentoUmUm)
        {
            var ids = new SortedSet<long>();
            var propriedadeChaveEstrangeira = mapeamentoUmUm.EstruturaRelacaoUmUm.EstruturaCampoChaveEstrangeira.Propriedade;
            foreach (var entidade in this.Entidades.Values)
            {
                var idChaveEstrangeira = Convert.ToInt64(propriedadeChaveEstrangeira.GetValue(entidade));
                if (idChaveEstrangeira > 0)
                {
                    if (!ids.Contains(idChaveEstrangeira))
                    {
                        ids.Add(idChaveEstrangeira);
                    }
                }
            }
            if (ids.Count == 0)
            {
                //gambiarra
                ids.Add(0);
            }
            return new FiltroMapeamentoIds(ids);
        }

        private BaseFiltroMapeamento RetornarFiltroMapeamentoRelacaoAbertaUmUmReversa(MapeamentoConsultaRelacaoAbertaUmUmReversa mapeamentoUmUm)
        {
            var ids = new SortedSet<long>();
            var propriedadeChaveEstrangeiraReversa = mapeamentoUmUm.EstruturaRelacaoUmUmReversa.EstruturaCampoChaveEstrageiraReversa.Propriedade;
            foreach (var entidade in this.Entidades.Values)
            {
                if (!ids.Contains(entidade.Id))
                {
                    ids.Add(entidade.Id);
                }
            }
            if (ids.Count == 0)
            {
                //gambiarra
                ids.Add(0);
            }
            return new FiltroMapeamentoReverso(mapeamentoUmUm.EstruturaRelacaoUmUmReversa.EstruturaCampoChaveEstrageiraReversa, ids);
        }

        private BaseFiltroMapeamento RetornarFiltroMapeamentoFilhos(MapeamentoConsultaRelacaoAbertaFilhos mapeamentoFilhos)
        {
            var idsChavePrimara = new SortedSet<long>(this.Entidades.Keys);
            var estruturaCampoChaveEstrangeira = mapeamentoFilhos.EstruturaRelacaoFilhos.EstruturaCampoChaveEstrangeira;
            return new FiltroMapeamentoIds(estruturaCampoChaveEstrangeira, idsChavePrimara);
        }
        #endregion

        private void MapearRelacaoAberta(Dictionary<long, Entidade> entidades, MapeamentoConsultaRelacaoAberta mapeamento)
        {
            switch (mapeamento)
            {
                case MapeamentoConsultaRelacaoAbertaPai mapeamentoConsultaRelacaoAbertaPai:

                    this.MapearRelacaoAbertaPai(entidades, mapeamentoConsultaRelacaoAbertaPai);
                    break;

                case MapeamentoConsultaRelacaoAbertaUmUm mapeamentoConsultaRelacaoAbertaUmUm:

                    this.MapearRelacaoAbertaUmUm(entidades, mapeamentoConsultaRelacaoAbertaUmUm);
                    break;

                case MapeamentoConsultaRelacaoAbertaUmUmReversa mapeamentoConsultaRelacaoAbertaUmUmReversa:

                    this.MapearRelacaoAbertaUmUmReversa(entidades, mapeamentoConsultaRelacaoAbertaUmUmReversa);
                    break;

                case MapeamentoConsultaRelacaoAbertaFilhos mapeamentoConsultaRelacaoAbertaFilhos:

                    this.MapearRelacaoAbertaFilhos(entidades, mapeamentoConsultaRelacaoAbertaFilhos);
                    break;

                default:

                    throw new ErroNaoSuportado($"O mapeamento {mapeamento.GetType().Name} não é suportado");
            }
        }

        private void MapearRelacaoAbertaPai(Dictionary<long, Entidade> entidadesRelacaoPai, MapeamentoConsultaRelacaoAbertaPai mapeamentoPai)
        {
            var propriedadeChaveEstrangeira = mapeamentoPai.EstruturaRelacaoPai.EstruturaCampoChaveEstrangeira.Propriedade;
            var propriedadeRelacaoPai = mapeamentoPai.EstruturaRelacao.Propriedade;

            foreach (var entidade in this.Entidades.Values)
            {
                if (entidade != null && ReflexaoUtil.TipoIgualOuHerda(entidade.GetType(), propriedadeChaveEstrangeira.DeclaringType))
                {
                    var idChaveEstrangeira = Convert.ToInt64(propriedadeChaveEstrangeira.GetValue(entidade));
                    if (idChaveEstrangeira > 0)
                    {
                        if (!entidadesRelacaoPai.ContainsKey(idChaveEstrangeira))
                        {
                            throw new Erro(String.Format("Não foi encontrado a chave estrangeira {0} ", idChaveEstrangeira));
                        }
                        var entidadeRelacaoPai = entidadesRelacaoPai[idChaveEstrangeira];
                        propriedadeRelacaoPai.SetValue(entidade, entidadeRelacaoPai);
                    }
                }

            }
        }

        private void MapearRelacaoAbertaUmUm(Dictionary<long, Entidade> entidadesRelacaoPai, MapeamentoConsultaRelacaoAbertaUmUm mapeamentoUmUm)
        {
            var propriedadeChaveEstrangeira = mapeamentoUmUm.EstruturaRelacaoUmUm.EstruturaCampoChaveEstrangeira.Propriedade;
            var propriedadeRelacaoUmUm = mapeamentoUmUm.EstruturaRelacao.Propriedade;

            foreach (var entidade in this.Entidades.Values)
            {
                var idChaveEstrangeira = Convert.ToInt64(propriedadeChaveEstrangeira.GetValue(entidade));
                if (idChaveEstrangeira > 0)
                {
                    if (!entidadesRelacaoPai.ContainsKey(idChaveEstrangeira))
                    {
                        throw new Erro(String.Format("Não foi encontrado a chave estrangeira {0} ", idChaveEstrangeira));
                    }
                    var entidadeRelacaoPai = entidadesRelacaoPai[idChaveEstrangeira];
                    propriedadeRelacaoUmUm.SetValue(entidade, entidadeRelacaoPai);
                }
            }
        }

        private void MapearRelacaoAbertaUmUmReversa(Dictionary<long, Entidade> entidadesRelacaaoReversa, MapeamentoConsultaRelacaoAbertaUmUmReversa mapeamentoUmUmREversa)
        {
            var propriedadeChaveEstrangeira = mapeamentoUmUmREversa.EstruturaRelacaoUmUmReversa.EstruturaCampoChaveEstrageiraReversa.Propriedade;
            var propriedadeRelacaoUmUmReversa = mapeamentoUmUmREversa.EstruturaRelacao.Propriedade;
            var propriedadeRelacao = mapeamentoUmUmREversa.EstruturaRelacaoUmUmReversa.EstruturaRelacaoUmUm?.Propriedade;

            foreach (var entidadeRelacaoReversa in entidadesRelacaaoReversa.Values)
            {
                var idChaveEstrangeiraReversa = Convert.ToInt64(propriedadeChaveEstrangeira.GetValue(entidadeRelacaoReversa));
                if (idChaveEstrangeiraReversa > 0)
                {
                    if (!this.Entidades.ContainsKey(idChaveEstrangeiraReversa))
                    {
                        throw new Erro(String.Format("Não foi encontrado a chave estrangeira {0} ", idChaveEstrangeiraReversa));
                    }
                    var entidade = this.Entidades[idChaveEstrangeiraReversa];
                    propriedadeRelacaoUmUmReversa.SetValue(entidade, entidadeRelacaoReversa);

                    //############################ CIRCULAR ########################
                    //atenção na hora de serializar
                    //referencia circular
                    propriedadeRelacao?.SetValue(entidadeRelacaoReversa, entidade);
                }
            }
        }

        private void MapearRelacaoAbertaFilhos(Dictionary<long, Entidade> entidadesFilho, MapeamentoConsultaRelacaoAbertaFilhos mapeamentoFilhos)
        {
            var gruposEntidadeFilhos = new Dictionary<long, List<Entidade>>();
            foreach (var entidade in this.Entidades)
            {
                gruposEntidadeFilhos.Add(entidade.Key, new List<Entidade>());
            }
            var propriedadeRelacaoFilhos = mapeamentoFilhos.EstruturaRelacaoFilhos.Propriedade;
            var tipoEntidadeFilho = ReflexaoUtil.RetornarTipoGenericoColecao(propriedadeRelacaoFilhos.PropertyType);
            var estruturaCampoChaveEstrangeira = mapeamentoFilhos.EstruturaRelacaoFilhos.EstruturaCampoChaveEstrangeira;
            var propriedadeChaveEstrangeira = estruturaCampoChaveEstrangeira.Propriedade;

            foreach (var entidadeFilho in entidadesFilho.Values)
            {
                var idChavaEstrangeira = ConverterUtil.ParaInt64(propriedadeChaveEstrangeira.GetValue(entidadeFilho));
                gruposEntidadeFilhos[idChavaEstrangeira].Add(entidadeFilho);
            }
            foreach (var grupoEntidadeFilhos in gruposEntidadeFilhos)
            {
                var entidade = this.Entidades[grupoEntidadeFilhos.Key];

                var tipoListaEntidade = typeof(ListaEntidades<>).MakeGenericType(tipoEntidadeFilho);
                var listaEntidadeFilhos = (IListaEntidades)Activator.CreateInstance(tipoListaEntidade);
                listaEntidadeFilhos.AdicionarEntidades(grupoEntidadeFilhos.Value);
                listaEntidadeFilhos.IsAberta = true;
                if(listaEntidadeFilhos.Count > 0)
                {
                    if(propriedadeRelacaoFilhos.GetSetMethod() == null)
                    {
                        throw new Erro($"A propriedade {propriedadeRelacaoFilhos.Name} declarada na entidade {propriedadeRelacaoFilhos.DeclaringType.Name} é somente leitura, Definir {{ get; set; }}");
                    }

                    propriedadeRelacaoFilhos.SetValue(entidade, listaEntidadeFilhos);
                }
               
            }
        }
    }
}