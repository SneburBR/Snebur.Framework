using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

using System.Linq;
using System.Reflection;

#if NetCore 
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif




namespace Snebur.Utilidade
{
    public class Conexao
    {
        public string ConnectionString { get; set; }
        //{
        //    get
        //    {
        //        return $"Data Source=CHA-SQLSERVER; Initial Catalog=Snebur;User Id =zyonadminuser; Password=tepeguei$%3812#;Connection Timeout=180";
        //    }
        //}

        public Conexao(string connectionString)
        {
            if (AplicacaoSnebur.Atual.ConnectionStrings[connectionString] != null)
            {
                this.ConnectionString = AplicacaoSnebur.Atual.ConnectionStrings[connectionString];
            }
            else
            {
                this.ConnectionString = connectionString;
            }
        }
        public DataTable RetornarDataTable(string sql, List<PropertyInfo> propriedadesChavePrimaria)
        {
            var dt = new DataTable();
            using (var conexao = new SqlConnection(this.ConnectionString))
            {
                conexao.Open();
                try
                {
                    using (var cmd = new SqlCommand(sql, conexao))
                    {
                        using (var ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(dt);
                        }
                    }
                    if (propriedadesChavePrimaria != null && propriedadesChavePrimaria.Count > 0)
                    {
                        var chavesPrimerias = propriedadesChavePrimaria.Select(x => dt.Columns[x.Name]).ToArray();
                        dt.PrimaryKey = chavesPrimerias;
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
            return dt;
        }

        public List<T> Mapear<T>(string sql)
        {
            var tipo = typeof(T);
            var propriedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetMethod.IsPublic && (x.SetMethod?.IsPublic ?? false));
            var propriedadesChavePrimaria = tipo.GetProperties().Where(x => x.GetCustomAttribute<KeyAttribute>() != null).ToList();
            var dataTable = this.RetornarDataTable(sql, propriedadesChavePrimaria);
            var retorno = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                var item = Activator.CreateInstance<T>();
                foreach (var propriedade in propriedades)
                {
                    var valor = row[propriedade.Name];
                    var valorTipado = ConverterUtil.Converter(valor, propriedade.PropertyType);
                    propriedade.SetValue(item, valorTipado);
                }
                retorno.Add(item);
            }
            return retorno;
        }

        public object RetornarValorScalar(string sql)
        {
            object valorEscalor;
            using (var conexao = new SqlConnection(this.ConnectionString))
            {
                conexao.Open();
                try
                {
                    using (var cmd = new SqlCommand(sql, conexao))
                    {
                        valorEscalor = cmd.ExecuteScalar();
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
            if (valorEscalor == DBNull.Value)
            {
                valorEscalor = null;
            }
            return valorEscalor;
        }

        public void ExecutarComando(string sql)
        {
            using (var conexao = new SqlConnection(this.ConnectionString))
            {
                conexao.Open();
                try
                {
                    using (var cmd = new SqlCommand(sql, conexao))
                    {
                        cmd.ExecuteNonQuery();
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

        public void ExecutarComandos(IEnumerable<string> sqls)
        {
            ExecutarComandos(sqls, null);
        }
        /// <summary>
        /// Executa o varios scripts 'sql' transacionado
        /// </summary>
        /// <param name="sqls"></param>
        /// <param name="acao"> A acao será invokada em cadas script </param>
        public void ExecutarComandos(IEnumerable<string> sqls, Action<SqlCommand> acao)
        {
            using (var conexao = new SqlConnection(this.ConnectionString))
            {
                conexao.Open();

                using (var trans = conexao.BeginTransaction())
                {
                    try
                    {
                        foreach (var sql in sqls)
                        {
                            using (var cmd = new SqlCommand(sql, conexao, trans))
                            {
                                acao?.Invoke(cmd);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        private SqlDbType RetornarSqlDbType(Type tipo)
        {
            if (Snebur.Utilidade.ReflexaoUtil.IsTipoNullable(tipo))
            {
                tipo = Snebur.Utilidade.ReflexaoUtil.RetornarTipoSemNullable(tipo);
            }
            switch (tipo.Name)
            {
                case nameof(Char):
                case nameof(String):

                    return SqlDbType.NVarChar;

                case nameof(Int32):

                    return SqlDbType.Int;

                default:

                    throw new Exception($"o tipo {tipo.Name} não é suportado");
            }
        }
    }
}