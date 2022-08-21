using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.UI;
using Snebur.Windows;

namespace System.Windows
{
    public static class FrameworkElementExtensao
    {
        public static bool MostrarMensagem(this FrameworkElement fe, string titulo, string mensagem, EnumBotoesAlerta botoesAlerta, string textoBotao = "")
        {

            if (Application.Current.MainWindow is BaseJanelaPrincipal janelaPrincipal)
            {
                return janelaPrincipal.MostrarMensagem(titulo, mensagem, botoesAlerta, textoBotao);
            }

            throw new Erro($"A janela principal não herda de {nameof(BaseJanelaPrincipal)}");
        }
    }
}
