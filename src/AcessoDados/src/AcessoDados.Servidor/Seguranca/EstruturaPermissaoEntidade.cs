using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;

namespace Snebur.AcessoDados.Seguranca
{
    internal class EstruturaPermissaoEntidade
    {
        internal IIdentificacao Identificacao { get; }

        internal IPermissaoEntidade PermissaoEntidade { get; }

        internal Dictionary<string, EstruturaPermissaoCampo> PermissoesCampo { get; } = new Dictionary<string, EstruturaPermissaoCampo>();

        internal Dictionary<string, EstruturaRestricaoFiltro> RestricoesFiltro { get; } = new Dictionary<string, EstruturaRestricaoFiltro>();

        internal IRegraOperacao RetornarRegraOperacao(EnumOperacao operacao)
        {
            switch (operacao)
            {
                case EnumOperacao.Leitura:

                    return this.PermissaoEntidade.Leitura;

                case EnumOperacao.Adicionar:

                    return this.PermissaoEntidade.Adicionar;

                case EnumOperacao.Atualizar:

                    return this.PermissaoEntidade.Atualizar;

                case EnumOperacao.Deletar:

                    return this.PermissaoEntidade.Deletar;

                default:

                    throw new Erro(String.Format("A operação não é suportada {0}", EnumUtil.RetornarDescricao(operacao)));
            }
        }

        internal EstruturaPermissaoEntidade(IPermissaoEntidade permissaoEntidade)
        {
            ValidacaoUtil.ValidarReferenciaNula(permissaoEntidade, nameof(permissaoEntidade));

            this.PermissaoEntidade = permissaoEntidade;

            ValidacaoUtil.ValidarReferenciaNula(permissaoEntidade.Leitura, nameof(permissaoEntidade.Leitura));
            ValidacaoUtil.ValidarReferenciaNula(permissaoEntidade.Atualizar, nameof(permissaoEntidade.Atualizar));
            ValidacaoUtil.ValidarReferenciaNula(permissaoEntidade.Adicionar, nameof(permissaoEntidade.Adicionar));
            ValidacaoUtil.ValidarReferenciaNula(permissaoEntidade.Deletar, nameof(permissaoEntidade.Deletar));

            foreach (var permissaoCampo in this.PermissaoEntidade.PermissoesCampo)
            {
                this.PermissoesCampo.Add(permissaoCampo.NomeCampo, new EstruturaPermissaoCampo(permissaoCampo));
            }
            foreach (var restricao in this.PermissaoEntidade.RestricoesEntidade)
            {
                throw new NotImplementedException();
            }
        }
    }
}