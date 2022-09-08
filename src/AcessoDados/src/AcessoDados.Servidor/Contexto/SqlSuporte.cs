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

        internal SqlSuporte(EnumFlagBancoNaoSuporte flags)
        {
            this.IsOffsetFetch = this.IsSuporta(flags, EnumFlagBancoNaoSuporte.OffsetFetch);
            this.IsColunaNomeTipoEntidade = this.IsSuporta(flags, EnumFlagBancoNaoSuporte.ColunaNomeTipoEntidade);
            this.IsUsuario = this.IsSuporta(flags, EnumFlagBancoNaoSuporte.Usuario);
            this.IsMigracao = this.IsSuporta(flags, EnumFlagBancoNaoSuporte.Migracao);
        }

        private bool IsSuporta(EnumFlagBancoNaoSuporte sqlNaoSuporta, EnumFlagBancoNaoSuporte flag)
        {
            if ((sqlNaoSuporta & flag) == flag)
            {
                return false;
            }
            return true;
        }
    }
}