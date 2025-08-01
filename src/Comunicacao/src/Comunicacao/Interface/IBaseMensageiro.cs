﻿using Snebur.Comunicacao.Dominio.Mensageiro;
using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao.Interface
{

    public delegate void EventoMensagemHandler<TBaseDominho>(Remetente usario, TBaseDominho mensagem) where TBaseDominho : BaseDominio;

    public delegate void EventoMensagemHandler(Remetente usario);

    [IgnorarInterfaceTS]
    public interface IBaseConexaoMensageiro
    {
        //event EventoMensagemHandler EventoNovaConectado;

        //event EventoMensagemHandler EventoConexaoDesconectada;
    }
}
