#if NET7_0
using Microsoft.Data.SqlClient;
using System;
#else
#endif

using System;
using System.Runtime.CompilerServices;

namespace Snebur.AcessoDados
{
    public class BancoDadosSuporta
    {
 
        public bool IsOffsetFetch { get; }
        public bool IsColunaNomeTipoEntidade { get; }
        public bool IsSessaoUsuario { get; }
        public bool IsSessaoUsuarioHerdada { get; }
        public bool IsSessaoUsuarioContextoAtual { get; }
        public bool IsMigracao { get; }
        public bool IsDataHoraUtc { get; }

        internal BancoDadosSuporta(BaseContextoDados contextoDados,
                                    EnumFlagBancoNaoSuportado flags)
        {
            this.IsOffsetFetch = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.OffsetFetch);
            this.IsColunaNomeTipoEntidade = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.ColunaNomeTipoEntidade);

            this.IsMigracao = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.Migracao);
            this.IsDataHoraUtc = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.DataHoraUtc);

           
            this.IsSessaoUsuarioContextoAtual = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.SessaoUsuario) &&
                                         this.IsSuporta(flags, EnumFlagBancoNaoSuportado.SessaoUsuarioHerdada);
            if (!this.IsSessaoUsuarioContextoAtual)
            {
                this.IsSessaoUsuarioHerdada = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.SessaoUsuarioHerdada);
            }
            this.IsSessaoUsuario = this.IsSessaoUsuarioContextoAtual || this.IsSessaoUsuarioHerdada;

        }

        private bool IsSuporta(EnumFlagBancoNaoSuportado sqlNaoSuporta, EnumFlagBancoNaoSuportado flag)
        {
            if ((sqlNaoSuporta & flag) == flag)
            {
                return false;
            }
            return true;
        }

        internal void ValidarSuporteSessaoUsuario()
        {
            if (this.IsSessaoUsuarioHerdada)
            {
                return;
            }
            throw new ErroBancoDadosSuporte("O banco de dados não suporta gerenciamento das sessão do usuário");
        }
    }

    public class ErroBancoDadosSuporte : Erro
    {
        public ErroBancoDadosSuporte(string mensagem) : base(mensagem) { }
    }
}