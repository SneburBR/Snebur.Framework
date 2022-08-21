﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Snebur.Dominio;
using Snebur.Linq;
using Snebur.Utilidade;

namespace Snebur.AcessoDados
{
    public abstract partial class BaseContextoDados : __BaseContextoDados, IServicoDados, IContextoDadosSemNotificar, IDisposable
    {

        private List<Entidade> RetornarEntidadesExcluirEmCascata(IEnumerable<Entidade> entidades, string relacoesEmCascata)
        {
            var entidadesRecuperadas = new List<Entidade>();

            var relacoes = relacoesEmCascata?.Split(',').Select(x => x.Trim()).Where(x => !String.IsNullOrEmpty(x)).ToArray();
            foreach (var entidade in entidades)
            {
                var consulta = this.RetornarConsulta<Entidade>(entidade.GetType());
                if (Debugger.IsAttached)
                {
                    consulta.IncluirDeletados();
                }
                if (relacoes != null && relacoes.Count() > 0)
                {
                    consulta = consulta.AbrirRelacoes(relacoes);
                }
                var entidadeRecuperada = consulta.Where(x => x.Id == entidade.Id).SingleOrDefault();
                if (entidadeRecuperada != null)
                {
                    entidadesRecuperadas.Add(entidadeRecuperada);
                }
            }

            return this.RetornarTodasEntidades(entidadesRecuperadas).ToList();
        }

        private HashSet<Entidade> RetornarTodasEntidades(IEnumerable<Entidade> entidadesRecuperada)
        {
            var retorno = new HashSet<Entidade>();
            foreach (var entidade in entidadesRecuperada)
            {
                retorno.AddRange(this.RetornarTodasEntidades(entidade));
            }
            return retorno;


        }
        private HashSet<Entidade> RetornarTodasEntidades(Entidade entidadeRecuperada)
        {
            var retorno = new HashSet<Entidade>();
            retorno.Add(entidadeRecuperada);
            var propriedades = entidadeRecuperada.GetType().GetProperties();
            foreach (var propriedade in propriedades)
            {
                if (propriedade.PropertyType.IsSubclassOf(typeof(Entidade)))
                {
                    var entidade = propriedade.GetValue(entidadeRecuperada);
                    if (entidade is Entidade entidadeTipada && entidadeTipada.Id > 0)
                    {
                        retorno.AddRange(this.RetornarTodasEntidades(entidadeTipada));
                    }
                }
                else if (ReflexaoUtil.TipoRetornaColecaoEntidade(propriedade.PropertyType))
                {
                    var entidades = propriedade.GetValue(entidadeRecuperada);
                    if (entidades is IEnumerable<Entidade> entidadesTipada && entidadesTipada != null)
                    {
                        retorno.AddRange(this.RetornarTodasEntidades(entidadesTipada));
                    }

                }
            }
            return retorno;
        }


