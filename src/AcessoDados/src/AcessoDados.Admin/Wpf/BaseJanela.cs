using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Snebur.AcessoDados.Admin
{
    public abstract class BaseJanela : Window
    {
        public BaseJanela()
        {
            

            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.KeyDown += this.BaseJanela_KeyDown;
        }

        private void BaseJanela_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.DialogResult = false;
            }
        }
    }
}
