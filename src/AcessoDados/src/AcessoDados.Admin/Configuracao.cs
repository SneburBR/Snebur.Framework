using System;
using System.Configuration;
using System.IO;

namespace Snebur.AcessoDados.Admin
{
    public class Configuracao
    {
        private const string NOME_TIPO_CONTEXTO = "NomeTipoContexto";
        private const string NOME_ASSEMBLY_CONTEXTO = "NomeAssemblyContexto";
        private const string NOME_ASSEMBLY_ENTIDADES = "NomeAssemblyEntidades";

        private const string CAMINHO_ASSEMBLY_CONTEXTO = "CaminhoAssemblyContexto";
        private const string CAMINHO_ASSEMBLY_ENTIDADES = "CaminhoAssemblyEntidades";

        //private static string _nomeTipoContexto;
        //    private static _nomeTipoContexto;


        public static string NomeTipoContexto
        {
            get
            {
                return Configuracao.RetornarConfiguracao(NOME_TIPO_CONTEXTO);
            }
        }

        public static string NomeAssemblyContexto
        {
            get
            {
                return Configuracao.RetornarConfiguracao(NOME_ASSEMBLY_CONTEXTO);
            }
        }

        public static string NomeAssemblyEntidades
        {
            get
            {
                return Configuracao.RetornarConfiguracao(NOME_ASSEMBLY_ENTIDADES);
            }
        }

        private static string RetornarConfiguracao(string chave)
        {
            var valor = ConfigurationManager.AppSettings[chave];
            if (String.IsNullOrEmpty(valor))
            {
                throw new Exception(String.Format("O configuracao {0} não foi definido no appSettings da aplicação ", chave));
            }
            return valor;
        }

        public static string RetornarCaminhoAssembly(string nomeAssembly)
        {
            var caminhoRelativoOuAbsolutoAssembly = Configuracao.RetornarCaminhoRelativoOuAbsolutoAssembly(nomeAssembly);
            if (!String.IsNullOrWhiteSpace(caminhoRelativoOuAbsolutoAssembly))
            {
                if(Path.IsPathRooted(caminhoRelativoOuAbsolutoAssembly))
                {
                    //caminho absoluto
                    return caminhoRelativoOuAbsolutoAssembly;
                }

                var caminhoAbsoluto = Path.Combine(AplicacaoSnebur.Atual.DiretorioAplicacao, caminhoRelativoOuAbsolutoAssembly);
                return Path.GetFullPath(caminhoAbsoluto);
            }
            return null;
        }

        private static string RetornarCaminhoRelativoOuAbsolutoAssembly(string nomeAssembly)
        {
            if (nomeAssembly == Configuracao.NomeAssemblyContexto)
            {
                return ConfigurationManager.AppSettings[CAMINHO_ASSEMBLY_CONTEXTO];
            }

            if (nomeAssembly == Configuracao.NomeAssemblyEntidades)
            {
                return ConfigurationManager.AppSettings[CAMINHO_ASSEMBLY_ENTIDADES];
            }
            return null;
        }
    }
}
;