using Snebur.Comunicacao;
using Snebur.Dominio;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Snebur.AcessoDados.Cliente
{
    public abstract class BaseServicoDadosCliente : BaseComunicacaoCliente, IServicoDados
    {

        public BaseServicoDadosCliente(string urlServicoDados) : base(urlServicoDados)
        {
        }

        #region IServicoDados

        public object RetornarValorScalar(EstruturaConsulta estruturaConsulta)
        {
            object[] parametros = { estruturaConsulta };
            return this.ChamarServico<object>(MethodBase.GetCurrentMethod(), parametros);
        }

        public ResultadoConsulta RetornarResultadoConsulta(EstruturaConsulta estruturaConsulta)
        {
            object[] parametros = { estruturaConsulta };
            return this.ChamarServico<ResultadoConsulta>(MethodBase.GetCurrentMethod(), parametros);
        }

        public ResultadoSalvar Salvar(List<Entidade> entidades)
        {
            object[] parametros = { entidades };
            return this.ChamarServico<ResultadoSalvar>(MethodBase.GetCurrentMethod(), parametros);
        }

        public ResultadoExcluir Excluir(List<Entidade> entidades, string relacoesEmCascata)
        {
            object[] parametros = { entidades, relacoesEmCascata };
            return this.ChamarServico<ResultadoExcluir>(MethodBase.GetCurrentMethod(), parametros);
        }

        public DateTime RetornarDataHora()
        {
            object[] parametros = { };
            return this.ChamarServico<DateTime>(MethodBase.GetCurrentMethod(), parametros);
        }

        //public DateTime RetornarDataHoraUTC()
        //{
        //    base.RetornarDataHoraUTC
        //    object[] parametros = { };
        //    return this.ChamarServico<DateTime>(MethodBase.GetCurrentMethod(), parametros);
        //}
        #endregion

        #region Credenciais

        //protected override CredencialServico CredencialServico
        //{
        //    get
        //    {
        //        throw new ErroNaoImplementado();
        //        //return CredencialServicoDados.ServicoDados;
        //    }
        //}

        //protected override CredencialUsuario CredencialUsuario
        //{
        //    get { return CredencialAnonimo.Anonimo; }
        //}

        #endregion
    }
}