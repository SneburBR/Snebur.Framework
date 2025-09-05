using Snebur.Utilidade;
using System.IO;

namespace Snebur.Seguranca;

public partial class Token
{
    public const int TEMPO_EXPIRAR_TOKEN_PADRAO = 10 * 60;

    public static ResultadoToken ValidarToken(string token, TimeSpan expirar)
    {
        var valores = RetornarValores(token);
        if (valores == null)
        {
            return ResultadoToken.Invalido;
        }
        var dataHora = (DateTime)(valores[0]);
        var chave = (Guid)(valores[1]);

        return new ResultadoToken(chave, dataHora, expirar);
    }

    public static ResultadoToken<T> ValidarToken<T>(string token, TimeSpan expirar) where T : struct
    {

        var valores = RetornarValores(token, typeof(T));
        if (valores == null)
        {
            return ResultadoToken<T>.Invalido;
        }

        var dataHora = Convert.ToDateTime(valores[0]);
        var chave = (Guid)(valores[1]);
        var item1 = valores[2];

        return new ResultadoToken<T>(chave, dataHora, expirar, (T)item1);
    }

    public static ResultadoToken<T1, T2> ValidarToken<T1, T2>(string token, TimeSpan expirar) where T1 : struct
                                                                                              where T2 : struct
    {
        var valores = RetornarValores(token, typeof(T1), typeof(T2));
        if (valores == null)
        {
            return ResultadoToken<T1, T2>.Invalido;
        }
        var dataHora = Convert.ToDateTime(valores[0]);
        var chave = (Guid)(valores[1]);
        var item1 = valores[2];
        var item2 = valores[3];
        return new ResultadoToken<T1, T2>(chave, dataHora, expirar, (T1)item1, (T2)item2);
    }

    public static ResultadoToken<T1, T2, T3> ValidarToken<T1, T2, T3>(string token, TimeSpan expirar) where T1 : struct
                                                                                                      where T2 : struct
                                                                                                      where T3 : struct
    {
        var valores = RetornarValores(token, typeof(T1), typeof(T2), typeof(T3));
        if (valores == null)
        {
            return ResultadoToken<T1, T2, T3>.Invalido;
        }
        var dataHora = Convert.ToDateTime(valores[0]);
        var chave = (Guid)(valores[1]);
        var item1 = (T1)valores[2];
        var item2 = (T2)valores[3];
        var item3 = (T3)valores[4];

        return new ResultadoToken<T1, T2, T3>(chave, dataHora, expirar, item1, item2, item3);
    }

    public static ResultadoToken<T1, T2, T3, T4> ValidarToken<T1, T2, T3, T4>(string token, TimeSpan expirar) where T1 : struct
                                                                                                              where T2 : struct
                                                                                                              where T3 : struct
                                                                                                              where T4 : struct
    {
        var valores = RetornarValores(token, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        if (valores == null)
        {
            return ResultadoToken<T1, T2, T3, T4>.Invalido;
        }
        var dataHora = Convert.ToDateTime(valores[0]);
        var chave = (Guid)(valores[1]);
        var item1 = (T1)valores[2];
        var item2 = (T2)valores[3];
        var item3 = (T3)valores[4];
        var item4 = (T4)valores[5];

        return new ResultadoToken<T1, T2, T3, T4>(chave, dataHora, expirar, item1, item2, item3, item4);

    }

    public static ResultadoToken<T1, T2, T3, T4, T5> ValidarToken<T1, T2, T3, T4, T5>(string token, TimeSpan expirar) where T1 : struct
                                                                                                                      where T2 : struct
                                                                                                                      where T3 : struct
                                                                                                                      where T4 : struct
                                                                                                                      where T5 : struct
    {
        var valores = RetornarValores(token, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        if (valores == null)
        {
            return ResultadoToken<T1, T2, T3, T4, T5>.Invalido;
        }
        var dataHora = Convert.ToDateTime(valores[0]);
        var chave = (Guid)(valores[1]);
        var item1 = (T1)valores[2];
        var item2 = (T2)valores[3];
        var item3 = (T3)valores[4];
        var item4 = (T4)valores[5];
        var item5 = (T5)valores[6];

        return new ResultadoToken<T1, T2, T3, T4, T5>(chave, dataHora, expirar, item1, item2, item3, item4, item5);
    }

    private static List<object>? RetornarValores(string token, params Type[] tipos)
    {

        if (String.IsNullOrEmpty(token))
        {
            return null;
        }

        if (!Base64Util.IsBase64String(token))
        {
            return null;
        }

        var totalBytes = 16 + 8 + 16 + 16; // porcaria + data + chave + porcara
        totalBytes += tipos.Sum(x => RetornarTamanhoTipo(x)); //bytes do valores
        if (tipos.Length > 0)
        {
            totalBytes += (tipos.Length) * 16; // bytes das porcaria
        }

        var bytes = Convert.FromBase64String(token);
        if (bytes.Length != totalBytes)
        {
            return null;
        }

        try
        {
            var valores = new List<object>();
            using (var ms = new MemoryStream(bytes))
            {
                ms.Seek(16, SeekOrigin.Begin);

                var bytesData = new byte[sizeof(long)];
                ms.Read(bytesData, 0, sizeof(long));
                var dataHora = RetornarValor(typeof(DateTime), bytesData);
                valores.Add(dataHora);

                var bytesChave = new byte[16];
                ms.Read(bytesChave, 0, 16);

                valores.Add(new Guid(bytesChave));

                foreach (var tipo in tipos)
                {
                    var bytesPorcaria = new byte[16];
                    ms.Read(bytesPorcaria, 0, 16);

                    var tamanho = RetornarTamanhoTipo(tipo);
                    var bytesValor = new byte[tamanho];
                    ms.Read(bytesValor, 0, tamanho);

                    var valor = RetornarValor(tipo, bytesValor);
                    valores.Add(valor);
                }
            }
            return valores;

        }
        catch (Exception)
        {
            return null;
        }
    }

    private static int RetornarTamanhoTipo(Type tipo)
    {
        switch (tipo.Name)
        {
            case nameof(Boolean):

                return sizeof(bool);

            case nameof(Int32):

                return sizeof(int);

            case nameof(Int64):

                return sizeof(long);

            case nameof(Double):

                return sizeof(double);

            case nameof(DateTime):

                return sizeof(long);

            case nameof(Guid):

                return 16;

            default:

                throw new ErroNaoSuportado(String.Format("O tipo não é suportado {0}", tipo.Name));

        }
    }

    private static object RetornarValor(Type tipo, byte[] bytes)
    {
        switch (tipo.Name)
        {
            case nameof(Boolean):

                return BitConverter.ToBoolean(bytes, 0);

            case nameof(Int32):

                return BitConverter.ToInt32(bytes, 0);

            case nameof(Int64):

                return BitConverter.ToInt64(bytes, 0);

            case nameof(Double):

                return BitConverter.ToDouble(bytes, 0);

            case nameof(DateTime):

                var ticks = BitConverter.ToInt64(bytes, 0);
                return new DateTime(ticks, DateTimeKind.Utc);

            case nameof(Guid):

                return new Guid(bytes);

            default:

                throw new ErroNaoSuportado(String.Format("O tipo não é suportado {0}", tipo.Name));

        }
    }
}
