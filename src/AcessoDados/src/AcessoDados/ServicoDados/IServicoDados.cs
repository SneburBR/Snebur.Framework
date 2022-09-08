using Snebur.Comunicacao;
using Snebur.Dominio;
using System;
using System.Collections.Generic;

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