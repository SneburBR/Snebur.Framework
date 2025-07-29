using Snebur.Dominio.Atributos;

namespace Snebur.Serializacao;

[IgnorarInterfaceTS]
public interface IBaseDominioReferencia : ICaminhoTipo
{
    bool IsSerializando { get; set; }
    Guid __IdentificadorUnico { get; }
    Guid? __IdentificadorReferencia { get; set; }
    bool? __IsBaseDominioReferencia { get; set; }
    Guid RetornarIdentificadorReferencia();
    //void LimparRefencia();
}
