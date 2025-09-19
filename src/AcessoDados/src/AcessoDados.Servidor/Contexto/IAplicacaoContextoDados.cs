
namespace Snebur.AcessoDados;

//public interface IAplicacaoContextoDados
//{
//    void NovoConexaoDados(BaseContextoDados baseContextoDados);
//    void ConexaoDadosDispensado(BaseContextoDados baseContextoDados);
//    BaseContextoDados RetornarContextoDadoAtual();
//}

public interface IAplicacaoContextoDados
{
    void NovoConexaoDados(BaseContextoDados baseContextoDados);
    void ConexaoDadosDispensado(BaseContextoDados baseContextoDados);
    TBaseContextoDados? RetornarContextoDadoAtual<TBaseContextoDados>()
        where TBaseContextoDados : BaseContextoDados;
}