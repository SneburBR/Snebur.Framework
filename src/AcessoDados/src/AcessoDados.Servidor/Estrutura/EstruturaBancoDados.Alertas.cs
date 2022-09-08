using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Linq;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Snebur.AcessoDados.Estrutura
{
    internal partial class EstruturaBancoDados
    {
        private void AnalisarAlertasEstruturaEntidade()
        {
            if (DebugUtil.IsAttached)
            {
                this.AnalisarPadraoNomeclaturaRelacoesChaveEstrangeira();
                this.AnalisarPropriedaadesChaveEstrangeiraSemRelacao();
                this.AnalisarAlertasEntidades();
                this.AnalisarNotificacaoPropriedadesAlteradas();

                if (this.Alertas.Count > 0)
                {
                    var mensagem = String.Join(System.Environment.NewLine, this.Alertas);
                    throw new Exception(mensagem);
                }
            }
        }

        private void AnalisarPropriedaadesChaveEstrangeiraSemRelacao()
        {
            foreach (var estruturaEntidade in this.EstruturasEntidade.Values)
            {
                foreach (var estruturaCampo in estruturaEntidade.EstruturasCampos.Values)
                {
                    var propriedade = estruturaCampo.Propriedade;
                    if (propriedade.PropertyType == typeof(string))
                    {
                        var atributoValidacaoTextoTamanho = propriedade.GetCustomAttribute<ValidacaoTextoTamanhoAttribute>();
                        if (atributoValidacaoTextoTamanho == null)
                        {
                            this.Alertas.Add($" A propriedade {propriedade.Name} da entidade {estruturaEntidade.TipoEntidade.Name} do tipo {nameof(String)} não possui o atributo {nameof(ValidacaoTextoTamanhoAttribute)} ");
                        }
                    }
                }
                var propriedades = ReflexaoUtil.RetornarPropriedades(estruturaEntidade.TipoEntidade).Where(x => ReflexaoUtil.PropriedadePossuiAtributo(x, typeof(ChaveEstrangeiraAttribute)) ||
                                                                                                                ReflexaoUtil.PropriedadePossuiAtributo(x, typeof(ChaveEstrangeiraRelacaoUmUmAttribute))).ToList();
                foreach (var propriedade in propriedades)
                {
                    var atributos = propriedade.GetCustomAttributes<BaseRelacaoAttribute>();
                    if (atributos.Count() > 1)
                    {
                        this.Alertas.Add($" A propriedade {propriedade.Name} da entidade {estruturaEntidade.TipoEntidade.Name}  possui mais de um atributo relação ");
                    }
                    else if (atributos.Count() == 0)
                    {
                        this.Alertas.Add($" A propriedade {propriedade.Name} da entidade {estruturaEntidade.TipoEntidade.Name} não possui relação ");
                    }
                    else
                    {
                        var atributo = atributos.Single();
                        if (!(atributo is RelacaoChaveEstrangeiraAttribute))
                        {
                            this.Alertas.Add($" A atributo {atributo.GetType().Name} da propriedade {propriedade.Name} não é um relação chave estrangeira");
                        }
                    }
                }
            }
        }

        private void AnalisarPadraoNomeclaturaRelacoesChaveEstrangeira()
        {
            foreach (var estruturaEntidade in this.EstruturasEntidade.Values)
            {
                var relacoes = estruturaEntidade.RetornarTodasRelacoesChaveEstrangeira();
                foreach (var relacao in relacoes)
                {
                    var nomeCampo = relacao.EstruturaCampoChaveEstrangeira.NomeCampo.ToLower();
                    if (!(nomeCampo.StartsWith("id") || nomeCampo.EndsWith("_id")))
                    {
                        this.Alertas.Add($" a chave estrangeira da propriedade {relacao.Propriedade.Name} na entidade {estruturaEntidade.TipoEntidade.Name} não possui o id no nome");
                    }
                    else
                    {
                        var nomePropriedade = relacao.Propriedade.Name.ToLower();
                        string nomeCampoSemId = null;
                        if (nomeCampo.EndsWith("_id"))
                        {
                            nomeCampoSemId = nomeCampo.Substring(0, nomeCampo.Length - 3);

                            if (nomePropriedade.EndsWith("id"))
                            {
                                nomePropriedade = nomePropriedade.Substring(0, nomePropriedade.Length - 2);
                            }
                        }
                        else if (nomeCampo.StartsWith("id"))
                        {
                            nomeCampoSemId = nomeCampo.Substring(2);

                            if (nomePropriedade.StartsWith("id"))
                            {
                                nomePropriedade = nomePropriedade.Substring(2);
                            }
                        }
                        if (nomePropriedade != nomeCampoSemId)
                        {
                            var nomeRelacao = relacao.GetType().Name.Replace(nameof(Attribute), String.Empty);

                            var atributoRelacao = (IIgnorarAlerta)relacao.Propriedade.GetCustomAttribute<BaseRelacaoAttribute>();
                            if (!atributoRelacao.IgnorarAlerta)
                            {
                                this.Alertas.Add($"Verique a nome do campo do chave estrangeira '{relacao.EstruturaCampoChaveEstrangeira.NomeCampo}' da propreidade {relacao.Propriedade.Name} ({nomeRelacao}) na entidade {estruturaEntidade.TipoEntidade.Name}" +
                                            $"\nPara não mostrar mais esse alerta, defina propriedade IgnorarAlerta para True ");
                            }
                        }
                    }
                }
                //  throw new Exception("Validar relacaoPai");
            }
        }

        private void AnalisarAlertasEntidades()
        {
            foreach (EstruturaEntidade estruturaEntidade in this.EstruturasEntidade.Values)
            {
                if (estruturaEntidade.Alertas.Count > 0)
                {
                    this.Alertas.AddRange(estruturaEntidade.Alertas);
                }
                if (!estruturaEntidade.IsAbstrata && this.AssemblyEntidade.GetTypes().Any(x => x.IsSubclassOf(estruturaEntidade.TipoEntidade)))
                {
                    if (!estruturaEntidade.IsAutorizarInstanciaNaoEspecializada)
                    {
                        this.Alertas.Add(String.Format("O tipo entidade '{0}' não é abstrato e possui classes especializadas, Configura como Abstrato", estruturaEntidade.TipoEntidade.Name));
                    }

                }
                if (estruturaEntidade.IsAbstrata && !this.AssemblyEntidade.GetTypes().Any(x => x.IsSubclassOf(estruturaEntidade.TipoEntidade)))
                {
                    this.Alertas.Add(String.Format("O tipo entidade '{0}' é abstrato e não possui classes especializadas, Configura como Abstrato", estruturaEntidade.TipoEntidade.Name));
                }
                var estruturasColecoes = new List<EstruturaRelacao>();
                estruturasColecoes.AddRange(estruturaEntidade.RetornarTodasRelacoesFilhos());
                estruturasColecoes.AddRange(estruturaEntidade.RetornarTodasRelacoesNn());

                foreach (var relacao in estruturasColecoes)
                {
                    var propriedade = relacao.Propriedade;
                    var tipoDefinicao = propriedade.PropertyType.GetGenericTypeDefinition();

                    if (tipoDefinicao != typeof(ListaEntidades<>))
                    {
                        this.Alertas.Add(String.Format(" A propriedade {0}  ('Colecao')  colecao não possui e definição ListaEntidades<TEntidade>, Declaração em {1} ", propriedade.Name, propriedade.DeclaringType.Name));
                    }
                }
                if (!estruturaEntidade.IsAbstrata)
                {
                    var instanciaEntidade = Activator.CreateInstance(estruturaEntidade.TipoEntidade);
                    foreach (var relacao in estruturasColecoes)
                    {
                        var propriedade = relacao.Propriedade;
                        var colecao = (ICollection)propriedade.GetValue(instanciaEntidade);
                        if (colecao == null)
                        {
                            this.Alertas.Add(String.Format(" A propriedade {0}  ('Colecao')  colecao não foi instancia na declaração em {1} ", propriedade.Name, propriedade.DeclaringType.Name));
                        }
                    }
                    foreach (var estrutuaTipoComplexo in estruturaEntidade.EstruturasTipoComplexao.Values)
                    {
                        var propriedade = estrutuaTipoComplexo.Propriedade;
                        var valorPropridade = propriedade.GetValue(instanciaEntidade);
                        if (valorPropridade == null)
                        {
                            var isValidarTipoComplexo = propriedade.GetCustomAttribute<IgnorarValidacaoTipoComplexo>() == null;
                            if (isValidarTipoComplexo)
                            {
                                this.Alertas.Add(String.Format(" A propriedade {0}  ('TipoComplexo')   não foi instancia na declaração em {1} ", propriedade.Name, propriedade.DeclaringType.Name));
                            }

                        }
                    }
                }
                //Validando as relacoes
                //RelacaoPai, RelacaoUmUm RelacaoUmUmReversao

                var propriedadesTipoEntidade = ReflexaoUtil.RetornarPropriedades(estruturaEntidade.TipoEntidade, true).Where(x =>
                                                                        x.PropertyType.IsSubclassOf(typeof(Entidade)) &&
                                                                        !ReflexaoUtil.PropriedadePossuiAtributo(x, typeof(NaoMapearAttribute))).ToList();

                var propriedadesRelacaoPai = estruturaEntidade.RetornarTodasRelacoesPai().Select(x => x.Propriedade).ToHashSet();
                var propriedadesRelacaoUmUm = estruturaEntidade.RetornarTodasRelacoesUmUm().Select(x => x.Propriedade).ToHashSet();
                var propriedadesRelacaoUmUmReverso = estruturaEntidade.RetornarTodasRelacoesUmUmReversa().Select(x => x.Propriedade).ToHashSet();

                foreach (var propriedade in propriedadesTipoEntidade)
                {
                    var atributoNaoMapear = propriedade.GetCustomAttribute<NaoMapearAttribute>();
                    if (atributoNaoMapear == null)
                    {
                        continue;
                    }
                    if (propriedadesRelacaoPai.Contains(propriedade))
                    {
                        continue;
                    }
                    if (propriedadesRelacaoUmUm.Contains(propriedade))
                    {
                        continue;
                    }
                    if (propriedadesRelacaoUmUmReverso.Contains(propriedade))
                    {
                        continue;
                    }
                    this.Alertas.Add(String.Format(" A propriedade {0} em {1}, o não foi encontrado o atribudo do tipo de relacao . Conferir o atributo RelacaoPai, RelacaoUmUm, RelacaoUmUmReverso ", propriedade.Name, propriedade.DeclaringType.Name));
                }
                //Colecoes
                var propriedadesTipoColecaEntidade = ReflexaoUtil.RetornarPropriedades(estruturaEntidade.TipoEntidade, true).Where(x => ReflexaoUtil.TipoRetornaColecao(x.PropertyType) && ReflexaoUtil.RetornarTipoGenericoColecao(x.PropertyType).IsSubclassOf(typeof(Entidade))).ToList();

                var propriedadesRelacaoColecacao = estruturaEntidade.RetornarTodasRelacoesFilhos().Select(x => x.Propriedade).ToList();
                propriedadesRelacaoColecacao.AddRange(estruturaEntidade.RetornarTodasRelacoesNn().Select(x => x.Propriedade).ToList());

                foreach (var propriedade in propriedadesTipoColecaEntidade)
                {
                    var atributoNaoMapear = propriedade.GetCustomAttribute<NaoMapearAttribute>();
                    if (atributoNaoMapear == null && !propriedadesRelacaoColecacao.Contains(propriedade))
                    {
                        this.Alertas.Add(String.Format(" A propriedade {0} (RelacaoColecao') não foi encontrado em {1}. Conferir o atributo RelacaoFilhos ou RelacaoNn ", propriedade.Name, propriedade.DeclaringType.Name));
                    }
                }
            }
        }


        private void AnalisarNotificacaoPropriedadesAlteradas()
        {
            foreach (var estruturaEntidade in this.EstruturasEntidade.Values)
            {
                var tipoEntidade = estruturaEntidade.TipoEntidade;
                foreach (var estruturaAlteracaoPropriedade in estruturaEntidade.EstruturasAlteracaoPropriedade)
                {
                    var atributo = estruturaAlteracaoPropriedade.Atributo;
                    var tipoPropriedadeRelacao = atributo.PropriedadeRelacao.PropertyType;
                    if (!ReflexaoUtil.TipoIgualOuHerda(tipoPropriedadeRelacao, tipoEntidade))
                    {
                        this.Alertas.Add($"O tipo {tipoPropriedadeRelacao.Name} da {nameof(NotificarAlteracaoPropriedadeAttribute.PropriedadeRelacao)} do atributo {nameof(NotificarAlteracaoPropriedadeAttribute)} não é compativel com a entidade {tipoEntidade.Name}");
                    }

                    var tipoPropriedade = ReflexaoUtil.RetornarTipoSemNullable(estruturaAlteracaoPropriedade.Propriedade.PropertyType);
                    var tipoPropriedadeValorAlterado = ReflexaoUtil.RetornarTipoSemNullable(estruturaAlteracaoPropriedade.Atributo.PropriedadeValorAlterado.PropertyType);

                    if (tipoPropriedade != tipoPropriedadeValorAlterado)
                    {
                        this.Alertas.Add($"O tipo '{tipoPropriedade.Name}' da propriedade {estruturaAlteracaoPropriedade.Propriedade.Name} em {tipoEntidade.Name} é diferente do tipo '{tipoPropriedadeValorAlterado.Name}' propriedade de alteracao '{atributo.PropriedadeValorAlterado.Name}' em '{atributo.TipoEntidadeAlteracaoPropriedade.Name}'");
                    }
                }
            }
        }

    }
}