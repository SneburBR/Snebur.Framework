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
    public class RegraOperacaoEntidadeViewModel : BaseRegraOperacaoViewModel
    {
        public PermissaoEntidadeViewModel PermissaoEntidadeViewModel { get; set; }

        public IRegraOperacao RegraOperacaoPai
        {
            get
            {
                var permissaoEntidadeViewModel = this.PermissaoEntidadeViewModel.PermissaoEntidadePai;
                while (permissaoEntidadeViewModel != null)
                {
                    var regraOperacaoViewModel = (RegraOperacaoEntidadeViewModel)ReflexaoUtil.RetornarValorPropriedade(permissaoEntidadeViewModel, this.NomeOperacao);
                    if (regraOperacaoViewModel.RegraOperacao != null)
                    {
                        return regraOperacaoViewModel.RegraOperacao;
                    }
                    permissaoEntidadeViewModel = permissaoEntidadeViewModel.PermissaoEntidadePai;
                }
                return null;
            }
        }

        public override bool Autorizado
        {
            get
            {
                return this.RegraOperacao?.Autorizado ??
                       this.RegraOperacaoPai.Autorizado;
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
                return this.RegraOperacao?.AvalistaRequerido ?? this.RegraOperacaoPai.AvalistaRequerido;
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
                return this.RegraOperacao?.AtivarLogAlteracao ?? this.RegraOperacaoPai.AtivarLogAlteracao;
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
                return this.RegraOperacao?.AtivarLogSeguranca ?? this.RegraOperacaoPai.AtivarLogSeguranca;
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
                return this.RegraOperacao?.MaximoRegistro ?? this.RegraOperacaoPai.MaximoRegistro;
            }
            set
            {
                var maximoRegestrioPai = this.RegraOperacaoPai?.MaximoRegistro ?? 0;
                if (maximoRegestrioPai > 0 && value > maximoRegestrioPai)
                {
                    value = maximoRegestrioPai;
                }
                base.MaximoRegistro = value;
            }
        }

        public RegraOperacaoEntidadeViewModel(PermissaoEntidadeViewModel permissaoEntidadeViewModel,
                                              IRegraOperacao regraOperacao,
                                              string nomeOperacao) :
                                              base(regraOperacao, nomeOperacao)
        {
            this.PermissaoEntidadeViewModel = permissaoEntidadeViewModel;
        }

        protected override bool AtualizarPropriedade(object value, [CallerMemberName] string nomePropriedade = "")
        {
            if (this.RegraOperacao == null)
            {

                var valorPropriedadeEntidade = ReflexaoUtil.RetornarValorPropriedade(this.RegraOperacaoPai, nomePropriedade);
                if ((valorPropriedadeEntidade == null ^ value == null) ||
                    (valorPropriedadeEntidade != null && !valorPropriedadeEntidade.Equals(value)))
                {

                    if (this.PermissaoEntidadeViewModel.PermissaoEntidade == null)
                    {
                        this.PermissaoEntidadeViewModel.AtribuirNovaEntidadePermissaoEntidade();
                    }

                    var regraOperacao = (IRegraOperacao)ReflexaoUtil.RetornarValorPropriedade(this.PermissaoEntidadeViewModel.PermissaoEntidade, this.NomeOperacao);
                    this.RegraOperacao = regraOperacao;

                    return true;
                }
                return false;
            }
            return true;
        }


    }
}
