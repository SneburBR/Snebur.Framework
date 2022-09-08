using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;

#if NET50
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace Snebur.AcessoDados.Manutencao
{
    internal partial class GerenciadorManutencao : IDisposable
    {

        private const string NOME_CAMPO_MANUTENCAO_SNEBUR = "ManutencaoSnebur";
        private const string NOME_PARAMETRO_MANUTENCAO_SNEBUR = "@MigrationId";

        private const string SQL_CRIAR_CAMPO_MANUTENCAO_SNEBUR = "IF NOT EXISTS  (select * from sys.columns where object_id = OBJECT_ID(N'[dbo].[__MigrationHistory]')  and [name] ='ManutencaoSnebur')  ALTER TABLE [dbo].[__MigrationHistory] ADD [ManutencaoSnebur] [bit] NOT NULL DEFAULT 0";
        private const string SQL_NOTIFICAR_MANUTENCAO_SNEBUR_FINALIZADA = " UPDATE [dbo].[__MigrationHistory] SET " + NOME_CAMPO_MANUTENCAO_SNEBUR + " = 1 WHERE " + NOME_CAMPO_MANUTENCAO_SNEBUR + " = 0  AND  MigrationId = " + NOME_PARAMETRO_MANUTENCAO_SNEBUR;
        private const string SQL_MANUTENCOES_SNEBUR_PENDENTE = "SELECT MigrationId from [dbo].[__MigrationHistory] WHERE  " + NOME_CAMPO_MANUTENCAO_SNEBUR + " = 0 order by MigrationId";

        private BaseContextoDados Contexto { get; }
        public BaseConexao Conexao { get; }
        private Dictionary<string, List<Type>> TiposManutencao { get; set; }
        private string NomeMigracaoAtual { get; set; }

        public GerenciadorManutencao(BaseContextoDados contexto)
        {
            this.Contexto = contexto;
            this.Conexao = this.Contexto.Conexao;
            this.TiposManutencao = this.RetornarTiposManutencao();
        }

        internal void Executar()
        {
            using (var conexao = this.Conexao.RetornarNovaConexao())
            {
                conexao.Open();
                try
                {
                    var historicos = this.RetornarHistoricosMigracaoPendente(conexao);
                    if (historicos.Count > 0)
                    {
                        this.ExecutarInterno(conexao, historicos);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        private void ExecutarInterno(DbConnection conexao, List<HistoricoMigracao> historicos)
        {
            foreach (var historico in historicos)
            {
                var manutencoes = this.RetornarManutencoes(historico);
                this.ValidarManutencoes(manutencoes);

                foreach (var manutencao in manutencoes)
                {
                    try
                    {
                        var historicoManutencao = this.RetornarHistoricoManutencao(historico, manutencao);
                        if (historicoManutencao == null || !historicoManutencao.IsSucesso)
                        {
                            this.Contexto.IniciarNovaTransacao();
                            manutencao.Executar();
                            this.SalvarHistoricoManutencao(historico, manutencao);
                            this.Contexto.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Contexto.Rollback();
                        this.SalvarHistoricoManutencao(historico, manutencao, ex);
                        throw;
                    }
                }
                this.NotificarManutecaoFinalizacao(conexao, historico);
            }
        }

        private void ValidarManutencoes(List<BaseManutencao> manutencoes)
        {
            if (this.IsExisteManutencoesComPrioridadeDuplicada(manutencoes))
            {
                var mensagem = this.RetornarMensagemErroPrioridadeDuplicadas(manutencoes);
                throw new Erro($"Existem menutenções com prioridade dupkicadas: \r\n {mensagem}");
            }
        }

        private bool IsExisteManutencoesComPrioridadeDuplicada(List<BaseManutencao> manutencoes)
        {
            if (manutencoes.Count > 1)
            {
                return manutencoes.GroupBy(x => x.Prioridade).Where(x => x.Count() > 1).Count() > 0;
            }
            return false;
        }

        private string RetornarMensagemErroPrioridadeDuplicadas(List<BaseManutencao> manutencoes)
        {
            var grupos = manutencoes.GroupBy(x => x.Prioridade).Where(x => x.Count() > 1).ToList();
            var sb = new StringBuilder();
            foreach (var grupo in grupos)
            {
                var nomesTipoManutecao = String.Join(", ", grupo.ToList().Select(x => x.GetType().Name));
                sb.Append($"As manuteções  '{nomesTipoManutecao}' possuem a memsagem prioridade '{grupo.Key}'");
            }
            sb.AppendLine("Sobre escreva a Prioridade Prioridade");
            return sb.ToString();
        }

        private void SalvarHistoricoManutencao(HistoricoMigracao historicoMigracao, BaseManutencao manutencao, Exception erro = null)
        {
            var historicoManutencao = this.RetornarHistoricoManutencao(historicoMigracao, manutencao);
            if (historicoManutencao == null)
            {
                historicoManutencao = (IHistoricoManutencao)Activator.CreateInstance(this.Contexto.TipoEntidadeHistoricoMenutencao);
            }
            historicoManutencao.MigrationId = historicoMigracao.MigrationId;
            historicoManutencao.IsSucesso = erro == null;
            historicoManutencao.NumeroTentativa += 1;
            historicoManutencao.MensagemErro = erro?.Message;
            historicoManutencao.NomeTipoManutencao = manutencao.GetType().Name;
            historicoManutencao.DataHoraUltimaExecucao = DateTime.UtcNow;
            historicoManutencao.Prioridade = manutencao.Prioridade;

            this.Contexto.Salvar(historicoManutencao);
        }

        private IHistoricoManutencao RetornarHistoricoManutencao(HistoricoMigracao historico, BaseManutencao manutencao)
        {
            return this.Contexto.RetornarConsulta<IHistoricoManutencao>(this.Contexto.TipoEntidadeHistoricoMenutencao).
                                Where(x => x.MigrationId == historico.MigrationId &&
                                           x.Prioridade == manutencao.Prioridade).SingleOrDefault();
        }



        private List<BaseManutencao> RetornarManutencoes(HistoricoMigracao historico)
        {
            var manutencoes = new List<BaseManutencao>();
            if (this.TiposManutencao.ContainsKey(historico.MigrationId))
            {
                var tipos = this.TiposManutencao[historico.MigrationId];
                foreach (var tipo in tipos)
                {
                    var manutencao = (BaseManutencao)Activator.CreateInstance(tipo, new object[] { this.Contexto });
                    manutencao.HistoricoMigracao = historico;
                    manutencoes.Add(manutencao);
                }
                return manutencoes.OrderBy(x => x.Prioridade).ToList();
            }
            return new List<BaseManutencao>();
        }

        #region Métodos privados

        private List<HistoricoMigracao> RetornarHistoricosMigracaoPendente(DbConnection conexao)
        {
            using (var cmdCampo = this.Conexao.RetornarNovoComando(SQL_CRIAR_CAMPO_MANUTENCAO_SNEBUR, null, conexao))
            {
                cmdCampo.ExecuteNonQuery();

                var dataTable = this.Conexao.RetornarDataTable(SQL_MANUTENCOES_SNEBUR_PENDENTE, new List<DbParameter>());
                var historicos = new List<HistoricoMigracao>();
                foreach (DataRow row in dataTable.Rows)
                {
                    historicos.Add(new HistoricoMigracao
                    {
                        MigrationId = row[nameof(HistoricoMigracao.MigrationId)].ToString()
                    });
                }
                return historicos.OrderBy(x => x.MigrationId).ToList();
            }
        }

        private void NotificarManutecaoFinalizacao(DbConnection conexao, HistoricoMigracao historico)
        {
            var parametros = new List<DbParameter>
            {
                new SqlParameter(NOME_PARAMETRO_MANUTENCAO_SNEBUR, SqlDbType.NVarChar, 255)
                {
                    Value = historico.MigrationId
                }
            };


            using (var cmdCampo = this.Conexao.RetornarNovoComando(SQL_NOTIFICAR_MANUTENCAO_SNEBUR_FINALIZADA, parametros, conexao))
            {
                cmdCampo.ExecuteNonQuery();
            }
        }

        private Dictionary<string, List<Type>> RetornarTiposManutencao()
        {
            var dicionario = new Dictionary<string, List<Type>>();
            var tipos = this.Contexto.GetType().Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(BaseManutencao)) && !x.IsAbstract).ToList();
            foreach (var tipo in tipos)
            {
                var atributoMigrationId = tipo.GetCustomAttribute<MigracaoIdAttribute>();
                if (atributoMigrationId == null)
                {
                    throw new Erro($"O atributo {nameof(MigracaoIdAttribute)} não foi encontrado no tipo {tipo.Name}");
                }

                var migrationId = atributoMigrationId.MigrationId;
                if (!dicionario.ContainsKey(migrationId))
                {
                    dicionario.Add(migrationId, new List<Type>());
                }
                dicionario[migrationId].Add(tipo);
            }
            return dicionario;
        }

        #endregion

        public void Dispose()
        {
            this.TiposManutencao.Clear();
            this.TiposManutencao = null;
        }
    }

    public class HistoricoMigracao
    {
        public string MigrationId { get; set; }

        public override string ToString()
        {
            return this.MigrationId;
        }
    }
}
