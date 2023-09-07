﻿using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class AtualizarValorPadrao
    {
        internal static void Atualizar(List<Entidade> entidades, BaseContextoDados contexto)
        {
            throw new Erro("Método obsoleto");
            //foreach (var entidade in entidades)
            //{
            //    if (entidade.Id == 0)
            //    {
            //        var propriedades = ReflexaoUtil.RetornarPropriedades(entidade.GetType());
            //        foreach (var propriedade in propriedades)
            //        {
            //            var atributos = propriedade.GetCustomAttributes();
            //            if (atributos.OfType<ValorPadraoIDSessaoUsuarioAttribute>().Count() > 0)
            //            {
            //                propriedade.SetValue(entidade, contexto.SessaoUsuario.Id);
            //                continue;
            //            }

            //            if (atributos.OfType<ValorPadraoIDUsuarioLogadoAttribute>().Count() > 0)
            //            {
            //                propriedade.SetValue(entidade, contexto.Usuario.Id);
            //                continue;
            //            }

            //            var atributoValorPadrao = AtualizarValorPadrao.RetornarAtributoValorPradao(propriedade);
            //            if (atributoValorPadrao != null)
            //            {
            //                var valorPropriedade = propriedade.GetValue(entidade);
            //                var valorPadrao = atributoValorPadrao.RetornarValorPadrao();
            //                if (valorPropriedade != valorPadrao)
            //                {
            //                    propriedade.SetValue(entidade, valorPadrao);
            //                }
            //            }
            //        }
            //    }
            //}
        }

        internal static void Atualizar(EntidadeAlterada entidadeAlterada,
                                       BaseContextoDados contexto)
        {
            var entidade = entidadeAlterada.Entidade;
            var entidadeInterna = (IEntidadeInterna)entidadeAlterada.Entidade;
            //var propriedades = ReflexaoUtil.RetornarPropriedades(entidade.GetType());
            //var propriedades = EntidadeUtil.RetornarPropriedadesCampos(entidade.GetType(), EnumFiltroPropriedadeCampo.IgnorarChavePrimaria |
            //                                                                               EnumFiltroPropriedadeCampo.IgnorarPropriedadeProtegida);
            //foreach (var propriedade in propriedades)
            //{
            var estruturaEntidade = entidadeAlterada.EstruturaEntidade;
            var estruturasCapaValorPadrao = RetornarEstruturasCamos(estruturaEntidade, entidade);

            if (estruturasCapaValorPadrao?.Length > 0)
            {
                entidadeInterna.DesativarValidacaoProprieadesAbertas();

                foreach (var estruturaCampo in estruturasCapaValorPadrao)
                {
                    var propriedade = estruturaCampo.Propriedade;


                    var valorPropriedade = propriedade.GetValue(entidade);
                    var valorPadrao = AtualizarValorPadrao.RetornarValorPropriedade(contexto,
                                                                                    estruturaCampo,
                                                                                    entidade,
                                                                                    propriedade,
                                                                                    valorPropriedade);
                    if (valorPadrao != null && valorPadrao != valorPropriedade)
                    {
                        entidadeInterna.AdicionarProprieadeAberta(propriedade.Name);

                        propriedade.SetValue(entidade, valorPadrao);
                        entidadeAlterada.PropriedadesAtualizadas.Add(propriedade);
                    }
                }
                entidadeInterna.AtivarValidacaoProprieadesAbertas();
            }
        }

        private static EstruturaCampo[] RetornarEstruturasCamos(EstruturaEntidade estruturaEntidade,
                                                                Entidade entidade)
        {
            if (entidade.Id == 0)
            {
                return estruturaEntidade.TodasEstruturasCamposValorPadraoInsert;
            }
            if (entidade.__PropriedadesAlteradas?.Count > 0)
            {
                return estruturaEntidade.TodasEstruturasCamposValorPadraoUpdate;
            }
            return null;
        }

        private static object RetornarValorPropriedade(BaseContextoDados contexto,
                                                       EstruturaCampo estruturaCampo,
                                                       Entidade entidade,
                                                       PropertyInfo propriedade,
                                                       object valorPropriedade)
        {

            var tipoValorPadrao = estruturaCampo.TipoValorPadrao;
            switch (tipoValorPadrao)
            {
                case EnumTipoValorPadrao.ValorPropriedadeNull:
                    {

                        var atributoValorPadrao = estruturaCampo.RetornarAtributoValorPadrao<IValorPadrao>();
                        return atributoValorPadrao.RetornarValorPadrao(contexto,
                                                                       entidade,
                                                                       valorPropriedade);
                    }
                case EnumTipoValorPadrao.Comum:
                    {
                        var atributoValorPadrao = estruturaCampo.RetornarAtributoValorPadrao<IValorPadrao>();
                        return atributoValorPadrao.RetornarValorPadrao(contexto,
                                                                       entidade,
                                                                       valorPropriedade);
                    }


                case EnumTipoValorPadrao.IndentificadorProprietario:

                    var identificadorProprietario = contexto.IdentificadorProprietario;

                    try
                    {
                        if (identificadorProprietario == ConfiguracaoUtil.IDENTIFICADOR_PROPRIETARIO_GLOBAL)
                        {
                            if (entidade is INormalizarIdentificadorProprietario normalizarIdentificadorProprietario)
                            {
                                var identificadorProprietarioNormalizado = normalizarIdentificadorProprietario.NormalizarIdentificadorProprietario(identificadorProprietario);
                                if (identificadorProprietarioNormalizado != ConfiguracaoUtil.IDENTIFICADOR_PROPRIETARIO_GLOBAL)
                                {
                                    return ConverterUtil.Converter(identificadorProprietarioNormalizado,
                                                                   propriedade.PropertyType);
                                }
                            }

                            return null;
                        }
                        return ConverterUtil.Converter(identificadorProprietario,
                                                       propriedade.PropertyType);

                    }
                    catch (Exception ex)
                    {
                        throw new Erro($"Não é possível converter  identificador do proprietário {identificadorProprietario} para {propriedade.PropertyType.Name}", ex);
                    }


                case EnumTipoValorPadrao.SessaoUsuario_Id:

                    contexto.SqlSuporte.ValidarSuporteSessaoUsuario();

                    var atributoSessaoUsuario = estruturaCampo.RetornarAtributoValorPadrao<ValorPadraoIDSessaoUsuarioAttribute>();
                    if (atributoSessaoUsuario.IsSomenteCadastro && entidade.Id > 0)
                    {
                        return null;
                    }
                    return contexto.SessaoUsuarioLogado.Id;

                case EnumTipoValorPadrao.UsuarioLogado_Id:

                    contexto.SqlSuporte.ValidarSuporteSessaoUsuario();
                    var atributoUsuarioLogado = estruturaCampo.RetornarAtributoValorPadrao<ValorPadraoIDUsuarioLogadoAttribute>();
                    if (!atributoUsuarioLogado.IsPermitirUsuarioAnonimo)
                    {
                        if (contexto.IsAnonimo)
                        {
                            throw new Erro("O usuário anonimo não tem permissão para salvar setar o valor padrão do id usuario logado");
                        }
                    }
                    return contexto.UsuarioLogado.Id;


                case EnumTipoValorPadrao.Nenhum:

                    throw new Exception("A estrutura da campo não possui nenhum atributo de valor padrão");

                default:
                    throw new Exception($"O tipo do valor padrão {tipoValorPadrao} não é suportado");
            }

        }

        private static IValorPadrao RetornarAtributoValorPradao(PropertyInfo propriedade)
        {
            var atributos = propriedade.GetCustomAttributes();
            var tipoIValorPadrao = typeof(IValorPadrao);
            return (IValorPadrao)atributos.Where(x => ReflexaoUtil.TipoImplementaInterface(x.GetType(), tipoIValorPadrao, true)).SingleOrDefault();
        }
    }
}