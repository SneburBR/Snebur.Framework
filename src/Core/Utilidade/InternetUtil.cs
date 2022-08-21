using Snebur.UI;
using System;
using System.Threading;

namespace Snebur.Utilidade
{
    public static class InternetUtil
    {
        private const int INTERVALO_MENSAGEM = 30;
        private static bool _mensagemAtiva = false;
        private static object _bloqueio = new object();
        //private static DateTime? _dataHoraUltimaVisualizacao;
        private static IJanelaAlerta Alerta;

        private const int TEMPO_ESPERAR = 10;

        public static void AguardarRestabelecerInternet()
        {
            if (!RedeUtil.InternetConectada())
            {
                while (!RedeUtil.InternetConectada())
                {
                    InternetUtil.MostrarMensagemSemInternet();
                    System.Threading.Thread.Sleep((int)TimeSpan.FromSeconds(TEMPO_ESPERAR).TotalMilliseconds);
                }
                InternetUtil.FecharMensagemSemInternet();
            }
        }

        public static void MostrarMensagemSemInternet()
        {
            if (AplicacaoSnebur.Atual.Alerta != null)
            {
                if (InternetUtil.IsMostrarMensagem())
                {
                    //InternetUtil._dataHoraUltimaVisualizacao = DateTime.Now;

                    if (!InternetUtil._mensagemAtiva)
                    {
                        lock (_bloqueio)
                        {
                            if (!InternetUtil._mensagemAtiva)
                            {
                                InternetUtil._mensagemAtiva = true;

                                ThreadUtil.ExecutarMainThread(() =>
                                {
                                    var titulo = "Sem conexão com a internet";
                                    var nomeProduto = AplicacaoSnebur.Atual.NomeAplicacao;
                                    var conteudo = String.Format("Não foi possível conectar o {0} com a internet.\nPor gentileza, verifique sua conexão.\nApós reestabelecer a conexão o sistema voltará automaticamente.", nomeProduto);

                                    InternetUtil.Alerta = AplicacaoSnebur.Atual.Alerta.Mostrar(conteudo,
                                                                                                 titulo,
                                                                                                 EnumTipoAlerta.Atencao,
                                                                                                 EnumBotoesAlerta.Nenhum, (resultado) =>
                                                                                                 {
                                                                                                     //InternetUtil._dataHoraUltimaVisualizacao = DateTime.Now;
                                                                                                     Thread.Sleep(100);
                                                                                                     InternetUtil._mensagemAtiva = false;
                                                                                                     InternetUtil.Alerta = null;
                                                                                                 });
                                });
                            }
                        }
                    }

                }
            }
        }

        private static bool IsMostrarMensagem()
        {
            lock (_bloqueio)
            {
                if (InternetUtil._mensagemAtiva)
                {
                    return false;
                }
                //if (InternetUtil._dataHoraUltimaVisualizacao.HasValue)
                //{
                //    var intervalo = DateTime.Now.TimeOfDay - InternetUtil._dataHoraUltimaVisualizacao.Value.TimeOfDay;
                //    if (Math.Abs(intervalo.TotalSeconds) < INTERVALO_MENSAGEM)
                //    {
                //        return false;
                //    }
                //}
                return true;
            }

            //return !RedeUtil.InternetConectada();
        }

        public static void FecharMensagemSemInternet()
        {
            lock (_bloqueio)
            {
                if (_mensagemAtiva && InternetUtil.Alerta != null)
                {

                    ThreadUtil.ExecutarMainThread(() =>
                    {
                        InternetUtil.Alerta.Fechar();
                    });
                }
            }
        }
    }
}