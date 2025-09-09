using Snebur.Linq;
using System.Diagnostics;

namespace Snebur.AcessoDados.Estrutura;

internal partial class EstruturaBancoDados
{
    private void AnalisarAlertasEstruturaEntidade()
    {
        if (DebugUtil.IsAttached)
        {
            //this.AnalisarPadraoNomeclaturaRelacoesChaveEstrangeira();
            this.AnalisarPropriedaadesChaveEstrangeiraSemRelacao();
            this.AnalisarAlertasEntidades();
            this.AnalisarNotificacaoPropriedadesAlteradas();

            if (this.Alertas.Count > 0)
            {
                var mensagem = String.Join(Environment.NewLine, this.Alertas);
                Trace.TraceWarning(mensagem);
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
            var propriedades = ReflexaoUtil.RetornarPropriedades(estruturaEntidade.TipoEntidade).Where(x => ReflexaoUtil.IsPropriedadePossuiAtributo(x, typeof(ChaveEstrangeiraAttribute)) ||
                                                                                                            ReflexaoUtil.IsPropriedadePossuiAtributo(x, typeof(ChaveEstrangeiraRelacaoAttribute))).ToList();
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

    //private void AnalisarPadraoNomeclaturaRelacoesChaveEstrangeira()
    //{
    //    foreach (var estruturaEntidade in this.EstruturasEntidade.Values)
    //    {
    //        var relacoes = estruturaEntidade.TodasRelacoesChaveEstrangeira();
    //        foreach (var relacao in relacoes)
    //        {
    //            var nomePropriedade = relacao.EstruturaCampoChaveEstrangeira.Propriedade.Name.ToLower();
    //            if (!(nomePropriedade.StartsWith("id") || nomePropriedade.EndsWith("_id")))
    //            {
    //                this.Alertas.Add($" a chave estrangeira da propriedade {relacao.Propriedade.Name} na entidade {estruturaEntidade.TipoEntidade.Name} não possui o id no nome");
    //            }
    //            else
    //            {
    //                //var nomePropriedade = relacao.Propriedade.Name.ToLower();
    //                string nomePropriedadeSemId = null;
    //                if (nomePropriedade.EndsWith("_id"))
    //                {
    //                    nomePropriedadeSemId = nomePropriedade.Substring(0, nomePropriedade.Length - 3);
    //                    if (nomePropriedade.EndsWith("id"))
    //                    {
    //                        nomePropriedade = nomePropriedade.Substring(0, nomePropriedade.Length - 3);
    //                    }
    //                }
    //                else if (nomePropriedade.StartsWith("id"))
    //                {
    //                    nomePropriedadeSemId = nomePropriedade.Substring(2);

    //                    if (nomePropriedade.StartsWith("id"))
    //                    {
    //                        nomePropriedade = nomePropriedade.Substring(2);
    //                    }
    //                }
    //                if (nomePropriedade != nomePropriedadeSemId)
    //                {
    //                    var nomeRelacao = relacao.GetType().Name.Replace(nameof(Attribute), String.Empty);

    //                    var atributoRelacao = (IIgnorarAlerta)relacao.Propriedade.GetCustomAttribute<BaseRelacaoAttribute>();
    //                    if (!atributoRelacao.IgnorarAlerta )
    //                    {
    //                        this.Alertas.Add($"Verifique a nome do campo do chave estrangeira '{relacao.EstruturaCampoChaveEstrangeira.Propriedade.Name}- Campo:{relacao.EstruturaCampoChaveEstrangeira.NomeCampo}' da propriedade {relacao.Propriedade.Name} ({nomeRelacao}) na entidade {estruturaEntidade.TipoEntidade.Name}" +
    //                                    $"\nPara não mostrar mais esse alerta, defina propriedade IgnorarAlerta para True ");
    //                    }
    //                }
    //            }
    //        }
    //        //  throw new Exception("Validar relacaoPai");
    //    }
    //}

    private void AnalisarAlertasEntidades()
    {
        foreach (EstruturaEntidade estruturaEntidade in this.EstruturasEntidade.Values)
        {
            if (estruturaEntidade.Alertas.Count > 0)
            {
                this.Alertas.AddRange(estruturaEntidade.Alertas);
            }
            if (!estruturaEntidade.IsAbstrata && this.AssemblyEntidades.GetTypes().Any(x => x.IsSubclassOf(estruturaEntidade.TipoEntidade)))
            {
                if (!estruturaEntidade.IsAutorizarInstanciaNaoEspecializada)
                {
                    this.Alertas.Add(String.Format("O tipo entidade '{0}' não é abstrato e possui classes especializadas, Configura como Abstrato", estruturaEntidade.TipoEntidade.Name));
                }
            }
            if (estruturaEntidade.IsAbstrata && !this.AssemblyEntidades.GetTypes().Any(x => x.IsSubclassOf(estruturaEntidade.TipoEntidade)))
            {
                this.Alertas.Add(String.Format("O tipo entidade '{0}' é abstrato e não possui classes especializadas, Configura como Abstrato", estruturaEntidade.TipoEntidade.Name));
            }
            var estruturasColecoes = new List<EstruturaRelacao>();
            estruturasColecoes.AddRange(estruturaEntidade.TodasRelacoesFilhos);
            estruturasColecoes.AddRange(estruturaEntidade.TodasRelacoesNn);

            foreach (var relacao in estruturasColecoes)
            {
                var propriedade = relacao.Propriedade;
                var tipoDefinicao = propriedade.PropertyType.GetGenericTypeDefinition();

                if (tipoDefinicao != typeof(ListaEntidades<>))
                {
                    this.Alertas.Add(String.Format(" A propriedade {0}  ('Coleção')  coleção não possui e definição ListaEntidades<TEntidade>, Declaração em {1} ", propriedade.Name, propriedade.DeclaringType?.Name));
                }
            }
            if (!estruturaEntidade.IsAbstrata)
            {
                var instanciaEntidade = Activator.CreateInstance(estruturaEntidade.TipoEntidade);
                foreach (var relacao in estruturasColecoes)
                {
                    var propriedade = relacao.Propriedade;
                    var colecao = propriedade.GetValue(instanciaEntidade) as ICollection;
                    if (colecao is null)
                    {
                        this.Alertas.Add(String.Format(" A propriedade {0}  ('Coleção')  coleção não foi instancia na declaração em {1} ", propriedade.Name, propriedade.DeclaringType?.Name));
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
                            this.Alertas.Add(String.Format(" A propriedade {0}  ('TipoComplexo')   não foi instancia na declaração em {1} ", propriedade.Name, propriedade.DeclaringType?.Name));
                        }
                    }
                }
            }
            //Validando as relações
            //RelacaoPai, RelacaoUmUm RelacaoUmUmReversao

            var propriedadesTipoEntidade = ReflexaoUtil.RetornarPropriedades(estruturaEntidade.TipoEntidade, true).Where(x =>
                                                                    x.PropertyType.IsSubclassOf(typeof(Entidade)) &&
                                                                    !ReflexaoUtil.IsPropriedadePossuiAtributo(x, typeof(NaoMapearAttribute))).ToList();

            var propriedadesRelacaoPai = estruturaEntidade.TodasRelacoesPai.Select(x => x.Propriedade).ToHashSet();
            var propriedadesRelacaoUmUm = estruturaEntidade.TodasRelacoesUmUm.Select(x => x.Propriedade).ToHashSet();
            var propriedadesRelacaoUmUmReverso = estruturaEntidade.TodasRelacoesUmUmReversa.Select(x => x.Propriedade).ToHashSet();

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
                this.Alertas.Add(String.Format(" A propriedade {0} em {1}, o não foi encontrado o atributo do tipo de relação . Conferir o atributo RelacaoPai, RelacaoUmUm, RelacaoUmUmReverso ", propriedade.Name, propriedade.DeclaringType?.Name));
            }
            //Coleções
            var propriedadesTipoColecaEntidade = ReflexaoUtil.RetornarPropriedades(estruturaEntidade.TipoEntidade, true)
                .Where(x => ReflexaoUtil.IsTipoRetornaColecao(x.PropertyType) && ReflexaoUtil.RetornarTipoGenericoColecao(x.PropertyType).IsSubclassOf(typeof(Entidade)))
                .ToList();

            var propriedadesRelacaoColecacao = estruturaEntidade.TodasRelacoesFilhos.Select(x => x.Propriedade).ToList();
            propriedadesRelacaoColecacao.AddRange(estruturaEntidade.TodasRelacoesNn.Select(x => x.Propriedade).ToList());

            foreach (var propriedade in propriedadesTipoColecaEntidade)
            {
                var atributoNaoMapear = propriedade.GetCustomAttribute<NaoMapearAttribute>();
                if (atributoNaoMapear == null && !propriedadesRelacaoColecacao.Contains(propriedade))
                {
                    this.Alertas.Add(String.Format(" A propriedade {0} (RelacaoColecao') não foi encontrado em {1}. Conferir o atributo RelacaoFilhos ou RelacaoNn ", propriedade.Name, propriedade.DeclaringType?.Name));
                }
            }
        }
    }

    private void AnalisarNotificacaoPropriedadesAlteradas()
    {
        foreach (var estruturaEntidade in this.EstruturasEntidade.Values)
        {
            var tipoEntidade = estruturaEntidade.TipoEntidade;
            var estruturaAlteracaoPropriedades = estruturaEntidade.RetornarEstruturasAlteracaoPropriedadeInterno();
            if (estruturaAlteracaoPropriedades?.Length > 0)
            {
                foreach (var estruturaAlteracaoPropriedade in estruturaAlteracaoPropriedades)
                {
                    var atributo = estruturaAlteracaoPropriedade.Atributo;
                    var tipoPropriedadeRelacao = atributo.PropriedadeRelacao.PropertyType;
                    if (!ReflexaoUtil.IsTipoIgualOuHerda(tipoPropriedadeRelacao, tipoEntidade))
                    {
                        this.Alertas.Add($"O tipo {tipoPropriedadeRelacao.Name} da {nameof(NotificarAlteracaoPropriedadeAttribute.PropriedadeRelacao)} do atributo {nameof(NotificarAlteracaoPropriedadeAttribute)} não é compatível com a entidade {tipoEntidade.Name}");
                    }

                    var tipoPropriedade = ReflexaoUtil.RetornarTipoSemNullable(estruturaAlteracaoPropriedade.Propriedade.PropertyType);
                    var tipoPropriedadeValorAlterado = ReflexaoUtil.RetornarTipoSemNullable(estruturaAlteracaoPropriedade.Atributo.PropriedadeValorAlterado.PropertyType);

                    if (tipoPropriedade != tipoPropriedadeValorAlterado)
                    {
                        this.Alertas.Add($"O tipo '{tipoPropriedade.Name}' da propriedade {estruturaAlteracaoPropriedade.Propriedade.Name} em {tipoEntidade.Name} é diferente do tipo '{tipoPropriedadeValorAlterado.Name}' propriedade de alteração '{atributo.PropriedadeValorAlterado.Name}' em '{atributo.TipoEntidadeAlteracaoPropriedade.Name}'");
                    }
                }
            }
        }
    }
}