using Snebur.Dominio;
using Snebur.Utilidade;

namespace Snebur.AcessoDados.Seguranca
{
    internal class EstruturaPermissaoCampo
    {
        internal IPermissaoCampo PermissaoCampo { get; }

        internal EstruturaPermissaoCampo(IPermissaoCampo permissaoCampo)
        {
            ValidacaoUtil.ValidarReferenciaNula(permissaoCampo, nameof(permissaoCampo));

            this.PermissaoCampo = permissaoCampo;

            ValidacaoUtil.ValidarReferenciaNula(permissaoCampo.Leitura, nameof(permissaoCampo.Leitura));
            ValidacaoUtil.ValidarReferenciaNula(permissaoCampo.Atualizar, nameof(permissaoCampo.Atualizar));
        }
    }
}
