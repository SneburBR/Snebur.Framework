﻿using Snebur.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.Utilidade
{
    public static partial class TextoUtil
    {
        private class TextoUtilConstantes
        {
            private const string NUMEROS = "0123456789";
            private const string LETRAS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefhgijklmnopqrstuvwxyz";
            private const string CARACTERES_PADRAO = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789ÀÁÂÃÈÉÊÌÍÎÒÓÔÕÚÛÜÇÑàáâãäèéêëìíîòóôõùúûüçñ-_.@,()+=;:~^`´&! ";

            //private const string ACENTOS = "ÀÁÂÃÈÉÊÌÍÎÒÓÔÕÚÛÜÇÑàáâãäèéêëìíîòóôõùúûüçñ";
            //private const string ACENTOS_MAPEADOS = "AAAAEEEIIIOOOOUUUCNaaaaaeeeeiiioooouuuucn";

            private const string ACENTOS = "ÀÁÂÃÈÉÊÌÍÎÒÓÔÕÚÛÜÇÑàáâãäèéêëìíîòóôõùúûüçñ´~^¨`";
            private const string ACENTOS_MAPEADOS = "AAAAEEEIIIOOOOUUUCNaaaaaeeeeiiioooouuuucn     ";
            private const string PONTOS_SINAIS = "+-.,";

            private static HashSet<char> _numeros;
            private static HashSet<char> _letras;
            private static HashSet<char> _letrasNumeros;
            private static HashSet<char> _caracteresPadrao;
            private static HashSet<char> _linhasTabulacoes;
            private static HashSet<char> _pontosSinais;
            private static HashSet<char> _numerosPontosSinais;
            private static Dictionary<char, char> _acentosMapeado;

            internal static HashSet<char> Numeros => LazyUtil.RetornarValorLazy(
                    ref _numeros,
                    () => TextoUtilConstantes.NUMEROS.ToArray().ToHashSet());
            internal static HashSet<char> PontosSinais => LazyUtil.RetornarValorLazy(
                    ref _pontosSinais,
                    () => TextoUtilConstantes.PONTOS_SINAIS.ToArray().ToHashSet());

            internal static HashSet<char> NumerosPontsSinais => LazyUtil.RetornarValorLazy(
                    ref _numerosPontosSinais,
                    () => (TextoUtilConstantes.NUMEROS + TextoUtilConstantes.PONTOS_SINAIS).ToArray().ToHashSet());

            internal static HashSet<char> Letras => LazyUtil.RetornarValorLazy(
                     ref _letras,
                     () => TextoUtilConstantes.LETRAS.ToArray().ToHashSet());

            internal static HashSet<char> LetrasNumeros => LazyUtil.RetornarValorLazy(
                    ref _letrasNumeros,
                    () => String.Concat(LETRAS, NUMEROS).ToArray().ToHashSet());

            internal static HashSet<char> CaracteresPadrao => LazyUtil.RetornarValorLazy(
                    ref _caracteresPadrao,
                    () => CARACTERES_PADRAO.ToArray().ToHashSet());

            internal static HashSet<char> LinhasTabulacoes => LazyUtil.RetornarValorLazy(
                    ref _linhasTabulacoes,
                    () =>
                    {
                        var retorno = new HashSet<char>();
                        foreach (var c in Environment.NewLine.ToArray())
                        {
                            retorno.Add(c);
                        }
                        char tab = '\u0009';
                        retorno.Add(tab);
                        return retorno;
                    });

            internal static Dictionary<char, char> AcentosMapeado => LazyUtil.RetornarValorLazy(
                    ref _acentosMapeado,
                    () =>
                    {
                        if (ACENTOS.Length != ACENTOS_MAPEADOS.Length)
                        {
                            throw new Erro("O números de ACENTOS é diferentes do ACENTOS_MAPEADOS mapeados");
                        }
                        var retorno = new Dictionary<char, char>();
                        var len = ACENTOS.Length;

                        for (var i = 0; i < len; i++)
                        {
                            var caracter = ACENTOS[i];
                            var caracterMepado = ACENTOS_MAPEADOS[i];
                            retorno.Add(caracter, caracterMepado);
                        }
                        return retorno;
                    });
        }
    }
}