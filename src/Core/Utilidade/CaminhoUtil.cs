﻿using System;
using System.IO;

namespace Snebur.Utilidade
{
    public static class CaminhoUtil
    {
        public static string RetornarCaminhoArquivoBackup(string caminhoArquivo)
        {
            return RetornarCaminhoArquivoBackup(caminhoArquivo, 0);
        }

        private static string RetornarCaminhoArquivoBackup(string caminhoArquivo, int contador = 0)
        {
            var fi = new FileInfo(caminhoArquivo);
            var diretorio = Path.Combine(fi.Directory.FullName, "bkp");
            diretorio = diretorio.RetornarPrimeirosCaracteres(DiretorioUtil.LIMITE_MAXIMO_CARACTERES_CAMINHO_PASTA + 3);
            DiretorioUtil.CriarDiretorio(diretorio);

            var nomeArquivo = Path.GetFileNameWithoutExtension(fi.Name);
            if (contador > 0)
            {
                nomeArquivo += "_" + contador;
            }
            nomeArquivo += fi.Extension;

            var caminho = CaminhoUtil.RetornarCaminhoArquivoSuportado(diretorio, nomeArquivo);
            if (File.Exists(caminho))
            {
                return RetornarCaminhoArquivoBackup(caminhoArquivo, contador + 1);
            }
            return caminho;
        }

        public static string RetornarCaminhoArquivoTemporario(string diretorio)
        {
            return RetornarCaminhoArquivoTemporario(diretorio, null);
        }

        public static string RetornarCaminhoArquivoTemporario(string diretorio, string extensao)
        {
            var nomeArquivoTemporario = CaminhoUtil.RetornarNomeArquivoTemporario(extensao);
            //if (!String.IsNullOrEmpty(extensao))
            //{
            //    nomeArquivoTemporario += extensao;
            //}
            var caminhoArquivoTemporario = Path.Combine(diretorio, nomeArquivoTemporario);
            if (File.Exists(caminhoArquivoTemporario))
            {
                return CaminhoUtil.RetornarCaminhoArquivoTemporario(diretorio, extensao);
            }
            return caminhoArquivoTemporario;
        }

        public static string RetornarNomeArquivoTemporario()
        {
            return CaminhoUtil.RetornarNomeArquivoTemporario(".tmp");
        }

        public static string RetornarNomeArquivoTemporario(string extensao)
        {
            if (String.IsNullOrWhiteSpace(extensao))
            {
                extensao = ".tmp";
            }
            if (!extensao.StartsWith("."))
            {
                extensao = "." + extensao;
            }
            return String.Format("{0}{1}", Guid.NewGuid().ToString().Substring(0, 5), extensao);
        }

        public static bool CaminhoIgual(string caminho1, string caminho2)
        {
            if (String.IsNullOrWhiteSpace(caminho1) ^ String.IsNullOrWhiteSpace(caminho2))
            {
                return false;
            }
            var caminhoAbsoluto1 = Path.GetFullPath(caminho1);
            var caminhoAbsoluto2 = Path.GetFullPath(caminho2);
            caminhoAbsoluto1 = caminhoAbsoluto1.Replace('/', '\\').ToLower();
            caminhoAbsoluto2 = caminhoAbsoluto2.Replace('/', '\\').ToLower();

            return String.Equals(caminhoAbsoluto1, caminhoAbsoluto2, StringComparison.OrdinalIgnoreCase);
        }

        public static string RetornarCaminhosAbsoluto(string caminhoRelativo, string caminhoBase)
        {
            var caminho = Path.Combine(caminhoBase, caminhoRelativo);
            return Path.GetFullPath(caminho);
        }

        public static string RetornarCaminhoRelativo(string caminhoAbsoluto, string caminhoDiretorioReferencia)
        {
            var fileSystem = RetornarFileSystemInfo(caminhoAbsoluto);
            return RetornarCaminhoRelativo(fileSystem, new DirectoryInfo(caminhoDiretorioReferencia));
        }

        public static string RetornarCaminhoRelativo(FileInfo arquivo, string caminhoBase)
        {
            return RetornarCaminhoRelativo(arquivo, new DirectoryInfo(caminhoBase));
        }

        public static string RetornarCaminhoRelativo(FileInfo arquivo, DirectoryInfo diretorioReferencia)
        {
            return RetornarCaminhoRelativo((FileSystemInfo)arquivo, diretorioReferencia);
        }

        public static string RetornarCaminhoRelativo(DirectoryInfo diretorio, DirectoryInfo diretorioReferencia)
        {
            return RetornarCaminhoRelativo((FileSystemInfo)diretorio, diretorioReferencia);
        }

