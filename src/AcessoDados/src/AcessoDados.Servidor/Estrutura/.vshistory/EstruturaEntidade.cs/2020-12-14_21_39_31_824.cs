using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;

namespace Snebur.AcessoDados.Estrutura
{
    internal partial class EstruturaEntidade
    {
        internal const int MAXIMO_REGISTRO_POR_CONSULTA = 1000000;

        protected EstruturaCampo EstruturaCampoOrdenacaoProtegido;
        protected EstruturaCampo EstruturaCampoIdentificadorProprietarioProtegido;
        protected EstruturaCampo EstruturaCampoDeletadoProtegido;
        protected EstruturaCampo EstruturaCampoUsuarioProtegido;

        #region Propriedades
        internal string NomeTipoEntidade { get; }

        internal Type TipoEntidade { get; }

        public bool IsBancoDadosNaoGerenciavel { get; }

        internal string Schema { get; }

        internal string NomeTabela { get; set; }

        internal string GrupoArquivoIndices { get; }

        internal EstruturaCampo EstruturaCampoChavePrimaria { get; }

        internal EstruturaCampo EstruturaCampoTipoEntidade { get; set; }

        internal EstruturaCampo EstruturaCampoNomeTipoEntidade { get; }

        internal EstruturaCampo EstruturaCampoIdentificadorProprietario
        {
            get
            {
                var estruturaEntidadeatual = this;

                while (estruturaEntidadeatual != null)
                {
                    var estruturaCampo = estruturaEntidadeatual.EstruturaCampoIdentificadorProprietarioProtegido;
                    if (estruturaCampo != null)
                    {
                        return estruturaCampo;
                    }
                    estruturaEntidadeatual = estruturaEntidadeatual.EstruturaEntidadeBase;
                }
                return null;
            }
        }

        internal EstruturaCampo EstruturaCampoOrdenacao
        {
            get
            {
                var estruturaEntidadeatual = this;
                while (estruturaEntidadeatual != null)
                {
                    var estruturaCampo = estruturaEntidadeatual.EstruturaCampoOrdenacaoProtegido;
                    if (estruturaCampo != null)
                    {
                        return estruturaCampo;
                    }
                    estruturaEntidadeatual = estruturaEntidadeatual.EstruturaEntidadeBase;
                }
                return null;
            }
        }

        internal EstruturaCampo EstruturaCampoDelatado
        {
            get
            {
                var estruturaEntidadeatual = this;
                while (estruturaEntidadeatual != null)
                {
                    var estruturaCampo = estruturaEntidadeatual.EstruturaCampoDeletadoProtegido;
                    if (estruturaCampo != null)
                    {
                        return estruturaCampo;
                    }
                    estruturaEntidadeatual = estruturaEntidadeatual.EstruturaEntidadeBase;
                }
                return null;
            }
        }

        internal EstruturaCampo EstruturaCampoUsuario
        {
            get
            {
                var estruturaEntidadeatual = this;
                while (estruturaEntidadeatual != null)
                {
                    var estruturaCampo = estruturaEntidadeatual.EstruturaCampoUsuarioProtegido;
                    if (estruturaCampo != null)
                    {
                        return estruturaCampo;
                    }
                    estruturaEntidadeatual = estruturaEntidadeatual.EstruturaEntidadeBase;
                }
                return null;
            }
        }

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
        internal bool IsExcluirRegistro { get; }
        public bool IsAutorizarInstanciaNaoEspecializada { get; }

        internal EnumInterfaceEntidade[] InterfacesImplementasFlags => EnumUtil.RetornarFlags<EnumInterfaceEntidade>(this.InterfacesImplementasEnum);

        internal int MaximoRegistroPorConsulta { get; }

        //internal bool IsUsuario { get; }
        /// <summary>
        /// Somente as compos da Classe,
        /// </summary>
        internal DicionarioEstrutura<EstruturaCampo> EstruturasCampos { get; }

        internal DicionarioEstrutura<EstruturaTipoComplexo> EstruturasTipoComplexao { get; }

        internal EstruturaEntidade EstruturaEntidadeBase { get; }

        //internal DicionarioEstrutura<EstruturaCampo> TodosCampos { get; set; }

        /// <summary>
        /// O estruiras estão na ordem da Hirarquia
        /// </summary>
        internal DicionarioEstrutura<EstruturaEntidade> EstruturasEntidadeBase { get; }

        /// <summary>
        /// Sem ordem da hirarquiva, utilizar os niveis
        /// </summary>
        internal DicionarioEstrutura<EstruturaEntidade> EstruturasEntidadeEspecializada { get; }

