using System.Data.Common;

namespace Snebur.AcessoDados;

internal partial class GerenciadorMigracao : IDisposable
{
    private BaseContextoDados Contexto { get; set; }
    private BaseConexao Conexao { get; set; }
    //private List<SqlMigracao> SqlsMigration { get; set; }

    private const string NOME_CAMPO_MIGRACAO_SNEBUR = "MigracaoSnebur";
    private const string SQL_CRIAR_CAMPO_MIGRACAO_SNEBUR = "IF NOT EXISTS (select * from sys.columns where object_id = OBJECT_ID(N'[dbo].[__MigrationHistory]')  and [name] ='" + NOME_CAMPO_MIGRACAO_SNEBUR + "') ALTER TABLE [dbo].[__MigrationHistory] ADD [" + NOME_CAMPO_MIGRACAO_SNEBUR + "] [bit] NOT NULL DEFAULT 0 ";
    private const string SQL_EXISTE_MIGRACAO_SNEBUR_PENDENTE = "select top 1 " + NOME_CAMPO_MIGRACAO_SNEBUR + " from [dbo].[__MigrationHistory] order by MigrationId desc";
    private const string SQL_MIGRACAO_SNEBUR_FINALIZADA = " UPDATE [dbo].[__MigrationHistory] SET " + NOME_CAMPO_MIGRACAO_SNEBUR + " = 1 WHERE MigracaoSnebur = 0 ";

    private GerenciadorMigracao(BaseContextoDados contexto)
    {
        this.Contexto = contexto;
        this.Conexao = this.Contexto.Conexao;
        //this.SqlsMigration = this.RetornarSqlsMigration();
    }

