using System;
using System.Collections.Generic;
using Snebur.Dominio;

namespace Snebur.AcessoDados
{
    public interface IContextoDadosSeguranca : IDisposable
    {
        bool IsAnonimo { get; }

        bool InicializandoContexto { get; }

        TiposSeguranca TiposSeguranca { get;  }

        //IUsuario UsuarioLogado { get; }

        ISessaoUsuario SessaoUsuarioLogado { get; }

        ResultadoSalvar SalvarSeguranca(IEntidade entidade);

        ResultadoSalvar SalvarSeguranca(List<IEntidade> entidades);

        ResultadoExcluir ExcluirSeguranca(IEntidade entidade);

        ResultadoExcluir ExcluirSeguranca(List<IEntidade> entidades);

        IConsultaEntidade<TEntidade> RetornarConsulta<TEntidade>() where TEntidade : IEntidade;

        IConsultaEntidade<TEntidade> RetornarConsulta<TEntidade>(Type tipoConsulta) where TEntidade : IEntidade;

        //List<IEntidade> RetornarResultadoConsulta(EstruturaConsulta estruturaConsulta);
       
        IUsuario RetornarUsuarioAnonimo();
    }
}