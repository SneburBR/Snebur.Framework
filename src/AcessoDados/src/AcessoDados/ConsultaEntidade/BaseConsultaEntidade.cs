using Snebur.Dominio;
using System;

namespace Snebur.AcessoDados
{
    public abstract partial class BaseConsultaEntidade
    {
        #region Propriedades

        protected Type TipoEntidadeConsulta { get; }

        public EstruturaConsulta EstruturaConsulta { get; }

        public __BaseContextoDados ContextoDados { get; }

        #endregion

        #region Construtores

        protected BaseConsultaEntidade(__BaseContextoDados contextoDados, Type tipoEntidadeConsulta) : this(contextoDados, tipoEntidadeConsulta, null)
        {
        }

        protected BaseConsultaEntidade(__BaseContextoDados contextoDados,
                                       Type tipoEntidadeConsulta,
                                       EstruturaConsulta estruturaConsulta)
        {
            this.TipoEntidadeConsulta = tipoEntidadeConsulta;
            this.ContextoDados = contextoDados;

            if (estruturaConsulta == null)
            {
                this.EstruturaConsulta = new EstruturaConsulta
                {
                    TipoEntidadeConsulta = tipoEntidadeConsulta,
                    NomeTipoEntidade = tipoEntidadeConsulta.Name,
                    TipoEntidadeAssemblyQualifiedName = tipoEntidadeConsulta.RetornarAssemblyQualifiedName()
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