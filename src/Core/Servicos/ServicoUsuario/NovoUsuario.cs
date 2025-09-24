using Snebur.Dominio.Atributos;
using System.Diagnostics.CodeAnalysis;

namespace Snebur.Dominio;

public abstract class NovoUsuario : BaseDominio
{
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public Func<object, string>? FuncRetornarIdentificadorUsuario { get; set; }

    [ValidacaoRequerido]
    [ValidacaoTextoTamanho(255)]
    public string? Nome { get; set; }

    [ValidacaoRequerido]
    [ValidacaoEmail]
    public string? Email { get; set; }

    [ValidacaoRequerido]
    [ValidacaoTelefone]
    public string? Telefone { get; set; }

    [ValidacaoRequerido]
    [ValidacaoSenha]
    public string? Senha { get; set; }

    [MemberNotNullWhen(true, nameof(Nome))]
    [MemberNotNullWhen(true, nameof(Email))]
    [MemberNotNullWhen(true, nameof(Telefone))]
    [MemberNotNullWhen(true, nameof(Senha))]
    public bool IsValido
        => !String.IsNullOrWhiteSpace(this.Nome)
        && !String.IsNullOrWhiteSpace(this.Email)
        && !String.IsNullOrWhiteSpace(this.Telefone)
        && !String.IsNullOrWhiteSpace(this.Senha);

    [IgnorarNormalizacao]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public string IdentificadorAmigavel
    {
        get
        {
            if (ValidacaoUtil.IsEmail(this.Email))
            {
                return this.Email;
            }

            if (ValidacaoUtil.IsTelefone(this.Telefone))
            {
                return this.Telefone;
            }
            return String.Empty;
        }
    }
}
public class NovoUsuarioCliente : NovoUsuario
{
    public required long Cliente_Id { get; set; }
}
public class NovoUsuarioAdmin : NovoUsuario
{
    public bool IsMaster { get; set; }
    public required long EmpresaAdmin_Id { get; set; }
}