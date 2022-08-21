using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Serializacao;
using Snebur.Utilidade;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal partial class SalvarEntidades
    {
        #region Alterações da propriedades - IAlteracaoPropriedade NotificarAlteracaoPropriedadeAttribute

        private HashSet<Entidade> RetornarEntidadesAlteracaoPropriedade(HashSet<Entidade> entidades)
        {
            var alteracoesPropriedade = new HashSet<Entidade>();
            foreach (var entidade in entidades)
            {
                //entidade.__PropriedadesAlteradas
                var estruturaEntidade = this.Contexto.EstruturaBancoDados.EstruturasEntidade[entidade.GetType().Name];
                foreach (var estruturaAlteracaoPropriedade in estruturaEntidade.RetornarTodasEstruturasAlteracaoPropriedade())
                {
                    var (isExisteAltracao, valorAtual) = this.IsExisteAlteracaoPropriedade(entidade, estruturaAlteracaoPropriedade);
                    if (isExisteAltracao)
                    {

                        var valorPropriedade = estruturaAlteracaoPropriedade.Propriedade.GetValue(entidade);
                        var atributo = estruturaAlteracaoPropriedade.Atributo;

                        var ultimaAlteracao = this.RetornarUtlimaAlteracao(entidade, atributo);
                        if (ultimaAlteracao != null)
                        {
                            ultimaAlteracao.DataHoraFimAlteracao = DateTime.UtcNow;
                            alteracoesPropriedade.Add((Entidade)ultimaAlteracao);
                        }

                        var novaAlteracao = (IAlteracaoPropriedade)Activator.CreateInstance(atributo.TipoEntidadeAlteracaoPropriedade);
                        novaAlteracao.ValorPropriedadeRelacao = entidade;
                        novaAlteracao.ValorPropriedadeAlterada = valorPropriedade;
                        novaAlteracao.ValorPropriedadeAntigo = valorAtual;

                        novaAlteracao.Usuario = this.Contexto.UsuarioLogado;
                        novaAlteracao.SessaoUsuario = this.Contexto.SessaoUsuarioLogado;

                        alteracoesPropriedade.Add((Entidade)novaAlteracao);
                    }
                }
            }
            return alteracoesPropriedade;
        }

        private IAlteracaoPropriedade RetornarUtlimaAlteracao(Entidade entidade, NotificarAlteracaoPropriedadeAttribute atributo)
        {
            var tipoAlteracao = atributo.TipoEntidadeAlteracaoPropriedade;
            var propriedadeRelacao = atributo.PropriedadeRelacao;
            var consulta = this.Contexto.RetornarConsulta<IAlteracaoPropriedade>(tipoAlteracao);
            var propriedadeChaveEstrangeiraRelacao = EntidadeUtil.RetornarPropriedadeChaveEstrangeira(tipoAlteracao, propriedadeRelacao);
            consulta.AdicionarFiltroPropriedade(propriedadeChaveEstrangeiraRelacao, EnumOperadorFiltro.Igual, entidade.Id);
            consulta = consulta.OrderByDescending(x => x.Id);
            var ultimaAlteracao = consulta.FirstOrDefault();
            return ultimaAlteracao;
        }

        #region Notificação de alteração de propriedade

        private (bool IsExisteAlteracao, object valorAntigo) IsExisteAlteracaoPropriedade(Entidade entidade, EstruturaAlteracaoPropriedade estruturaAlteracaoPropriedade)
        {
            if (entidade.Id == 0)
            {
                return (estruturaAlteracaoPropriedade.Atributo.IsNotificarNovoCadastro, null);
            }

            if (entidade.__PropriedadesAlteradas?.Count > 0)
            {
                if (estruturaAlteracaoPropriedade.IsTipoComplexo)
                {
                    var campos = estruturaAlteracaoPropriedade.EstruturaTipoComplexo.EstruturasCampo.Values.Select(x => x.NomeCampo);
                    if (campos.Any(x => entidade.__PropriedadesAlteradas.ContainsKey(x)))
                    {
                        return this.IsExisteAlteracaoTipoComplexo(entidade, estruturaAlteracaoPropriedade);
                    }
                }
                else
                {
                    if (entidade.__PropriedadesAlteradas.ContainsKey(estruturaAlteracaoPropriedade.Propriedade.Name))
                    {
                        return this.IsExisteAlteracaoTipoPrimario(entidade, estruturaAlteracaoPropriedade);
                    }
                }
            }
            return (false, null);
        }

        private (bool IsExisteAlteracao, object valorAntigo) IsExisteAlteracaoTipoComplexo(Entidade entidade,
                                                                                           EstruturaAlteracaoPropriedade estruturaAlteracaoPropriedade)
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
            var valorAtualTipoComplexo = this.RetornarValorTipoComplexoAtual(estruturaTipoComplexo, dataTable);
            var novoValorTipoComplexo = (BaseTipoComplexo)estruturaTipoComplexo.Propriedade.GetValue(entidade);
            var isExisteAlteradao = !Util.SaoIgual(valorAtualTipoComplexo, novoValorTipoComplexo);
            return (isExisteAlteradao, valorAtualTipoComplexo);
        }

        private BaseTipoComplexo RetornarValorTipoComplexoAtual(EstruturaTipoComplexo estruturaTipoComplexo, DataTable dataTable)
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
                                                                                           EstruturaAlteracaoPropriedade estruturaAlteracaoPropriedade)
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

        #endregion

        #endregion

        #region   Alterações da propriedades genéricas- IAlteracaoPropriedade NotificarAlteracaoPropriedadeAttribute

        private HashSet<Entidade> RetornarEntidadesAlteracaoPropriedadeGenericas(HashSet<Entidade> entidades)
        {
            var alteracoesPropriedade = new  HashSet<Entidade>();
            foreach (var entidade in entidades)
            {
                var estruturaEntidade = this.Contexto.EstruturaBancoDados.EstruturasEntidade[entidade.GetType().Name];
                foreach (var estruturaAlteracaoPropriedade in estruturaEntidade.RetornarTodasEstruturasAlteracaoPropriedadeGenerica())
                {
                    var (isExisteAltracao, valorAtual) = this.IsExisteAlteracaoPropriedadeGerencia(entidade, estruturaAlteracaoPropriedade);
                    if (isExisteAltracao)
                    {

                        var valorPropriedade = estruturaAlteracaoPropriedade.Propriedade.GetValue(entidade);
                        //var atributo = estruturaAlteracaoPropriedade.Atributo;

                        var ultimaAlteracao = this.RetornarUtlimaAlteracaoGenerica(entidade, estruturaEntidade, estruturaAlteracaoPropriedade);
                        if (ultimaAlteracao != null)
                        {
                            ultimaAlteracao.DataHoraFimAlteracao = DateTime.UtcNow;
                            alteracoesPropriedade.Add((Entidade)ultimaAlteracao);
                        }

                        var novaAlteracao = (IAlteracaoPropriedadeGenerica)Activator.CreateInstance(EstruturaBancoDados.Atual.TipoEntidadeNotificaoPropriedadeAlteradaGenerica);
                        novaAlteracao.IdEntidade = entidade.Id;

                        novaAlteracao.ValorPropriedadeAlterada = SerializacaoUtil.SerializarTipoSimples(valorPropriedade);
                        novaAlteracao.ValorPropriedadeAntigo = SerializacaoUtil.SerializarTipoSimples(valorAtual);

                        novaAlteracao.NomeTipoEntidade = estruturaEntidade.NomeTipoEntidade;
                        novaAlteracao.NomePropriedade = estruturaAlteracaoPropriedade.Propriedade.Name;
                        novaAlteracao.TipoPrimario = estruturaAlteracaoPropriedade.EstruturaCampo.TipoPrimarioEnum;
                        novaAlteracao.Usuario = this.Contexto.UsuarioLogado;
                        novaAlteracao.SessaoUsuario = this.Contexto.SessaoUsuarioLogado;

                        alteracoesPropriedade.Add((Entidade)novaAlteracao);
                    }
                }
            }
            return alteracoesPropriedade;
        }

        private IAlteracaoPropriedadeGenerica RetornarUtlimaAlteracaoGenerica(Entidade entidade, 
                                                                              EstruturaEntidade estruturaEntidade,
                                                                              EstruturaAlteracaoPropriedadeGenerica estruturaAlteracaoPropriedade)
        {
            var tipoAlteracao = EstruturaBancoDados.Atual.TipoEntidadeNotificaoPropriedadeAlteradaGenerica;
            var consulta = this.Contexto.RetornarConsulta<IAlteracaoPropriedadeGenerica>(tipoAlteracao);
            consulta = consulta.Where(x => x.IdEntidade == entidade.Id &&
                                           x.NomeTipoEntidade == estruturaEntidade.NomeTipoEntidade &&
                                           x.NomePropriedade == estruturaAlteracaoPropriedade.Propriedade.Name).
                                           OrderByDescending(x => x.Id);

            return consulta.FirstOrDefault();
        }

        private (bool IsExisteAlteracao, string valorAntigo) IsExisteAlteracaoPropriedadeGerencia(Entidade entidade,
                                                                                                  EstruturaAlteracaoPropriedadeGenerica estruturaAlteracaoPropriedadeGenerica)
        {
            var estruturaEntidade = estruturaAlteracaoPropriedadeGenerica.EstruturaEntidade;
            if (entidade.Id == 0 )
            {
                return (false, null);
            }

            if (entidade.__PropriedadesAlteradas != null && entidade.__PropriedadesAlteradas.ContainsKey(estruturaAlteracaoPropriedadeGenerica.Propriedade.Name))
            {
                var propriedadeAlterada = entidade.__PropriedadesAlteradas[estruturaAlteracaoPropriedadeGenerica.Propriedade.Name];

                var estruturaCampo = estruturaAlteracaoPropriedadeGenerica.EstruturaCampo;
                var sqlValorAtual = new StringBuilderSql();
                sqlValorAtual.AppendFormat(" SELECT [{0}] FROM [{1}].[{2}] WHERE [{3}] = {4}", estruturaCampo.NomeCampo, estruturaEntidade.Schema, estruturaEntidade.NomeTabela, estruturaEntidade.EstruturaCampoChavePrimaria.NomeCampo, entidade.Id);

                var valorBancoAtual = ConverterUtil.ParaString(this.Conexao.RetornarValorScalar(sqlValorAtual.ToString(), null), false);
                var valorBancoAtualTipado = ConverterUtil.ParaString(ConverterUtil.Converter(valorBancoAtual, estruturaCampo.Propriedade.PropertyType));
                var novoValor = ConverterUtil.ParaString(estruturaCampo.Propriedade.GetValue(entidade), false);

                var isExisteAlteracao = this.IsExisteAlteracaoPropriedadeGenerica(estruturaCampo, novoValor, valorBancoAtualTipado);
                return (isExisteAlteracao, valorBancoAtualTipado);
            }
            return (false, null);
        }

        private bool IsExisteAlteracaoPropriedadeGenerica(EstruturaCampo estruturaCampo, string novoValor, string valorBancoAtualTipado)
        {
            return novoValor != valorBancoAtualTipado;
        }

        #endregion


    }
}
