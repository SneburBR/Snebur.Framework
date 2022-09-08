﻿using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Snebur.AcessoDados
{
    public abstract partial class __BaseContextoDados : IContextoDados
    {
        internal protected List<PropertyInfo> PropriedadesConsultaEntidade { get; set; }

        public bool IsValidarNomeTabelaEntidade { get; set; } = true;

        public IUsuario UsuarioLogado => this.RetornarUsuarioLogado();

        public bool IsDispensado { get; private set; }

        protected __BaseContextoDados()
        {
            this.PropriedadesConsultaEntidade = this.RetornarPropriedadesIConsultaEntidade();
            this.InicializarPropriedadesConsulta();
        }

        internal Type RetornarTipoEntidadeImplementaInterafe(Type tipoInterface)
        {
            var assemblyEntidades = this.PropriedadesConsultaEntidade.First().
                                         PropertyType.GetGenericArguments().Single().Assembly;

            return assemblyEntidades.GetAccessibleTypes().Where(x => ReflexaoUtil.TipoImplementaInterface(x, tipoInterface, true)).Single();
        }

        public abstract object RetornarValorScalar(EstruturaConsulta estruturaConsulta);

        #region Métodos públicos

        public T RetornarValorScalar<T>(EstruturaConsulta estruturaConsulta)
        {
            var valorScalar = this.RetornarValorScalar(estruturaConsulta);
            return ConverterUtil.Converter<T>(valorScalar);
        }

        public IConsultaEntidade<TEntidade> RetornarConsulta<TEntidade>() where TEntidade : IEntidade
        {
            return new ConsultaEntidade<TEntidade>(this, typeof(TEntidade));
        }

        public IConsultaEntidade<TEntidade> RetornarConsulta<TEntidade>(Type tipoConsulta) where TEntidade : IEntidade
        {
            return new ConsultaEntidade<TEntidade>(this, tipoConsulta);
            //return new ConstrutorConsultaEntidade<TEntidade>(this, tipoConsulta);
        }

        public EstruturaConsulta RetornarEstruturaConsulta<TEntidade>() where TEntidade : IEntidade
        {
            return new ConsultaEntidade<TEntidade>(this, typeof(TEntidade)).RetornarEstruturaConsulta();
        }

        public EstruturaConsulta RetornarEstruturaConsulta(Type tipoEntidade)
        {
            if (!tipoEntidade.IsSubclassOf(typeof(Entidade)))
            {
                throw new ErroNaoSuportado(String.Format("O tipo de entidade não é suportado {0}", tipoEntidade.Name));
            }
            var tipoConstrutorConsutla = typeof(ConstrutorConsultaEntidade<>);
            var tipoTipado = tipoConstrutorConsutla.MakeGenericType(tipoEntidade);
            object[] parametros = { this };
            object[] atributos = { };
            var construtor = Activator.CreateInstance(tipoTipado, parametros, atributos);
            return ((IConsultaEntidade)construtor).RetornarEstruturaConsulta();
        }
        #endregion

        #region Métodos privados

        private void InicializarPropriedadesConsulta()
        {
            object[] construtor = { this };
            var tipoConstrutorConsultaEntidade = typeof(ConstrutorConsultaEntidade<>);

            foreach (PropertyInfo propriedade in this.PropriedadesConsultaEntidade)
            {
                var tipoEntidade = propriedade.PropertyType.GetGenericArguments().Single();
                var tipoConstrutorConsultaEntidadeTipado = tipoConstrutorConsultaEntidade.MakeGenericType(tipoEntidade);

                ConstrutorConsultaEntidade consultaEntidade = (ConstrutorConsultaEntidade)Activator.CreateInstance(tipoConstrutorConsultaEntidadeTipado, construtor);
                propriedade.SetValue(this, consultaEntidade);
            }
        }
        #endregion

        #region Métodos protegidos

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected List<PropertyInfo> RetornarPropriedadesIConsultaEntidade()
        {
            return ContextoDadosUtil.RetornarPropriedadesIConsultaEntidade(this.GetType());
        }
        #endregion

        #region Métodos abstratos

        public abstract ResultadoConsulta RetornarResultadoConsulta(EstruturaConsulta estruturaConsulta);

        public abstract IUsuario RetornarUsuarioLogado();

        #endregion

        #region IContextoDados

        public abstract ResultadoSalvar Salvar(Entidade entidade);

        public abstract ResultadoSalvar Salvar(List<Entidade> entidades);

        public abstract ResultadoExcluir Excluir(Entidade entidade);

        public abstract ResultadoExcluir Excluir(List<Entidade> entidades);

        public abstract ResultadoExcluir Excluir(Entidade entidade, string relacoesEmCascata);

        public abstract ResultadoExcluir Excluir(List<Entidade> entidades, string relacoesEmCascata);

        public abstract DateTime RetornarDataHora();

        public abstract DateTime RetornarDataHoraUTC();

        public abstract bool Ping();

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            if (!this.IsDispensado)
            {
                var propriedadesConsultaEntidade = this.RetornarPropriedadesIConsultaEntidade();
                object[] construtor = { this };
                foreach (PropertyInfo propriedade in propriedadesConsultaEntidade)
                {
                    propriedade.SetValue(this, null);
                }
                this.DisposeInterno();

            }
            this.IsDispensado = true;
        }

        protected virtual void DisposeInterno()
        {

        }


        #endregion

    }
}