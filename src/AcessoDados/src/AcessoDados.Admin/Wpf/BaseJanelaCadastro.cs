using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Admin
{
    public abstract class BaseJanelaCadastro : BaseJanela
    {
        public BaseJanelaCadastro() : base()
        {

        }

        public bool Salvar(Entidade entidade)
        {
            try
            {
                using (var contexto = ContextoDadosUtil.RetornarContextoDados())
                {
                    var resultado = contexto.SalvarSeguranca(entidade);
                    return resultado.IsSucesso;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

    }
}
