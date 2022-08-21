using System;
using System.Data;
using Snebur.Utilidade;

namespace Snebur.AcessoDados
{
    public static class ConfiguracaoAcessoDados
    {
        public static string IdentificadorProprietarioGlobal => ConfiguracaoUtil.IDENTIFICADOR_PROPRIETARIO_GLOBAL;

        internal static bool ApelidoUnicoCurto { get; set; } = true;

        private static EnumTipoBancoDados? _tipoBancoDadosEnum;

        internal static EnumTipoBancoDados TipoBancoDadosEnum
        {
            get
            {
                if (!_tipoBancoDadosEnum.HasValue)
                {
                    _tipoBancoDadosEnum = EnumTipoBancoDados.SQL_SERVER;
                    //throw new Erro("O banco dados não definido");
                }
                return _tipoBancoDadosEnum.Value;
            }
            set
            {
                if (_tipoBancoDadosEnum.HasValue && (_tipoBancoDadosEnum.Value != value))
                {
                    throw new Erro("O tipo de banco dados ja foi definido com um valor diferente");
                }
                _tipoBancoDadosEnum = value;
            }
        }

        public static IsolationLevel IsolamentoLevelSalvarPadrao
        {
            get
            {
                return IsolationLevel.ReadCommitted;
                //return IsolationLevel.Snapshot;
            }
        }

        public static IsolationLevel IsolamentoLevelConsultaPadrao
        {
            get
            {
                //return IsolationLevel.ReadCommitted;
                return IsolationLevel.Snapshot;
            }
        }
    }
}