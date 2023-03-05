using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Snebur.Seguranca
{
    public partial class Token
    {
        private const string TOKEN_PADRAO = "fd35dd69-f0fc-43fd-b035-7e659b986b70";

        private static Guid? _chave;
        internal static Guid CHAVE => LazyUtil.RetornarValorLazyComBloqueio(ref _chave, Token.RetornarChaveToken);

        private static Guid RetornarChaveToken()
        {
            var tokenString = ConfiguracaoUtil.AppSettings["CHAVE_TOKEN"];
            if (tokenString != null && Guid.TryParse(tokenString, out var token) && token != Guid.Empty)
            {
                return token;
            }
            return new Guid(Token.TOKEN_PADRAO);
        }

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
                if (data.Kind != DateTimeKind.Utc)
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
