using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Snebur.Dominio;
using Snebur.Utilidade;

namespace Snebur.AcessoDados.Admin.ViewModels
{
    public abstract class BaseRegraOperacaoViewModel : BaseViewModel, IBaseRegraOperacao
    {
        private bool _isAtivo = true;

        public Visibility Visibilidade { get; set; }

        public event EventHandler RegraAlterada;

        public string NomeOperacao { get; }

        public bool IsAtivo
        {
            get
            {
                return this._isAtivo;
            }
            set
            {
                this._isAtivo = value;
                if (!value)
                {
                    this.Autorizado = false;
                }
                this.NotificarPropriedadeAlterada();
            }
        }

        public virtual IRegraOperacao RegraOperacao { get; set; }

        public virtual bool Autorizado
        {
            get
            {
                return this.RegraOperacao?.Autorizado ?? false;
            }
            set
            {
                if (this.AtualizarPropriedade(value))
                {
                    if (!value)
                    {
                        this.AvalistaRequerido = false;
                        this.AtivarLogAlteracao = false;
                        this.AtivarLogSeguranca = true;
                    }
                    else
                    {
                        this.AtivarLogSeguranca = false;
                    }

                    this.RegraOperacao.Autorizado = value;
                    this.NotificarPropriedadeAlterada();
                }
                this.NotificarRegraAlterada();
            }
        }

        public virtual bool AvalistaRequerido
        {
            get
            {
                return this.RegraOperacao?.AvalistaRequerido ?? false;
            }
            set
            {
                if (this.AtualizarPropriedade(value))
                {
                    this.RegraOperacao.AvalistaRequerido = value;
                }
                this.NotificarPropriedadeAlterada();
            }
        }

        public virtual bool AtivarLogAlteracao
        {
            get
            {
                return this.RegraOperacao?.AtivarLogAlteracao ?? false;
            }
            set
            {
                if (this.AtualizarPropriedade(value))
                {
                    this.RegraOperacao.AtivarLogAlteracao = value;
                }
                this.NotificarPropriedadeAlterada();
            }
        }

        public virtual bool AtivarLogSeguranca
        {
            get
            {
                return this.RegraOperacao?.AtivarLogSeguranca ?? false;
            }
            set
            {
                if (this.AtualizarPropriedade(value))
                {
                    this.RegraOperacao.AtivarLogSeguranca = value;
                }
                this.NotificarPropriedadeAlterada();
            }
        }

        public virtual int MaximoRegistro
        {
            get
            {
                return this.RegraOperacao?.MaximoRegistro ?? 0;
            }
            set
            {
                if (this.AtualizarPropriedade(value))
                {
                    this.RegraOperacao.MaximoRegistro = value;
                }

            }
        }
        
        public BaseRegraOperacaoViewModel(IRegraOperacao regraOperacao, string nomeOperacao)
        {
            //ValidacaoUtil.ValidarReferenciaNula(regraOperacao, nameof(regraOperacao));
            //ValidacaoUtil.ValidarReferenciaNula(nomeOperacao, nameof(nomeOperacao));

            this.RegraOperacao = regraOperacao;
            this.NomeOperacao = nomeOperacao;
            this.Visibilidade = Visibility.Visible;
        }

        protected BaseRegraOperacaoViewModel()
        {
            this.Visibilidade = Visibility.Collapsed;
            //this.RegraOperacao = Activator.CreateInstance(Repositorio.TiposSeguranca.TipoRegraOperacao) as IRegraOperacao;
        }

        protected virtual void NotificarRegraAlterada()
        {
            this.RegraAlterada?.Invoke(this, EventArgs.Empty);
        }

        protected override void NotificarPropriedadeAlterada([CallerMemberName] string nomePropriedade = "")
        {
            base.NotificarPropriedadeAlterada(nomePropriedade);
            this.NotificarRegraAlterada();
        }

        protected abstract bool AtualizarPropriedade(object value, [CallerMemberName] string nomePropriedade = "");
    }
}
