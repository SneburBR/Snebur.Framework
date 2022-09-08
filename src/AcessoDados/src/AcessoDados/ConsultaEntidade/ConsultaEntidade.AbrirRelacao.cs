using Snebur.AcessoDados.Ajudantes;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Snebur.AcessoDados
{
    public partial class ConsultaEntidade<TEntidade> where TEntidade : IEntidade
    {
        //public ConsultaEntidade<TEntidade> AbrirRelacaoTipada<TRelacao>(Expression<Func<TEntidade, TRelacao>> expressao, Expression<Func<TRelacao, object>> expressaoPropriedade) where TRelacao : IEntidade
        //{
        //    var caminhoRelacaoTipada = 
        //    throw new NotImplementedException();
        //}

        public ConsultaEntidade<TEntidade> AbrirRelacao(List<PropertyInfo> propriedades)
        {
            return this.AbrirRelacao(this.EstruturaConsulta, propriedades, false);
        }

        public ConsultaEntidade<TEntidade> AbrirRelacao(string caminhoPropriedade)
        {
            return this.AbrirRelacao(this.EstruturaConsulta, caminhoPropriedade, false);
        }

        public ConsultaEntidade<TEntidade> AbrirRelacao<TRelacao>(Expression<Func<TEntidade, TRelacao>> expressao) where TRelacao : IEntidade
        {
            var expressaoBase = (Expression)expressao;
            return this.AbrirRelacao(this.EstruturaConsulta, expressaoBase, false);
        }

        public ConsultaEntidade<TEntidade> AbrirRelacao(Expression expressao)
        {
            return this.AbrirRelacao(this.EstruturaConsulta, expressao, false);
        }

        public ConsultaEntidade<TEntidade> AbrirRelacoes<TRelacao>(params Expression<Func<TEntidade, TRelacao>>[] expressoes) where TRelacao : IEntidade
        {
            foreach (var expressao in expressoes)
            {
                this.AbrirRelacao(expressao);
            }
            return this;
        }

        public ConsultaEntidade<TEntidade> AbrirRelacoes(params string[] caminhosPropriedade)
        {
            foreach (var caminhoPropriedade in caminhosPropriedade)
            {
                this.AbrirRelacao(caminhoPropriedade);
            }
            return this;
        }

        public ConsultaEntidade<TEntidade> AbrirRelacoes(params Expression[] expressoes)
        {
            foreach (var expressao in expressoes)
            {
                this.AbrirRelacao(this.EstruturaConsulta, expressao, false);
            }
            return this;
        }

        private ConsultaEntidade<TEntidade> AbrirRelacao(EstruturaConsulta estruturaConsulta, Expression expressao, bool filtro)
        {
            var propriedades = ExpressaoUtil.RetornarPropriedades(expressao);
            return this.AbrirRelacao(estruturaConsulta, propriedades, filtro);
        }

        private ConsultaEntidade<TEntidade> AbrirRelacaoFiltro(EstruturaConsulta estruturaConsulta, string caminhoPropriedade)
        {
            return this.AbrirRelacao(estruturaConsulta, caminhoPropriedade, true);
        }

        private ConsultaEntidade<TEntidade> AbrirRelacao(EstruturaConsulta estruturaConsulta, string caminhoPropriedade, bool filtro)
        {
            var tipoEntidadeConsulta = this.TipoEntidadeConsulta.IsSubclassOf(estruturaConsulta.TipoEntidadeConsulta) ? this.TipoEntidadeConsulta : estruturaConsulta.TipoEntidadeConsulta;
            var propriedades = ReflexaoUtil.RetornarPropriedadesCaminho(tipoEntidadeConsulta, caminhoPropriedade);
            return this.AbrirRelacao(estruturaConsulta, propriedades, filtro);
        }



        private ConsultaEntidade<TEntidade> AbrirRelacao(EstruturaConsulta estruturaConsulta, List<PropertyInfo> propriedades, bool filtro)
        {
            var propriedadesCaminho = new List<PropertyInfo>();
            var estruturaConsultaAtual = estruturaConsulta;

            foreach (var p in propriedades)
            {
                var propriedade = this.NormalizarPropriedadeRelacaoEspecializda(p);
                propriedadesCaminho.Add(propriedade);

                var caminhoPropriedadeParcial = AjudanteConsultaEntidade.RetornarCaminhoPropriedade(propriedadesCaminho);

                ErroUtil.ValidarStringVazia(caminhoPropriedadeParcial, nameof(caminhoPropriedadeParcial));

                //if (!consultaAcessoDadosAtual.RelacoesAberta.ContainsKey(caminhoPropriedadeParcial))
                if (this.ExisteRelacaoAberta(estruturaConsultaAtual, caminhoPropriedadeParcial, filtro))
                {
                    if (AjudanteConsultaEntidade.PropriedadeRetornarListaEntidade(propriedade))
                    {
                        estruturaConsultaAtual = estruturaConsultaAtual.ColecoesAberta[caminhoPropriedadeParcial].EstruturaConsulta;
                        propriedadesCaminho.Clear();
                    }
                }
                else
                {
                    if (propriedade.PropertyType.IsSubclassOf(typeof(Entidade)) ||
                        ReflexaoUtil.TipoImplementaInterface(propriedade.PropertyType, typeof(IEntidade), false))
                    {
                        var tipoEntidade = propriedade.PropertyType;
                        if (tipoEntidade.IsInterface)
                        {
                            propriedade = AjudanteConsultaEntidade.RetornarPropriedadeInterface(estruturaConsultaAtual.TipoEntidadeConsulta, propriedade);
                            tipoEntidade = propriedade.PropertyType;

                            if (!(tipoEntidade.IsSubclassOf(typeof(Entidade))))
                            {
                                throw new InvalidOperationException($"O tipo da propriedade da relação não um entidade");
                            }
                        }
                        var relacaoAbertaEntidade = new RelacaoAbertaEntidade
                        {
                            CaminhoPropriedade = caminhoPropriedadeParcial,
                            NomeTipoEntidade = tipoEntidade.Name,
                            TipoEntidadeAssemblyQualifiedName = tipoEntidade.AssemblyQualifiedName,
                            NomeTipoDeclarado = propriedade.DeclaringType?.Name,
                            TipoDeclaradoAssemblyQualifiedName = propriedade.DeclaringType?.AssemblyQualifiedName
                        };

                        if (filtro)
                        {
                            estruturaConsultaAtual.RelacoesAbertaFiltro.Add(caminhoPropriedadeParcial, relacaoAbertaEntidade);
                        }
                        else
                        {
                            estruturaConsultaAtual.RelacoesAberta.Add(caminhoPropriedadeParcial, relacaoAbertaEntidade);
                        }
                    }
                    else if (AjudanteConsultaEntidade.PropriedadeRetornarListaEntidade(propriedade))
                    {
                        if (filtro)
                        {
                            throw new ErroNaoSuportado("Não é posivel suporta abrir realação filtro nas coleções");
                        }
                        var tipoPropridadeItemBaseEntidade = AjudanteConsultaEntidade.RetornarTipoListaEntidade(estruturaConsultaAtual.TipoEntidadeConsulta, propriedade);

                        var relacaoAbertaListaEntidade = new RelacaoAbertaColecao
                        {
                            CaminhoPropriedade = caminhoPropriedadeParcial,
                            NomeTipoEntidade = tipoPropridadeItemBaseEntidade.Name,
                            TipoEntidadeAssemblyQualifiedName = tipoPropridadeItemBaseEntidade.AssemblyQualifiedName,
                            NomeTipoDeclarado = propriedade.DeclaringType.Name,
                            TipoDeclaradoAssemblyQualifiedName = propriedade.DeclaringType.AssemblyQualifiedName
                        };

                        relacaoAbertaListaEntidade.EstruturaConsulta.TipoEntidadeConsulta = tipoPropridadeItemBaseEntidade;
                        relacaoAbertaListaEntidade.EstruturaConsulta.NomeTipoEntidade = tipoPropridadeItemBaseEntidade.Name;
                        relacaoAbertaListaEntidade.EstruturaConsulta.TipoEntidadeAssemblyQualifiedName = tipoPropridadeItemBaseEntidade.AssemblyQualifiedName;

                        estruturaConsultaAtual.ColecoesAberta.Add(caminhoPropriedadeParcial, relacaoAbertaListaEntidade);

                        estruturaConsultaAtual = relacaoAbertaListaEntidade.EstruturaConsulta; propriedadesCaminho.Clear();
                    }
                    else
                    {
                        if (!filtro)
                        {
                            var mensagemErro = $"O tipo {propriedade.PropertyType.Name} da Propriedade {propriedade.Name} não  é uma relação ";
                            throw new Erro(mensagemErro);
                        }
                    }
                }
            }
            return this;
        }

        private PropertyInfo NormalizarPropriedadeRelacaoEspecializda(PropertyInfo propriedade)
        {
            var atributoPropriedadeEspecializada = propriedade.GetCustomAttribute<PropriedadeTSEspecializadaAttribute>();
            if (atributoPropriedadeEspecializada != null)
            {
                var propridadeEspecializada = propriedade.DeclaringType.GetProperty(atributoPropriedadeEspecializada.NomePropriedade);
                if (propridadeEspecializada == null)
                {
                    throw new Erro($"A propriedade {propriedade.Name} com especializacao especializada  {atributoPropriedadeEspecializada.NomePropriedade} não foi encontrada,talvez não seja uma relação mapeaada.");
                }
                return this.NormalizarPropriedadeRelacaoEspecializda(propridadeEspecializada);
            }
            return propriedade;
        }

        private bool ExisteRelacaoAberta(EstruturaConsulta estruturaConsultaAtual, string caminhoPropreidade, bool filtro)
        {
            if (filtro)
            {
                return estruturaConsultaAtual.RelacoesAbertaFiltro.ContainsKey(caminhoPropreidade);
            }
            else
            {
                return estruturaConsultaAtual.RelacoesAberta.ContainsKey(caminhoPropreidade) || estruturaConsultaAtual.ColecoesAberta.ContainsKey(caminhoPropreidade);
            }
        }
    }
}