        public static string RetornarCaminhoRelativo(FileSystemInfo absoluto, DirectoryInfo baseReferencia)
        {
            Func<FileSystemInfo, string> retornarCaminho = fsi =>
            {
                var d = fsi as DirectoryInfo;
                return d == null ? fsi.FullName : d.FullName.TrimEnd('\\') + "\\";
            };

            var caminhoBaseReferencia = retornarCaminho(baseReferencia);
            var caminhoAbsoluto = retornarCaminho(absoluto);

            var uriBaseReferencia = new Uri(caminhoBaseReferencia);
            var uriAbsoluta = new Uri(caminhoAbsoluto);

            var uriRelativa = uriBaseReferencia.MakeRelativeUri(uriAbsoluta);
            var caminhoRelativo = Uri.UnescapeDataString(uriRelativa.ToString());
            return caminhoRelativo;
        }

        private static FileSystemInfo RetornarFileSystemInfo(string caminho, bool ignorarErro = false)
        {
            if (File.Exists(caminho))
            {
                return new FileInfo(caminho);
            }
            if (Directory.Exists(caminho))
            {
                return new DirectoryInfo(caminho);
            }
            var nomeArquivo = new FileInfo(Path.GetFileName(caminho));
            if (nomeArquivo.Extension.Length > 0)
            {
                return new FileInfo(caminho);
            }
            return new DirectoryInfo(caminho);

            throw new Exception($"o caminho não existe ou não é valido {caminho} ");
        }

        public static bool IsFilho(string caminhoPai, string caminhoFilho, EnumTipoCaminho tipoCaminho = EnumTipoCaminho.CaminhoWindows)
        {
            caminhoPai = NormalizarCaminho(caminhoPai, tipoCaminho);
            caminhoFilho = NormalizarCaminho(caminhoFilho, tipoCaminho);
            return caminhoFilho.ToLower().Contains(caminhoPai.ToLower());
        }

        public static string NormalizarCaminho(string caminho, EnumTipoCaminho tipoCaminho)
        {
            switch (tipoCaminho)
            {
                case EnumTipoCaminho.CaminhoWeb:

                    return caminho.Replace(@"\", "/");

                case EnumTipoCaminho.CaminhoWindows:
                    caminho = Path.GetFullPath(caminho);
                    return caminho.Replace("/", @"\");

                default:
                    throw new Erro($"Caminho não suportado {tipoCaminho}");
            }
            //var barra = tipoCaminho == EnumTipoCaminho.CaminhoWindows ? "\\" : "/";
        }

        public static string RetornarCaminhoArquivoSuportado(string diretorio, string nomeArquivo)
        {
            if (((diretorio.Length + nomeArquivo.Length) <= ArquivoUtil.TAMANHO_MAXIMO_CAMINHO_ARQUIVO))
            {
                return Path.Combine(diretorio, nomeArquivo);
            }
            if (diretorio.Length > (DiretorioUtil.LIMITE_MAXIMO_CARACTERES_CAMINHO_PASTA + 3))
            {
                diretorio = diretorio.RetornarPrimeirosCaracteres(DiretorioUtil.LIMITE_MAXIMO_CARACTERES_CAMINHO_PASTA);
                throw new Exception($"O tamanho do caminho do diretorio não é suportado {diretorio}");
            }
            var extensao = Path.GetExtension(nomeArquivo);
            var limiteSuportado = ArquivoUtil.TAMANHO_MAXIMO_CAMINHO_ARQUIVO - diretorio.Length - extensao.Length - 3;

            if (limiteSuportado > nomeArquivo.Length)
            {
                throw new Exception("o limite suporta não pode ser maior do nome do arquivo");
            }
            var nomeArquivoSemExtensao = Path.GetFileNameWithoutExtension(nomeArquivo);
            var divisao = (limiteSuportado / 2) - 1;
            var parteInicio = nomeArquivoSemExtensao.Substring(0, divisao);
            var parteFim = nomeArquivoSemExtensao.Substring(nomeArquivoSemExtensao.Length - divisao);
            var parteRandom = Path.GetFileName(Path.GetFileNameWithoutExtension(CaminhoUtil.RetornarNomeArquivoTemporario())).RetornarUtlimosCaracteres(3);

            var nomeArquivoSuportado = String.Concat(parteInicio, "..", parteFim);

            if (nomeArquivoSuportado.Length > limiteSuportado)
            {
                throw new Exception("O nome do arquivo suportado não pode ser maior limite suportado");
            }
            nomeArquivoSuportado += parteRandom;
            nomeArquivoSuportado += extensao;

            var caminhoArquivoSuportado = Path.Combine(diretorio, nomeArquivoSuportado);
            if (File.Exists(caminhoArquivoSuportado))
            {
                return RetornarCaminhoArquivoSuportado(diretorio, nomeArquivo);
            }
            return caminhoArquivoSuportado;
        }

        public static FileInfo RetornarCaminhoArquivoRecursivo(FileInfo arquivo)
        {
            try
            {
                while (!arquivo.Exists)
                {
                    arquivo = new FileInfo(Path.Combine(arquivo.Directory.Parent.FullName, arquivo.Name));
                    if (arquivo.Directory.Parent == null)
                    {
                        return null;
                    }
                }
                if(arquivo.Exists)
                {
                    return arquivo;
                }
                
            }
            catch
            {

            }
            return null;

        }
    }

    public enum EnumTipoCaminho
    {
        CaminhoWeb = 1,
        CaminhoWindows = 2,
    }
}