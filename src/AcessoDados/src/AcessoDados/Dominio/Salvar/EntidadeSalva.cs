using System;
using System.Collections.Generic;

namespace Snebur.AcessoDados
{
    public class EntidadeSalva : BaseAcessoDados
    {

        #region Campos Privados

        private long _id;
        private Guid _identificadorUnicoEntidade;
        private string? _caminhoTipoEntidadeSalva;

        #endregion

        public long Id { get => this.RetornarValorPropriedade(this._id); set => this.NotificarValorPropriedadeAlterada(this._id, this._id = value); }

        public Guid IdentificadorUnicoEntidade { get => this.RetornarValorPropriedade(this._identificadorUnicoEntidade); set => this.NotificarValorPropriedadeAlterada(this._identificadorUnicoEntidade, this._identificadorUnicoEntidade = value); }

        public string? CaminhoTipoEntidadeSalva { get => this.RetornarValorPropriedade(this._caminhoTipoEntidadeSalva); set => this.NotificarValorPropriedadeAlterada(this._caminhoTipoEntidadeSalva, this._caminhoTipoEntidadeSalva = value); }

        public List<PropriedadeComputada> PropriedadesComputada { get; set; } = new List<PropriedadeComputada>();
    }
    //Public Class PropriedadeComputada
    //      Inherits BaseTipoServico

    //    Property Nome As String

    //    Property Valor As Object
    //End Class
}