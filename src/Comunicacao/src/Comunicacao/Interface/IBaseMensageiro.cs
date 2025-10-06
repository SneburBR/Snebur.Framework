using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao;

public delegate void EventoMensagemHandler<TBaseDominho>(Remetente usario, TBaseDominho mensagem) where TBaseDominho : BaseDominio;

public delegate void EventoMensagemHandler(Remetente usario);

[IgnorarInterfaceTS]
[IgnorarServicoTS]
public interface IBaseConexaoMensageiro
{
    //event EventoMensagemHandler EventoNovaConectado;

    //event EventoMensagemHandler EventoConexaoDesconectada;
}
