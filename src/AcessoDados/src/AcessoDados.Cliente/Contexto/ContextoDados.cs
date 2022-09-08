using Snebur.AcessoDados.Cliente;
using Snebur.Comunicacao;
using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.AcessoDados
{
    public abstract class BaseContextoDados : __BaseContextoDados, IBaseServico, IServicoDados
    {
        private BaseServicoDadosCliente ServicoDados { get; }

        public BaseContextoDados() : base()
        {
            this.ServicoDados = this.RetornarServicoDadosCliente();
        }

        #region IServicoDados

        public override object RetornarValorScalar(EstruturaConsulta estruturaConsulta)
        {
            return this.ServicoDados.RetornarValorScalar(estruturaConsulta);
        }

        public override ResultadoConsulta RetornarResultadoConsulta(EstruturaConsulta estruturaConsulta)
        {
            var resultado = this.ServicoDados.RetornarResultadoConsulta(estruturaConsulta);
            var entidades = resultado.Entidades.Cast<Entidade>().ToList();
            this.NormalizarEntidadesConsulta(entidades);
            return resultado;
        }

        public ResultadoSalvar Salvar(params Entidade[] entidades)
        {
            var lista = new List<Entidade>();
            lista.AddRange(entidades);
            return this.Salvar(lista);
        }

        public ResultadoSalvar Salvar(ListaEntidades<Entidade> entidades)
        {
            var lista = new List<Entidade>();
            lista.AddRange(entidades);
            return this.Salvar(lista);
        }

        public ResultadoExcluir Excluir(params Entidade[] entidades)
        {
            var lista = new List<Entidade>();
            lista.AddRange(entidades);
            return this.Excluir(lista, String.Empty);
        }

        public ResultadoExcluir Exclur(ListaEntidades<Entidade> entidades)
        {
            var lista = new List<Entidade>();
            lista.AddRange(entidades);
            return this.Excluir(lista, String.Empty);
        }

        public override ResultadoSalvar Salvar(Entidade entidade)
        {
            return this.Salvar(new List<Entidade> { entidade });
        }

        public override ResultadoSalvar Salvar(List<Entidade> entidades)
        {
            var resultadoSalvar = this.ServicoDados.Salvar(entidades);
            if (!resultadoSalvar.IsSucesso)
            {
                var descricaoEntidades = String.Join(",", entidades.Select(x => $"{x.GetType().Name}({x.Id})"));
                throw new Erro($"Não foi possivel salvar as entidade {descricaoEntidades}\r\n{resultadoSalvar.MensagemErro}");
            }
            this.NormailizarEntidadeSalvar(resultadoSalvar, entidades);
            return resultadoSalvar;
        }

        public override ResultadoExcluir Excluir(Entidade entidade)
        {
            return this.Excluir(new List<Entidade> { entidade });
        }

        public override ResultadoExcluir Excluir(Entidade entidade, string relacoesEmCascata)
        {
            return this.Excluir(new List<Entidade> { entidade }, String.Empty);
        }

        public override ResultadoExcluir Excluir(List<Entidade> entidades)
        {
            return this.Excluir(entidades, String.Empty);
        }

        public override ResultadoExcluir Excluir(List<Entidade> entidades, string relacoesEmCascata)
        {
            return this.ServicoDados.Excluir(entidades, relacoesEmCascata);
        }



        //public override DateTime RetornarDataHora(bool utc = true)
        //{
        //    return this.ServicoDados.RetornarDataHora(utc);
        //}

        #endregion

        #region  Normalizar entidades Salvar

        private void NormailizarEntidadeSalvar(ResultadoSalvar resultadoSalvar, List<Entidade> entidades)
        {
            var todasEntidades = this.RetornarTodasEntidadesSalvas(entidades);
            foreach (var entidadeSalva in resultadoSalvar.EntidadesSalvas)
            {
                if (todasEntidades.ContainsKey(entidadeSalva.IdentificadorUnicoEntidade))
                {
                    var entidade = todasEntidades[entidadeSalva.IdentificadorUnicoEntidade];
                    if (entidade.Id == 0)
                    {
                        entidade.Id = entidadeSalva.Id;
                        foreach (var propriedadeComputada in entidadeSalva.PropriedadesComputada)
                        {
                            var propriedade = entidade.GetType().GetProperty(propriedadeComputada.NomePropriedade);
                            propriedade.SetValue(entidade, propriedadeComputada.Valor);
                        }
                        entidade.AtivarControladorPropriedadeAlterada();
                    }
                }
            }
        }

        private void NormalizarEntidadesSalvar(Entidade entidade, Dictionary<Guid, Entidade> entidadesEncontradas)
        {
            if (entidade == null) return;
            if (entidadesEncontradas.ContainsKey(entidade.RetornarIdentificadorReferencia()))
            {
                return;
            }
            else
            {
                entidadesEncontradas.Add(entidade.RetornarIdentificadorReferencia(), entidade);
            }
            var propriedades = ReflexaoUtil.RetornarPropriedades(entidade.GetType());
            foreach (var propriedade in propriedades)
            {
                if (ReflexaoUtil.TipoRetornaColecaoEntidade(propriedade.PropertyType))
                {
                    var colecao = (IEnumerable)propriedade.GetValue(entidade);
                    if (colecao != null)
                    {
                        foreach (var itemEntidade in colecao)
                        {
                            this.NormalizarEntidadesSalvar(itemEntidade as Entidade, entidadesEncontradas);
                        }
                    }
                }
                if (ReflexaoUtil.IsTipoEntidade(propriedade.PropertyType))
                {
                    var entidadeRelacao = (Entidade)propriedade.GetValue(entidade);
                    this.NormalizarEntidadesSalvar(entidadeRelacao, entidadesEncontradas);
                }
            }
        }

        private Dictionary<Guid, Entidade> RetornarTodasEntidadesSalvas(List<Entidade> entidades)
        {
            var todasEntidade = new Dictionary<Guid, Entidade>();
            foreach (var entidade in entidades)
            {
                this.NormalizarEntidadesSalvar(entidade, todasEntidade);
            }
            return todasEntidade;
        }

        #endregion

        #region Normalizar consulta

        private void NormalizarEntidadesConsulta(List<Entidade> entidades)
        {
            var todasEntidades = this.RetornarTodasEntidadesConsulta(entidades);
            foreach (var entidade in todasEntidades.Values)
            {
                entidade.AtivarControladorPropriedadeAlterada();
            }
        }

        private Dictionary<Guid, Entidade> RetornarTodasEntidadesConsulta(List<Entidade> entidades)
        {
            var todasEntidade = new Dictionary<Guid, Entidade>();
            foreach (var entidade in entidades)
            {
                this.NormalizarEntidadesConsulta(entidade, todasEntidade);
            }
            return todasEntidade;
        }

        private void NormalizarEntidadesConsulta(Entidade entidade, Dictionary<Guid, Entidade> entidadesEncontradas)
        {
            if (entidade == null) return;
            if (entidadesEncontradas.ContainsKey(entidade.RetornarIdentificadorReferencia()))
            {
                return;
            }
            else
            {
                entidadesEncontradas.Add(entidade.RetornarIdentificadorReferencia(), entidade);
            }
            var propriedades = ReflexaoUtil.RetornarPropriedades(entidade.GetType());
            foreach (var propriedade in propriedades)
            {
                if (ReflexaoUtil.TipoRetornaColecaoEntidade(propriedade.PropertyType))
                {
                    var colecao = (IEnumerable)propriedade.GetValue(entidade);
                    if (colecao != null)
                    {
                        foreach (var itemEntidade in colecao)
                        {
                            this.NormalizarEntidadesConsulta(itemEntidade as Entidade, entidadesEncontradas);
                        }
                    }
                }
                if (ReflexaoUtil.IsTipoEntidade(propriedade.PropertyType))
                {
                    var entidadeRelacao = (Entidade)propriedade.GetValue(entidade);
                    this.NormalizarEntidadesConsulta(entidadeRelacao, entidadesEncontradas);
                }
            }
        }

        #endregion

        #region IBaseServico

        public override bool Ping()
        {
            return this.ServicoDados.Ping();
        }

        public override DateTime RetornarDataHora()
        {
            return this.ServicoDados.RetornarDataHora();
        }

        public override DateTime RetornarDataHoraUTC()
        {
            return this.ServicoDados.RetornarDataHoraUTC();
        }
        #endregion

        protected abstract BaseServicoDadosCliente RetornarServicoDadosCliente();
    }
}