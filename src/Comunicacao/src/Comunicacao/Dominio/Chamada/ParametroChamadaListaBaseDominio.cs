﻿using Snebur.Dominio;
using System.Collections.Generic;

namespace Snebur.Comunicacao
{
    public class ParametroChamadaListaBaseDominio : ParametroChamadaLista
    {
        #region Campos Privados

        private string _nomeTipoBaseDominio;
        private string _nomeNamespaceTipoBaseDominio;

        #endregion

        public string NomeTipoBaseDominio { get => this.RetornarValorPropriedade(this._nomeTipoBaseDominio); set => this.NotificarValorPropriedadeAlterada(this._nomeTipoBaseDominio, this._nomeTipoBaseDominio = value); }

        public string NomeNamespaceTipoBaseDominio { get => this.RetornarValorPropriedade(this._nomeNamespaceTipoBaseDominio); set => this.NotificarValorPropriedadeAlterada(this._nomeNamespaceTipoBaseDominio, this._nomeNamespaceTipoBaseDominio = value); }

        public List<BaseDominio> BasesDominio { get; set; }
    }
}