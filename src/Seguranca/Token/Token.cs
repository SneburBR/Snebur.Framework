using System;
using System.Collections.Generic;
using System.IO;

namespace Snebur.Seguranca
{
    public partial class Token
    {
        internal static Guid CHAVE { get; set; } = new Guid("3dda61dd-a784-4fff-a2fe-14e5e337e363");

        private static List<Type> TiposPermitido { get; set; } = new List<Type>() { typeof(int), typeof(long), typeof(DateTime), typeof(Guid) };

        public static string RetornarNovoToken()
        {
            return Token.RetornarToken();
        }

        public static string RetornarNovoToken<T>(T p1) where T : struct
        {
            return Token.RetornarToken(p1);
        }

        public static string RetornarNovoToken<T1, T2>(T1 p1, T2 p2) where T1 : struct where T2 : struct
        {
            return Token.RetornarToken(p1, p2);
        }

        public static string RetornarNovoToken<T1, T2, T3>(T1 p1, T2 p2, T3 p3) where T1 : struct
                                                                                where T2 : struct
                                                                                where T3 : struct
        {
            return Token.RetornarToken(p1, p2, p3);
        }

        public static string RetornarNovoToken<T1, T2, T3, T4>(T1 p1, T2 p2, T3 p3, T4 p4) where T1 : struct
                                                                                           where T2 : struct
                                                                                           where T3 : struct
                                                                                           where T4 : struct
        {
            return Token.RetornarToken(p1, p2, p3, p4);
        }

        public static string RetornarNovoToken<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) where T1 : struct
                                                                                                      where T2 : struct
                                                                                                      where T3 : struct
                                                                                                      where T4 : struct
                                                                                                      where T5 : struct
        {
            return Token.RetornarToken(p1, p2, p3, p4, p5);
        }


        public static string RetornarToken(params object[] valores)
        {
            var agoraUtc = DateTime.UtcNow.Add(AplicacaoSnebur.Atual.DiferencaDataHoraUtcServidor);
            var novaLista = new List<object>();
            novaLista.Add(Guid.NewGuid());
            novaLista.Add(agoraUtc);
            novaLista.Add(CHAVE);
            novaLista.Add(Guid.NewGuid());

            if (valores != null && valores?.Length > 0)
            {
                foreach (var valor in valores)
                {
                    novaLista.Add(valor);
                    novaLista.Add(Guid.NewGuid());
                }
            }

            using (var ms = new MemoryStream())
            {
                foreach (var item in novaLista)
                {
                    var bytes = RetornarBytes(item);
                    ms.Write(bytes, 0, bytes.Length);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static byte[] RetornarBytes(object valor)
        {
            if (valor is bool logico)
            {
                return BitConverter.GetBytes(logico);
            }

            if (valor is Int32 numeroInteiro)
            {
                return BitConverter.GetBytes(numeroInteiro);
            }

            if (valor is Int64 numeroLongo)
            {
                return BitConverter.GetBytes(numeroLongo);
            }

            if (valor is Double numeroDuplo)
            {
                return BitConverter.GetBytes(numeroDuplo);
            }

            if (valor is DateTime data)
            {
                if(data.Kind!= DateTimeKind.Utc)
                {
                    data = data.ToUniversalTime();
                }
                return BitConverter.GetBytes(data.Ticks);
                //return BitConverter.GetBytes(Convert.ToDateTime(valor).ToBinary());
            }

            if (valor is Guid guid)
            {
                return guid.ToByteArray();
            }

            throw new ErroNaoSuportado(String.Format("O tipo não é suportado {0}", valor.GetType().Name));
        }
    }
}
