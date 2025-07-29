using Snebur.Dominio.Atributos;

namespace Snebur.UI;

[IgnorarInterfaceTS]
public interface IJanelaAlerta : IDisposable
{
    void Fechar();
}
