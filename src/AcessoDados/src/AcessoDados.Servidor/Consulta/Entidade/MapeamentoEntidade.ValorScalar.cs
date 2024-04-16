using Snebur.AcessoDados.Estrutura;
using Snebur.Utilidade;
using System;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class MapeamentoEntidadeValorScalar : BaseMapeamentoEntidade
    {

        internal MapeamentoEntidadeValorScalar(BaseMapeamentoConsulta mapeamentoConsulta,
                                               EstruturaEntidade estruturaEntidade,
                                               EstruturaBancoDados estruturaBancoDados,
                                               BaseConexao conexaoDB,
                                               BaseContextoDados contexto) : base(mapeamentoConsulta,
                                                                                    estruturaEntidade,
                                                                                    estruturaBancoDados,
                                                                                    conexaoDB,
                                                                                    contexto)
        {
        }

        internal protected override string RetornarSqlCampos()
        {
            string campo = "*";

            if (!String.IsNullOrEmpty(this.EstruturaConsulta.CaminhoPropriedadeFuncao))
            {
                if (!this.TodasEstruturaCampoApelidoMapeado.ContainsKey(this.EstruturaConsulta.CaminhoPropriedadeFuncao))
                {
                    throw new Erro(String.Format(" A caminho da propriedade não foi encontrado. verificar relação aberta {0} ", this.EstruturaConsulta.CaminhoPropriedadeFuncao));
                }
                var estruturaMapeamentoCampo = this.TodasEstruturaCampoApelidoMapeado[this.EstruturaConsulta.CaminhoPropriedadeFuncao];
                campo = AjudanteSql.RetornarSqlFormatado(estruturaMapeamentoCampo.CaminhoBanco);
            }
            switch (this.EstruturaConsulta.TipoFuncaoEnum)
            {
                case (EnumTipoFuncao.Existe):

                    return String.Format(" TOP 1 (1) ", campo);

                case (EnumTipoFuncao.Contar):

                    return String.Format(" COUNT({0}) ", campo);

                case (EnumTipoFuncao.Maximo):

                    return String.Format(" MAX({0}) ", campo);

                case (EnumTipoFuncao.Minimo):

                    return String.Format(" MIN({0}) ", campo);

                case (EnumTipoFuncao.Somar):

                    return String.Format(" SUM({0}) ", campo);

                case (EnumTipoFuncao.Media):

                    return String.Format(" AVG({0}) ", campo);

                default:

                    throw new Erro(String.Format("O tipo da consulta valor scalar não suporta função {0}", EnumUtil.RetornarDescricao(this.EstruturaConsulta.TipoFuncaoEnum)));
            }
        }
    }
}