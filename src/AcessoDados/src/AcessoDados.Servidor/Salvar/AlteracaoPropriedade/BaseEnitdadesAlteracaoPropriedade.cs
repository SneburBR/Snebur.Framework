using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class BaseEnitdadesAlteracaoPropriedade
    {
        protected readonly BaseContextoDados Contexto;

        public BaseEnitdadesAlteracaoPropriedade(BaseContextoDados contexto)
        {
            this.Contexto = contexto;
        }

        protected BaseConexao Conexao => this.Contexto.Conexao;

        protected (bool IsExisteAlteracao, object valorAntigo) IsExisteAlteracaoPropriedade(Entidade entidade,
                                                                                           IEstruturaAlteracaoPropriedade estruturaAlteracaoPropriedade)
        {
            if (entidade.Id == 0)
            {
                return (estruturaAlteracaoPropriedade.IsNotificarNovoCadastro, null);
            }

            var propriedadesAlteradas = entidade.__PropriedadesAlteradas;
            if (propriedadesAlteradas?.Count > 0)
            {
                if (estruturaAlteracaoPropriedade.IsTipoComplexo)
                {
                    //var campos = estruturaAlteracaoPropriedade.EstruturaTipoComplexo.EstruturasCampo.Values.Select(x => x.NomeCampo);


                    var tupleEstruturaPropriedadeAlterada = estruturaAlteracaoPropriedade.EstruturaTipoComplexo.
                                                                         EstruturasCampo.Values.
                                                                         Where(x => propriedadesAlteradas.ContainsKey(x.NomeCampo)).
                                                                         Select(x => (x, propriedadesAlteradas[x.NomeCampo])).
                                                                         ToList();


                    if (tupleEstruturaPropriedadeAlterada.Count > 0)
                    {
                        return this.IsExisteAlteracaoTipoComplexo(entidade,
                                                                  estruturaAlteracaoPropriedade,
                                                                  tupleEstruturaPropriedadeAlterada);

                    }

                }
                else
                {

                    if (propriedadesAlteradas.TryGetValue(estruturaAlteracaoPropriedade.Propriedade.Name,
                                                          out var propriedadeAlterada))
                    {

                        return this.IsExisteAlteracaoTipoPrimario(entidade,
                                                                  estruturaAlteracaoPropriedade,
                                                                  propriedadeAlterada);
                    }
                }
            }
            return (false, null);
        }

        protected (bool IsExisteAlteracao, object valorAntigo) IsExisteAlteracaoTipoComplexo(Entidade entidade,
                                                                                            IEstruturaAlteracaoPropriedade estruturaAlteracaoPropriedade,
                                                                                            List<(EstruturaCampo, PropriedadeAlterada)> estruturasCampos)
        {
            if (estruturaAlteracaoPropriedade.IsVerificarAlteracaoBanco)
            {
                return this.IsExisteAlteracaoTipoComplexoBanco(entidade, estruturaAlteracaoPropriedade);
            }

            var estruturaTipoComplexo = estruturaAlteracaoPropriedade.EstruturaTipoComplexo;
            var novoValorTipoComplexo = (BaseTipoComplexo)estruturaTipoComplexo.Propriedade.GetValue(entidade);
            var valorAntigo = novoValorTipoComplexo.Clone();

            foreach (var (estruturaCampo, propriedadeAlterada) in estruturasCampos)
            {
                var valorTipado = ConverterUtil.Converter(propriedadeAlterada.AntigoValor, estruturaCampo.Propriedade.PropertyType);
                estruturaCampo.Propriedade.SetValue(valorAntigo, valorTipado);
            }

            var isExisteAlteracao = !novoValorTipoComplexo.Equals(valorAntigo);
            return (isExisteAlteracao, valorAntigo);
        }

        private (bool IsExisteAlteracao, object valorAntigo) IsExisteAlteracaoTipoComplexoBanco(Entidade entidade,
                                                                                                IEstruturaAlteracaoPropriedade estruturaAlteracaoPropriedade)
        {
            var estruturaEntidade = estruturaAlteracaoPropriedade.EstruturaEntidade;
            var estruturaTipoComplexo = estruturaAlteracaoPropriedade.EstruturaTipoComplexo;
            var campos = estruturaTipoComplexo.EstruturasCampo.Select(x => x.Value.NomeCampoSensivel);
            var sqlCampos = String.Join(", ", campos);

            var sqlValorAtual = new StringBuilderSql();
            sqlValorAtual.AppendFormat(" SELECT [Id], {0} FROM [{1}].[{2}] WHERE [{3}] = {4}", sqlCampos,
                                                                                               estruturaEntidade.Schema,
                                                                                               estruturaEntidade.NomeTabela,
                                                                                               estruturaEntidade.EstruturaCampoChavePrimaria.NomeCampo,
                                                                                               entidade.Id);

            var dataTable = this.Conexao.RetornarDataTable(sqlValorAtual.ToString(), null);
            var valorAtualTipoComplexo = this.RetornarValorTipoComplexo(estruturaTipoComplexo, dataTable);
            var novoValorTipoComplexo = (BaseTipoComplexo)estruturaTipoComplexo.Propriedade.GetValue(entidade);
            var isExisteAlteradao = !Util.SaoIgual(valorAtualTipoComplexo, novoValorTipoComplexo);
            return (isExisteAlteradao, valorAtualTipoComplexo);
        }

        private BaseTipoComplexo RetornarValorTipoComplexo(EstruturaTipoComplexo estruturaTipoComplexo,
                                                           DataTable dataTable)
        {
            var tipoComplexo = (BaseTipoComplexo)Activator.CreateInstance(estruturaTipoComplexo.Tipo);
            if (dataTable.Rows.Count == 1)
            {
                var linha = dataTable.Rows[0];
                foreach (var estrturaCampo in estruturaTipoComplexo.EstruturasCampo.Values)
                {
                    if (dataTable.Columns.Contains(estrturaCampo.NomeCampo))
                    {
                        var valorProprieade = linha[estrturaCampo.NomeCampo];
                        var valorPrpriedadeTipado = ConverterUtil.Converter(valorProprieade, estrturaCampo.Propriedade.PropertyType);
                        estrturaCampo.Propriedade.SetValue(tipoComplexo, valorPrpriedadeTipado);
                    }
                }
            }
            return tipoComplexo;
        }

        private (bool IsExisteAlteracao, object valorAntigo) IsExisteAlteracaoTipoPrimario(Entidade entidade,
                                                                                           IEstruturaAlteracaoPropriedade estruturaAlteracaoPropriedade,
                                                                                           PropriedadeAlterada propriedadeAlterada)
        {
            if (estruturaAlteracaoPropriedade.IsVerificarAlteracaoBanco ||
                propriedadeAlterada.AntigoValor == null)
            {
                return this.IsExisteAlteracaoTipoPrimarioBanco(entidade,
                                                               estruturaAlteracaoPropriedade);

            }
            var novoValor = estruturaAlteracaoPropriedade.Propriedade.GetValue(entidade);
            var valorAntigoTipado = ConverterUtil.Converter(propriedadeAlterada.AntigoValor,
                                                            estruturaAlteracaoPropriedade.Propriedade.PropertyType);

            var isExisteAlteracao = Util.SaoIgual(novoValor, valorAntigoTipado) == false;
            return (isExisteAlteracao, valorAntigoTipado);
        }

        private (bool IsExisteAlteracao, object valorAntigo) IsExisteAlteracaoTipoPrimarioBanco(Entidade entidade,
                                                                                                IEstruturaAlteracaoPropriedade estruturaAlteracaoPropriedade)
        {
            var estruturaEntidade = estruturaAlteracaoPropriedade.EstruturaEntidade;
            var estruturaCampo = estruturaAlteracaoPropriedade.EstruturaCampo;
            var sqlValorAtual = new StringBuilderSql();
            sqlValorAtual.AppendFormat(" SELECT [{0}] FROM [{1}].[{2}] WHERE [{3}] = {4}", estruturaCampo.NomeCampo, estruturaEntidade.Schema,
                                                                                           estruturaEntidade.NomeTabela, estruturaEntidade.EstruturaCampoChavePrimaria.NomeCampo,
                                                                                           entidade.Id);

            var valorBancoAtual = this.Conexao.RetornarValorScalar(sqlValorAtual.ToString(), null);
            var valorBancoAtualTipado = ConverterUtil.Converter(valorBancoAtual, estruturaCampo.Propriedade.PropertyType);
            var novoValor = estruturaCampo.Propriedade.GetValue(entidade);
            var isExisteAlteracao = this.IsExisteAlteracaoPropriedade(estruturaCampo, novoValor, valorBancoAtualTipado);
            return (isExisteAlteracao, valorBancoAtualTipado);
        }

        private bool IsExisteAlteracaoPropriedade(EstruturaCampo estruturaCampo, object novoValor, object valorBancoAtualTipado)
        {
            if (estruturaCampo.Propriedade.PropertyType.IsEnum)
            {
                return Convert.ToInt32(novoValor) != Convert.ToInt32(valorBancoAtualTipado);
            }
            else
            {
                return !Util.SaoIgual(novoValor, valorBancoAtualTipado);
            }
        }
    }
}
