using Snebur.AcessoDados.Seguranca;
using System.Diagnostics.CodeAnalysis;

namespace Snebur.AcessoDados;

public abstract class Resultado : BaseAcessoDados
{

    #region Campos Privados

    private bool _isSucesso;
    private EnumPermissao _permissao;

    #endregion

    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public ErroAcessoDados? Erro { get; set; } = null;

    [MemberNotNullWhen(false, nameof(Erro))]
    public bool IsSucesso { get => this.GetPropertyValue(this._isSucesso); set => this.SetProperty(this._isSucesso, this._isSucesso = value); }

    public EnumPermissao Permissao { get => this.GetPropertyValue(this._permissao); set => this.SetProperty(this._permissao, this._permissao = value); }
}