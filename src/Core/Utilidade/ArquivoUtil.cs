using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Snebur.Utilidade
{
    public static class ArquivoUtil
    {
        public const int TAMANHO_MAXIMO_CAMINHO_ARQUIVO = 257;
        public const int MAXIMO_CARACTERES_NOME_ARQUVIO_PADRAO = 35;

        #region Arquivos

        public static Task<bool> DeletarArquivoAsync(string caminho, bool ignorarErro = false, bool isForcar = false)
        {
            return Task.Factory.StartNew(() => DeletarArquivo(caminho, ignorarErro, isForcar));
        }

        public static bool DeletarArquivo(FileInfo fi, bool ignorarErro = false, bool isForcar = false)
        {
            return DeletarArquivo(fi.FullName, ignorarErro, isForcar);
        }

        public static bool DeletarArquivo(string caminho, bool ignorarErro = false, bool isForcar = false)
        {
            return ArquivoUtil.DeletarArquivo(caminho, ignorarErro, isForcar, 0);
        }

        public static bool SalvarStream(Stream stream,
                                        string caminhoDestino)
        {
            try
            {
                using (var ms = StreamUtil.RetornarMemoryStream(stream))
                {
                    File.WriteAllBytes(caminhoDestino, ms.ToArray());
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogUtil.ErroAsync(ex);
                return false;
            }
        }

        public static bool CopiarParaDiretorio(string caminhoOrigem, string diretorioDestino)
        {
            var caminhoDestino = Path.Combine(diretorioDestino, Path.GetFileName(caminhoOrigem));
            return MoverArquivo(caminhoOrigem,
                                caminhoDestino);
        }
        /// <summary>
        /// Retorna no um novo nome do aarquivo com extensao alterada, não nome é alterada no local fisico
        /// </summary>
        /// <param name="nomeArquivo"></param>
        /// <param name="extensao"></param>
        /// <returns></returns>
        public static string AlterarExtensaoNomeArquivo(string nomeArquivo, string extensao)
        {
            var nomeArquivoExtensao = Path.GetFileNameWithoutExtension(nomeArquivo) + ArquivoUtil.NormalizarExtensao(extensao);
            var diretorio = Path.GetDirectoryName(nomeArquivo);
            return Path.Combine(diretorio, nomeArquivoExtensao);
        }

        public static string NormalizarExtensao(string extensao)
        {
            if (!extensao.StartsWith("."))
            {
                return "." + extensao;
            }
            return extensao;
        }

        public static string RetornarVersao(string arquivo)
        {
            return FileVersionInfo.GetVersionInfo(arquivo).FileVersion;
        }

        private static bool DeletarArquivo(string caminho, bool ignorarErro, bool forcar, int tentativa)
        {
            if (!File.Exists(caminho))
            {
                return true;
            }
            try
            {
                ArquivoUtil.RemovendoAtributosSomenteLeitura(caminho);
                File.Delete(caminho);
                return true;
            }
            catch (Exception erro)
            {
                if (!(erro is UnauthorizedAccessException) && (forcar && tentativa < 20))
                {
                    System.Threading.Thread.Sleep(500 * tentativa);
                    return ArquivoUtil.DeletarArquivo(caminho, ignorarErro, forcar, tentativa + 1);
                }
                else
                {
                    if (!ignorarErro)
                    {
                        throw new ErroExcluirArquivo(String.Format("Não foi possível deletar o arquivo {0}", caminho), erro);
                    }
                    return false;
                }
            }
        }

        public static string RetornarSubExtenmsao(string caminho)
        {
            var length = caminho.Length;
            var index = -1;
            for (var i = length; --i >= 0;)
            {
                char ch = caminho[i];
                if (ch == '.')
                {
                    if (i != length - 1)
                    {
                        if (index > 0)
                        {
                            return caminho.Substring(i, length - i);
                        }
                        index = i;
                    }
                    //return String.Empty;
                }
                if (ch == Path.DirectorySeparatorChar ||
                    ch == Path.AltDirectorySeparatorChar ||
                    ch == Path.VolumeSeparatorChar)
                {
                    break;
                }
            }
            if (index > 0)
            {
                return caminho.Substring(index, length - index);
            }
            return String.Empty;
        }

        public static byte[] RetornarTodosBytes(string caminhoParte)
        {
            return StreamUtil.RetornarTodosBytes(caminhoParte);
        }

        private static void RemovendoAtributosSomenteLeitura(string caminho)
        {
            if (File.Exists(caminho))
            {
                try
                {
                    var atributos = File.GetAttributes(caminho);
                    if ((atributos & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        atributos = RemoverAtributo(atributos, FileAttributes.ReadOnly);
                        File.SetAttributes(caminho, atributos);
                    }
                }
                catch
                {
                }
            }
        }

        private static FileAttributes RemoverAtributo(FileAttributes atributos, FileAttributes atributoRemover)
        {
            return atributos & ~atributoRemover;
        }

        public static void CopiarArquivo(string caminhoOrigem, string caminhoDestinio)
        {
            ArquivoUtil.CopiarArquivo(caminhoOrigem, caminhoDestinio, true);
        }

        public static void CopiarArquivo(string caminhoOrigem, string caminhoDestinio, bool sobreEscrever)
        {
            ArquivoUtil.CopiarArquivo(caminhoOrigem, caminhoDestinio, sobreEscrever, false, false);
        }

        public static void CopiarArquivo(string caminhoOrigem, string caminhoDestinio, bool sobreEscrever, bool ignorarErroSobreEscrever)
        {
            ArquivoUtil.CopiarArquivo(caminhoOrigem, caminhoDestinio, sobreEscrever, ignorarErroSobreEscrever, false);
        }

        public static void CopiarArquivo(string caminhoOrigem,
                                         string caminhoDestinio,
                                         bool sobreEscrever,
                                         bool ignorarErroSobreEscrever,
                                         bool ignorarErroNaoExisteArquivoOrigem)
        {
            if (!File.Exists(caminhoOrigem))
            {
                if (!ignorarErroNaoExisteArquivoOrigem)
                {
                    throw new Erro(String.Format("O caminho do origem não foi encontrado {0} ", caminhoOrigem));
                }
                return;
            }
            if (ArquivoUtil.CaminhoIgual(caminhoOrigem, caminhoDestinio))
            {
                if (!ignorarErroNaoExisteArquivoOrigem)
                {
                    throw new Erro($"O caminho arquivo de origem: '{caminhoOrigem}' é mesmo de destino: {caminhoDestinio}");
                }
                return;
            }
            var arquivoDestino = new FileInfo(caminhoDestinio);
            DiretorioUtil.CriarDiretorio(arquivoDestino.Directory.FullName);

            if (arquivoDestino.Exists)
            {
                if (sobreEscrever)
                {
                    ArquivoUtil.DeletarArquivo(arquivoDestino.FullName, ignorarErroSobreEscrever, true);
                }
                else
                {
                    if (!ignorarErroSobreEscrever)
                    {
                        throw new Erro(String.Format("O arquivo {0} de destino já existe {0}", caminhoDestinio));
                    }
                    return;
                }
            }
            try
            {
                File.Copy(caminhoOrigem, caminhoDestinio);
            }
            catch
            {
                CopiarUsandoFileStream(caminhoOrigem, caminhoDestinio, true);
            }
        }

        public static bool CopiarUsandoFileStream(string caminhoOrigem,
                                                  string caminhoDestinio, bool isIgnorarErro)
        {
            try
            {
                DeletarArquivo(caminhoDestinio, false, true);
                StreamUtil.CopiarArquivo(caminhoOrigem, caminhoDestinio);
                return true;
            }
            catch (Exception ex)
            {
                if (isIgnorarErro)
                {
                    return false;
                }
                throw new Erro($"Não foi pssivel copiar o arquivo {caminhoOrigem} para{caminhoDestinio}", ex);
            }
        }

        public static string RetornarNomeArquivo(string nomeArquivo, string extensao)
        {
            if (nomeArquivo.EndsWith(extensao, StringComparison.InvariantCultureIgnoreCase))
            {
                return nomeArquivo;
            }
            extensao = ArquivoUtil.NormalizarExtensao(extensao);
            return String.Format("{0}{1}", nomeArquivo, extensao);
        }
        #endregion

        /// <summary>
        /// Retornar no nome arquivo com o final do contador
        /// Ex. teste.jpg (1) teste(1).jpg
        /// </summary>
        /// <param name="nomeArquivo"></param>
        /// <param name="copia"></param>
        /// <returns></returns>
        ///  

        //public static string RetornarNomeArquivoCopia(string nomeArquivo, int copia)
        //{
        //    return ArquivoUtil.RetornarNomeArquivoCopia(nomeArquivo, copia, false);
        //}

        public static string RetornarNomeArquivoCopia(string nomeArquivo,
                                                      int copia,
                                                      bool incluirPalavraCopia = true,
                                                      bool incluirEspaco = true)
        {
            var nomeArquivoSemExentesao = Path.GetFileNameWithoutExtension(nomeArquivo);
            var extensao = Path.GetExtension(nomeArquivo);
            var palavraCopia = (incluirPalavraCopia) ? " cópia" : String.Empty;
            var espaco = incluirEspaco ? " " : String.Empty;
            var nomeArquivoContador = String.Format("{0}{1}{2}({3}){4}", nomeArquivoSemExentesao, palavraCopia, espaco, copia.ToString(), extensao);
            return nomeArquivoContador;
        }

        public static string RetornarCaminhoArquivoCopia(string caminhoArquivo, int copia, bool incluirPalabraCopia = false)
        {
            return ArquivoUtil.RetornarCaminhoArquivoCopia(new FileInfo(caminhoArquivo), copia, incluirPalabraCopia);
        }

        public static string RetornarCaminhoArquivoCopia(FileInfo fi, int copia, bool incluirPalabraCopia = false)
        {
            var nomeArquivo = ArquivoUtil.RetornarNomeArquivoCopia(fi.Name, copia, incluirPalabraCopia);
            return System.IO.Path.Combine(fi.Directory.FullName, nomeArquivo);
        }

        public static string RetornarCaminhoArquivoCopia(string caminhoArquivo, bool incluirPalabraCopia = false, bool incluirEspaco = true)
        {
            return RetornarCaminhoArquivoCopia(new FileInfo(caminhoArquivo), incluirPalabraCopia, incluirEspaco);
        }

        public static string RetornarCaminhoArquivoCopia(FileInfo fi, bool incluirPalabraCopia = false, bool incluirEspaco = true)
        {
            DiretorioUtil.CriarDiretorio(fi.Directory.FullName);

            var contador = 1;
            var nomeArquivo = ArquivoUtil.RetornarNomeArquivoCopia(fi.Name, contador, incluirPalabraCopia, incluirEspaco);
            var caminhoCopia = System.IO.Path.Combine(fi.Directory.FullName, nomeArquivo);

            while (File.Exists(caminhoCopia))
            {
                contador++;

                nomeArquivo = ArquivoUtil.RetornarNomeArquivoCopia(fi.Name, contador, incluirPalabraCopia, incluirEspaco);
                caminhoCopia = System.IO.Path.Combine(fi.Directory.FullName, nomeArquivo);
            }
            return caminhoCopia;
        }

        public static string FormatarNomeArquivoParaWeb(string nomeArquivo, int maximoCaracteres = MAXIMO_CARACTERES_NOME_ARQUVIO_PADRAO)
        {
            return FormatacaoUtil.FormatarNomeArquivoParaWeb(nomeArquivo, maximoCaracteres);
        }

        public static bool MoverArquivo(string caminhoOrigem,
                                        string caminhoDestino,
                                        bool isSobreEscrever = false,
                                        bool ignorarErro = false)
        {
            if (!File.Exists(caminhoOrigem))
            {
                if (!ignorarErro)
                {
                    throw new Erro($"o caminho de origem não existe {caminhoOrigem}");
                }
                return false;
            }
            if (isSobreEscrever)
            {
                ArquivoUtil.DeletarArquivo(caminhoDestino, true, ignorarErro);
            }
            try
            {
                File.Move(caminhoOrigem, caminhoDestino);
                return true;
            }
            catch
            {
                ArquivoUtil.DeletarArquivo(caminhoOrigem, ignorarErro, true);
                File.Move(caminhoOrigem, caminhoDestino);
                return true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="diretorio"></param>
        /// <param name="nomeArquivo"></param>
        /// <returns></returns>
        /// 

        #region Diretorios

        public static bool DiretorioIgual(string caminhoDiretorio1, string caminhoDiretorio2)
        {
            return CaminhoIgual(caminhoDiretorio1, caminhoDiretorio2);
        }

        public static bool CaminhoIgual(string caminho1, string caminho2)
        {
            caminho1 = NormalizarCaminhoArquivo(caminho1);
            caminho2 = NormalizarCaminhoArquivo(caminho2);
            return caminho1.Equals(caminho2, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string NormalizarCaminhoArquivo(string caminhoArquivo)
        {
            return Path.GetFullPath(new Uri(caminhoArquivo).LocalPath)
               .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Trim();
            //caminhoArquivo = caminhoArquivo.ToLower().Replace("/", "\\");
            //return caminhoArquivo.Trim();
        }
        #endregion

        #region Texto

        public static string LerTexto(FileInfo arquivoScript)
        {
            return LerTexto(arquivoScript.FullName);
        }

        public static string LerTexto(string caminho)
        {
            return LerTexto(caminho, Encoding.UTF8);
        }

        public static string LerTexto(string caminho, bool utf8)
        {
            var encoding = (utf8) ? Encoding.UTF8 : Encoding.Default;
            return LerTexto(caminho, encoding);
        }

        public static string LerTexto(string caminho, Encoding encoding)
        {
            return File.ReadAllText(caminho, encoding);
        }

        public static void SalvarTexto(string caminho, string texto)
        {
            ArquivoUtil.SalvarTexto(caminho, texto, System.Text.Encoding.UTF8);
        }

        public static void SalvarTexto(string caminho, string texto, Encoding encoding)
        {
            ErroUtil.ValidarReferenciaNula(caminho, nameof(caminho));
            ErroUtil.ValidarReferenciaNula(texto, nameof(texto));
            ErroUtil.ValidarReferenciaNula(encoding, nameof(encoding));

            var infoArquivo = new FileInfo(caminho);
            DiretorioUtil.CriarDiretorio(infoArquivo.Directory.FullName);

            if (infoArquivo.Exists)
            {
                ArquivoUtil.DeletarArquivo(caminho);
            }
            try
            {
                File.WriteAllText(caminho, texto, encoding);
            }
            catch (Exception erro)
            {
                throw new Erro(String.Format("Não foi possível salvar o arquivo de texto {0}", texto), erro);
            }
        }

        public static Task SalvarArquivoTextoAsync(string caminhoArquivo,
                                                   string texto,
                                                   Encoding encoding = null)
        {
            return Task.Factory.StartNew(() => SalvarArquivoTexto(caminhoArquivo, texto, encoding));
        }

        public static void SalvarArquivoTexto(string caminhoArquivo, string texto)
        {
            SalvarArquivoTexto(caminhoArquivo, texto, Encoding.UTF8);
        }
        public static void SalvarArquivoTexto(string caminhoArquivo,
                                              string texto,
                                              Encoding encoding)
        {
            SalvarArquivoTexto(caminhoArquivo, texto, encoding, 0, false);
        }

        private static void SalvarArquivoTexto(string caminhoArquivo,
                                               string texto,
                                               Encoding encoding,
                                               int tentatava,
                                               bool temmporario)
        {
            var fi = new FileInfo(caminhoArquivo);
            DiretorioUtil.CriarDiretorio(fi.Directory.FullName);
            try
            {
                if (fi.Exists)
                {
                    DeletarArquivo(caminhoArquivo);
                }
                File.WriteAllText(caminhoArquivo, texto, encoding ?? Encoding.UTF8);
            }
            catch (Exception)
            {
                if (tentatava > 5)
                {
                    if (temmporario)
                    {
                        throw;
                    }
                    else
                    {
                        tentatava = 0;
                        temmporario = true;

                        var nomeArquivoTemporaio = String.Format("{0}.{1}_temp{2}",
                                                                 Path.GetFileNameWithoutExtension(fi.Name),
                                                                 new Random().Next(10000, 99000).ToString(),
                                                                 fi.Extension);

                        caminhoArquivo = Path.Combine(fi.Directory.FullName, nomeArquivoTemporaio);
                    }
                }
                System.Threading.Thread.Sleep(1000);
                SalvarArquivoTexto(caminhoArquivo, texto, encoding, tentatava += 1, temmporario);
            }
        }

        public static List<string> FiltroExtensoes(List<string> arquivos, List<string> extensoes)
        {
            var extensoesComPonto = RetornarExtensoesNormalizadas(extensoes);
            return arquivos.Where(x => extensoesComPonto.Contains(Path.GetExtension(x), new IgnorarCasoSensivel())).ToList();
        }

        public static List<string> RetornarExtensoesNormalizadas(List<string> extensoes)
        {
            return extensoes.Select(x => ArquivoUtil.NormalizarExtensao(x)).ToList();
        }
        #endregion
    }
}