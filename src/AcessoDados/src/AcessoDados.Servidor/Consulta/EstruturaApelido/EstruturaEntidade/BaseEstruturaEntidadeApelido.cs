using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento
{
    internal abstract partial class BaseEstruturaEntidadeApelido : BaseEstruturaApelido, IDisposable
    {

        internal EstruturaCampoApelido EstruturaCampoApelidoChavePrimaria { get; set; }

        internal EstruturaEntidade EstruturaEntidade { get; set; }

        internal List<EstruturaEntidadeApelidoBase> EstruturasEntidadeMapeadaBase { get; set; }

        internal List<EstruturaCampoApelido> EstruturasCampoApelido { get; set; }

        internal List<EstruturaEntidadeApelido> EstruturasEntidadeRelacaoMapeadaInterna { get; set; }

        internal BaseEstruturaEntidadeApelido(BaseMapeamentoConsulta mapeamentoConsulta,
                                              string apelidoEntidadeMapeada,
                                              EstruturaEntidade estruturaEntidade
                                              ) :
                                              base(mapeamentoConsulta,
                                                   apelidoEntidadeMapeada, String.Format("[{0}].[{1}]", estruturaEntidade.Schema, estruturaEntidade.NomeTabela))
                                                   
        {
            this.EstruturaEntidade = estruturaEntidade;
            this.EstruturasEntidadeMapeadaBase = new List<EstruturaEntidadeApelidoBase>();
            this.EstruturasEntidadeRelacaoMapeadaInterna = new List<EstruturaEntidadeApelido>();
            this.EstruturasCampoApelido = new List<EstruturaCampoApelido>();
        }

        internal string CaminhoCampoNomeTipoEntidade()
        {
            var estrutura = this.RetornarEstruturaEntidadeApelidoCampoNomeTipoEntidade();
            return string.Format(" {0}.[__NomeTipoEntidade] As [__NomeTipoEntidade]  ", estrutura.Apelido);
        }

        private BaseEstruturaEntidadeApelido RetornarEstruturaEntidadeApelidoCampoNomeTipoEntidade()
        {
            if (this.EstruturasEntidadeMapeadaBase.Count() > 0)
            {
                return this.EstruturasEntidadeMapeadaBase.Where(x=> x.EstruturaEntidade.EstruturaEntidadeBase == null).Single();
            }
            return this;
        }
        #region IDisposable

        public void Dispose()
        {
            this.EstruturasEntidadeMapeadaBase?.Clear();
            this.EstruturasEntidadeRelacaoMapeadaInterna?.Clear();
            this.EstruturasCampoApelido?.Clear();

            this.EstruturasEntidadeMapeadaBase = null;
            this.EstruturasEntidadeRelacaoMapeadaInterna = null;
            this.EstruturasCampoApelido = null;
        }
        #endregion
    }
}