    internal void Migrar()
    {
        //this.SqlsMigration = this.RetornarSqlsMigration();
        using (var conexao = this.Conexao.RetornarNovaConexao())
        {
            conexao.Open();
            try
            {
                if (this.ExisteMigracaoSneburPendente(conexao))
                {
                    var sqlsMigracao = this.RetornarSqlsMigration();
                    foreach (var sqlMigration in sqlsMigracao)
                    {
                        if (!this.ExisteMigracao(conexao, sqlMigration))
                        {
                            var sql = sqlMigration.RetornarSql();
                            using (var cmd = this.Conexao.RetornarNovoComando(sql, null, conexao))
                            {
                                try
                                {
                                    cmd.CommandTimeout = 50000;
                                    cmd.ExecuteNonQuery();
                                    DepuracaoUtil.EscreverSaida(this.Contexto, new List<ParametroInfo>(), sql);
                                }
                                catch (Exception erro)
                                {
                                    var sql2 = sqlMigration.RetornarSql();
                                    throw new ErroMigracao(String.Format("Erro migration : {0}", sql), erro);
                                }
                            }
                        }
                    }
                    this.NotificarMigracaoSneburFinalizadao(conexao);
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

    private void NotificarMigracaoSneburFinalizadao(DbConnection conexao)
    {
        using (var cmd = this.Conexao.RetornarNovoComando(SQL_MIGRACAO_SNEBUR_FINALIZADA, null, conexao))
        {
            cmd.ExecuteNonQuery();
        }
    }

    private bool ExisteMigracaoSneburPendente(DbConnection conexao)
    {
        using (var cmdCampo = this.Conexao.RetornarNovoComando(SQL_CRIAR_CAMPO_MIGRACAO_SNEBUR, null, conexao))
        {
            cmdCampo.ExecuteNonQuery();

            using (var cmdExistePendencia = this.Conexao.RetornarNovoComando(SQL_EXISTE_MIGRACAO_SNEBUR_PENDENTE, null, conexao))
            {
                return !ConverterUtil.ParaBoolean(cmdExistePendencia.ExecuteScalar());
            }
        }
    }

    private bool ExisteMigracao(DbConnection conexao, SqlMigracao sqlMigration)
    {
        if ((sqlMigration is SqlValorPadraoDataHoraServidor sqlValorPadraoDataHoraServidor) &&
            ((ConfiguracaoAcessoDados.TipoBancoDadosEnum == EnumTipoBancoDados.PostgreSQL)))
        {
            var sqlExiste = sqlValorPadraoDataHoraServidor.RetornrSqlExisteGatilho();
            using (var cmd = this.Conexao.RetornarNovoComando(sqlExiste, null, conexao))
            {
                return ConverterUtil.Converter<bool>(cmd.ExecuteScalar());
            }
        }
        return false;
    }

    private List<SqlMigracao> RetornarSqlsMigration()
    {
        var sqlsMigration = new List<SqlMigracao>();
        var tiposEntidade = this.Contexto.EstruturaBancoDados.TiposEntidade.Values;

        foreach (var tipoEntidade in tiposEntidade)
        {
            var estruturaEntidade = this.Contexto.EstruturaBancoDados.EstruturasEntidade[tipoEntidade.Name];
            if (this.PossuiValidacaoUnicoComposta(tipoEntidade))
            {
                var atributos = tipoEntidade.GetCustomAttributes<ValidacaoUnicoCompostaAttribute>(false);
                foreach (var atributo in atributos.Where(x => x.IsIgnorarMigracao == false))
                {
                    sqlsMigration.Add(new SqlValidacaoUnico(estruturaEntidade,
                                                            atributo.Propriedades,
                                                            atributo.Filtros));
                }
            }

            var propriedadesValidacaoUnico = this.RetornarPropriedadesValidacaoUnico(tipoEntidade);
            foreach (var (propriedades, filtros) in propriedadesValidacaoUnico)
            {
                sqlsMigration.Add(new SqlValidacaoUnico(estruturaEntidade, propriedades, filtros));
                var sql = sqlsMigration.Last().RetornarSql();

            }

            var propriedadesValorPadraoDataHoraServidor = this.RetornarPropriedadesDataHoraServidor(tipoEntidade);
            foreach (var propriedade in propriedadesValorPadraoDataHoraServidor)
            {
                var atributo = propriedade.GetCustomAttribute<ValorPadraoDataHoraServidorAttribute>();
                Guard.NotNull(atributo);
                sqlsMigration.Add(new SqlValorPadraoDataHoraServidor(estruturaEntidade, propriedade, atributo.IsDataHoraUTC));
            }

            var propriedadesIndexar = this.RetornarPropriedadesIndexar(tipoEntidade);
            foreach (var propriedade in propriedadesIndexar)
            {
                var atributo = propriedade.GetCustomAttribute<IndexarAttribute>();
                Guard.NotNull(atributo);
                if (!atributo.IsIgnorarMigracao)
                {
                    sqlsMigration.Add(new SqlIndexar(estruturaEntidade, new PropriedadeIndexar(propriedade, atributo.NomeIndice)));
                }
            }

            var propriedadesIndexarTextoCompleto = this.RetornarPropriedadesIndexarTextoCompleto(tipoEntidade);
            if (propriedadesIndexarTextoCompleto.Count > 0)
            {
                var atributos = propriedadesIndexarTextoCompleto.Select(x => x.GetCustomAttribute<IndexarTextoCompletoAttribute>());
                if (!atributos.Any(x => x?.IsIgnorarMigracao == true))
                {
                    sqlsMigration.Add(new SqlIndexarTextoCompleto(estruturaEntidade, propriedadesIndexarTextoCompleto));
                }
            }

            if (ReflexaoUtil.IsTipoImplementaInterface(tipoEntidade, typeof(IOrdenacao), true))
            {
                var (propriedade, atributo) = OrdenacaoUtil.RetornarPropriedadeOrdenacao(tipoEntidade);
                var isIgnorarMigracao = atributo?.IsIgnorarMigracao ?? false;

                if (!isIgnorarMigracao)
                {
                    var ordenacaoNovoRegistro = (atributo != null) ? atributo.OrdenacaoNovoRegistro : EnumOrdenacaoNovoRegistro.Fim;
                    sqlsMigration.Add(new SqlOrdenacao(estruturaEntidade, propriedade, ordenacaoNovoRegistro));
                }
            }
        }
        return sqlsMigration;
    }

    private bool PossuiValidacaoUnicoComposta(Type tipoEntidade)
    {
        return ReflexaoUtil.IsTipoPossuiAtributo(tipoEntidade, typeof(ValidacaoUnicoCompostaAttribute), false);
    }

    private List<(List<PropriedadeIndexar>, List<FiltroPropriedadeIndexar>)> RetornarPropriedadesValidacaoUnico(Type tipoEntidade)
    {
        //############################################### analisar melhor a regra quando o o valor nulo podem ser duplicados
        var grupoPropriedadesIndexar = new List<(List<PropriedadeIndexar>, List<FiltroPropriedadeIndexar>)>();
        var propriedades = ReflexaoUtil.RetornarPropriedades(tipoEntidade, true);
        foreach (var propriedade in propriedades)
        {
            var atributoValidacaoUnico = propriedade.GetCustomAttribute<ValidacaoUnicoAttribute>();
            if (atributoValidacaoUnico != null && atributoValidacaoUnico.IsIgnorarMigracao == false)
            {
                var propriedadesIndexar = new List<PropriedadeIndexar>();
                var propriedadesFiltros = new List<FiltroPropriedadeIndexar>();
                propriedadesFiltros.AddRange(atributoValidacaoUnico.Filtros);

                propriedadesIndexar.Add(new PropriedadeIndexar(propriedade,
                                                               atributoValidacaoUnico.IsIgnorarNulo,
                                                               atributoValidacaoUnico.IsIgnorarZero));

                if (ReflexaoUtil.IsTipoImplementaInterface(tipoEntidade, typeof(IDeletado)))
                {
                    var propriedadeIsDeletado = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nameof(IDeletado.IsDeletado), true);
                    var propriedadeDataHoraDeletado = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nameof(IDeletado.DataHoraDeletado), true);
                    if (propriedadeIsDeletado != null)
                    {
                        propriedadesFiltros.Add(new FiltroPropriedadeIndexar(propriedadeIsDeletado,
                                                                      EnumOperadorComparacao.Igual,
                                                                      "0"));
                    }
                    if (propriedadeDataHoraDeletado != null)
                    {
                        propriedadesFiltros.Add(new FiltroPropriedadeIndexar(propriedadeDataHoraDeletado,
                                                EnumOperadorComparacao.Igual,
                                                null));
                    }
                }
                grupoPropriedadesIndexar.Add((propriedadesIndexar, propriedadesFiltros));
            }
        }
        return grupoPropriedadesIndexar;
    }

    private List<PropertyInfo> RetornarPropriedadesDataHoraServidor(Type tipoEntidade)
    {
        return this.RetornarPropriedadesPossuiAtributo<ValorPadraoDataHoraServidorAttribute>(tipoEntidade);
    }

    private List<PropertyInfo> RetornarPropriedadesIndexar(Type tipoEntidade)
    {
        return this.RetornarPropriedadesPossuiAtributo<IndexarAttribute>(tipoEntidade);
    }

    private List<PropertyInfo> RetornarPropriedadesIndexarTextoCompleto(Type tipoEntidade)
    {
        return this.RetornarPropriedadesPossuiAtributo<IndexarTextoCompletoAttribute>(tipoEntidade);
    }

    private List<PropertyInfo> RetornarPropriedadesPossuiAtributo<TAtributo>(Type tipoEntidade) where TAtributo : Attribute
    {
        var propriedades = ReflexaoUtil.RetornarPropriedades(tipoEntidade, true);
        return propriedades.Where(x => ReflexaoUtil.IsPropriedadePossuiAtributo(x, typeof(TAtributo))).ToList();
    }

    #region IDisposable

    public void Dispose()
    {
    }

    #endregion
}