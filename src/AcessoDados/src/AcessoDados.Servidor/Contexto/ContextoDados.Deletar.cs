using Snebur.AcessoDados.Servidor.Salvar;
using Snebur.Dominio;
using Snebur.Linq;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Snebur.AcessoDados
{
    public abstract partial class BaseContextoDados : __BaseContextoDados, IServicoDados, IContextoDadosSemNotificar, IDisposable
    {
        public override ResultadoDeletar Deletar(IEnumerable<IEntidade> entidades)
        {
            return this.Deletar(entidades, String.Empty, false);
        }

        public ResultadoDeletar Deletar<TEntidade>(TEntidade entidade, Expression<Func<TEntidade, object>> expressaoPropriedade) where TEntidade : Entidade
        {
            var relacoes = Util.RetornarRelacoesAbertas(expressaoPropriedade);
            return this.Deletar(new List<IEntidade> { entidade }, relacoes);
        }

        public ResultadoDeletar Deletar<TEntidade>(IEnumerable<TEntidade> entidades, Expression<Func<TEntidade, object>> expressaoPropriedade) where TEntidade : Entidade
        {
            var relacoes = Util.RetornarRelacoesAbertas(expressaoPropriedade);
            return this.Deletar(entidades, relacoes);
        }

        public ResultadoDeletar Deletar<TEntidade>(TEntidade entidade, params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {
            var relacoes = Util.RetornarRelacoesAbertas(expressoesPropriedade);
            return this.Deletar(new List<IEntidade> { entidade }, relacoes);
        }

        public ResultadoDeletar Deletar<TEntidade>(IEnumerable<TEntidade> entidades, params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {
            var relacoes = Util.RetornarRelacoesAbertas(expressoesPropriedade);
            return this.Deletar(entidades.ToList<IEntidade>(), relacoes);
        }

        public override ResultadoDeletar Deletar(IEnumerable<IEntidade> entidades,
                                                 string relacoesEmCascata)
        {
            return this.Deletar(entidades, relacoesEmCascata, false);
        }

        public override ResultadoDeletar Deletar(IEntidade entidade)
        {
            return this.Deletar(new List<Entidade> { entidade });
        }

        public override ResultadoDeletar Deletar(IEntidade entidade, string relacoesEmCascata)
        {
            return this.Deletar(new List<IEntidade> { entidade }, relacoesEmCascata);
        }




        public ResultadoDeletar DeletarRegistro<TEntidade>(IEnumerable<TEntidade> entidades, params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {
            var relacoesEmCascata = Util.RetornarRelacoesAbertas(expressoesPropriedade);
            return this.DeletarRegistro(entidades, relacoesEmCascata);
        }

        public ResultadoDeletar DeletarRegistro<TEntidade>(TEntidade entidade, params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {
            var relacoesEmCascata = Util.RetornarRelacoesAbertas(expressoesPropriedade);
            return this.DeletarRegistro(new List<IEntidade> { entidade }, relacoesEmCascata);
        }

        public ResultadoDeletar DeletarRegistro(IEntidade entidade, string relacoesEmCascata)
        {
            return this.DeletarRegistro(new List<IEntidade> { entidade }, relacoesEmCascata);
        }
        public ResultadoDeletar DeletarRegistro(IEnumerable<IEntidade> entidades,
                                            string relacoesEmCascata)
        {
            return this.Deletar(entidades,
                                relacoesEmCascata,
                                false,
                                EnumOpcaoSalvar.DeletarRegistro);
        }

        private ResultadoDeletar Deletar(IEnumerable<IEntidade> entidades,
                                        string relacoesEmCascata,
                                        bool ignorarErro,
                                        EnumOpcaoSalvar opcaoSalvar = EnumOpcaoSalvar.Deletar)
        {
            this.ValidarSessaoUsuario();

            var entidadesDeletar = this.RetornarEntidadesDeletarEmCascata(entidades, relacoesEmCascata);
            using (var salvar = new SalvarEntidades(this, entidadesDeletar.ToHashSet(), opcaoSalvar, false))
            {
                var resultado = salvar.Salvar();
                if (resultado.Erro != null && !ignorarErro)
                {
                    throw resultado.Erro;
                }
                return resultado as ResultadoDeletar;
            }
        }

        private List<Entidade> RetornarEntidadesDeletarEmCascata(IEnumerable<IEntidade> entidades, string relacoesEmCascata)
        {
            var entidadesRecuperadas = new List<Entidade>();

            var relacoes = relacoesEmCascata?.Split(',').Select(x => x.Trim()).Where(x => !String.IsNullOrEmpty(x)).ToArray();
            if (relacoes.Count() == 0)
            {
                return this.RetornarTodasEntidades(entidades.Cast<Entidade>()).ToList();
            }

            var gruposEntidades = entidades.GroupBy(x => x.GetType());
            foreach (var grupo in gruposEntidades)
            {
                var consulta = this.RetornarConsulta<Entidade>(grupo.Key);
                if (DebugUtil.IsAttached)
                {
                    consulta.IncluirDeletados();
                }
                if (relacoes != null && relacoes.Count() > 0)
                {
                    consulta = consulta.AbrirRelacoes(relacoes);
                }
                var ids = grupo.Select(x => x.Id).ToList();
                var entidadesRecuperada = consulta.WhereIds(ids).ToList();
                if (entidadesRecuperada?.Count > 0)
                {
                    entidadesRecuperadas.AddRange(entidadesRecuperada);
                }
            }
            return this.RetornarTodasEntidades(entidadesRecuperadas).ToList();

            //foreach (var entidade in entidades)
            //{
            //    var consulta = this.RetornarConsulta<Entidade>(entidade.GetType());

            //    //if (DebugUtil.IsAttached)
            //    //{
            //    //    consulta.IncluirDeletados();
            //    //}

            //    if (relacoes != null && relacoes.Count() > 0)
            //    {

            //        consulta = consulta.AbrirRelacoes(relacoes);
            //    }
            //    var entidadeRecuperada = consulta.Where(x => x.Id == entidade.Id).SingleOrDefault();
            //    if (entidadeRecuperada != null)
            //    {
            //        entidadesRecuperadas.Add(entidadeRecuperada);
            //    }
            //}

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
                    if (entidade is Entidade entidadeTipada && entidadeTipada.__IsNewEntity)
                    {
                        retorno.AddRange(this.RetornarTodasEntidades(entidadeTipada));
                    }
                }
                else if (ReflexaoUtil.IsTipoRetornaColecaoEntidade(propriedade.PropertyType))
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


        //private List<Entidade> RetornarEntidadesDeletarEmCascataObsoleto(IEnumerable<Entidade> entidades, string relacoesEmCascata)
        //{
        //    if (String.IsNullOrEmpty(relacoesEmCascata))
        //    {
        //        return entidades.ToList<Entidade>();
        //    }

        //    var retorno = new HashSet<Entidade>();
        //    //var tipoEntidade = entidades.First().GetType();

        //    var caminhosPropriedade = relacoesEmCascata.Split(',').Select(x => x.Trim());

        //    foreach (var entidade in entidades)
        //    {
        //        foreach (var caminhoPropriedade in caminhosPropriedade)
        //        {

        //            var entidadesAtual = new List<Entidade> { entidade };

        //            foreach (var nomePropriedade in caminhoPropriedade.Split('.'))
        //            {
        //                if (!String.IsNullOrWhiteSpace(nomePropriedade) && nomePropriedade != "Incluir")
        //                {
        //                    if (entidadesAtual.Count > 0)
        //                    {
        //                        foreach (var entidadeAtual in entidadesAtual.ToList())
        //                        {
        //                            var tipoEntidadeAtual = entidadeAtual.GetType();
        //                            var propriedadeRelacao = ReflexaoUtil.RetornarPropriedade(tipoEntidadeAtual, nomePropriedade);

        //                            this.ValidarEntidadeParaDeletar(entidade);

        //                            if (ReflexaoUtil.TipoRetornaColecaoEntidade(propriedadeRelacao.PropertyType))
        //                            {
        //                                var entidadesRecuperada = this.RecuperarEntidades(entidadeAtual, propriedadeRelacao, tipoEntidadeAtual);
        //                                entidadesAtual.AddRangeNew(entidadesRecuperada);
        //                                retorno.AddRange(entidadesRecuperada);
        //                            }
        //                            else
        //                            {
        //                                var entidadeRecuperada = RecuperarEntidade(entidadeAtual, nomePropriedade, tipoEntidadeAtual);
        //                                retorno.Add(entidadeRecuperada);
        //                                var entidadeAtualRelecao = propriedadeRelacao.GetValue(entidadeRecuperada) as Entidade;
        //                                entidadesAtual.Clear();
        //                                if (entidadeAtualRelecao != null)
        //                                {
        //                                    entidadesAtual.Add(entidadeAtualRelecao);
        //                                    retorno.Add(entidadeAtualRelecao);
        //                                }

        //                            }
        //                            //retorno.AddRange(this.RetornarEntidadesDeletarEmCascata(entidadeRecuperada, propriedadeRelacao.ToQueue()));

        //                        }
        //                    }


        //                }

        //            }

        //            if (entidadesAtual.Count > 0)
        //            {
        //                foreach (var entidadeAtual in entidadesAtual)
        //                {
        //                    var entidadeReceuperada = RecuperarEntidade(entidadeAtual, null, entidadeAtual.GetType());
        //                    retorno.Add(entidadeReceuperada);
        //                }

        //            }

        //        }
        //    }
        //    return retorno.ToList();

        //    Entidade RecuperarEntidade(Entidade entidadeAtual, string nomePropriedade, Type tipoEntidadeAtual)
        //    {
        //        var consulta = this.RetornarConsulta<Entidade>(tipoEntidadeAtual);
        //        if (DebugUtil.IsAttached)
        //        {
        //            consulta.IncluirDeletados();
        //        }
        //        if (!String.IsNullOrWhiteSpace(nomePropriedade))
        //        {
        //            consulta = consulta.AbrirRelacao(nomePropriedade);
        //        }
        //        var entidadeRecuperada = consulta.Where(x => x.Id == entidadeAtual.Id).
        //                                          Single();
        //        return entidadeRecuperada;
        //    }
        //}

        //private List<Entidade> RecuperarEntidades(Entidade entidadeAtual, PropertyInfo propriedadeRelacao, Type tipoEntidadeAtual)
        //{
        //    var consulta = this.RetornarConsulta<Entidade>(tipoEntidadeAtual);
        //    if (DebugUtil.IsAttached)
        //    {
        //        consulta.IncluirDeletados();
        //    }
        //    if (!String.IsNullOrWhiteSpace(propriedadeRelacao.Name))
        //    {
        //        consulta = consulta.AbrirColecao(propriedadeRelacao.Name);
        //    }
        //    var entidadeRecuperada = consulta.Where(x => x.Id == entidadeAtual.Id).
        //                                      Single();

        //    var entidades = propriedadeRelacao.GetValue(entidadeRecuperada) as IEnumerable<Entidade>;
        //    if (entidades != null)
        //    {
        //        return entidades.ToList<Entidade>();
        //    }
        //    return new List<Entidade>();

        //}

        //private void ValidarEntidadeParaDeletar(Entidade entidade)
        //{
        //    if (entidade is IDeletado entidadeDeletada)
        //    {
        //        if (entidadeDeletada.IsDeletado)
        //        {
        //            if (!DebugUtil.IsAttached)
        //            {
        //                throw new Erro($"A entidade do tipo '{entidade.GetType().Name}' (id: {entidade.Id}) já foi deletada");
        //            }
        //        }
        //    }

        //}


    }
}