        /// <summary>
        /// Key é Nivel = 0, 1, 3
        /// </summary>
        internal DicionarioEstrutura<NivelEstruturaEntidadeEspecializada> NiveisEstruturasEntidadesEspecializada { get; }

        internal DicionarioEstrutura<EstruturaRelacao> EstruturasRelacoes { get; }

        internal List<EstruturaCampo> EstruturasCamposComputado { get; }

        internal List<EstruturaAlteracaoPropriedade> EstruturasAlteracaoPropriedade { get; private set; }

        internal List<EstruturaAlteracaoPropriedadeGenerica> EstruturasAlteracaoPropriedadeGenerica { get; private set; }

        internal List<string> Alertas = new List<string>();

        #endregion

        #region Construtor

        internal EstruturaEntidade(Type tipo, EstruturaEntidade estruturaEntidadeBase, bool isBancoDadosNaoGerenciavel)
        {
            this.TipoEntidade = tipo;
            this.NomeTipoEntidade = tipo.Name;
            this.IsBancoDadosNaoGerenciavel = isBancoDadosNaoGerenciavel;

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
            this.IsExcluirRegistro = ReflexaoUtil.TipoPossuiAtributo(this.TipoEntidade, typeof(ExcluirRegristroAttribute), true);
            this.IsAutorizarInstanciaNaoEspecializada = ReflexaoUtil.TipoPossuiAtributo(this.TipoEntidade, typeof(AutorizarInstanciaNaoEspecializadaAttribute), true);
            this.IsSomenteLeitura = this.RetornarIsSomenteLeitura();
             
            if (this.IsChavePrimariaAutoIncrimento)
            {
                if (this.IsAbstrata && isBancoDadosNaoGerenciavel || !isBancoDadosNaoGerenciavel)
                {
                    this.EstruturaCampoNomeTipoEntidade = this.EstruturasCampos[EntidadeUtil.PropriedadeNomeTipoEntidade.Name];
                }
                //this.EstruturaCampoNomeTipoEntidade = this.RetornarEstruturaCampoNomeTipoEntidade();

            }
            this.EstruturasCamposComputado = this.RetornarEstruturasCamposComputado();
            this.MaximoRegistroPorConsulta = this.RetornarMaximoRegistroPorConsulta();

            this.Alertas.AddRange(this.EstruturasCampos.Values.SelectMany(x => x.Alertas));

            this.EstruturaCampoIdentificadorProprietarioProtegido = this.RetornarEstruturaCampoIdentificadorProprietario();
            this.EstruturaCampoOrdenacaoProtegido = this.RetornarEstruturaCampoOrdenacao();
            this.EstruturaCampoDeletadoProtegido = this.RetornarEstruturaCampoDeletado();
            this.EstruturaCampoUsuarioProtegido = this.RetornarEstruturaEstruturaCampoUsuario();

            //if (!this.EstruturasCampos.ContainsKey(NOME_PROPRIEDADE_ID))
            //{
            //    this.EstruturasCampos.Add(NOME_PROPRIEDADE_ID, this.EstruturaCampoChavePrimaria);
            //}
        }

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
        #endregion

        #region Metodos Internos

        /// <summary>
        /// Associa a EstruturaRelacaoPai na estruturaRelacaoFilhos
        /// </summary>
        internal void AssociarEstruturaRalacaos()
        {
            var estruturasRelacaoUmUmReversa = this.EstruturasRelacoes.Values.OfType<EstruturaRelacaoUmUmReversa>().ToList();
            foreach (var estruturaRelacaoUmUmReversa in estruturasRelacaoUmUmReversa)
            {
                var estruturaRelacaoUmUm = estruturaRelacaoUmUmReversa.EstruturaEntidadeUmUmReversa.RetornarTodasRelacoesUmUm().
                                                                       Where(x => x.EstruturaEntidadeChaveEstrangeiraDeclarada == estruturaRelacaoUmUmReversa.EstruturaEntidade).SingleOrDefault();

                estruturaRelacaoUmUmReversa.EstruturaRelacaoUmUm = estruturaRelacaoUmUm;
            }
            var estruturasRelacaoFilhos = this.EstruturasRelacoes.Values.OfType<EstruturaRelacaoFilhos>().ToList();
            foreach (var estruturaRelacaoFilhos in estruturasRelacaoFilhos)
            {
                //var estruturaRelacaoPai = estruturaRelacaoFilhos.EstruturaEntidadeFilho.EstruturasRelacoes.Values.OfType<EstruturaRelacaoPai>().Where(x => x.EstruturaEntidadePai == estruturaRelacaoFilhos.EstruturaEntidade).Single();
                var todasRelacoesPai = estruturaRelacaoFilhos.EstruturaEntidadeFilho.RetornarTodasRelacoesPai();
                var estruturaRelacaoPai = todasRelacoesPai.Where(x => x.EstruturaEntidadeChaveEstrangeiraDeclarada == estruturaRelacaoFilhos.EstruturaEntidade).SingleOrDefault();
                if (estruturaRelacaoPai == null)
                {
                    throw new Exception($"Não foi encontrado a propriedade de relação pai do tipo {estruturaRelacaoFilhos.EstruturaEntidade.TipoEntidade.Name} na entidade {estruturaRelacaoFilhos.EstruturaEntidadeFilho.TipoEntidade.Name}");
                }
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
                    throw new Erro(String.Format("O atributo relacao do tipo {0} não é suportado", proriedade.PropertyType.Name));
                }
            }
        }

