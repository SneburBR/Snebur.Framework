using Snebur.AcessoDados.Estrutura;
using Snebur.Utilidade;
using System;
using System.Data;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class ParametroCampo
    {
        internal EstruturaCampo EstruturaCampo { get; }

        internal SqlDbType Tipo { get; }

        internal string Nome { get; }

        internal object Valor { get; }

        internal string NomeParametro { get; }

        internal ParametroCampo(EstruturaCampo estruturaCampo, object valor)
        {
            this.EstruturaCampo = estruturaCampo;
            this.Nome = this.EstruturaCampo.NomeParametro;
            this.Tipo = this.EstruturaCampo.TipoSql;
            this.Valor = this.RetornarValor(valor);

            if (!estruturaCampo.IsAceitaNulo && valor == null)
            {
                var mensagem = $" O parâmetro {this.EstruturaCampo.NomeParametro} em {this.EstruturaCampo.EstruturaEntidade.NomeTabela} não aceita valor nulo";
                throw new ErroParametro(mensagem);
            }
        }

        private object RetornarValor(object valor)
        {
            if (valor == null)
            {
                return DBNull.Value;
            }
            else
            {
                if (this.EstruturaCampo.IsFormatarSomenteNumero)
                {
                    return TextoUtil.RetornarSomenteNumeros(valor.ToString());
                }
                return valor;
            }
        }
    }
}