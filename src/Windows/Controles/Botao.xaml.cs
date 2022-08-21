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

namespace Snebur.Windows
{
    public partial class Botao : UserControl
    {
        public static readonly DependencyProperty TextoBotaoProperty = DependencyProperty.Register("TextoBotao", typeof(string), typeof(Botao), new PropertyMetadata("Ok"));
        public static readonly DependencyProperty CorTextoProperty = DependencyProperty.Register("CorTexto", typeof(Brush), typeof(Botao), new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty FundoProperty = DependencyProperty.Register("Fundo", typeof(Brush), typeof(Botao), new PropertyMetadata(Brushes.LightGray));

        public string TextoBotao
        {
            get { return (string)GetValue(TextoBotaoProperty); }
            set { SetValue(TextoBotaoProperty, value); }
        }

        public Brush CorTexto
        {
            get { return (Brush)GetValue(CorTextoProperty); }
            set { SetValue(CorTextoProperty, value); }
        }

        public Brush Fundo
        {
            get { return (Brush)GetValue(FundoProperty); }
            set { SetValue(FundoProperty, value); }
        }

        public Botao()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        private void BtnPersonalizado_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.SetCursor(Cursors.Hand);
        }

        private void BtnPersonalizado_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.SetCursor(Cursors.Arrow);
        }
    }
}
