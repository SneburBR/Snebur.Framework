using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.AcessoDados.Estrutura;
using Snebur.AcessoDados.Dominio;
using System.Reflection;

namespace Snebur.AcessoDados.Mapeamento
{
    internal partial class MapeamentoConsulta
    {
        internal void MontarMapamentosConsultaRelacaoAberta()
        {
            var propriedadesParcial = new List<PropertyInfo>();
            var mapeamentosRelacaoAberta = new DicionarioEstrutura<MapeamentoConsultaRelacaoAberta>();

            foreach (var relacaoAberta in this.EstruturaConsulta.RetornarTodasRelacoesAberta())
            {
                var caminhoPropriedade = relacaoAberta.CaminhoPropriedade;
                var nomesPropriedade = caminhoPropriedade.Split(".".ToCharArray());
                MapeamentoConsulta mapeamentoAtual = this;

                foreach (var nomePropriedade in nomesPropriedade)
                {
                    if (mapeamentoAtual.MapeamentosRelacaoAberta.ContainsKey(nomePropriedade))
                    {
                        mapeamentoAtual = mapeamentoAtual.MapeamentosRelacaoAberta[nomePropriedade];
                    }
                    else
                    {
                        var mapeamentoRelacaoAberta = this.RetornarNovoMapeamentoConsultaRelacaoAberta(mapeamentoAtual, nomePropriedade, relacaoAberta);

                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            //validação de rotina
                            if (mapeamentoRelacaoAberta.EstruturaRelacao.Propriedade.Name != nomePropriedade)
                            {
                                throw new Exception("O nome da propriedade é diferenca relacação mapaeamento ");
                            }
                        }
                        mapeamentoAtual.MapeamentosRelacaoAberta.Add(nomePropriedade, mapeamentoRelacaoAberta);
                        //this.AdicionarRelacaoAberta(mapeamentoAtual, nomePropriedade, relacaoAbarta);
                        mapeamentoAtual = mapeamentoRelacaoAberta;
                    }
                }
            }
        }

        public MapeamentoConsultaRelacaoAberta RetornarNovoMapeamentoConsultaRelacaoAberta(MapeamentoConsulta mapeamentoConsultaPai,
                                                                                           string nomePropriedade,
                                                                                           BaseRelacaoAberta relacaoAberta)
        {
            var estruturaEntidade = mapeamentoConsultaPai.EstruturaEntidade;
            var estruturaRelacao = estruturaEntidade.RetornarEstruturaRelacao(nomePropriedade, EnumOpcaoRetornarRelacao.Tudo, false, relacaoAberta);

            var estruturaConsulta = this.RetornarEstruturaConsultaRelacao(estruturaRelacao, relacaoAberta);

            if (estruturaRelacao is EstruturaRelacaoPai)
            {
                var estruturaRelacaoPai = (EstruturaRelacaoPai)estruturaRelacao;

                return new MapeamentoConsultaRelacaoAbertaPai(estruturaConsulta,
                                                              this.EstruturaBancoDados,
                                                              this.ConexaoDB,
                                                              mapeamentoConsultaPai,
                                                              estruturaRelacaoPai,
                                                              relacaoAberta, 
                                                              this.Contexto);
            }
            if (estruturaRelacao is EstruturaRelacaoUmUm)
            {
                var estruturaRelacaoUmUm = (EstruturaRelacaoUmUm)estruturaRelacao;

                return new MapeamentoConsultaRelacaoAbertaUmUm(estruturaConsulta,
                                                               this.EstruturaBancoDados,
                                                               this.ConexaoDB,
                                                               mapeamentoConsultaPai,
                                                               estruturaRelacaoUmUm,
                                                               relacaoAberta,
                                                               this.Contexto);
            }
            if (estruturaRelacao is EstruturaRelacaoUmUmReversa)
            {
                var estruturaRelacaoUmUmReversa = (EstruturaRelacaoUmUmReversa)estruturaRelacao;

                return new MapeamentoConsultaRelacaoAbertaUmUmReversa(estruturaConsulta,
                                                                      this.EstruturaBancoDados,
                                                                      this.ConexaoDB,
                                                                      mapeamentoConsultaPai,
                                                                      estruturaRelacaoUmUmReversa,
                                                                      relacaoAberta, 
                                                                      this.Contexto);
            }
            if (estruturaRelacao is EstruturaRelacaoFilhos estruturaRelacaoFilhos)
            {
                return new MapeamentoConsultaRelacaoAbertaFilhos(estruturaConsulta,
                                                                 this.EstruturaBancoDados,
                                                                 this.ConexaoDB,
                                                                 mapeamentoConsultaPai,
                                                                 estruturaRelacaoFilhos,
                                                                 relacaoAberta,
                                                                 this.Contexto);
            }
            if (estruturaRelacao is EstruturaRelacaoNn)
            {
                var estruturaRelacaoNn = (EstruturaRelacaoNn)estruturaRelacao;

                return new MapeamentoConsultaRelacaoAbertaNn(estruturaConsulta,
                                                             this.EstruturaBancoDados,
                                                             this.ConexaoDB,
                                                             mapeamentoConsultaPai,
                                                             estruturaRelacaoNn,
                                                             relacaoAberta,
                                                             this.Contexto);
            }
            throw new ErroNaoSuportado(String.Format("O tipo estruturaRelacao não é suportado {0} ", estruturaEntidade.GetType().Name));
        }

