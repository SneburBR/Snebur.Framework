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

        internal static void Atualizar(EntidadeAlterada entidadeAlterada, BaseContextoDados contexto)
        {
            var entidade = entidadeAlterada.Entidade;
            if (entidade.Id == 0)
            {
                //var propriedades = ReflexaoUtil.RetornarPropriedades(entidade.GetType());
                var propriedades = EntidadeUtil.RetornarPropriedadesCampos(entidade.GetType(), EnumFiltroPropriedadeCampo.IgnorarChavePrimaria |
                                                                                               EnumFiltroPropriedadeCampo.IgnorarPropriedadeProtegida);
                foreach (var propriedade in propriedades)
                {
                    var valorPropriedade = propriedade.GetValue(entidade);
                    var valorPadrao = AtualizarValorPadrao.RetornarValorPropriedade(contexto, 
                                                                                    entidade, 
                                                                                    propriedade,
                                                                                    valorPropriedade);
                    if (valorPadrao != null && valorPadrao != valorPropriedade)
                    {
                        propriedade.SetValue(entidade, valorPadrao);
                        entidadeAlterada.PropriedadesAtualizadas.Add(propriedade);
                    }
                }
            }
        }

        private static object RetornarValorPropriedade(BaseContextoDados contexto,
                                                       Entidade entidade,
                                                       PropertyInfo propriedade,
                                                       object valorPropriedade)
        {
            var atributos = propriedade.GetCustomAttributes();
            if (atributos.Count() > 0)
            {
                var atributoSessaoUsuario = atributos.OfType<ValorPadraoIDSessaoUsuarioAttribute>().SingleOrDefault();
                if (atributoSessaoUsuario != null)
                {
                    contexto.SqlSuporte.ValidarSuporteSessaoUsuario();

                    if (atributoSessaoUsuario.IsSomenteCadastro && entidade.Id > 0)
                    {
                        return null;
                    }
                    return contexto.SessaoUsuarioLogado.Id;
                }

                var atributoUsuarioLogado = atributos.OfType<ValorPadraoIDUsuarioLogadoAttribute>().SingleOrDefault();
                if (atributoUsuarioLogado != null)
                {
                    contexto.SqlSuporte.ValidarSuporteSessaoUsuario();
                    if (!atributoUsuarioLogado.IsPermitirUsuarioAnonimo)
                    {
                        if (contexto.IsAnonimo)
                        {
                            throw new Erro("O usuário anonimo não tem permissão para salvar setar o valor padrão do id usuario logado");
                        }
                    }
                    return contexto.UsuarioLogado.Id;
                }
                var atributoValorPadrao = AtualizarValorPadrao.RetornarAtributoValorPradao(propriedade);
                if (atributoValorPadrao != null)
                {
                    return atributoValorPadrao.RetornarValorPadrao(contexto, 
                                                                   entidade, 
                                                                   valorPropriedade);
                }

                if (atributos.OfType<PropriedadeIdentificadorProprietarioAttribute>().SingleOrDefault() != null)
                {
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

                }

            }
            return null;
        }

        private static IValorPadrao RetornarAtributoValorPradao(PropertyInfo propriedade)
        {
            var atributos = propriedade.GetCustomAttributes();
            var tipoIValorPadrao = typeof(IValorPadrao);
            return (IValorPadrao)atributos.Where(x => ReflexaoUtil.TipoImplementaInterface(x.GetType(), tipoIValorPadrao, true)).SingleOrDefault();
        }
    }
}