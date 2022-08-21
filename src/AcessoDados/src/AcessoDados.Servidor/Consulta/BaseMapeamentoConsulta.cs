using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.AcessoDados.Dominio;
using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class BaseMapeamentoConsulta : IDisposable
    {
        #region Propriedades 

        internal EstruturaBancoDados EstruturaBancoDados { get; set; }

        internal BaseConexao ConexaoDB { get; set; }

        internal int ContadorApelido { get; set; }

        internal Dictionary<String, BaseEstruturaApelido> EstruturasApelido { get; set; }

        internal Type TipoEntidade { get; set; }

        internal EstruturaEntidade EstruturaEntidade { get; set; }

        internal EstruturaConsulta EstruturaConsulta { get; set; }

        internal BaseContextoDados Contexto { get; }

        #endregion

        #region Construtor

        internal BaseMapeamentoConsulta(EstruturaConsulta consulta,
                                        EstruturaBancoDados estruturaBancoDados,
                                        BaseConexao ConexaoDB,
                                        BaseContextoDados contexto)
        {
            this.EstruturaConsulta = consulta;
            this.EstruturaBancoDados = estruturaBancoDados;
            this.ConexaoDB = ConexaoDB;
            this.Contexto = contexto;
            this.EstruturasApelido = new Dictionary<String, BaseEstruturaApelido>();
        }
        #endregion

        internal int RetornarContadorApelido()
        {
            this.ContadorApelido += 1;
            return this.ContadorApelido;
        }
        #region IDisposable

        public virtual void Dispose()
        {
        }
        #endregion
    }
}