using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.Depuracao
{
    public class MensagemLog : Mensagem
    {

		#region Campos Privados

        private string _mensagem;
        private EnumTipoLog _tipoLog;

		#endregion

        public string Mensagem { get => this.RetornarValorPropriedade(this._mensagem); set => this.NotificarValorPropriedadeAlterada(this._mensagem, this._mensagem = value); }

        public EnumTipoLog TipoLog { get => this.RetornarValorPropriedade(this._tipoLog); set => this.NotificarValorPropriedadeAlterada(this._tipoLog, this._tipoLog = value); }
    }

    public enum EnumTipoLog
    {
        Normal,
        Alerta,
        Erro,
        Sucesso,
        Acao 
    }
}