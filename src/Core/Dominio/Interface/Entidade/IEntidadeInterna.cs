using Snebur.Dominio.Atributos;

namespace Snebur.Dominio;

[IgnorarInterfaceTS]
public interface IEntidadeInterna : IEntidade
{
    List<string>? __PropriedadesAbertas { get; }

    List<string>? __PropriedadesAutorizadas { get; }

    //void AtivarControladorPropriedadeAlterada();

    void AtribuirPropriedadesAbertas(List<string> PropriedadesAberta);

    void AtribuirPropriedadesAutorizadas(List<string> PropriedadesAutorizadas);

    void AdicionarProprieadeAberta(string nomePropriedade);

    void DesativarValidacaoProprieadesAbertas();

    void AtivarValidacaoProprieadesAbertas();

    void NotifyIsNotNewEntity();
}