        private EstruturaConsulta RetornarEstruturaConsultaRelacao(EstruturaRelacao estruturaRelacao, BaseRelacaoAberta relacaoAberta)
        {
            var estruturaConsulta = this.RetornarNovaEstruturaConsulta(relacaoAberta);
            switch (estruturaRelacao)
            {
                case EstruturaRelacaoPai estruturaRelacaoPai:

                    estruturaConsulta.NomeTipoEntidade = estruturaRelacaoPai.EstruturaEntidadeChaveEstrangeiraDeclarada.TipoEntidade.Name;
                    estruturaConsulta.TipoEntidadeAssemblyQualifiedName = estruturaRelacaoPai.EstruturaEntidadeChaveEstrangeiraDeclarada.TipoEntidade.AssemblyQualifiedName;
                    return estruturaConsulta;

                case EstruturaRelacaoUmUm estruturaRelacaoUmUm:

                    estruturaConsulta.NomeTipoEntidade = estruturaRelacaoUmUm.EstruturaEntidadeChaveEstrangeiraDeclarada.TipoEntidade.Name;
                    estruturaConsulta.TipoEntidadeAssemblyQualifiedName = estruturaRelacaoUmUm.EstruturaEntidadeChaveEstrangeiraDeclarada.TipoEntidade.AssemblyQualifiedName;
                    return estruturaConsulta;

                case EstruturaRelacaoUmUmReversa estruturaRelacaoUmUmReversa:

                    estruturaConsulta.NomeTipoEntidade = estruturaRelacaoUmUmReversa.EstruturaEntidadeUmUmReversa.TipoEntidade.Name;
                    estruturaConsulta.TipoEntidadeAssemblyQualifiedName = estruturaRelacaoUmUmReversa.EstruturaEntidadeUmUmReversa.TipoEntidade.AssemblyQualifiedName;
                    return estruturaConsulta;

                case EstruturaRelacaoFilhos estruturaRelacaoFilhos:

                    //if (relacaoAberta is RelacaoAbertaEntidade)
                    //{
                    //    throw new ErroOperacaoInvalida($"A relacaoAberta não ser do tipo entidade, e sim to tipo coleção");
                    //}

                    estruturaConsulta.NomeTipoEntidade = estruturaRelacaoFilhos.EstruturaEntidadeFilho.TipoEntidade.Name;
                    estruturaConsulta.TipoEntidadeAssemblyQualifiedName = estruturaRelacaoFilhos.EstruturaEntidadeFilho.TipoEntidade.AssemblyQualifiedName;
                    return estruturaConsulta;

                case EstruturaRelacaoNn estruturaRelacaoNn:

                    var tipoEntidadeConsulta = estruturaRelacaoNn.EstruturaEntidadeFilho.TipoEntidade;
                    estruturaConsulta.NomeTipoEntidade = tipoEntidadeConsulta.Name;
                    estruturaConsulta.TipoEntidadeAssemblyQualifiedName = tipoEntidadeConsulta.AssemblyQualifiedName;
                    return estruturaConsulta;

                default:

                    throw new ErroNaoSuportado($"estruturaRelacao não suportado '{estruturaRelacao.GetType().Name}'");
            }
        }

