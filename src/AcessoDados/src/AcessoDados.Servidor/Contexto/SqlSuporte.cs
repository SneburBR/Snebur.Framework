#if NET7_0
using Microsoft.Data.SqlClient;
using System;
#else
#endif

using System;

namespace Snebur.AcessoDados
{
    public class BancoDadosSuporta
    {
        public bool IsOffsetFetch { get; }
        public bool IsColunaNomeTipoEntidade { get; }
        public bool IsSessaoUsuario { get; }
        public bool IsMigracao { get; }
        public bool IsDataHoraUtc { get; } 

        internal BancoDadosSuporta(BaseContextoDados contextoDados,
                                    EnumFlagBancoNaoSuportado flags)
        {
            this.IsOffsetFetch = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.OffsetFetch);
            this.IsColunaNomeTipoEntidade = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.ColunaNomeTipoEntidade);
            this.IsSessaoUsuario = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.SessaoUsuario);
            this.IsMigracao = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.Migracao);
            this.IsDataHoraUtc = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.DataHoraUtc);
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
            if (this.IsSessaoUsuario)
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