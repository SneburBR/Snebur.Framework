using Snebur.AcessoDados.Ajudantes;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using Snebur.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ConsultaEntidade<TEntidade> AbrirRelacoes(params Expression<Func<TEntidade, object>>[] expressoes) 
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



        private ConsultaEntidade<TEntidade> AbrirRelacao(EstruturaConsulta estruturaConsulta,
                                                         List<PropertyInfo> propriedades,
                                                         bool isFiltro)
        {
            var propriedadesCaminhoParcial = new List<PropertyInfo>();
            var propriedadesCaminhoComplemento = new List<PropertyInfo>();
            var estruturaConsultaAtual = estruturaConsulta;

            foreach (var p in propriedades)
            {
                var propriedade = this.NormalizarPropriedadeRelacaoEspecializda(p);
                propriedadesCaminhoParcial.Add(propriedade);
                propriedadesCaminhoComplemento.Add(propriedade);

                var caminhoPropriedadeParcial = AjudanteConsultaEntidade.RetornarCaminhoPropriedade(propriedadesCaminhoParcial);
                var caminhoPropriedadeCompleto = AjudanteConsultaEntidade.RetornarCaminhoPropriedade(propriedadesCaminhoComplemento);
                var caminhoPropriedade = AjudanteConsultaEntidade.RetornarCaminhoPropriedade(propriedadesCaminhoParcial);

                ErroUtil.ValidarStringVazia(caminhoPropriedadeParcial, nameof(caminhoPropriedadeParcial));

                //if (!consultaAcessoDadosAtual.RelacoesAberta.ContainsKey(caminhoPropriedadeParcial))
                if (this.IsExisteRelacaoAberta(estruturaConsultaAtual, caminhoPropriedadeParcial, isFiltro))
                {
                    this.ValidarPropriedadeMapeamento(propriedade,
                                                      estruturaConsultaAtual,
                                                      caminhoPropriedadeParcial,
                                                      caminhoPropriedadeCompleto,
                                                      isFiltro);

                    if (AjudanteConsultaEntidade.IsPropriedadeRetornarListaEntidade(propriedade))
                    {
                        estruturaConsultaAtual = estruturaConsultaAtual.ColecoesAberta[caminhoPropriedadeParcial].EstruturaConsulta;
                        propriedadesCaminhoParcial.Clear();
                    }
                }
                else
                {
                    if (propriedade.PropertyType.IsSubclassOf(typeof(Entidade)) ||
                        ReflexaoUtil.IsTipoImplementaInterface(propriedade.PropertyType, typeof(IEntidade), false))
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
                            Propriedade = propriedade,
                            CaminhoPropriedade = caminhoPropriedadeParcial,
                            NomeTipoEntidade = tipoEntidade.Name,
                            //TipoEntidadeAssemblyQualifiedName = tipoEntidade.AssemblyQualifiedName,
                            TipoEntidadeAssemblyQualifiedName = tipoEntidade.RetornarAssemblyQualifiedName(),
                            NomeTipoDeclarado = propriedade.DeclaringType?.Name,
                            TipoDeclaradoAssemblyQualifiedName = propriedade.DeclaringType?.RetornarAssemblyQualifiedName()
                            //TipoDeclaradoAssemblyQualifiedName = propriedade.DeclaringType?.AssemblyQualifiedName
                        };

                        if (isFiltro)
                        {
                            estruturaConsultaAtual.RelacoesAbertaFiltro.Add(caminhoPropriedadeParcial, relacaoAbertaEntidade);
                        }
                        else
                        {
                            estruturaConsultaAtual.RelacoesAberta.Add(caminhoPropriedadeParcial, relacaoAbertaEntidade);
                        }
                    }
                    else if (AjudanteConsultaEntidade.IsPropriedadeRetornarListaEntidade(propriedade))
                    {
                        if (isFiltro)
                        {
                            throw new ErroNaoSuportado("Não é possível suporta abrir relação filtro nas coleções");
                        }
                        var tipoPropridadeItemBaseEntidade = AjudanteConsultaEntidade.RetornarTipoListaEntidade(estruturaConsultaAtual.TipoEntidadeConsulta, propriedade);

                        var relacaoAbertaListaEntidade = new RelacaoAbertaColecao
                        {
                            Propriedade = propriedade,
                            CaminhoPropriedade = caminhoPropriedadeParcial,
                            NomeTipoEntidade = tipoPropridadeItemBaseEntidade.Name,
                            TipoEntidadeAssemblyQualifiedName = tipoPropridadeItemBaseEntidade.RetornarAssemblyQualifiedName(),
                            //TipoEntidadeAssemblyQualifiedName = tipoPropridadeItemBaseEntidade.AssemblyQualifiedName,
                            NomeTipoDeclarado = propriedade.DeclaringType.Name,
                            TipoDeclaradoAssemblyQualifiedName = propriedade.DeclaringType.RetornarAssemblyQualifiedName(),
                            EstruturaConsulta = new EstruturaConsulta
                            {
                                IsIncluirDeletados = this.EstruturaConsulta.IsIncluirDeletados,
                                IsIncluirInativos = this.EstruturaConsulta.IsIncluirInativos,
                                TipoEntidadeConsulta = tipoPropridadeItemBaseEntidade,
                                NomeTipoEntidade = tipoPropridadeItemBaseEntidade.Name,
                                TipoEntidadeAssemblyQualifiedName = tipoPropridadeItemBaseEntidade.RetornarAssemblyQualifiedName()
                            }
                        };

                        estruturaConsultaAtual.ColecoesAberta.Add(caminhoPropriedadeParcial, relacaoAbertaListaEntidade);
                        estruturaConsultaAtual = relacaoAbertaListaEntidade.EstruturaConsulta;
                        propriedadesCaminhoParcial.Clear();
                    }
                    else
                    {
                        if (!isFiltro)
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

        private bool IsExisteRelacaoAberta(EstruturaConsulta estruturaConsultaAtual,
                                           string caminhoPropreidade, bool isFiltro)
        {
            if (isFiltro)
            {
                return estruturaConsultaAtual.RelacoesAbertaFiltro.ContainsKey(caminhoPropreidade);
            }
            else
            {
                return estruturaConsultaAtual.RelacoesAberta.ContainsKey(caminhoPropreidade) ||
                       estruturaConsultaAtual.ColecoesAberta.ContainsKey(caminhoPropreidade);
            }
        }

        private void ValidarPropriedadeMapeamento(PropertyInfo propriedade,
                                                  EstruturaConsulta estruturaConsultaAtual,
                                                  string caminhoPropreidade,
                                                  string caminhoPropriedadeCompleto,
                                                  bool isFiltro)
        {
            var mapemaneto = this.RetornarMapeamento(estruturaConsultaAtual,
                                                     caminhoPropreidade,
                                                     isFiltro);

            if (mapemaneto.Propriedade.DeclaringType != propriedade.DeclaringType)
            {
                var tipos = new Type[] { propriedade .DeclaringType,
                                         mapemaneto.Propriedade.DeclaringType };


                var mensagem = $"Ambiguidade no caminho '{caminhoPropriedadeCompleto}' para abrir relação.\r\n" +
                               $"Tipos  {String.Join(", ", tipos.Select(x => x.Name))} com mesmo caminho.\r\n" +
                               $"Suporte para ambiguidade de caminhos não implementado.\r\n" +
                               $"Sugestão: Separar as consultas.";

                throw new Exception(mensagem);
            }
        }

        private BaseRelacaoAberta RetornarMapeamento(EstruturaConsulta estruturaConsultaAtual,
                                                     string caminhoPropreidade,
                                                     bool isFiltro)
        {
            if (isFiltro)
            {
                return estruturaConsultaAtual.RelacoesAbertaFiltro.GetValueOrDefault(caminhoPropreidade);
            }

            if (estruturaConsultaAtual.RelacoesAberta.ContainsKey(caminhoPropreidade))
            {
                return estruturaConsultaAtual.RelacoesAberta[caminhoPropreidade];
            }
            if (estruturaConsultaAtual.ColecoesAberta.ContainsKey(caminhoPropreidade))
            {
                return estruturaConsultaAtual.ColecoesAberta[caminhoPropreidade];
            }

            throw new Exception($"mapeamento não encontrado {estruturaConsultaAtual}.{caminhoPropreidade}");
        }
    }
}