        internal void PreencherEstruturasAlteracaoPropriedade()
        {
            this.EstruturasAlteracaoPropriedade = this.RetornarEstruturasAlteracaoPropriedade();
            this.EstruturasAlteracaoPropriedadeGenerica = this.RetornarEstruturasAlteracaoPropriedadeGenerica();
        }

        internal EstruturaCampo RetornarEstruturaCampo(string chave)
        {
            EstruturaCampo estruturaCampo;

            if (this.EstruturasCampos.TryGetValue(chave, out estruturaCampo))
            {
                return estruturaCampo;
            }
            if (this.EstruturaCampoNomeTipoEntidade.Propriedade.Name == chave)
            {
                return this.EstruturaCampoNomeTipoEntidade;
            }
            if (System.Diagnostics.Debugger.IsAttached)
            {
                if (this.EstruturasCampos.Any(x => x.Key.Equals(chave, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var parChave = this.EstruturasCampos.Where(x => x.Key.Equals(chave, StringComparison.InvariantCultureIgnoreCase)).Single();
                    estruturaCampo = parChave.Value;
                    var alerta = String.Format(" A chave do estrtura campo {0} deveria ser {1} - em {2}", chave, parChave.Key, this.TipoEntidade.Name);
                    this.Alertas.Add(alerta);
                    return estruturaCampo;
                }
            }
            if (this.EstruturaEntidadeBase == null)
            {
                throw new Erro(String.Format("A relação {0} não foi encontrado em {1} - {2}", chave, this.TipoEntidade.Name, this.NomeTabela));
            }
            return this.EstruturaEntidadeBase.RetornarEstruturaCampo(chave);
        }

        internal List<EstruturaRelacaoFilhos> RetornarTodasRelacoesFilhos()
        {
            return this.RetornarTodasEstruturas<EstruturaRelacaoFilhos>();
        }

        internal List<EstruturaRelacaoChaveEstrangeira> RetornarTodasRelacoesChaveEstrangeira()
        {
            return this.RetornarTodasEstruturas<EstruturaRelacaoChaveEstrangeira>();
        }

        internal List<EstruturaRelacaoPai> RetornarTodasRelacoesPai()
        {
            return this.RetornarTodasEstruturas<EstruturaRelacaoPai>();
        }

        internal List<EstruturaRelacaoUmUm> RetornarTodasRelacoesUmUm()
        {
            return this.RetornarTodasEstruturas<EstruturaRelacaoUmUm>();
        }

        internal List<EstruturaRelacaoUmUmReversa> RetornarTodasRelacoesUmUmReversa()
        {
            return this.RetornarTodasEstruturas<EstruturaRelacaoUmUmReversa>();
        }

        internal List<EstruturaRelacaoNn> RetornarTodasRelacoesNn()
        {
            return this.RetornarTodasEstruturas<EstruturaRelacaoNn>();
        }

        internal List<EstruturaAlteracaoPropriedade> RetornarTodasEstruturasAlteracaoPropriedade()
        {
            var estruturas = new List<EstruturaAlteracaoPropriedade>();
            var estruturaAtual = this;
            while (estruturaAtual != null)
            {
                estruturas.AddRange(estruturaAtual.EstruturasAlteracaoPropriedade);
                estruturaAtual = estruturaAtual.EstruturaEntidadeBase;
            }
            return estruturas;
        }

        internal List<EstruturaAlteracaoPropriedadeGenerica> RetornarTodasEstruturasAlteracaoPropriedadeGenerica()
        {

            var estruturas = new List<EstruturaAlteracaoPropriedadeGenerica>();
            var estruturaAtual = this;
            while (estruturaAtual != null)
            {
                estruturas.AddRange(estruturaAtual.EstruturasAlteracaoPropriedadeGenerica);
                estruturaAtual = estruturaAtual.EstruturaEntidadeBase;
            }
            return estruturas;

           
        }


        #endregion

        #region Metodos privados

        private List<TEstruturaRelacao> RetornarTodasEstruturas<TEstruturaRelacao>()
        {
            var estruturasRelacao = new List<TEstruturaRelacao>();
            var estruturaAtual = this;
            while (estruturaAtual != null)
            {
                estruturasRelacao.AddRange(estruturaAtual.EstruturasRelacoes.Values.OfType<TEstruturaRelacao>());
                estruturaAtual = estruturaAtual.EstruturaEntidadeBase;
            }
            return estruturasRelacao;
        }

        private DicionarioEstrutura<EstruturaCampo> RetornarEstruturasCampo()
        {
            var estruturasCampos = new DicionarioEstrutura<EstruturaCampo>();
            var propriedadesCampo = AjudanteEstruturaBancoDados.RetornarPropriedadesCampos(this.TipoEntidade);
            foreach (var propriedadeCampo in propriedadesCampo)
            {
                if (!this.IsAbstrata && this.IsBancoDadosNaoGerenciavel &&
                    propriedadeCampo.Name == "__NomeTipoEntidade")
                {
                    continue;
                }

                estruturasCampos.Add(propriedadeCampo.Name, this.RetornarNovaEstruturaCampo(propriedadeCampo));
            }

            return estruturasCampos;
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

        public EstruturaCampo RetornarEstruturaoNomeTipoEntidade()
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
                throw new Exception($"A entidade { propriedade.PropertyType.Name } não foi encontrada, declare-a no DbSet do contexto de migração e atualizar a extenção");
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
                    throw new Erro(String.Format("O atributo chave estangeira da Ligacao RelacaoFilho nao foi definido, Entidade {0} - Propriedade {1} - EntidadePai {2}", this.TipoEntidade.Name, propriedade.Name, estruturaEntidadePai.TipoEntidade.Name));
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
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Trace.Write($" Testear relacao filhos  {propriedade.DeclaringType.Name}{propriedade.Name} ");
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
                    throw new Erro($"Não foi encontrado um chave estrangeira para a propriedade { propriedadeRelacao.Name} na entidade {propriedadeRelacao.DeclaringType.Name} ");
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
                throw new Exception($"Entidade {atributoRelacaoNn.TipoEntidadeRelacao.Name} não foi declarada o DbSet no Contexto de migracao ou extesão e extensão não foi acionada");
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
        /// Campos computado, são campos se teraão alteracoes quando uma nova enitdade for salvar, essa computação será realizada pelo banco de dados
        /// Ex, ValorPadraoDataHoraServidor, IOrdenacao
        /// </summary>
        /// <returns></returns>
        private List<EstruturaCampo> RetornarEstruturasCamposComputado()
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

                if (propriedade.GetCustomAttribute<PropriedadeComputadoAttribute>() != null)
                {
                    estruturasCampoComputado.Add(estruturaCampo);
                }
            }
            return estruturasCampoComputado;
        }

        private List<EstruturaAlteracaoPropriedade> RetornarEstruturasAlteracaoPropriedade()
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

            return estruturasAlteracaoPropriedade;
        }

