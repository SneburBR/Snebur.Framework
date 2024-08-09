using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio;
using Snebur.Reflexao;
using Snebur.Serializacao;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class EntidadesAlteracaoPropriedadeGenerica : BaseEnitdadesAlteracaoPropriedade
    {

        public EntidadesAlteracaoPropriedadeGenerica(BaseContextoDados contexto) : base(contexto)
        {
        }

        internal HashSet<Entidade> RetornarEntidadesAlteracaoPropriedadeGenericas(HashSet<Entidade> entidades)
        {
            var alteracoesPropriedade = new HashSet<Entidade>();
            foreach (var entidade in entidades)
            {
                var estruturaEntidade = this.Contexto.EstruturaBancoDados.EstruturasEntidade[entidade.GetType().Name];
                foreach (var estruturaAlteracaoPropriedade in estruturaEntidade.TodasEstruturasAlteracaoPropriedadeGenerica)
                {
                    var (isExisteAltracao, valorAntigo) = this.IsExisteAlteracaoPropriedade(entidade,
                                                                                          estruturaAlteracaoPropriedade);
                    if (isExisteAltracao)
                    {
                        var valorPropriedade = estruturaAlteracaoPropriedade.Propriedade.GetValue(entidade);
                        if (estruturaAlteracaoPropriedade.IsIgnorarValorAntigoNull && valorAntigo == null)
                        {
                            continue;
                        }

                        if (estruturaAlteracaoPropriedade.IsIgnorarZeroIgualNull)
                        {
                            if ((valorPropriedade is null || valorPropriedade.Equals(0)) &&
                                (valorAntigo is null || valorAntigo.Equals(0)))
                            {
                                continue;
                            }
                        }

                        if (estruturaAlteracaoPropriedade.IsSalvarDataHoraFimAlteracao)
                        {
                            var ultimaAlteracao = this.RetornarUtlimaAlteracaoGenerica(entidade, estruturaEntidade, estruturaAlteracaoPropriedade);
                            if (ultimaAlteracao != null)
                            {
                                ultimaAlteracao.DataHoraFimAlteracao = this.Contexto.EstruturaBancoDados.IsDateTimeUtc ? DateTime.UtcNow : DateTime.Now;
                                alteracoesPropriedade.Add((Entidade)ultimaAlteracao);
                            }
                        }

                        var novaAlteracao = (IAlteracaoPropriedadeGenerica)Activator.CreateInstance(this.Contexto.EstruturaBancoDados.TipoEntidadeNotificaoPropriedadeAlteradaGenerica);
                        novaAlteracao.IdEntidade = entidade.IdEntidadeHistoricoGenerico;

                     
                        if (estruturaAlteracaoPropriedade.IsProteger)
                        {
                            novaAlteracao.ValorPropriedadeAlterada =  FormatacaoUtil.Proteger(valorPropriedade?.ToString()).
                                                                                     RetornarPrimeirosCaracteres(5000);
                            novaAlteracao.ValorPropriedadeAntigo = FormatacaoUtil.Proteger(valorAntigo?.ToString()).
                                                                                  RetornarPrimeirosCaracteres(5000); ;
                        }
                        else
                        {
                            novaAlteracao.ValorPropriedadeAlterada = SerializacaoUtil.SerializarTipoSimples(valorPropriedade).
                                                                               RetornarPrimeirosCaracteres(5000);

                            novaAlteracao.ValorPropriedadeAntigo = SerializacaoUtil.SerializarTipoSimples(valorAntigo).
                                                                                      RetornarPrimeirosCaracteres(5000);

                        }

                        novaAlteracao.IdNamespace = estruturaEntidade.EstruturaBancoDados.IdNamespace;
                        novaAlteracao.NomeTipoEntidade = estruturaEntidade.NomeTipoEntidade;
                        novaAlteracao.NomePropriedade = estruturaAlteracaoPropriedade.Propriedade.Name;
                        novaAlteracao.TipoPrimario = estruturaAlteracaoPropriedade.EstruturaCampo?.TipoPrimarioEnum;
                        novaAlteracao.IsTipoComplexo = estruturaAlteracaoPropriedade.IsTipoComplexo;
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
            var contextoSessaoUsuario = this.Contexto.ContextoSessaoUsuario;
            var tipoAlteracao = contextoSessaoUsuario.EstruturaBancoDados.TipoEntidadeNotificaoPropriedadeAlteradaGenerica;
            var consulta = contextoSessaoUsuario.RetornarConsulta<IAlteracaoPropriedadeGenerica>(tipoAlteracao);
            consulta = consulta.Where(x => x.IdEntidade == entidade.Id &&
                                           x.NomeTipoEntidade == estruturaEntidade.NomeTipoEntidade &&
                                           x.NomePropriedade == estruturaAlteracaoPropriedade.Propriedade.Name).
                                           OrderByDescending(x => x.Id);

            return consulta.FirstOrDefault();
        }

        //private (bool IsExisteAlteracao, string valorAntigo) IsExisteAlteracaoPropriedadeGerencia(Entidade entidade,
        //                                                                                          EstruturaAlteracaoPropriedadeGenerica estruturaAlteracaoPropriedadeGenerica)
        //{
        //    var estruturaEntidade = estruturaAlteracaoPropriedadeGenerica.EstruturaEntidade;
        //    if (entidade.Id == 0)
        //    {
        //        return (estruturaAlteracaoPropriedadeGenerica.IsNotificarNovoCadastro, null);
        //    }

        //    if (entidade.__PropriedadesAlteradas != null &&
        //        entidade.__PropriedadesAlteradas.Count > 0)
        //    {


        //        if (estruturaAlteracaoPropriedadeGenerica.IsTipoComplexo)
        //        {

        //        }

        //        var propriedadeAlterada = entidade.__PropriedadesAlteradas[estruturaAlteracaoPropriedadeGenerica.Propriedade.Name];
        //        var estruturaCampo = estruturaAlteracaoPropriedadeGenerica.EstruturaCampo;
        //        var sqlValorAtual = new StringBuilderSql();
        //        sqlValorAtual.AppendFormat(" SELECT [{0}] FROM [{1}].[{2}] WHERE [{3}] = {4}", estruturaCampo.NomeCampo,
        //                                                                                       estruturaEntidade.Schema,
        //                                                                                       estruturaEntidade.NomeTabela,
        //                                                                                       estruturaEntidade.EstruturaCampoChavePrimaria.NomeCampo,
        //                                                                                       entidade.Id);

        //        var valorBancoAtual = ConverterUtil.ParaString(this.Conexao.RetornarValorScalar(sqlValorAtual.ToString(), null), false);
        //        var valorBancoAtualTipado = ConverterUtil.ParaString(ConverterUtil.Converter(valorBancoAtual, estruturaCampo.Propriedade.PropertyType));
        //        var novoValor = ConverterUtil.ParaString(estruturaCampo.Propriedade.GetValue(entidade), false);

        //        var isExisteAlteracao = this.IsExisteAlteracaoPropriedadeGenerica(estruturaCampo, novoValor, valorBancoAtualTipado);
        //        return (isExisteAlteracao, valorBancoAtualTipado);
        //    }
        //    return (false, null);
        //}

        //private bool IsExisteAlteracaoPropriedadeGenerica(EstruturaCampo estruturaCampo,
        //                                                  string novoValor,
        //                                                  string valorBancoAtualTipado)
        //{
        //    return novoValor != valorBancoAtualTipado;
        //}

    }
}
