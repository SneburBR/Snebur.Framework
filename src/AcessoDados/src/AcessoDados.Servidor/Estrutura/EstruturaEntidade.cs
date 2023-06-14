using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Linq;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Snebur.AcessoDados.Estrutura
{
    internal partial class EstruturaEntidade
    {
        internal const int MAXIMO_REGISTRO_POR_CONSULTA = 1000;

        private EstruturaCampo _estruturaCampoIdentificadorProprietario;
        private EstruturaCampo _estruturaCampoOrdenacao;
        private EstruturaCampo _estruturaCampoDelatado;
        private EstruturaCampo _estruturaCampoUsuario;

        private EstruturaRelacaoFilhos[] _todasRelacoesFilhos;
        private EstruturaRelacaoChaveEstrangeira[] _todasRelacoesChaveEstrangeira;
        private EstruturaRelacaoPai[] _todasRelacoesPai;
        private EstruturaRelacaoUmUm[] _todasRelacoesUmUm;
        private EstruturaRelacaoUmUmReversa[] _todasRelacoesUmUmReversa;
        private EstruturaRelacaoNn[] _todasRelacoesNn;
        private EstruturaCampo[] _todasEstruturaCamposValorPadraoInsert;
        private EstruturaCampo[] _todasEstruturaCamposValorPadraoUpdate;

        private readonly EstruturaCampo EstruturaCampoIdentificadorProprietarioInterno;
        private readonly EstruturaCampo EstruturaCampoOrdenacaoInterno;
        private readonly EstruturaCampo EstruturaCampoDeletadoInterno;
        private readonly EstruturaCampo EstruturaCampoUsuarioInterno;
        private readonly EstruturaCampo[] EstruturasCamposValorPadraoInsertInterno;
        private readonly EstruturaCampo[] EstruturasCamposValorPadraoUpdateInterno;

        private EstruturaAlteracaoPropriedade[] EstruturasAlteracaoPropriedadeInterno;
        private EstruturaAlteracaoPropriedadeGenerica[] EstruturasAlteracaoPropriedadeGenericaInterno;

        #region Propriedades
        internal EstruturaCampo EstruturaCampoChavePrimaria { get; }
        internal EstruturaCampo EstruturaCampoTipoEntidade { get; set; }
        internal EstruturaCampo EstruturaCampoNomeTipoEntidade { get; }
        internal string NomeTipoEntidade { get; }
        public EstruturaBancoDados EstruturaBancoDados { get; }
        internal Type TipoEntidade { get; }
        public BancoDadosSuporta SqlSuporte { get; }
        internal string Schema { get; }
        internal string NomeTabela { get; set; }
        internal string GrupoArquivoIndices { get; }
        internal bool IsChavePrimariaAutoIncrimento { get; }
        internal bool IsAbstrata { get; }
        internal bool IsEntidadeRelacaoNn { get; }
        internal EnumInterfaceEntidade InterfacesImplementasEnum { get; }
        internal bool IsImplementaInterfaceIDeletado { get; }
        internal bool IsImplementaInterfaceIAtivo { get; }
        internal bool IsImplementaInterfaceIOrdenacao { get; }
        internal bool IsImplementaInterfaceIAtividadeUsuario { get; }
        internal bool IsImplementaInterfaceIUsuario { get; }
        internal bool IsImplementaInterfaceISessaoUsuario { get; }
        internal bool IsSomenteLeitura { get; }
        internal bool IsDeletarRegistro { get; }
        public bool IsAutorizarInstanciaNaoEspecializada { get; }
        internal EnumInterfaceEntidade[] InterfacesImplementasFlags => EnumUtil.RetornarFlags<EnumInterfaceEntidade>(this.InterfacesImplementasEnum);
        internal int MaximoRegistroPorConsulta { get; }
        internal DicionarioEstrutura<EstruturaCampo> EstruturasCampos { get; }
        internal DicionarioEstrutura<EstruturaTipoComplexo> EstruturasTipoComplexao { get; }
        internal EstruturaEntidade EstruturaEntidadeBase { get; }
        internal DicionarioEstrutura<EstruturaEntidade> EstruturasEntidadeBase { get; }
        internal DicionarioEstrutura<EstruturaEntidade> EstruturasEntidadeEspecializada { get; }
        internal DicionarioEstrutura<NivelEstruturaEntidadeEspecializada> NiveisEstruturasEntidadesEspecializada { get; }
        internal DicionarioEstrutura<EstruturaRelacao> EstruturasRelacoes { get; }
        internal List<EstruturaCampo> EstruturasCamposComputadoBanco { get; }
        internal List<EstruturaCampo> EstruturasCamposComputadoServico { get; }

        internal List<string> Alertas = new List<string>();

        internal bool IsPossuiEntidadesEspecializacao => this.IsAbstrata || this.EstruturasEntidadeEspecializada.Count > 0;

        #endregion

        #region Propriedades Lazy

        internal EstruturaCampo EstruturaCampoIdentificadorProprietario
                 => this.RetornarValorRecursivo(ref this._estruturaCampoIdentificadorProprietario,
                                                        x => x.EstruturaCampoIdentificadorProprietarioInterno);

        internal EstruturaCampo EstruturaCampoOrdenacao
                 => this.RetornarValorRecursivo(ref this._estruturaCampoOrdenacao,
                                                        x => x.EstruturaCampoOrdenacaoInterno);
        internal EstruturaCampo EstruturaCampoDelatado
                 => this.RetornarValorRecursivo(ref this._estruturaCampoDelatado,
                                                        x => x.EstruturaCampoDeletadoInterno);

        internal EstruturaCampo EstruturaCampoUsuario
                 => this.RetornarValorRecursivo(ref this._estruturaCampoUsuario,
                                                        x => x.EstruturaCampoUsuarioInterno);

        internal EstruturaCampo[] TodasEstruturasCamposValorPadraoInsert
                 => this.RetornarValoresRecursivo(ref this._todasEstruturaCamposValorPadraoInsert,
                                                   x => x.EstruturasCamposValorPadraoInsertInterno);
        internal EstruturaCampo[] TodasEstruturasCamposValorPadraoUpdate
                 => this.RetornarValoresRecursivo(ref this._todasEstruturaCamposValorPadraoUpdate,
                                                  x => x.EstruturasCamposValorPadraoUpdateInterno);

        private EstruturaAlteracaoPropriedadeGenerica[] _todasEstruturasAlteracaoPropriedadeGenerica;
        private EstruturaAlteracaoPropriedade[] _todasEstruturasAlteracaoPropriedade;
        internal EstruturaAlteracaoPropriedadeGenerica[] TodasEstruturasAlteracaoPropriedadeGenerica
                 => this.RetornarValoresRecursivo(ref this._todasEstruturasAlteracaoPropriedadeGenerica,
                                                  x => x.EstruturasAlteracaoPropriedadeGenericaInterno);

        internal EstruturaAlteracaoPropriedade[] TodasEstruturasAlteracaoPropriedade
                 => this.RetornarValoresRecursivo(ref this._todasEstruturasAlteracaoPropriedade,
                                                  x => x.EstruturasAlteracaoPropriedadeInterno);

        internal EstruturaRelacaoFilhos[] TodasRelacoesFilhos
                 => this.RetornarTodasEstruturasRelacaoRecursiva(ref this._todasRelacoesFilhos);

        internal EstruturaRelacaoChaveEstrangeira[] TodasRelacoesChaveEstrangeira()
                 => this.RetornarTodasEstruturasRelacaoRecursiva(ref this._todasRelacoesChaveEstrangeira);

        internal EstruturaRelacaoPai[] TodasRelacoesPai()
                 => this.RetornarTodasEstruturasRelacaoRecursiva(ref this._todasRelacoesPai);

        internal EstruturaRelacaoUmUm[] TodasRelacoesUmUm()
                 => this.RetornarTodasEstruturasRelacaoRecursiva(ref this._todasRelacoesUmUm);

        internal EstruturaRelacaoUmUmReversa[] TodasRelacoesUmUmReversa()
                 => this.RetornarTodasEstruturasRelacaoRecursiva(ref this._todasRelacoesUmUmReversa);

        internal EstruturaRelacaoNn[] TodasRelacoesNn()
                 => this.RetornarTodasEstruturasRelacaoRecursiva(ref this._todasRelacoesNn);

        #endregion

        #region Construtor

        internal EstruturaEntidade(EstruturaBancoDados estruturaBancoDados,
                                    Type tipo,
                                   EstruturaEntidade estruturaEntidadeBase,
                                   BancoDadosSuporta sqlSuporte)
        {
            this.EstruturaBancoDados = estruturaBancoDados;
            this.TipoEntidade = tipo;
            this.NomeTipoEntidade = tipo.Name;
            this.SqlSuporte = sqlSuporte;

            this.IsAbstrata = tipo.IsAbstract || ReflexaoUtil.TipoPossuiAtributo(tipo, typeof(AbstratoAttribute));
            this.InterfacesImplementasEnum = this.RetornarInterfacesEntidade();
            //this.IsImplementaInterfaceIDeletado = ReflexaoUtil.TipoImplementaInterface(tipo, typeof(IDeletado), false);
            //this.IsImplementaInterfaceIOrdenacao = ReflexaoUtil.TipoImplementaInterface(tipo, typeof(IOrdenacao), false);
            //this.IsUsuario = ReflexaoUtil.TipoImplementaInterface(this.TipoEntidade, typeof(IUsuario), false);
            this.NomeTabela = AjudanteEstruturaBancoDados.RetornarNomeTabela(tipo);

            this.Schema = AjudanteEstruturaBancoDados.RetornarSchema(tipo);
            this.GrupoArquivoIndices = AjudanteEstruturaBancoDados.RetornarGrupoArquivoIndices(tipo);

            this.EstruturasEntidadeBase = new DicionarioEstrutura<EstruturaEntidade>();
            this.EstruturasEntidadeEspecializada = new DicionarioEstrutura<EstruturaEntidade>();
            this.EstruturasCampos = new DicionarioEstrutura<EstruturaCampo>();
            //  this.TodosCampos = new DicionarioEstrutura<EstruturaCampo>();
            this.NiveisEstruturasEntidadesEspecializada = new DicionarioEstrutura<NivelEstruturaEntidadeEspecializada>();

            this.EstruturasRelacoes = new DicionarioEstrutura<EstruturaRelacao>();

            this.EstruturaCampoChavePrimaria = this.RetornarEstruturaChavePrimaria();
            this.EstruturaCampoNomeTipoEntidade = this.RetornarEstruturaoNomeTipoEntidade();
            this.EstruturasCampos = this.RetornarEstruturasCampo();

            this.EstruturasCamposValorPadraoInsertInterno = this.RetornarEstruturasCampoValorPadraoInsert();
            this.EstruturasCamposValorPadraoUpdateInterno = this.RetornarEstruturasCampoValorPadraoUpdate();

            this.EstruturasTipoComplexao = this.RetornarEstruturasTipoComplexo();

            this.IsEntidadeRelacaoNn = this.RetornarIsEntidadeRelacaoNn();
            this.EstruturaEntidadeBase = estruturaEntidadeBase;
            this.IsChavePrimariaAutoIncrimento = this.RetornarIsChavePrimariaAutoIncrimento();

            this.IsImplementaInterfaceIDeletado = this.InterfacesImplementasEnum.HasFlag(EnumInterfaceEntidade.IDeletado);
            this.IsImplementaInterfaceIAtivo = this.InterfacesImplementasEnum.HasFlag(EnumInterfaceEntidade.IAtivo);
            this.IsImplementaInterfaceIOrdenacao = this.InterfacesImplementasEnum.HasFlag(EnumInterfaceEntidade.IOrdenacao);
            this.IsImplementaInterfaceIAtividadeUsuario = this.InterfacesImplementasEnum.HasFlag(EnumInterfaceEntidade.IAtividadeUsuario);
            this.IsImplementaInterfaceIUsuario = this.InterfacesImplementasEnum.HasFlag(EnumInterfaceEntidade.IUsuario);
            this.IsImplementaInterfaceISessaoUsuario = this.InterfacesImplementasEnum.HasFlag(EnumInterfaceEntidade.ISessaoUsuario);
            this.IsDeletarRegistro = ReflexaoUtil.TipoPossuiAtributo(this.TipoEntidade, typeof(DeletarRegristroAttribute), true);
            this.IsAutorizarInstanciaNaoEspecializada = ReflexaoUtil.TipoPossuiAtributo(this.TipoEntidade, typeof(AutorizarInstanciaNaoEspecializadaAttribute), true);
            this.IsSomenteLeitura = this.RetornarIsSomenteLeitura();

            //if (this.IsChavePrimariaAutoIncrimento)
            //{
            //    if (this.IsAbstrata && isBancoDadosNaoGerenciavel || !isBancoDadosNaoGerenciavel)
            //    {
            //        this.EstruturaCampoNomeTipoEntidade = this.EstruturasCampos[EntidadeUtil.PropriedadeNomeTipoEntidade.Name];
            //    }
            //    //this.EstruturaCampoNomeTipoEntidade = this.RetornarEstruturaCampoNomeTipoEntidade();
            //}
            this.EstruturasCamposComputadoBanco = this.RetornarEstruturasCamposComputadoBanco();
            this.EstruturasCamposComputadoServico = this.RetornarEstruturasCamposComputadoServico();
            this.MaximoRegistroPorConsulta = this.RetornarMaximoRegistroPorConsulta();

            this.Alertas.AddRange(this.EstruturasCampos.Values.SelectMany(x => x.Alertas));

            this.EstruturaCampoIdentificadorProprietarioInterno = this.RetornarEstruturaCampoIdentificadorProprietario();
            this.EstruturaCampoOrdenacaoInterno = this.RetornarEstruturaCampoOrdenacao();
            this.EstruturaCampoDeletadoInterno = this.RetornarEstruturaCampoDeletado();
            this.EstruturaCampoUsuarioInterno = this.RetornarEstruturaEstruturaCampoUsuario();

            //if (!this.EstruturasCampos.ContainsKey(NOME_PROPRIEDADE_ID))
            //{
            //    this.EstruturasCampos.Add(NOME_PROPRIEDADE_ID, this.EstruturaCampoChavePrimaria);
            //}
        }



        #endregion

        #region Métodos Internos

        internal void AssociarEstruturaRalacaos()
        {
            var estruturasRelacaoUmUmReversa = this.EstruturasRelacoes.Values.OfType<EstruturaRelacaoUmUmReversa>().ToList();
            foreach (var estruturaRelacaoUmUmReversa in estruturasRelacaoUmUmReversa)
            {
                var estruturaRelacaoUmUm = estruturaRelacaoUmUmReversa.EstruturaEntidadeUmUmReversa.TodasRelacoesUmUm().
                                                                       Where(x => x.EstruturaEntidadeChaveEstrangeiraDeclarada == estruturaRelacaoUmUmReversa.EstruturaEntidade).SingleOrDefault();

                estruturaRelacaoUmUmReversa.EstruturaRelacaoUmUm = estruturaRelacaoUmUm;
            }
            var estruturasRelacaoFilhos = this.EstruturasRelacoes.Values.OfType<EstruturaRelacaoFilhos>().ToList();
            foreach (var estruturaRelacaoFilhos in estruturasRelacaoFilhos)
            {
                //var estruturaRelacaoPai = estruturaRelacaoFilhos.EstruturaEntidadeFilho.EstruturasRelacoes.Values.OfType<EstruturaRelacaoPai>().Where(x => x.EstruturaEntidadePai == estruturaRelacaoFilhos.EstruturaEntidade).Single();
                var estruturaRelacaoPai = estruturaRelacaoFilhos.EstruturaEntidadeFilho.RetornarEstruturaRelacaoPai(estruturaRelacaoFilhos);
                estruturaRelacaoFilhos.EstruturaRelacaoPai = estruturaRelacaoPai;
            }
            var estruturasRelacaoNn = this.EstruturasRelacoes.Values.OfType<EstruturaRelacaoNn>().ToList();
            foreach (var estruturaRelacaoNn in estruturasRelacaoNn)
            {
                var relacoesPai = estruturaRelacaoNn.EstruturaEntidadeRelacaoNn.EstruturasRelacoes.Values.OfType<EstruturaRelacaoPai>().ToList();

                estruturaRelacaoNn.EstruturaRelacaoPaiEntidadePai = relacoesPai.Where(x => x.EstruturaEntidadeChaveEstrangeiraDeclarada == estruturaRelacaoNn.EstruturaEntidadePai).Single();
                estruturaRelacaoNn.EstruturaRelacaoPaiEntidadeFilho = relacoesPai.Where(x => x.EstruturaEntidadeChaveEstrangeiraDeclarada == estruturaRelacaoNn.EstruturaEntidadeFilho).Single();
            }
        }
        internal void PreencherRelacoes(DicionarioEstrutura<EstruturaEntidade> estruturasEntidade)
        {
            var propriedades = AjudanteEstruturaBancoDados.RetornarPropriedadesRelacao(this.TipoEntidade);
            foreach (var proriedade in propriedades)
            {
                var atributoRelacao = proriedade.GetCustomAttribute<BaseRelacaoAttribute>();
                if (atributoRelacao is RelacaoPaiAttribute)
                {
                    this.AdicionarEstruturaRelacaoPai(proriedade, estruturasEntidade);
                }
                else if (atributoRelacao is RelacaoUmUmAttribute)
                {
                    this.AdicionarEstruturaRelacaoUmUm(proriedade, estruturasEntidade);
                }
                else if (atributoRelacao is RelacaoUmUmReversaAttribute)
                {
                    this.AdicionarEstruturaRelacaoUmUmReversa(proriedade, estruturasEntidade);
                }
                else if (atributoRelacao is RelacaoFilhosAttribute)
                {
                    this.AdicionarEstruturaRelacaoFilhos(proriedade, estruturasEntidade);
                }
                else if (atributoRelacao is RelacaoNnAttribute)
                {
                    this.AdicionarEstruturaRelacaoNn(proriedade, estruturasEntidade);
                }
                else if (atributoRelacao is RelacaoNnEspecializadaAttribute)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    throw new Erro(String.Format("O atributo relação do tipo {0} não é suportado", proriedade.PropertyType.Name));
                }
            }
        }

        internal void PreencherEstruturasAlteracaoPropriedade()
        {
            this.EstruturasAlteracaoPropriedadeInterno = this.RetornarEstruturasAlteracaoPropriedade();
            this.EstruturasAlteracaoPropriedadeGenericaInterno = this.RetornarEstruturasAlteracaoPropriedadeGenerica();
        }

        internal EstruturaCampo RetornarEstruturaCampo(string chave)
        {
            return this.RetornarEstruturaCampoInterno(chave) ??
                      throw new Erro($"A estrutura campo {chave} não foi encontrada na {this.NomeTipoEntidade}");
        }

        internal EstruturaAlteracaoPropriedade[] RetornarEstruturasAlteracaoPropriedadeInterno()
        {
            return this.EstruturasAlteracaoPropriedadeInterno;
        }

        #endregion

        #region Métodos recursivos

        private T RetornarValorRecursivo<T>(ref T _valor,
                                         Func<EstruturaEntidade, T> funcRetornarEstruturaCampo) where T : class
        {


            return LazyUtil.RetornarValorLazy(ref _valor,
                () =>
                {
                    if (!this.EstruturaBancoDados.IsEstruturasEntidadeMontada)
                    {
                        throw new Exception("A  estrutura do banco de dados não foi montada");
                    }

                    var estruturaEntidadeatual = this;

                    while (estruturaEntidadeatual != null)
                    {
                        var estruturaCampo = funcRetornarEstruturaCampo(estruturaEntidadeatual);
                        if (estruturaCampo != null)
                        {
                            return estruturaCampo;
                        }
                        estruturaEntidadeatual = estruturaEntidadeatual.EstruturaEntidadeBase;
                    }
                    return default;
                });

        }

        private T[] RetornarValoresRecursivo<T>(ref T[] _valores,
                                          Func<EstruturaEntidade, T[]> funcRetornarEstruturaCampo) where T : class
        {
            return LazyUtil.RetornarValorLazy(ref _valores,
                () =>
                {
                    if (!this.EstruturaBancoDados.IsEstruturasEntidadeMontada)
                    {
                        throw new Exception("A  estrutura do banco de dados não foi montada");
                    }

                    var valores = new List<T>();
                    var estruturaEntidadeatual = this;
                    while (estruturaEntidadeatual != null)
                    {
                        var valoresAtual = funcRetornarEstruturaCampo(estruturaEntidadeatual);
                        valores.AddRangeNotNull(valoresAtual);
                        estruturaEntidadeatual = estruturaEntidadeatual.EstruturaEntidadeBase;
                    }
                    return valores.ToArray();
                });
        }


        private TEstruturaRelacao[] RetornarTodasEstruturasRelacaoRecursiva<TEstruturaRelacao>(ref TEstruturaRelacao[] valor)
        {
            return LazyUtil.RetornarValorLazyComBloqueio(ref valor, () =>
              {
                  if (!this.EstruturaBancoDados.IsEstruturasEntidadeMontada)
                  {
                      throw new Exception("A  estrutura do banco de dados não foi montada");
                  }

                  var estruturasRelacao = new List<TEstruturaRelacao>();
                  var estruturaAtual = this;
                  while (estruturaAtual != null)
                  {
                      estruturasRelacao.AddRange(estruturaAtual.EstruturasRelacoes.Values.OfType<TEstruturaRelacao>());
                      estruturaAtual = estruturaAtual.EstruturaEntidadeBase;
                  }
                  return estruturasRelacao.ToArray();
              });

        }

        #endregion

        #region Métodos privados

        private EnumInterfaceEntidade RetornarInterfacesEntidade()
        {
            EnumInterfaceEntidade interfaces = 0;
            foreach (var tipoInterface in EntidadeUtil.TiposInterfaceEntidade)
            {
                if (this.TipoEntidade.GetInterface(tipoInterface.Name) != null)
                {
                    var enumInterface = EnumUtil.RetornarValorEnum<EnumInterfaceEntidade>(tipoInterface.Name);
                    interfaces = interfaces | enumInterface;
                }
            }
            return interfaces;
        }

        private EstruturaRelacaoPai RetornarEstruturaRelacaoPai(EstruturaRelacaoFilhos estruturaRelacaoFilhos)
        {
            var todasRelacoesPai = estruturaRelacaoFilhos.EstruturaEntidadeFilho.TodasRelacoesPai();
            var estruturaCampoChaveEstragentira = estruturaRelacaoFilhos.EstruturaCampoChaveEstrangeira;
            if (estruturaCampoChaveEstragentira == null)
            {
                throw new Erro($"A estrutura do campo chave estrangeira não foi definida na relação filhos {estruturaRelacaoFilhos.Propriedade.Name} na entidade {estruturaRelacaoFilhos.EstruturaEntidade.TipoEntidade.Name}");
            }

            var estruturasRelacaoPai = todasRelacoesPai.Where(x => x.EstruturaEntidadeChaveEstrangeiraDeclarada == estruturaRelacaoFilhos.EstruturaEntidade &&
                                                                   x.EstruturaCampoChaveEstrangeira == estruturaRelacaoFilhos.EstruturaCampoChaveEstrangeira).ToList();
            if (estruturasRelacaoPai.Count == 0)
            {
                throw new Exception($"Não foi encontrado a propriedade de relação pai do tipo {estruturaRelacaoFilhos.EstruturaEntidade.TipoEntidade.Name} na entidade {estruturaRelacaoFilhos.EstruturaEntidadeFilho.TipoEntidade.Name}");
            }

            if (estruturasRelacaoPai.Count > 1)
            {
                throw new Exception($"Existe mais uma propriedade de relação pai do tipo {estruturaRelacaoFilhos.EstruturaEntidade.TipoEntidade.Name} na entidade {estruturaRelacaoFilhos.EstruturaEntidadeFilho.TipoEntidade.Name}." +
                                    $"Defina a propriedade na relação no atributo RelacaoFilhos. ");
            }
            return estruturasRelacaoPai.Single();
        }

        private EstruturaCampo RetornarEstruturaCampoInterno(string chave)
        {
            if (this.EstruturasCampos.TryGetValue(chave, out var estruturaCampo))
            {
                return estruturaCampo;
            }

            if (this.EstruturaCampoNomeTipoEntidade.Propriedade.Name == chave)
            {
                return this.EstruturaCampoNomeTipoEntidade;
            }

            if (DebugUtil.IsAttached)
            {
                if (this.EstruturasCampos.Any(x => x.Key.Equals(chave, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var parChave = this.EstruturasCampos.Where(x => x.Key.Equals(chave, StringComparison.InvariantCultureIgnoreCase)).Single();
                    estruturaCampo = parChave.Value;
                    var alerta = $" A chave do estrutura campo '{chave}' está incorreta, CORRETO  '{parChave.Key}'";
                    this.Alertas.Add(alerta);
                    return estruturaCampo;
                }
            }

            return this.EstruturaEntidadeBase?.RetornarEstruturaCampoInterno(chave);
        }

        private DicionarioEstrutura<EstruturaCampo> RetornarEstruturasCampo()
        {
            var estruturasCampos = new DicionarioEstrutura<EstruturaCampo>();
            var propriedadesCampo = AjudanteEstruturaBancoDados.RetornarPropriedadesCampos(this.TipoEntidade);
            foreach (var propriedadeCampo in propriedadesCampo)
            {
                if (!this.IsAbstrata && this.SqlSuporte.IsColunaNomeTipoEntidade &&
                    propriedadeCampo.Name == "__NomeTipoEntidade")
                {
                    continue;
                }

                estruturasCampos.Add(propriedadeCampo.Name, this.RetornarNovaEstruturaCampo(propriedadeCampo));
            }

            return estruturasCampos;
        }

        private EstruturaCampo[] RetornarEstruturasCampoValorPadraoInsert()
        {
            return this.EstruturasCampos.Where(x => x.Value.AtributoValorPadrao != null).
                                         Select(x => x.Value).
                                         ToArray();

        }

        private EstruturaCampo[] RetornarEstruturasCampoValorPadraoUpdate()
        {
            return this.EstruturasCamposValorPadraoInsertInterno.Where(x => x.AtributoValorPadrao.IsValorPadraoOnUpdate).
                                                                 ToArray();

        }

        private DicionarioEstrutura<EstruturaTipoComplexo> RetornarEstruturasTipoComplexo()
        {
            var estruturasTipoComplexo = new DicionarioEstrutura<EstruturaTipoComplexo>();
            var propriedadesTipoComplexos = AjudanteEstruturaBancoDados.RetornarPropriedadesTipoComplexo(this.TipoEntidade);
            foreach (var propriedade in propriedadesTipoComplexos)
            {
                estruturasTipoComplexo.Add(propriedade.Name, new EstruturaTipoComplexo(propriedade, this));
            }
            //adicionando os campos
            foreach (var estruturaTipoComplexo in estruturasTipoComplexo.Values)
            {
                foreach (var estruturaCampo in estruturaTipoComplexo.EstruturasCampo.Values)
                {
                    this.EstruturasCampos.Add(estruturaCampo.NomeCampo, estruturaCampo);
                }
            }
            return estruturasTipoComplexo;
        }

        private EstruturaCampo RetornarNovaEstruturaCampo(PropertyInfo propriedadeCampo)
        {
            if (this.EstruturasCampos.ContainsKey(propriedadeCampo.Name))
            {
                throw new Erro(String.Format("A estrutura entidade {0} já possui o campo {1}", this.TipoEntidade.Name, propriedadeCampo.Name));
            }
            return new EstruturaCampo(propriedadeCampo, this);
        }

        private EstruturaCampo RetornarEstruturaChavePrimaria()
        {
            var proriedadeChavePrimaria = AjudanteEstruturaBancoDados.RetornarPropriedadeChavePrimaria(this.TipoEntidade);
            return new EstruturaCampo(proriedadeChavePrimaria, this);
        }

        private EstruturaCampo RetornarEstruturaoNomeTipoEntidade()
        {
            var propriedadeTipoEntidade = ReflexaoUtil.RetornarPropriedade<Entidade>(x => x.__NomeTipoEntidade);
            return new EstruturaCampo(propriedadeTipoEntidade, this);
        }

        private bool RetornarIsEntidadeRelacaoNn()
        {
            var atributo = this.TipoEntidade.GetCustomAttribute(typeof(EntidadeRelacaoNnAttribute), true);
            return (atributo != null);
        }

        private bool RetornarIsSomenteLeitura()
        {
            var atributo = this.TipoEntidade.GetCustomAttribute(typeof(SomenteLeituraAttribute), true);
            return (atributo != null);
        }

        private void AdicionarEstruturaRelacaoPai(PropertyInfo propriedade, DicionarioEstrutura<EstruturaEntidade> estruturasEntidade)
        {
            if (!estruturasEntidade.ContainsKey(propriedade.PropertyType.Name))
            {
                throw new Exception($"A entidade {propriedade.PropertyType.Name} não foi encontrada, declare-a no DbSet do contexto de migração e atualizar a extensão");
            }

            var estruturaEntidadePai = estruturasEntidade[propriedade.PropertyType.Name];
            var nomeCampoChaveEstrangeira = AjudanteEstruturaBancoDados.RetornarNomeCampoChaveEstrangeira(propriedade);

            if (!this.EstruturasCampos.ContainsKey(nomeCampoChaveEstrangeira))
            {
                throw new Exception($"Não existe o campo {nomeCampoChaveEstrangeira} em {this.TipoEntidade.Name} ");
            }
            var estruturaCampoChaveEstrangeira = this.EstruturasCampos[nomeCampoChaveEstrangeira];

            this.EstruturasRelacoes.Add(propriedade.Name, new EstruturaRelacaoPai(propriedade, this, estruturaEntidadePai, estruturaCampoChaveEstrangeira));
        }

        private void AdicionarEstruturaRelacaoUmUm(PropertyInfo propriedade, DicionarioEstrutura<EstruturaEntidade> estruturasEntidade)
        {
            var estruturaEntidadePai = estruturasEntidade[propriedade.PropertyType.Name];
            var nomeCampoChaveEstrangeira = AjudanteEstruturaBancoDados.RetornarNomeCampoChaveEstrangeira(propriedade);
            var estruturaCampoChaveEstrangeira = this.EstruturasCampos[nomeCampoChaveEstrangeira];

            this.EstruturasRelacoes.Add(propriedade.Name, new EstruturaRelacaoUmUm(propriedade, this, estruturaEntidadePai, estruturaCampoChaveEstrangeira));
        }

        private void AdicionarEstruturaRelacaoUmUmReversa(PropertyInfo propriedade, DicionarioEstrutura<EstruturaEntidade> estruturasEntidade)
        {
            var atributoRelacaoUmUmReversa = propriedade.GetCustomAttribute<RelacaoUmUmReversaAttribute>();
            var estruturaEntidadePai = estruturasEntidade[propriedade.PropertyType.Name];
            var nomeCampoLigacaoRelacaoPai = atributoRelacaoUmUmReversa.NomePropriedadeChaveEstrangeiraReversa;

            if (String.IsNullOrEmpty(nomeCampoLigacaoRelacaoPai))
            {
                var propriedadeLigacaoRelacaoFilho = AjudanteEstruturaBancoDados.RetornarPropriedadeRelacao(estruturaEntidadePai.TipoEntidade, propriedade.PropertyType, EnumTipoRelacao.RelacaoUmUm);
                var atributoChaveEstrangeira = propriedadeLigacaoRelacaoFilho.RetornarAtributoChaveEstrangeira();

                if (atributoChaveEstrangeira == null)
                {
                    throw new Erro(String.Format("O atributo chave estrangeira da Ligação RelacaoFilho não foi definido, Entidade {0} - Propriedade {1} - EntidadePai {2}", this.TipoEntidade.Name, propriedade.Name, estruturaEntidadePai.TipoEntidade.Name));
                }
                nomeCampoLigacaoRelacaoPai = atributoChaveEstrangeira.Name;
            }
            var estruturaCampoLigacaoRelacaoPai = estruturaEntidadePai.RetornarEstruturaCampo(nomeCampoLigacaoRelacaoPai);

            var estruturaRelacaoFilho = new EstruturaRelacaoUmUmReversa(propriedade, this, estruturaEntidadePai, estruturaCampoLigacaoRelacaoPai);
            this.EstruturasRelacoes.Add(propriedade.Name, estruturaRelacaoFilho);
        }

        private void AdicionarEstruturaRelacaoFilhos(PropertyInfo propriedade, DicionarioEstrutura<EstruturaEntidade> estruturasEntidade)
        {
            var nomeCampoChaveEstrangeira = this.RetornarNomeCampoLigacaoRelacaoFilhos(propriedade);
            var tipoEntidadeFilho = AjudanteEstruturaBancoDados.RetornarTipoEntidadeLista(propriedade);
            if (!estruturasEntidade.ContainsKey(tipoEntidadeFilho.Name))
            {
                throw new Exception($"A entidade do tipo {tipoEntidadeFilho.Name} não possui propriedade de inicialização no contexto. Verificar propriedade DbSet ");
            }

            var estruturaEntidadeFilho = estruturasEntidade[tipoEntidadeFilho.Name];

            var estruturaCampoLigacaoFilho = estruturaEntidadeFilho.RetornarEstruturaCampo(nomeCampoChaveEstrangeira);

            var estruturaRelacaoFilhos = new EstruturaRelacaoFilhos(propriedade, this, estruturaEntidadeFilho, estruturaCampoLigacaoFilho);
            this.EstruturasRelacoes.Add(propriedade.Name, estruturaRelacaoFilhos);

        }

        private string RetornarNomeCampoLigacaoRelacaoFilhos(PropertyInfo propriedade)
        {
            var atributoRelacaoFilhos = propriedade.GetCustomAttribute<RelacaoFilhosAttribute>();
            if (!String.IsNullOrEmpty(atributoRelacaoFilhos.NomePropriedadeChaveEstrangeira))
            {
                if (DebugUtil.IsAttached)
                {
                    System.Diagnostics.Trace.TraceWarning($"Testear relação filhos  {propriedade.DeclaringType.Name}{propriedade.Name} ");
                }
                return atributoRelacaoFilhos.NomePropriedadeChaveEstrangeira;
            }
            else
            {
                var tipoEntidadeFilho = AjudanteEstruturaBancoDados.RetornarTipoEntidadeLista(propriedade);
                var propriedadeRelacao = AjudanteEstruturaBancoDados.RetornarPropriedadeRelacao(tipoEntidadeFilho, this.TipoEntidade, EnumTipoRelacao.RelacaoPai);
                var atributoChaveEstrangeira = propriedadeRelacao.RetornarAtributoChaveEstrangeira();
                if (atributoChaveEstrangeira == null)
                {
                    throw new Erro($"Não foi encontrado um chave estrangeira para a propriedade {propriedadeRelacao.Name} na entidade {propriedadeRelacao.DeclaringType.Name} ");
                }
                return atributoChaveEstrangeira.Name;
            }
        }

        private void AdicionarEstruturaRelacaoNn(PropertyInfo propriedade, DicionarioEstrutura<EstruturaEntidade> estruturasEntidade)
        {
            var atributoRelacaoNn = propriedade.GetCustomAttribute<RelacaoNnAttribute>();
            var tipoEntidadeFilho = AjudanteEstruturaBancoDados.RetornarTipoEntidadeLista(propriedade);

            if (!estruturasEntidade.ContainsKey(atributoRelacaoNn.TipoEntidadeRelacao.Name))
            {
                throw new Exception($"Entidade {atributoRelacaoNn.TipoEntidadeRelacao.Name} não foi declarada o DbSet no Contexto de migração ou extensão e extensão não foi acionada");
            }

            var estruturaEntidadeRelacaoNn = estruturasEntidade[atributoRelacaoNn.TipoEntidadeRelacao.Name];
            var estruturaEntidadePai = this;
            var estruturaEntidadeFilho = estruturasEntidade[tipoEntidadeFilho.Name];

            var propriedadeRelacaoEntidadePai = AjudanteEstruturaBancoDados.RetornarPropriedadeRelacao(atributoRelacaoNn.TipoEntidadeRelacao, this.TipoEntidade, EnumTipoRelacao.RelacaoPai);
            var propriedadeRelacaoEntidadeFilho = AjudanteEstruturaBancoDados.RetornarPropriedadeRelacao(atributoRelacaoNn.TipoEntidadeRelacao, tipoEntidadeFilho, EnumTipoRelacao.RelacaoPai);

            var estruturasRelacaoPai = estruturaEntidadeRelacaoNn.EstruturasRelacoes.Values.OfType<EstruturaRelacaoPai>().ToList();

            var nomeCampoChaveEstrangeiraPai = AjudanteEstruturaBancoDados.RetornarNomeCampoChaveEstrangeira(propriedadeRelacaoEntidadePai);
            var nomeCampoChaveEstrangeiraFilho = AjudanteEstruturaBancoDados.RetornarNomeCampoChaveEstrangeira(propriedadeRelacaoEntidadeFilho);
            var estruturaCampoChaveEstrangeiraPai = estruturaEntidadeRelacaoNn.RetornarEstruturaCampo(nomeCampoChaveEstrangeiraPai);
            var estruturaCampoChaveEstrangeiraFilho = estruturaEntidadeRelacaoNn.RetornarEstruturaCampo(nomeCampoChaveEstrangeiraFilho);

            var estruturaRelacaoNn = new EstruturaRelacaoNn(propriedade, estruturaEntidadeRelacaoNn, estruturaEntidadePai, estruturaEntidadeFilho, estruturaCampoChaveEstrangeiraPai, estruturaCampoChaveEstrangeiraFilho);

            this.EstruturasRelacoes.Add(propriedade.Name, estruturaRelacaoNn);
        }

        private bool RetornarIsChavePrimariaAutoIncrimento()
        {
            if (this.TipoEntidade.BaseType == typeof(Entidade))
            {
                return true;
            }
            if (AjudanteEstruturaBancoDados.TipoEntidadeBaseNaoMepeada(this.TipoEntidade.BaseType))
            {
                return true;
            }
            return false;
        }

        private EstruturaCampo RetornarEstruturaCampoNomeTipoEntidade()
        {
            var propriedadeNomeTipoEntidade = ReflexaoUtil.RetornarPropriedade<Entidade>(x => x.__NomeTipoEntidade);
            return this.RetornarNovaEstruturaCampo(propriedadeNomeTipoEntidade);
        }

        private EstruturaCampo RetornarEstruturaCampoIdentificadorProprietario()
        {
            return this.EstruturasCampos.Values.Where(x => ReflexaoUtil.PropriedadePossuiAtributo(x.Propriedade, typeof(PropriedadeIdentificadorProprietarioAttribute))).SingleOrDefault();
        }

        private EstruturaCampo RetornarEstruturaEstruturaCampoUsuario()
        {
            return this.EstruturasCampos.Values.Where(x => ReflexaoUtil.PropriedadePossuiAtributo(x.Propriedade, typeof(ValorPadraoIDUsuarioLogadoAttribute))).SingleOrDefault();
        }


        /// <summary>
        /// Campos computado, são campos se terão alterações quando uma nova entidade for salvar, essa computação será realizada pelo banco de dados
        /// Ex, ValorPadraoDataHoraServidor, IOrdenacao
        /// </summary>
        /// <returns></returns>
        private List<EstruturaCampo> RetornarEstruturasCamposComputadoBanco()
        {
            var estruturasCampoComputado = new List<EstruturaCampo>();
            var tipoIOrdenacao = typeof(IOrdenacao);

            var entiadeImplementaOrdenacao = ReflexaoUtil.TipoImplementaInterface(this.TipoEntidade, tipoIOrdenacao, true);
            var nomeProprieadeOrdenacao = ReflexaoUtil.RetornarNomePropriedade<IOrdenacao>(x => x.Ordenacao);


            foreach (var estruturaCampo in this.EstruturasCampos.Values)
            {
                var propriedade = estruturaCampo.Propriedade;

                if (ReflexaoUtil.PropriedadePossuiAtributo(propriedade, typeof(ValorPadraoDataHoraServidorAttribute)))
                {
                    estruturasCampoComputado.Add(estruturaCampo);
                    continue;
                }
                if (entiadeImplementaOrdenacao && (propriedade.Name == nomeProprieadeOrdenacao))
                {
                    estruturasCampoComputado.Add(estruturaCampo);
                }

                if (propriedade.GetCustomAttribute<PropriedadeComputadaBancoAttribute>() != null)
                {
                    estruturasCampoComputado.Add(estruturaCampo);
                }
            }
            return estruturasCampoComputado;
        }

        private List<EstruturaCampo> RetornarEstruturasCamposComputadoServico()
        {
            var estruturasCampoComputado = new List<EstruturaCampo>();
            foreach (var estruturaCampo in this.EstruturasCampos.Values)
            {
                var propriedade = estruturaCampo.Propriedade;
                var atributoCampoComputado = propriedade.GetCustomAttribute<BasePropriedadeComputadaAttribute>();
                if (atributoCampoComputado != null)
                {
                    if (!atributoCampoComputado.GetType().IsSubclassOf(typeof(PropriedadeComputadaBancoAttribute)))
                    {
                        estruturasCampoComputado.Add(estruturaCampo);
                    }
                }
            }
            return estruturasCampoComputado;
        }

        private EstruturaAlteracaoPropriedade[] RetornarEstruturasAlteracaoPropriedade()
        {
            var estruturasAlteracaoPropriedade = new List<EstruturaAlteracaoPropriedade>();
            var propriedades = ReflexaoUtil.RetornarPropriedades(this.TipoEntidade, true, true);
            foreach (var propriedade in propriedades)
            {
                var atributo = propriedade.GetCustomAttribute<NotificarAlteracaoPropriedadeAttribute>();
                if (atributo != null)
                {
                    if (this.EstruturasCampos.ContainsKey(propriedade.Name))
                    {
                        var estruturaCampo = this.EstruturasCampos[propriedade.Name];
                        estruturasAlteracaoPropriedade.Add(new EstruturaAlteracaoPropriedade(propriedade, this, estruturaCampo, atributo));
                        continue;
                    }

                    if (this.EstruturasTipoComplexao.ContainsKey(propriedade.Name))
                    {
                        var estrturaTipoComplexo = this.EstruturasTipoComplexao[propriedade.Name];
                        estruturasAlteracaoPropriedade.Add(new EstruturaAlteracaoPropriedade(propriedade, this, estrturaTipoComplexo, atributo));
                        continue;
                    }

                    if (this.EstruturasRelacoes.ContainsKey(propriedade.Name))
                    {
                        var estrturaRelacao = this.EstruturasRelacoes[propriedade.Name];
                        if (estrturaRelacao is EstruturaRelacaoPai estruturaRelacaoPai)
                        {
                            estruturasAlteracaoPropriedade.Add(new EstruturaAlteracaoPropriedade(propriedade, this, estruturaRelacaoPai.EstruturaCampoChaveEstrangeira, atributo));
                        }
                    }

                    throw new Erro($"A propriedade {propriedade.Name} do tipo {propriedade.PropertyType.Name} declarada em {propriedade.DeclaringType.Name} não suporta o atributo " +
                                    $"{atributo.GetType().Name}<{atributo.TipoEntidadeAlteracaoPropriedade.Name},nameof({atributo.PropriedadeRelacao}), nameof({atributo.PropriedadeValorAlterado.Name})> ");
                }
            }

            return estruturasAlteracaoPropriedade.ToArray();
        }
        private EstruturaAlteracaoPropriedadeGenerica[] RetornarEstruturasAlteracaoPropriedadeGenerica()
        {
            var atributoNotificarTodasPropriedade = this.TipoEntidade.GetCustomAttribute<NotificarTodasAlteracoesPropriedadeGenericaAttribute>(false);
            var isNotificarTodasPropriedades = atributoNotificarTodasPropriedade != null;
             
            var estruturasAlteracaoPropriedade = new List<EstruturaAlteracaoPropriedadeGenerica>();
            var propriedades = ReflexaoUtil.RetornarPropriedades(this.TipoEntidade, true, true);
            foreach (var propriedade in propriedades)
            {
                var atributoNotificarPropriedade = propriedade.GetCustomAttribute<NotificarAlteracaoPropriedadeGenericaAttribute>();
                var isNotificarAlteracaoPropriedade = isNotificarTodasPropriedades || atributoNotificarPropriedade != null;
                if (isNotificarAlteracaoPropriedade)
                {
                    var atributo = atributoNotificarPropriedade as INotificarAlteracaoPropriedade ??
                                   atributoNotificarTodasPropriedade as INotificarAlteracaoPropriedade;

                    if (atributo == null)
                    {
                        throw new Erro("O atributo notificar alteração da propriedade não está definido");
                    }

                    if (this.EstruturasCampos.ContainsKey(propriedade.Name))
                    {
                        var estruturaCampo = this.EstruturasCampos[propriedade.Name];
                        estruturasAlteracaoPropriedade.Add(new EstruturaAlteracaoPropriedadeGenerica(propriedade,
                                                           this, estruturaCampo,
                                                           atributo));
                        continue;
                    }

                    if (this.EstruturasTipoComplexao.ContainsKey(propriedade.Name))
                    {
                        var estrturaTipoComplexo = this.EstruturasTipoComplexao[propriedade.Name];
                        estruturasAlteracaoPropriedade.Add(new EstruturaAlteracaoPropriedadeGenerica(
                                                                propriedade,
                                                                this,
                                                                estrturaTipoComplexo,
                                                                atributo));
                        continue;
                    }

                    if (atributoNotificarPropriedade != null)
                    {
                        throw new Erro($"A propriedade {propriedade.Name} do tipo {propriedade.PropertyType.Name}" +
                                       $" declarada em {propriedade.DeclaringType.Name} não suporta o atributo {atributoNotificarPropriedade.GetType().Name} ");
                    }
                }
            }

            return estruturasAlteracaoPropriedade.ToArray();
        }

        private int RetornarMaximoRegistroPorConsulta()
        {
            if (Debugger.IsAttached)
            {
                return Int32.MaxValue;
            }

            var atributoMaximoRegistroPorConsulta = this.TipoEntidade.GetCustomAttribute<MaximoRegistroPorConsultaAttribute>();
            if (atributoMaximoRegistroPorConsulta != null)
            {
                return atributoMaximoRegistroPorConsulta.MaximoRegistroPorConsulta;
            }
            return EstruturaEntidade.MAXIMO_REGISTRO_POR_CONSULTA;
        }

        private EstruturaCampo RetornarEstruturaCampoOrdenacao()
        {
            if (ReflexaoUtil.TipoImplementaInterface(this.TipoEntidade, typeof(IOrdenacao), true))
            {
                var nomePropriedadeOrdenacao = ReflexaoUtil.RetornarNomePropriedade<IOrdenacao>(x => x.Ordenacao);
                if (this.EstruturasCampos.ContainsKey(nomePropriedadeOrdenacao))
                {
                    return this.EstruturasCampos[nomePropriedadeOrdenacao];
                }
            }
            return null;
        }

        private EstruturaCampo RetornarEstruturaCampoDeletado()
        {
            if (ReflexaoUtil.TipoImplementaInterface(this.TipoEntidade, typeof(IDeletado), true))
            {
                var nomePropriedadeOrdenacao = ReflexaoUtil.RetornarNomePropriedade<IDeletado>(x => x.IsDeletado);
                return this.EstruturasCampos[nomePropriedadeOrdenacao];
            }
            return null;
        }
        #endregion

        public override string ToString()
        {
            return String.Format("{0} - {1}", this.TipoEntidade.Name, base.ToString());
        }

        internal int RetornarMaximoConsulta(int take)
        {
            if (take == 0)
            {
                return this.MaximoRegistroPorConsulta;
            }
            return Math.Min(take, this.MaximoRegistroPorConsulta);
        }
    }
}