        private List<EstruturaAlteracaoPropriedadeGenerica> RetornarEstruturasAlteracaoPropriedadeGenerica()
        {
            var estruturasAlteracaoPropriedade = new List<EstruturaAlteracaoPropriedadeGenerica>();
            var propriedades = ReflexaoUtil.RetornarPropriedades(this.TipoEntidade, true, true);
            foreach (var propriedade in propriedades)
            {
                var atributo = propriedade.GetCustomAttribute<NotificarAlteracaoPropriedadeGenericaAttribute>();
                if (atributo != null)
                {
                    if (this.EstruturasCampos.ContainsKey(propriedade.Name))
                    {
                        var estruturaCampo = this.EstruturasCampos[propriedade.Name];
                        estruturasAlteracaoPropriedade.Add(new EstruturaAlteracaoPropriedadeGenerica(propriedade, this, estruturaCampo, atributo));
                        continue;
                    }

                    throw new Erro($"A propriedade {propriedade.Name} do tipo {propriedade.PropertyType.Name} declarada em {propriedade.DeclaringType.Name} não suporta o atributo " +
                                  $"{atributo.GetType().Name}  ");
                }
            }

            return estruturasAlteracaoPropriedade;
        }

        private int RetornarMaximoRegistroPorConsulta()
        {
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
                return this.EstruturasCampos[nomePropriedadeOrdenacao];
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
    }
}