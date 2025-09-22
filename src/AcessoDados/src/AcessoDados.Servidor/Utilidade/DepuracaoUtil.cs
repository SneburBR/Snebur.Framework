using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using Snebur.Linq;
using Microsoft.Data.SqlClient;


namespace Snebur.AcessoDados;

internal class DepuracaoUtil
{
    internal const string CATEGORIA = "Commando sql";

    internal static void EscreverSaida(BaseContextoDados contexto,
                                       DbParameterCollection collection,
                                      string sql)
    {
        if (DebugUtil.IsAttached && ConfiguracaoUtil.IsLogDebugOutputSql)
        {
            var parametros = new List<ParametroInfo>();

            foreach (SqlParameter parametro in collection)
            {
                parametros.Add(new ParametroInfo
                {
                    SqlDbType = parametro.SqlDbType,
                    ParameterName = parametro.ParameterName,
                    Value = parametro.Value,
                    Size = parametro.Size
                });
            }
            EscreverSaida(contexto, parametros, sql);
        }
    }

    internal static void EscreverSaida(
        BaseContextoDados contexto,
        List<ParametroInfo>? parametros,
        string sql)
    {
        if (DebugUtil.IsAttached && ConfiguracaoUtil.IsLogDebugOutputSql)
        {
            Task.Run(() =>
            {
                EscreverSaidaInterno(contexto,
                                     parametros,
                                     sql);
            });
        }
    }

    private static void EscreverSaidaInterno(
        BaseContextoDados contexto,
        List<ParametroInfo>? parametros,
        string sql)
    {

#if DEBUG
        if (DebugUtil.IsAttached && ConfiguracaoUtil.IsLogDebugOutputSql)
        {
            lock (contexto.Comandos.SyncLock())
            {
                var comandos = contexto.Comandos;
                if (parametros != null)
                {
                    comandos.Add("SET DATEFORMAT 'dmy' ");
                    foreach (var p in parametros)
                    {
                        var declaracao = $"DECLARE  {p.ParameterName} AS {p.SqlDbType.ToString().ToUpper()}";
                        {
                            if (p.Size > 0)
                            {
                                declaracao += $"({p.Size.ToString()})";
                            }
                        }
                        comandos.Add(declaracao);
                    }

                    foreach (var p in parametros)
                    {
                        comandos.Add($"SET  {p.ParameterName} = '{p.Value}'");
                    }
                }
                comandos.Add(sql.RemoverQuebraLinhas(true));

                Trace.WriteLine("--  INICIO COMANDOS: " + DateTime.Now);
                foreach (var comando in comandos.ToList())
                {
                    var comandoFormatado = comando.RemoverQuebraLinhas(true);
                    Trace.WriteLine(comandoFormatado);
                }
                Trace.WriteLine("-- FIM");

            }
        }
#endif

    }
}
