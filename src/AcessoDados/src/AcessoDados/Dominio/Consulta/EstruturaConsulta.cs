using Snebur.AcessoDados.Seguranca;
using Snebur.Dominio.Atributos;
using System;
using System.Collections.Generic;

namespace Snebur.AcessoDados
{
    public class EstruturaConsulta : BaseAcessoDados, IEstruturaConsultaSeguranca
    {  
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        internal Type TipoEntidadeConsulta { get; set; }

        public string NomeTipoEntidade { get; set; }

        public string TipoEntidadeAssemblyQualifiedName { get; set; }

        public bool IsIncluirDeletados { get; set; }

        public bool IsIncluirInativos { get; set; }

        public bool IsDesativarOrdenacao { get; set; }

        public int Take { get; set; }

        public int Skip { get; set; }

        public int PaginaAtual { get; set; }

        public string CaminhoPropriedadeFuncao { get; set; }

        public EnumTipoFuncao TipoFuncaoEnum { get; set; }

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

         
        #region IEstruturaConsultaSeguranca  

        private List<string> _propriedadesAutorizadas;

        [NaoMapear]
        [IgnorarGlobalizacao]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        List<string> IEstruturaConsultaSeguranca.PropriedadesAutorizadas { get => this._propriedadesAutorizadas; }

        public bool ContarRegistros { get; set; }

        void IEstruturaConsultaSeguranca.AtribuirPropriedadeAutorizadas(List<string> propriedadesAutorizadas)
        {
            this._propriedadesAutorizadas = propriedadesAutorizadas;
        }
        #endregion
    }
}