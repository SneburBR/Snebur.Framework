﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Snebur.Utilidade
{
    public static class StreamUtil
    {
        //private const int TAMANHO_BUFFER = 5;
        private const int TAMANHO_BUFFER_PADRAO = 32 * 1024;
        private const int TAMANHO_BUFFER_MINIMO = 1024;

        /// <summary>
        /// Ler as stream, buffer a buffer e retornar um MemoryStream, util,
        /// Quando usar, em stream conecta remotamente.
        /// Ex. request.GetRequestStream ou response.GetReponseStream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MemoryStream RetornarMemoryStream(Stream stream,
                                                        Action<StreamProgressEventArgs> callbackProgresso = null)
        {
            return StreamUtil.RetornarMemoryStreamBuferizada(stream,
                                                             TAMANHO_BUFFER_PADRAO,
                                                             callbackProgresso);
        }
        public static MemoryStream RetornarMemoryStreamBuferizada(Stream stream,
                                                                  Action<StreamProgressEventArgs> callbackProgresso = null,
                                                                  long contentLenght = 0)
        {
            return StreamUtil.RetornarMemoryStreamBuferizada(stream,
                                                             TAMANHO_BUFFER_PADRAO,
                                                             callbackProgresso,
                                                             contentLenght);
        }

        public static MemoryStream RetornarMemoryStreamBuferizada(Stream streamOrigem,
                                                                  int tamanhoBuffer,
                                                                  Action<StreamProgressEventArgs> callbackProgresso = null,
                                                                  long contentLenght = 0)
        {
            if (streamOrigem.CanSeek)
            {
                streamOrigem.Seek(0, SeekOrigin.Begin);
            }
            var ms = new MemoryStream();
            StreamUtil.SalvarStreamBufferizada(streamOrigem, ms, tamanhoBuffer, callbackProgresso, contentLenght);
            return ms;
        }
        public static void SalvarStreamBufferizada(Stream streamOrigem, Stream streamDestino)
        {
            StreamUtil.SalvarStreamBufferizada(streamOrigem, streamDestino, TAMANHO_BUFFER_PADRAO);
        }

        public static void SalvarStreamBufferizada(Stream streamOrigem,
                                                   Stream streamDestino,
                                                   int tamanhoBuffer,
                                                   Action<StreamProgressEventArgs> callbackProgresso = null,
                                                   long contentLenght = 0)
        {
            var totalBytesRecebitos = 0;
            var totalBytes = streamOrigem.CanSeek ? streamOrigem.Length : contentLenght;

            if (streamOrigem.CanSeek)
            {
                streamOrigem.Seek(0, SeekOrigin.Begin);
            }

            tamanhoBuffer = Math.Max(TAMANHO_BUFFER_MINIMO, TAMANHO_BUFFER_PADRAO);
             

            while (true)
            {
                var buffer = new byte[tamanhoBuffer];
                var bytesRecebido = streamOrigem.Read(buffer, 0, tamanhoBuffer);
                streamDestino.Write(buffer, 0, bytesRecebido);
                totalBytesRecebitos += bytesRecebido;

                if (callbackProgresso != null)
                {
                    callbackProgresso(new StreamProgressEventArgs(totalBytesRecebitos, totalBytes));
                }

                if (bytesRecebido == 0)
                {
                    break;
                }
                streamDestino.Flush();
            }
            if (streamDestino.CanSeek)
            {
                streamDestino.Seek(0, SeekOrigin.Begin);
            }
        }

        public static async Task SalvarStreamBufferizadaAsync(Stream streamOrigem,
                                                            Stream streamDestino,
                                                             int tamanhoBuffer = TAMANHO_BUFFER_PADRAO)
        {
            if (streamOrigem.CanSeek)
            {
                streamOrigem.Seek(0, SeekOrigin.Begin);
            }
            while (true)
            {
                var buffer = new byte[tamanhoBuffer];
                var bytesRecebido = streamOrigem.Read(buffer, 0, tamanhoBuffer);
                await streamDestino.WriteAsync(buffer, 0, bytesRecebido);

                if (bytesRecebido == 0)
                {
                    break;
                }
                await streamDestino.FlushAsync();
            }
            if (streamDestino.CanSeek)
            {
                streamDestino.Seek(0, SeekOrigin.Begin);
            }
        }

        public static void SalvarStream(string caminhoArquivo, Stream stream)
        {
            if (stream is MemoryStream)
            {
                StreamUtil.SalvarMemoryStream(caminhoArquivo, (MemoryStream)stream);
            }
            else
            {
                using (var ms = StreamUtil.RetornarMemoryStream(stream))
                {
                    StreamUtil.SalvarMemoryStream(caminhoArquivo, ms);
                }
            }
        }

        private static void SalvarMemoryStream(string caminhoArquivo, MemoryStream stream)
        {
            File.WriteAllBytes(caminhoArquivo, stream.ToArray());
        }

        public static FileStream OpenRead(string caminhoArquivo)
        {
            if (!File.Exists(caminhoArquivo))
            {
                throw new ErroArquivoNaoEncontrado(caminhoArquivo);
            }
            return new FileStream(caminhoArquivo, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public static FileStream CreateWrite(string caminhoArquivo)
        {
            if (File.Exists(caminhoArquivo))
            {
                ArquivoUtil.DeletarArquivo(caminhoArquivo, false, true);
            }
            DiretorioUtil.CriarDiretorio(Path.GetDirectoryName(caminhoArquivo));
            return new FileStream(caminhoArquivo, FileMode.Create, FileAccess.Write, FileShare.Write);
        }
        public static FileStream CreateOrOpenWrite(string caminhoArquivo, bool isForcar = false)
        {
            DiretorioUtil.CriarDiretorio(Path.GetDirectoryName(caminhoArquivo));
            var tentativa = 0;
            while (true)
            {
                try
                {
                    
                    return new FileStream(caminhoArquivo, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
                }
                catch (IOException)
                {
                    tentativa += 1;
                    if (!isForcar || (tentativa > 10))
                    {
                        throw;
                    }
                    Thread.Sleep(500);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public static byte[] RetornarTodosBytes(string caminhoParte)
        {
            using (var fs = StreamUtil.OpenRead(caminhoParte))
            {
                using (var ms = RetornarMemoryStreamBuferizada(fs))
                {
                    return ms.ToArray();
                }
            }
        }

        public static FileStream OpenWrite(string caminhoArquivo)
        {
            if (File.Exists(caminhoArquivo))
            {
                ArquivoUtil.DeletarArquivo(caminhoArquivo, false, true);
            }
            return new FileStream(caminhoArquivo, FileMode.Create, FileAccess.Write, FileShare.Write);
        }
        public static void SetarPonteiroInicio(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
        }

        public static void CopiarArquivo(string caminhoOrigem,
                                         string caminhoDestinio)
        {
            StreamUtil.CopiarArquivo(caminhoOrigem, caminhoDestinio, TAMANHO_BUFFER_PADRAO);
        }

        public static void CopiarArquivo(string caminhoOrigem,
                                         string caminhoDestinio,
                                         Action<StreamProgressEventArgs> callbackProgresso = null)
        {
            StreamUtil.CopiarArquivo(caminhoOrigem, 
                                     caminhoDestinio, 
                                     TAMANHO_BUFFER_PADRAO, 
                                     callbackProgresso);

        }
        public static void CopiarArquivo(string caminhoOrigem,
                                         string caminhoDestinio,
                                         int tamanhoBuffer = TAMANHO_BUFFER_PADRAO,
                                         Action<StreamProgressEventArgs> callbackProgresso = null)
        {
            if (!File.Exists(caminhoOrigem))
            {
                throw new FileNotFoundException(caminhoOrigem);
            }
            using (var streamOrigem = StreamUtil.OpenRead(caminhoOrigem))
            using (var streamDestino = StreamUtil.CreateWrite(caminhoDestinio))
            {
                StreamUtil.SalvarStreamBufferizada(streamOrigem, streamDestino,
                                                  tamanhoBuffer,
                                                  callbackProgresso);
            }
        }
    }
}