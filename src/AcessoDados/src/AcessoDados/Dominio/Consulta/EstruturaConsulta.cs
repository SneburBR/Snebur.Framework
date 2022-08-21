using System;
using System.Collections.Generic;
using Snebur.AcessoDados.Seguranca;
using Snebur.Dominio.Atributos;

namespace Snebur.AcessoDados
{
    public class EstruturaConsulta : BaseAcessoDados, IEstruturaConsultaSeguranca
    {
		#region Campos Privados

        private string _nomeTipoEntidade;
        private string _tipoEntidadeAssemblyQualifiedName;
        private bool _isMostrarDeletados;
        private bool _isMostrarInativos;
        private bool _isDesativarOrdenacao;
        private int _take;
        private int _skip;
        private int _paginaAtual;
        private string _caminhoPropriedadeFuncao;
        private EnumTipoFuncao _tipoFuncaoEnum;
        private bool _contarRegistros;

		#endregion

        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        internal Type TipoEntidadeConsulta { get; set; }

        public string NomeTipoEntidade { get => this.RetornarValorPropriedade(this._nomeTipoEntidade); set => this.NotificarValorPropriedadeAlterada(this._nomeTipoEntidade, this._nomeTipoEntidade = value); }

        public string TipoEntidadeAssemblyQualifiedName { get => this.RetornarValorPropriedade(this._tipoEntidadeAssemblyQualifiedName); set => this.NotificarValorPropriedadeAlterada(this._tipoEntidadeAssemblyQualifiedName, this._tipoEntidadeAssemblyQualifiedName = value); }

        public bool IsMostrarDeletados { get => this.RetornarValorPropriedade(this._isMostrarDeletados); set => this.NotificarValorPropriedadeAlterada(this._isMostrarDeletados, this._isMostrarDeletados = value); }

        public bool IsMostrarInativos { get => this.RetornarValorPropriedade(this._isMostrarInativos); set => this.NotificarValorPropriedadeAlterada(this._isMostrarInativos, this._isMostrarInativos = value); }

        public bool IsDesativarOrdenacao { get => this.RetornarValorPropriedade(this._isDesativarOrdenacao); set => this.NotificarValorPropriedadeAlterada(this._isDesativarOrdenacao, this._isDesativarOrdenacao = value); }

        public int Take { get => this.RetornarValorPropriedade(this._take); set => this.NotificarValorPropriedadeAlterada(this._take, this._take = value); }

        public int Skip { get => this.RetornarValorPropriedade(this._skip); set => this.NotificarValorPropriedadeAlterada(this._skip, this._skip = value); }

        public int PaginaAtual { get => this.RetornarValorPropriedade(this._paginaAtual); set => this.NotificarValorPropriedadeAlterada(this._paginaAtual, this._paginaAtual = value); }

        public string CaminhoPropriedadeFuncao { get => this.RetornarValorPropriedade(this._caminhoPropriedadeFuncao); set => this.NotificarValorPropriedadeAlterada(this._caminhoPropriedadeFuncao, this._caminhoPropriedadeFuncao = value); }

        public EnumTipoFuncao TipoFuncaoEnum { get => this.RetornarValorPropriedade(this._tipoFuncaoEnum); set => this.NotificarValorPropriedadeAlterada(this._tipoFuncaoEnum, this._tipoFuncaoEnum = value); }

        [CriarInstanciaTS]
        public FiltroGrupoE FiltroGrupoE { get; set; } = new FiltroGrupoE();

        [CriarInstanciaTS]
        public FiltroGrupoOU FiltroGrupoOU { get; set; } = new FiltroGrupoOU();

        public Dictionary<string, Ordenacao> Ordenacoes { get; set; } = new Dictionary<string, Ordenacao>();

        //Temporario
        //[IgnorarPropriedadeTSReflexao]
        public Dictionary<string, RelacaoAbertaEntidade> RelacoesAbertaFiltro { get; set; } = new Dictionary<string, RelacaoAbertaEntidade>();

        //Temporario
        //[IgnorarPropriedadeTSReflexao]
        public Dictionary<string, RelacaoAbertaEntidade> RelacoesAberta { get; set; } = new Dictionary<string, RelacaoAbertaEntidade>();

        //Temporario
        //[IgnorarPropriedadeTSReflexao]
        public Dictionary<string, RelacaoAbertaColecao> ColecoesAberta { get; set; } = new Dictionary<string, RelacaoAbertaColecao>();

        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        public List<string> PropriedadesAbertas { get; set; } = new List<string>();

        public EstruturaConsulta()
        {
        }
        [IgnorarConstrutorTS]
        public EstruturaConsulta(Type tipoEntidadeConsulta)
        {
        }
        #region IEstruturaConsultaSeguranca  

        private HashSet<string> _propriedadesAutorizadas;

        [NaoMapear]
        [IgnorarGlobalizacao]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        HashSet<string> IEstruturaConsultaSeguranca.PropriedadesAutorizadas { get => this._propriedadesAutorizadas; }

        public bool ContarRegistros { get => this.RetornarValorPropriedade(this._contarRegistros); set => this.NotificarValorPropriedadeAlterada(this._contarRegistros, this._contarRegistros = value); }

        void IEstruturaConsultaSeguranca.AtribuirPropriedadeAutorizadas(HashSet<string> propriedadesAutorizadas)
        {
            this._propriedadesAutorizadas = propriedadesAutorizadas;
        }
        #endregion
    }
}