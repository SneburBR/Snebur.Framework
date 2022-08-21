using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;

namespace Snebur.Dominio
{
    public class NovoUsuario : BaseDominio
    {
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public Func<object, string> FuncRetornarIdentificadorUsuario { get; set; }

        [ValidacaoRequerido]
        [ValidacaoTextoTamanho(255)]
        public string Nome { get; set; }

        [ValidacaoRequerido]
        [ValidacaoEmail]
        public string Email { get; set; }

        [ValidacaoRequerido]
        [ValidacaoTelefone]
        public string Telefone { get; set; }

        [ValidacaoRequerido]
        [ValidacaoSenha]
        public string Senha { get; set; }

        [IgnorarNormalizacao]
        [IgnorarPropriedadeTS]
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
}