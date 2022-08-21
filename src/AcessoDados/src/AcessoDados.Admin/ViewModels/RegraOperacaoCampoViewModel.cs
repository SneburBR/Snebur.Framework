using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using Snebur.Utilidade;

namespace Snebur.AcessoDados.Admin.ViewModels
{
    public class RegraOperacaoCampoViewModel : BaseRegraOperacaoViewModel
    {
        public PermissaoCampoViewModel PermissaoCampoViewModel { get; }

        public override IRegraOperacao RegraOperacao
        {
            get
            {
                if (this.PermissaoCampoViewModel?.PermissaoCampo != null)
                {
                    return (IRegraOperacao)ReflexaoUtil.RetornarValorPropriedade(this.PermissaoCampoViewModel.PermissaoCampo, this.NomeOperacao);
                }
                return null;
            }
        }

        public override bool Autorizado
        {
            get
            {
                return this.RegraOperacao?.Autorizado ??
                       this.RegraOperacaoEntidadeViewModel.Autorizado;
            }
            set
            {
                base.Autorizado = value;
            }
        }

        public override bool AvalistaRequerido
        {
            get
            {
                return this.RegraOperacao?.AvalistaRequerido ?? this.RegraOperacaoEntidadeViewModel.AvalistaRequerido;
            }
            set
            {
                base.AvalistaRequerido = value;
            }
        }

        public override bool AtivarLogAlteracao
        {
            get
            {
                return this.RegraOperacao?.AtivarLogAlteracao ?? this.RegraOperacaoEntidadeViewModel.AtivarLogAlteracao;
            }
            set
            {
                base.AtivarLogAlteracao = value;
            }
        }

        public override bool AtivarLogSeguranca
        {
            get
            {
                return this.RegraOperacao?.AtivarLogSeguranca ?? this.RegraOperacaoEntidadeViewModel.AtivarLogSeguranca;
            }
            set
            {
                base.AtivarLogSeguranca = value;
            }
        }

        public override int MaximoRegistro
        {
            get
            {
                return this.RegraOperacao?.MaximoRegistro ?? this.RegraOperacaoEntidadeViewModel.MaximoRegistro;
            }
            set
            {
                var maximoRegistroEntidade = this.RegraOperacaoEntidadeViewModel.MaximoRegistro;
                if (maximoRegistroEntidade > 0 && value > maximoRegistroEntidade)
                {
                    value = maximoRegistroEntidade;
                }
                base.MaximoRegistro = value;
            }
        }

        public RegraOperacaoEntidadeViewModel RegraOperacaoEntidadeViewModel { get; }

        public RegraOperacaoCampoViewModel(PermissaoCampoViewModel permissaoCampoViewModel,
                                           IRegraOperacao regraOperacao,
                                            string nomeOperacao,
                                           RegraOperacaoEntidadeViewModel regraOperacaoEntidaeViewModel) :
                                           base(regraOperacao, nomeOperacao)
        {
            this.PermissaoCampoViewModel = permissaoCampoViewModel;
            this.RegraOperacaoEntidadeViewModel = regraOperacaoEntidaeViewModel;
            this.IsAtivo = this.RegraOperacaoEntidadeViewModel.Autorizado;
            this.RegraOperacaoEntidadeViewModel.RegraAlterada += this.RegraOperacaoEntidadeViewModel_Alterada;

        }

        private void RegraOperacaoEntidadeViewModel_Alterada(object sender, EventArgs e)
        {
            this.IsAtivo = this.RegraOperacaoEntidadeViewModel.Autorizado;
            if (!this.RegraOperacaoEntidadeViewModel.Autorizado)
            {
                this.Autorizado = false;
            }
        }

        public RegraOperacaoCampoViewModel(PermissaoCampoViewModel permissaoCampoViewModel) : base()
        {
            this.PermissaoCampoViewModel = permissaoCampoViewModel;
        }

        protected override bool AtualizarPropriedade(object value, [CallerMemberName] string nomePropriedade = "")
        {
            if (this.RegraOperacao == null)
            {
                var regraOperacao = this.RegraOperacaoEntidadeViewModel.RegraOperacao ??
                                    this.RegraOperacaoEntidadeViewModel.RegraOperacaoPai;

                var valorPropriedadeEntidade = ReflexaoUtil.RetornarValorPropriedade(regraOperacao, nomePropriedade);
                if ((valorPropriedadeEntidade == null ^ value == null) ||
                    (valorPropriedadeEntidade != null && !valorPropriedadeEntidade.Equals(value)))
                {
                    this.PermissaoCampoViewModel.AtribuirNovaEntidadePermissaoCampo();
                    return true;
                }
                return false;
            }
            return true;
        }

        protected override void NotificarRegraAlterada()
        {
            base.NotificarRegraAlterada();
            this.RegraOperacaoEntidadeViewModel?.
                 PermissaoEntidadeViewModel?.
                 NotificarPropriedadesPermissaoAtivaPermissaoCompleta();
        }


    }
}
