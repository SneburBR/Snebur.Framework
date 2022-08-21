using System;
using System.Collections.Generic;
using System.Linq;
using Snebur.Utilidade;

namespace Snebur.AcessoDados.Estrutura
{
    internal partial class EstruturaEntidade
    {
     

        internal EstruturaRelacao RetornarEstruturaRelacao(string chave,
                                                           bool incluirEstruuraBase = true,
                                                           bool ignorarErro = false)
        {
            EstruturaRelacao estruturaRelacao;

            if (this.EstruturasRelacoes.TryGetValue(chave, out estruturaRelacao))
            {
                return estruturaRelacao;
            }
            else
            {
                if (incluirEstruuraBase && this.EstruturaEntidadeBase != null)
                {
                    return this.EstruturaEntidadeBase.RetornarEstruturaRelacao(chave, incluirEstruuraBase, ignorarErro);
                }
                if (!ignorarErro)
                {
                    throw new Erro(String.Format("A relação {0} não foi encontrado nem {1}", chave, this.TipoEntidade.Name));
                }
                return null;
            }
        }

        internal EstruturaRelacao RetornarEstruturaRelacao(string chave,
                                                          EnumOpcaoRetornarRelacao opcoes,
                                                          bool ignorarErro,
                                                          BaseRelacaoAberta relacaoAberta)
        {

            switch (opcoes)
            {
                case EnumOpcaoRetornarRelacao.EntidadeAtual:

                    return this.RetornarEstruturaRelacao(chave, false, ignorarErro);

                case EnumOpcaoRetornarRelacao.EntidadesBase:

                    return this.RetornarEstruturaRelacao(chave, true, ignorarErro);

                case EnumOpcaoRetornarRelacao.Tudo:

                    return this.RetornarEstruturaRelacaoEstruturasEspecializadas(chave, ignorarErro, relacaoAberta);

                default:

                    throw new Erro(String.Format("O a opção {0} não é suportada ", EnumUtil.RetornarDescricao(opcoes)));
            }
        }

        private EstruturaRelacao RetornarEstruturaRelacaoEstruturasEspecializadas(string chave, bool ignorarErro, BaseRelacaoAberta relacaoAberta)
        {
            var estruturaRelacao = this.RetornarEstruturaRelacao(chave, true, true);
            if (estruturaRelacao != null)
            {
                return estruturaRelacao;
            }
            else
            {
                if (!String.IsNullOrEmpty(relacaoAberta.TipoDeclaradoAssemblyQualifiedName))
                {
                    var tipo = Type.GetType(relacaoAberta.TipoDeclaradoAssemblyQualifiedName);
                    if (tipo == null)
                    {
                        throw new Erro($"O tipo {relacaoAberta.TipoDeclaradoAssemblyQualifiedName}  não foi encontrado");

                    }

                    var estruturaEntidade = EstruturaBancoDados.Atual.EstruturasEntidade[tipo.Name];
                    return estruturaEntidade.RetornarEstruturaRelacao(chave, true, false);
                }
                else
                {
                    var estruturasRelacaoEncontradas = new List<EstruturaRelacao>();
                    var estruturasEspecializadasUltimoNivel = this.RetornarEstruturasEspecializadaUltimoNivel();
                    foreach (var estruturaEspecializada in estruturasEspecializadasUltimoNivel)
                    {
                        var estrutraRelacaoEspecializa = estruturaEspecializada.RetornarEstruturaRelacao(chave, true, true);
                        if (estrutraRelacaoEspecializa != null)
                        {
                            estruturasRelacaoEncontradas.Add(estrutraRelacaoEspecializa);
                        }
                    }
                    if (estruturasRelacaoEncontradas.Count == 1)
                    {
                        return estruturasRelacaoEncontradas.Single();
                    }
                    if (estruturasRelacaoEncontradas.Count > 1)
                    {



                        throw new Erro(String.Format("Foram encotrados {0}  relações '{1}' nas classes especializadas de  {1}", estruturasRelacaoEncontradas.Count,
                                                                                                                                                        chave, this.TipoEntidade.Name));
                    }
                    if (!ignorarErro)
                    {
                        throw new Erro(String.Format("A relação {0} não foi encontrado nem {1}{2}", chave, this.Schema, this.NomeTabela));
                    }
                    return null;
                }

            }
        }

        private List<EstruturaEntidade> RetornarEstruturasEspecializadaUltimoNivel()
        {
            var estruturasEntidade = new List<EstruturaEntidade>();
            this.PreencherUltimosNivelEstruturaEntidadeEspecializada(estruturasEntidade, this);
            return estruturasEntidade;
        }

        private void PreencherUltimosNivelEstruturaEntidadeEspecializada(List<EstruturaEntidade> estruturasEntidadeEspecializada,
                                                                         EstruturaEntidade estruturaEntidade)
        {
            if (estruturaEntidade.EstruturasEntidadeEspecializada.Count == 0)
            {
                estruturasEntidadeEspecializada.Add(estruturaEntidade);
            }
            foreach (var estruturaEspecializada in estruturaEntidade.EstruturasEntidadeEspecializada.Values)
            {
                this.PreencherUltimosNivelEstruturaEntidadeEspecializada(estruturasEntidadeEspecializada, estruturaEspecializada);
            }
        }
    }
}