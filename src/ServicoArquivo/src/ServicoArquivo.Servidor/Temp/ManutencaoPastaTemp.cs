using Snebur.Utilidade;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Snebur.ServicoArquivo.Servidor
{
    public class ManutencaoPastaTemp
    {
        private static bool Inicializado;
        public static string CaminhoTemp { get; set; }

        private static object bloqueio = new object();
        public static void IncializarAsync(string caminhoTemp)
        {
            Task.Factory.StartNew(() => Inicializar(caminhoTemp));
        }

        public static void Inicializar(string caminhoTemp)
        {
            lock (bloqueio)
            {
                if (!Inicializado)
                {
                    ManutencaoPastaTemp.CaminhoTemp = caminhoTemp;
                    var pastaTemp = new DirectoryInfo(caminhoTemp);

                    foreach (var arquivo in pastaTemp.GetFiles())
                    {
                        if (arquivo.Extension.ToLower() == ".tmp" && arquivo.CreationTime.AddDays(2) < DateTime.Now)
                        {
                            ArquivoUtil.DeletarArquivo(arquivo.FullName, true);
                        }
                    }
                    Inicializado = true;
                }
            }
        }
        static ManutencaoPastaTemp()
        {
        }
    }
}