using System.Diagnostics.CodeAnalysis;

namespace Snebur.Seguranca;

[Plural("Credenciais")]
public abstract class Credencial : BaseDominio, ICredencial
{
    public string? IdentificadorUsuario { get; set; }

    public string? IdentificadorAmigavel { get; set; }

    public string? Senha { get; set; }

    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public bool IsAnonimo 
        => Util.SaoIgual(this.IdentificadorUsuario, CredencialAnonimo.Anonimo.IdentificadorUsuario);

    [MemberNotNullWhen(true, nameof(IdentificadorUsuario))]
    [MemberNotNullWhen(true, nameof(Senha))]
    public bool IsValido
        => !String.IsNullOrEmpty(this.IdentificadorUsuario) &&
           !String.IsNullOrEmpty(this.Senha);

    public Credencial()
    {
    }

    [IgnorarConstrutorTS]
    public Credencial(string? identificadorUsuario, string? senha)
    {
        this.IdentificadorUsuario = identificadorUsuario;
        this.Senha = senha;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not null)
        {
            if ((obj is Credencial) ||
                (ReflexaoUtil.IsTipoImplementaInterface(obj.GetType(), typeof(ICredencial), false)))
            {
                var credencialValidar = (ICredencial)obj;
                return this.Validar(credencialValidar);
            }
        }
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public bool Validar(ICredencial credencial)
    {
        return CredencialUtil.ValidarCredencial(this, credencial);
        //return this.Validar(credencial.IdentificadorUsuario, credencial.Senha);
    }

    public bool Validar(string? identificadorUsuario, string? senha)
    {
        if (!this.IsValido)
        {
            return false;
        }
        return CredencialUtil.ValidarCredencial(this, new CredencialUsuario(identificadorUsuario, senha));
    }

    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public bool IsEmpty
    {
        get
        {
            return String.IsNullOrEmpty(this.IdentificadorUsuario) && String.IsNullOrEmpty(this.Senha);
        }
    }
    #region ICredencial

    string? ICredencial.IdentificadorUsuario
        => this.IdentificadorUsuario;
    

    string? ICredencial.Senha 
        => this.Senha;

    #endregion

}