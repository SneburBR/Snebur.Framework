using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Snebur.AcessoDados.Admin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            AppDomain.CurrentDomain.AssemblyResolve += this.App_AssemblyResolve;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AplicacaoSnebur.Atual = new AplicacaoAcessoDadosAdmin();
            this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var erro = e.Exception;
            MessageBox.Show(App.Current.MainWindow, erro.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            System.Environment.Exit(0);
        }

        private Assembly App_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var nomeAssembly = args.Name.Split(',').First();
            if ((nomeAssembly == Configuracao.NomeAssemblyContexto) ||
                (nomeAssembly == Configuracao.NomeAssemblyEntidades))
            {
                var caminhoAssembly = Configuracao.RetornarCaminhoAssembly(nomeAssembly);
                if (!String.IsNullOrWhiteSpace(caminhoAssembly))
                {
                    if (!File.Exists(caminhoAssembly))
                    {
                        throw new ErroArquivoNaoEncontrado(caminhoAssembly);
                    }
                    var assembly = Assembly.Load(File.ReadAllBytes(caminhoAssembly));

                    //var nomeQualificadoTipoContexto = $"{Configuracao.NomeAssemblyContexto}.{Configuracao.NomeTipoContexto}, {Configuracao.NomeAssemblyContexto}";
                    //var xxx = assembly.GetType(nomeQualificadoTipoContexto);
                    //xxx = assembly.GetType("Snebur.Sigi.AcessoDados.ContextoSigi");
                    //var tipos = assembly.GetTypes().ToList();

                    return assembly;
                }
            }
            return null;
        }
    }
}
