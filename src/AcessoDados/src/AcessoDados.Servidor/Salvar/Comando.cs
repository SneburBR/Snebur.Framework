using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal abstract class Comando : IDisposable
    {
        internal EntidadeAlterada EntidadeAlterada { get; }
        internal EstruturaEntidade EstruturaEntidade { get; }
        internal DbCommand DbCommand { get; }
        internal Entidade Entidade { get; }
        internal string SqlCommando { get; set; }
        internal List<EstruturaCampo> EstruturasCampoParametro { get; } = new List<EstruturaCampo>();

        internal Comando(EntidadeAlterada entidadeAlterada, EstruturaEntidade estruturaEntidade)
        {
            this.EntidadeAlterada = entidadeAlterada;
            this.Entidade = this.EntidadeAlterada.Entidade;
            this.EstruturaEntidade = estruturaEntidade;
        }

        internal List<ParametroCampo> RetornarParametros()
        {
            var parametros = new List<ParametroCampo>();
            foreach (var estruturaCampo in this.EstruturasCampoParametro)
            {
                var valor = this.RetornarValorCampo(estruturaCampo);
                var pametro = new ParametroCampo(estruturaCampo, valor);
                parametros.Add(pametro);
            }
            return parametros;
        }

        private object RetornarValorCampo(EstruturaCampo estruturaCampo)
        {
            if (estruturaCampo.IsRelacaoChaveEstrangeira)
            {
                var idRelacaoChaveEstrangeira = this.RetornarValorIdRelacaoChaveEstrangeira(estruturaCampo);
                if (idRelacaoChaveEstrangeira == 0)
                {
                    return null;
                }
                return idRelacaoChaveEstrangeira;
            }
            var valorProprieade = this.RetornarValorCampoPrimario(estruturaCampo);
            if (valorProprieade is Double valorTipado)
            {
                if (Double.IsNaN(valorTipado))
                {
                    return 0d;
                }
            }
            return this.RetornarValorCampoPrimario(estruturaCampo);
        }

        private long RetornarValorIdRelacaoChaveEstrangeira(EstruturaCampo estruturaCampo)
        {
            var entidadeRelacao = (Entidade)estruturaCampo.EstruturaRelacaoChaveEstrangeira.Propriedade.GetValue(this.Entidade);
            if (entidadeRelacao != null)
            {
                return entidadeRelacao.Id;
            }
            return Convert.ToInt64(this.RetornarValorCampoPrimario(estruturaCampo));

        }

        private object RetornarValorCampoPrimario(EstruturaCampo estruturaCampo)
        {
            if (estruturaCampo.IsTipoComplexo)
            {
                var propriedadeTipoComplexo = estruturaCampo.PropriedadeTipoComplexo;
                var valorTipoComplexao = ReflexaoUtil.RetornarValorPropriedade(this.Entidade, propriedadeTipoComplexo);
                if (valorTipoComplexao == null)
                {
                    throw new ErroNaoDefinido(String.Format("O valor do tipo complexo não pode ser nulo {0}.{1} ", propriedadeTipoComplexo.DeclaringType.Name, propriedadeTipoComplexo.Name));
                }
                var valorPropriedade = ReflexaoUtil.RetornarValorPropriedade(valorTipoComplexao, estruturaCampo.Propriedade);
                if (estruturaCampo.Tipo.IsEnum)
                {
                    return Convert.ToInt32(valorPropriedade);
                }
                return valorPropriedade;
            }

            if (estruturaCampo.Tipo.IsEnum)
            {
                var valorPropriedade = ReflexaoUtil.RetornarValorPropriedade(this.Entidade, estruturaCampo.Propriedade);
                return Convert.ToInt32(valorPropriedade);
            }
            return ReflexaoUtil.RetornarValorPropriedade(this.Entidade, estruturaCampo.Propriedade);
        }

        public override string ToString()
        {
            return this.SqlCommando;
        }

        #region IDisposable 

        public void Dispose()
        {
        }
        #endregion
    }
}