using Snebur.Dominio;
using Snebur.Utilidade;
using System;

namespace Snebur.AcessoDados
{
    public abstract partial class BaseConsultaEntidade
    {
        #region Propriedades

        protected Type TipoEntidadeConsulta { get; set; }

        public EstruturaConsulta EstruturaConsulta { get; set; }

        public __BaseContextoDados ContextoDados { get; set; }

        #endregion

        #region Construtores

        public BaseConsultaEntidade(__BaseContextoDados contextoDados, Type tipoEntidadeConsulta) : this(contextoDados, tipoEntidadeConsulta, null)
        {
        }

        public BaseConsultaEntidade(__BaseContextoDados contextoDados, Type tipoEntidadeConsulta, EstruturaConsulta estruturaConsulta)
        {
            this.TipoEntidadeConsulta = tipoEntidadeConsulta;
            this.ContextoDados = contextoDados;

            if (estruturaConsulta == null)
            {
                this.EstruturaConsulta = new EstruturaConsulta
                {
                    TipoEntidadeConsulta = tipoEntidadeConsulta,
                    NomeTipoEntidade = tipoEntidadeConsulta.Name,
                    TipoEntidadeAssemblyQualifiedName = tipoEntidadeConsulta.AssemblyQualifiedName
                };
            }
            else
            {
                this.EstruturaConsulta = estruturaConsulta;
            }
            if (tipoEntidadeConsulta.IsInterface)
            {
                throw new ErroNaoSuportado("Interface não é suportado, usar outro construtor");
            }
            if (!tipoEntidadeConsulta.IsSubclassOf(typeof(Entidade)))
            {
                throw new ErroNaoSuportado("O tipo da entidade não é suportado");
            }
        }
        #endregion

        public override string ToString()
        {
            return String.Format("Consulta {0}  {1}", this.TipoEntidadeConsulta.Name, base.ToString());
        }
    }
}