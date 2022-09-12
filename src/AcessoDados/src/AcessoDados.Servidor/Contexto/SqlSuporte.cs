#if NET50
using Microsoft.Data.SqlClient;
#else
#endif

namespace Snebur.AcessoDados
{
    public class SqlSuporte
    {
        public bool IsOffsetFetch { get; }
        public bool IsColunaNomeTipoEntidade { get; }
        public bool IsUsuario { get; }
        public bool IsMigracao { get; }

        internal SqlSuporte(EnumFlagBancoNaoSuportado flags)
        {
            this.IsOffsetFetch = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.OffsetFetch);
            this.IsColunaNomeTipoEntidade = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.ColunaNomeTipoEntidade);
            this.IsUsuario = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.Usuario);
            this.IsMigracao = this.IsSuporta(flags, EnumFlagBancoNaoSuportado.Migracao);
        }

        private bool IsSuporta(EnumFlagBancoNaoSuportado sqlNaoSuporta, EnumFlagBancoNaoSuportado flag)
        {
            if ((sqlNaoSuporta & flag) == flag)
            {
                return false;
            }
            return true;
        }
    }
}