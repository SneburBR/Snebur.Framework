using System;
using System.Collections.Generic;
using Snebur.Dominio;
using Snebur.Comunicacao;

namespace Snebur.AcessoDados
{
    public interface IServicoDados : IBaseServico
    {
        object RetornarValorScalar(EstruturaConsulta estruturaConsulta);

        ResultadoConsulta RetornarResultadoConsulta(EstruturaConsulta estruturaConsulta);

        ResultadoSalvar Salvar(List<Entidade> entidades);

        ResultadoExcluir Excluir(List<Entidade> entidades, string relacoesEmCascata);

        DateTime RetornarDataHora();

        DateTime RetornarDataHoraUTC();
    }
}