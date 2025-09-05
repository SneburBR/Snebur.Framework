using Snebur.AcessoDados.Estrutura;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snebur.AcessoDados.Manutencao
{
    public abstract class BaseManutencao
    {
        protected BaseContextoDados Contexto { get; }
        private BaseConexao Conexao { get; }

        internal protected virtual int Prioridade { get; } = 0;
        internal protected HistoricoMigracao HistoricoMigracao { get; internal set; }

        public BaseManutencao(BaseContextoDados contexto)
        {
            this.Contexto = contexto;
            this.Conexao = this.Contexto.Conexao;
        }

        internal protected abstract void Executar();

        protected void ExecutarSql(string sql)
        {
            this.Conexao.ExecutarComando(sql, new List<ParametroInfo>());
        }

        protected void ExecutarSql(string sql, List<ParametroInfo> parametros)
        {
            this.Conexao.ExecutarComando(sql, parametros);
        }
        protected object RetornarValorScalar(string sql)
        {
            return this.Conexao.RetornarValorScalar(sql, new List<ParametroInfo>());
        }

        protected object RetornarValorScalar(string sql, List<ParametroInfo> parametros)
        {
            return this.Conexao.RetornarValorScalar(sql, parametros);
        }

        protected void ExclirIndice<TEntidade>(string nomeIndice) where TEntidade : Entidade
        {
            var tipoEntidade = typeof(TEntidade);
            var nomeTabela = AjudanteEstruturaBancoDados.RetornarNomeTabela(tipoEntidade);
            var nomeSchema = AjudanteEstruturaBancoDados.RetornarNomeSchemaTabela(tipoEntidade);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($" IF EXISTS (select * from sys.indexes where object_id = OBJECT_ID(N'[{nomeSchema}].[{nomeTabela}]') AND name = N'{nomeIndice}') ");
            sb.AppendLine($"DROP INDEX [{nomeIndice}] ON [{nomeSchema}].[{nomeTabela}]");

            var sql = sb.ToString();
            this.ExecutarSql(sql);
        }

        protected string RetornarNomeTabela<T>(bool inclurColchetes = true) where T : Entidade
        {
            var tipo = typeof(T);
            var atributoTabela = tipo.GetCustomAttribute<TableAttribute>(true);
            if (atributoTabela == null)
            {
                throw new Erro($"O atributo {nameof(TableAttribute)} não foi encontrado no tipo '{tipo.Name}'");
            }
            if (String.IsNullOrWhiteSpace(atributoTabela.Schema))
            {
                if (inclurColchetes)
                {
                    return $"[{atributoTabela.Name}]";
                }
                return atributoTabela.Name;
            }

            if (inclurColchetes)
            {
                return $"[{atributoTabela.Schema}].[{atributoTabela.Name}]";
            }
            return $"{atributoTabela.Schema}.{atributoTabela.Name}";
        }
    }

    public abstract class BaseManutencao<TContexto> : BaseManutencao where TContexto : BaseContextoDados
    {
        protected new TContexto Contexto { get; }

        public BaseManutencao(TContexto contexto) : base(contexto)
        {
            this.Contexto = contexto;
        }
    }
}