        private List<Entidade> RetornarEntidadesExcluirEmCascataObsoleto(IEnumerable<Entidade> entidades, string relacoesEmCascata)
        {
            if (String.IsNullOrEmpty(relacoesEmCascata))
            {
                return entidades.ToList<Entidade>();
            }

            var retorno = new HashSet<Entidade>();
            //var tipoEntidade = entidades.First().GetType();

            var caminhosPropriedade = relacoesEmCascata.Split(',').Select(x => x.Trim());

            foreach (var entidade in entidades)
            {
                foreach (var caminhoPropriedade in caminhosPropriedade)
                {

                    var entidadesAtual = new List<Entidade> { entidade };

                    foreach (var nomePropriedade in caminhoPropriedade.Split('.'))
                    {
                        if (!String.IsNullOrWhiteSpace(nomePropriedade) && nomePropriedade != "Incluir")
                        {
                            if (entidadesAtual.Count > 0)
                            {
                                foreach (var entidadeAtual in entidadesAtual.ToList())
                                {
                                    var tipoEntidadeAtual = entidadeAtual.GetType();
                                    var propriedadeRelacao = ReflexaoUtil.RetornarPropriedade(tipoEntidadeAtual, nomePropriedade);

                                    this.ValidarEntidadeParaDeletar(entidade);

                                    if (ReflexaoUtil.TipoRetornaColecaoEntidade(propriedadeRelacao.PropertyType))
                                    {
                                        var entidadesRecuperada = this.RecuperarEntidades(entidadeAtual, propriedadeRelacao, tipoEntidadeAtual);
                                        entidadesAtual.AddRangeNew(entidadesRecuperada);
                                        retorno.AddRange(entidadesRecuperada);
                                    }
                                    else
                                    {
                                        var entidadeRecuperada = RecuperarEntidade(entidadeAtual, nomePropriedade, tipoEntidadeAtual);
                                        retorno.Add(entidadeRecuperada);
                                        var entidadeAtualRelecao = propriedadeRelacao.GetValue(entidadeRecuperada) as Entidade;
                                        entidadesAtual.Clear();
                                        if (entidadeAtualRelecao != null)
                                        {
                                            entidadesAtual.Add(entidadeAtualRelecao);
                                            retorno.Add(entidadeAtualRelecao);
                                        }

                                    }
                                    //retorno.AddRange(this.RetornarEntidadesExcluirEmCascata(entidadeRecuperada, propriedadeRelacao.ToQueue()));

                                }
                            }


                        }

                    }

                    if (entidadesAtual.Count > 0)
                    {
                        foreach (var entidadeAtual in entidadesAtual)
                        {
                            var entidadeReceuperada = RecuperarEntidade(entidadeAtual, null, entidadeAtual.GetType());
                            retorno.Add(entidadeReceuperada);
                        }

                    }

                }
            }
            return retorno.ToList();

            Entidade RecuperarEntidade(Entidade entidadeAtual, string nomePropriedade, Type tipoEntidadeAtual)
            {
                var consulta = this.RetornarConsulta<Entidade>(tipoEntidadeAtual);
                if (Debugger.IsAttached)
                {
                    consulta.IncluirDeletados();
                }
                if (!String.IsNullOrWhiteSpace(nomePropriedade))
                {
                    consulta = consulta.AbrirRelacao(nomePropriedade);
                }
                var entidadeRecuperada = consulta.Where(x => x.Id == entidadeAtual.Id).
                                                  Single();
                return entidadeRecuperada;
            }
        }

        private List<Entidade> RecuperarEntidades(Entidade entidadeAtual, PropertyInfo propriedadeRelacao, Type tipoEntidadeAtual)
        {
            var consulta = this.RetornarConsulta<Entidade>(tipoEntidadeAtual);
            if (Debugger.IsAttached)
            {
                consulta.IncluirDeletados();
            }
            if (!String.IsNullOrWhiteSpace(propriedadeRelacao.Name))
            {
                consulta = consulta.AbrirColecao(propriedadeRelacao.Name);
            }
            var entidadeRecuperada = consulta.Where(x => x.Id == entidadeAtual.Id).
                                              Single();

            var entidades = propriedadeRelacao.GetValue(entidadeRecuperada) as IEnumerable<Entidade>;
            if (entidades != null)
            {
                return entidades.ToList<Entidade>();
            }
            return new List<Entidade>();

        }

        private void ValidarEntidadeParaDeletar(Entidade entidade)
        {
            if (entidade is IDeletado entidadeDeletada)
            {
                if (entidadeDeletada.IsDeletado)
                {
                    if (!Debugger.IsAttached)
                    {
                        throw new Erro($"A entidade do tipo '{entidade.GetType().Name}' (id: {entidade.Id}) já foi deletada");
                    }
                }
            }

        }


        //private List<Entidade> RetornarEntidadesExcluirEmCascata(Entidade entidade, Queue<PropertyInfo> propriedades)
        //{
        //    var retorno = new List<Entidade>();
        //    //var fila = propriedades.ToQueue();
        //    var entidadeAtual = entidade;

        //    while (propriedades.Count > 0)
        //    {
        //        var propriedade = propriedades.Dequeue();
        //        var valorPropriedade = ReflexaoUtil.RetornarValorPropriedade(entidadeAtual, propriedade);

        //        if (valorPropriedade != null)
        //        {
        //            if (valorPropriedade is Entidade entidadeRelacao)
        //            {
        //                retorno.Add(entidadeRelacao);
        //                entidadeAtual = entidadeRelacao;
        //            }
        //            else if (valorPropriedade is IEnumerable colecao)
        //            {
        //                retorno.AddRange(colecao.Cast<Entidade>());
        //                if (propriedades.Count > 0)
        //                {
        //                    foreach (var entidadeFilho in colecao.OfType<Entidade>())
        //                    {
        //                        retorno.AddRange(this.RetornarEntidadesExcluirEmCascata(entidadeFilho, propriedades.ToQueue()));

        //                    }
        //                }
        //                propriedades.Clear();
        //            }
        //            else
        //            {
        //                throw new ErroNaoSuportado($"O valor da propriedade não é suportado {valorPropriedade.GetType().Name}");
        //            }
        //        }
        //    }
        //    return retorno.ToList();
        //}

    }
}
