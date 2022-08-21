﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Snebur.Utilidade
{
    public static partial class ReflexaoUtil
    {
        private const string NOME_EMPRESA_PADRAO = "Snebur Sistemas";
        private const string NOME_SNEBUR = "Snebur";
        private static Assembly _assemblyEntrada;

        public static Assembly AssemblyEntrada => ThreadUtil.RetornarValorComBloqueio(ref _assemblyEntrada,
                                                                                      ReflexaoUtil.RetornarAssemblyEntradaInterno);

        private static Assembly RetornarAssemblyEntradaInterno()
        {
            var assemblyEntrada = Assembly.GetEntryAssembly();
            if (assemblyEntrada != null)
            {
                return assemblyEntrada;
            }
            var tipoAplicacao = AplicacaoSnebur.Atual.TipoAplicacao;
            if (tipoAplicacao == Dominio.EnumTipoAplicacao.ExtensaoVisualStudio)
            {
                return AplicacaoSnebur._aplicacao.GetType().Assembly;
            }
            if (AplicacaoSnebur._aplicacao != null && AplicacaoSnebur._aplicacao.GetType() != typeof(AplicacaoSneburInterna))
            {
                return AplicacaoSnebur._aplicacao.GetType().Assembly;
            }
            var assemblites = AppDomain.CurrentDomain.GetAssemblies();
            if (SistemaUtil.TipoAplicacao == Dominio.EnumTipoAplicacao.DotNet_UnitTest)
            {
                foreach (var assembly in assemblites)
                {
                    var assemblyName = new AssemblyName(assembly.FullName);
                    if (assemblyName.Name.StartsWith(NOME_SNEBUR) && assemblyName.Name.ToLower().Contains("test"))
                    {
                        return assembly;
                    }
                }
            }
            throw new Erro("Não foi possivel retornar o assembly de entrada, defina a class da aplicação snebur");
        }

        public static Version RetornarVersaoEntrada()
        {
            var assemblyEntrada = ReflexaoUtil.AssemblyEntrada;
            return RetornarVersaoAssembly(assemblyEntrada);
        }

        public static List<Tuple<string, Version>> RetornarNomesVersaoAssemblies()
        {
            var retorno = new List<Tuple<string, Version>>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                retorno.Add(RetornarNomeVersaoAssembly(assembly));
            }
            return retorno;
        }

        public static Version RetornarVersaoAssembly(Assembly assembly)
        {
            //return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.ver;
            return assembly.GetName().Version;
        }

        public static Tuple<string, Version> RetornarNomeVersaoAssembly(Assembly assembly)
        {
            ErroUtil.ValidarReferenciaNula(assembly, nameof(assembly));
            var assemblyName = assembly.GetName();
            return new Tuple<string, Version>(assemblyName.Name, assemblyName.Version);
        }

        public static Tuple<string, Version> RetornarNomeVersaoAssemblyAplicacao()
        {
            var assemblyEntrada = ReflexaoUtil.AssemblyEntrada;
            return ReflexaoUtil.RetornarNomeVersaoAssembly(assemblyEntrada);
        }

        public static string RetornarNomeAplicacao()
        {
            var assemblyEntrada = ReflexaoUtil.AssemblyEntrada;
            var atributoTituloProduto = assemblyEntrada.GetCustomAttribute<AssemblyProductAttribute>();

            if (String.IsNullOrWhiteSpace(atributoTituloProduto?.Product))
            {
                throw new Exception(String.Format("O atributo {0} não foi definido no AssemblyInfo", nameof(AssemblyProductAttribute)));
            }
            return atributoTituloProduto.Product;
        }

        public static string RetornarNomeEmpresa()
        {
            var assemblyEntrada = ReflexaoUtil.AssemblyEntrada;
            var atributoNomeEmpresa = assemblyEntrada.GetCustomAttribute<AssemblyCompanyAttribute>();

            if (String.IsNullOrWhiteSpace(atributoNomeEmpresa?.Company))
            {
                if (System.Diagnostics.Debugger.IsAttached && SistemaUtil.TipoAplicacao != Dominio.EnumTipoAplicacao.DotNet_UnitTest)
                {
                    throw new Exception(String.Format("O atributo {0} não foi definido no AssemblyInfo", nameof(AssemblyCompanyAttribute)));
                }
                return NOME_EMPRESA_PADRAO;
            }
            return atributoNomeEmpresa.Company;
        }
    }
}