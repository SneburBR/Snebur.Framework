using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;

namespace Snebur.Seguranca
{
    public class CredencialUsuario : Credencial
    {
        public string Nome { get; set; }

        public string IdentificadorAmigavel { get; set; }

        //public bool IsAnonimo => this.Validar(CredencialAnonimo.Anonimo);

        public CredencialUsuario()
        {
        }
        [IgnorarConstrutorTS]
        public CredencialUsuario(string identificadorUsuario, string senha) : base(identificadorUsuario, senha)
        {
        }
        //public CredencialUsuario(string identificadorUsuario, string senha, Guid identificadorSessaoUsuario) : base(identificadorUsuario, senha)
        //{
        //    this.IdentificadorSessaoUsuario = identificadorSessaoUsuario;
        //}

        public string RetornarIdentificadorEmail()
        {
            if (ValidacaoUtil.IsEmail(this.IdentificadorAmigavel))
            {
                return this.IdentificadorAmigavel;
            }
            if (ValidacaoUtil.IsEmail(this.IdentificadorUsuario))
            {
                return this.IdentificadorUsuario;
            }
            return null;
        }

        public string RetornarIdentificadorTelefone()
        {
            if (ValidacaoUtil.IsTelefone(this.IdentificadorAmigavel))
            {
                return this.IdentificadorAmigavel;
            }
            if (ValidacaoUtil.IsTelefone(this.IdentificadorUsuario))
            {
                return this.IdentificadorUsuario;
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            if (obj is CredencialUsuario credencial)
            {
                return CredencialUtil.ValidarCredencial(this, credencial);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            if (!String.IsNullOrEmpty(this.IdentificadorUsuario) &&
                !String.IsNullOrEmpty(this.Senha))
            {
                return (this.IdentificadorUsuario?.Trim().ToLower() + this.Senha).GetHashCode();
            }
            return base.GetHashCode();
        }
    }
}