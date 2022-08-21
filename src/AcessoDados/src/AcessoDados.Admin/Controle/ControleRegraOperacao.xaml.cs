using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Snebur.AcessoDados.Admin.ViewModels;

namespace Snebur.AcessoDados.Admin
{
    
    public partial class ControleRegraOperacao : UserControl
    {
        public static readonly DependencyProperty RegraoOperacaoProperty = DependencyProperty.Register("RegraOperacao",
                                                                                               typeof(BaseRegraOperacaoViewModel),
                                                                                               typeof(ControleRegraOperacao));
        public BaseRegraOperacaoViewModel RegraOperacao
        {
            get
            {
                return (BaseRegraOperacaoViewModel)this.GetValue(ControleRegraOperacao.RegraoOperacaoProperty);
            }
            set
            {
                this.SetValue(ControleRegraOperacao.RegraoOperacaoProperty, value);
                //this.DataContext = this;
            }
        }

        public ControleRegraOperacao()
        {
            InitializeComponent();

            this.DataContext = this;
        }
    }
}