        private EstruturaConsulta RetornarNovaEstruturaConsulta(BaseRelacaoAberta relacaoAberta)
        {
            if (relacaoAberta is RelacaoAbertaColecao)
            {
                return (relacaoAberta as RelacaoAbertaColecao).EstruturaConsulta;
            }
            else
            {
                var estruturaConsulta = new EstruturaConsulta();
                estruturaConsulta.IsMostrarDeletados = this.EstruturaConsulta.IsMostrarDeletados;
                estruturaConsulta.IsMostrarInativos = this.EstruturaConsulta.IsMostrarInativos;
                return estruturaConsulta;
            }
        }

        private void AdicionarRelacaoAberta(MapeamentoConsulta mapeamentoConsulta, string nomePropriedade, BaseRelacaoAberta relacaoAberta)
        {
            var consulta = mapeamentoConsulta.EstruturaConsulta;
            if (!(consulta.RelacoesAberta.ContainsKey(nomePropriedade) || consulta.ColecoesAberta.ContainsKey(nomePropriedade)))
            {
                var estruturaEntidade = mapeamentoConsulta.EstruturaEntidade;
                var estruturaRelacao = estruturaEntidade.RetornarEstruturaRelacao(nomePropriedade, EnumOpcaoRetornarRelacao.Tudo, false, relacaoAberta);

                if (estruturaRelacao is EstruturaRelacaoFilhos)
                {
                    if (!(relacaoAberta is RelacaoAbertaColecao))
                    {
                        throw new ErroOperacaoInvalida();
                    }
                    var estruturaRelacaoFilhos = (EstruturaRelacaoFilhos)estruturaRelacao;

                    var relacaoAbartaColecao = new RelacaoAbertaColecao
                    {
                        NomeTipoEntidade = estruturaRelacaoFilhos.EstruturaEntidadeFilho.TipoEntidade.Name,
                        TipoEntidadeAssemblyQualifiedName = estruturaRelacaoFilhos.EstruturaEntidadeFilho.TipoEntidade.AssemblyQualifiedName,
                        NomeTipoDeclarado = estruturaRelacao.Propriedade.DeclaringType.Name,
                        TipoDeclaradoAssemblyQualifiedName = estruturaRelacao.Propriedade.DeclaringType.AssemblyQualifiedName,
                        CaminhoPropriedade = nomePropriedade,
                    };

                    relacaoAbartaColecao.EstruturaConsulta = ((RelacaoAbertaColecao)relacaoAberta).EstruturaConsulta;

                    consulta.ColecoesAberta.Add(nomePropriedade, relacaoAbartaColecao);

                    return;
                }
                if (estruturaRelacao is EstruturaRelacaoNn)
                {
                    if (!(relacaoAberta is RelacaoAbertaColecao))
                    {
                        throw new ErroOperacaoInvalida("relacaoAberta");
                    }
                    var estruturaRelacaoNn = (EstruturaRelacaoNn)estruturaRelacao;

                    var relacaoAbartaColecao = new RelacaoAbertaColecao
                    {
                        NomeTipoEntidade = estruturaRelacaoNn.EstruturaEntidadeFilho.TipoEntidade.Name,
                        TipoEntidadeAssemblyQualifiedName = estruturaRelacaoNn.EstruturaEntidadeFilho.TipoEntidade.AssemblyQualifiedName,
                        NomeTipoDeclarado = estruturaRelacao.Propriedade.DeclaringType.Name,
                        TipoDeclaradoAssemblyQualifiedName = estruturaRelacao.Propriedade.DeclaringType.AssemblyQualifiedName,
                        CaminhoPropriedade = nomePropriedade,
                    };

                    relacaoAbartaColecao.EstruturaConsulta = ((RelacaoAbertaColecao)relacaoAberta).EstruturaConsulta;

                    consulta.ColecoesAberta.Add(nomePropriedade, relacaoAbartaColecao);

                    return;
                }
                if (estruturaRelacao is EstruturaRelacaoPai)
                {
                    var estruturaRelacaoPai = (EstruturaRelacaoPai)estruturaRelacao;
                    var relacaoAbartaColecao = new RelacaoAbertaEntidade
                    {
                        NomeTipoEntidade = estruturaRelacaoPai.EstruturaEntidadeChaveEstrangeiraDeclarada.TipoEntidade.Name,
                        TipoEntidadeAssemblyQualifiedName = estruturaRelacaoPai.EstruturaEntidadeChaveEstrangeiraDeclarada.TipoEntidade.AssemblyQualifiedName,
                        NomeTipoDeclarado = estruturaRelacao.Propriedade.DeclaringType.Name,
                        TipoDeclaradoAssemblyQualifiedName = estruturaRelacao.Propriedade.DeclaringType.AssemblyQualifiedName,
                        CaminhoPropriedade = nomePropriedade,
                    };

                    consulta.RelacoesAberta.Add(nomePropriedade, relacaoAbartaColecao);

                    return;
                }
                if (estruturaRelacao is EstruturaRelacaoUmUm)
                {
                    var estruturaRelacaoUmUm = (EstruturaRelacaoUmUm)estruturaRelacao;
                    var relacaoAbartaColecao = new RelacaoAbertaColecao
                    {
                        NomeTipoEntidade = estruturaRelacaoUmUm.EstruturaEntidadeChaveEstrangeiraDeclarada.TipoEntidade.Name,
                        TipoEntidadeAssemblyQualifiedName = estruturaRelacaoUmUm.EstruturaEntidadeChaveEstrangeiraDeclarada.TipoEntidade.AssemblyQualifiedName,
                        NomeTipoDeclarado = estruturaRelacao.Propriedade.DeclaringType.Name,
                        TipoDeclaradoAssemblyQualifiedName = estruturaRelacao.Propriedade.DeclaringType.AssemblyQualifiedName,
                        CaminhoPropriedade = nomePropriedade,
                    };

                    consulta.ColecoesAberta.Add(nomePropriedade, relacaoAbartaColecao);
                    return;
                }
                if (estruturaRelacao is EstruturaRelacaoUmUmReversa)
                {
                    var estruturaRelacaoUmUmReversa = (EstruturaRelacaoUmUmReversa)estruturaRelacao;
                    var relacaoAbartaColecao = new RelacaoAbertaColecao
                    {
                        NomeTipoEntidade = estruturaRelacaoUmUmReversa.EstruturaEntidadeUmUmReversa.TipoEntidade.Name,
                        TipoEntidadeAssemblyQualifiedName = estruturaRelacaoUmUmReversa.EstruturaEntidadeUmUmReversa.TipoEntidade.AssemblyQualifiedName,
                        NomeTipoDeclarado = estruturaRelacao.Propriedade.DeclaringType.Name,
                        TipoDeclaradoAssemblyQualifiedName = estruturaRelacao.Propriedade.DeclaringType.AssemblyQualifiedName,
                        CaminhoPropriedade = nomePropriedade,
                    };

                    consulta.ColecoesAberta.Add(nomePropriedade, relacaoAbartaColecao);

                    return;
                }
                throw new ErroNaoSuportado("estruturaRelacao não suportada");
            }
        }
    }
}