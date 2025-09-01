using System.Windows;
using System.Windows.Controls;

namespace Snebur.Windows;

public abstract class BaseControleRotulo : UserControl
{
    public static readonly DependencyProperty RotuloProperty = DependencyProperty.Register("Rotulo", typeof(string), typeof(BaseControleRotulo), 
                                                                                                                     new PropertyMetadata(String.Empty));
    public string Rotulo
    {
        get { return (string)this.GetValue(RotuloProperty); }
        set { this.SetValue(RotuloProperty, value); }
    }
}
