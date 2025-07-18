using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class EntidadesAlteracaoPropriedade : BaseEnitdadesAlteracaoPropriedade
    {
        public EntidadesAlteracaoPropriedade(BaseContextoDados contexto ) : base(contexto)
        {

        }

        internal HashSet<Entidade> RetornarEntidadesAlteracaoPropriedade(HashSet<Entidade> entidades)
        {
            var alteracoesPropriedade = new HashSet<Entidade>();
            foreach (var entidade in entidades)
            {
                //entidade.__PropriedadesAlteradas
                var estruturaEntidade = this.Contexto.EstruturaBancoDados.EstruturasEntidade[entidade.GetType().Name];
                foreach (var estruturaAlteracaoPropriedade in estruturaEntidade.TodasEstruturasAlteracaoPropriedade)
                {
                    var (isExisteAltracao, valorAntigo) = this.IsExisteAlteracaoPropriedade(entidade, estruturaAlteracaoPropriedade);
                    if (isExisteAltracao)
                    {
                        var valorPropriedade = estruturaAlteracaoPropriedade.Propriedade.GetValue(entidade);
                        var atributo = estruturaAlteracaoPropriedade.Atributo;

                        if (estruturaAlteracaoPropriedade.IsSalvarDataHoraFimAlteracao)
                        {
                            var ultimaAlteracao = this.RetornarUtlimaAlteracao(entidade, atributo);
                            if (ultimaAlteracao != null)
                            {
                                ultimaAlteracao.DataHoraFimAlteracao = DateTime.UtcNow;
                                alteracoesPropriedade.Add((Entidade)ultimaAlteracao);
                            }
                        }
                         
                        var novaAlteracao = (IAlteracaoPropriedade)Activator.CreateInstance(atributo.TipoEntidadeAlteracaoPropriedade);
                        novaAlteracao.ValorPropriedadeRelacao = entidade;
                        novaAlteracao.ValorPropriedadeAlterada = valorPropriedade;
                        novaAlteracao.ValorPropriedadeAntigo = valorAntigo;
                        novaAlteracao.Usuario = this.Contexto.UsuarioLogado;
                        novaAlteracao.SessaoUsuario = this.Contexto.SessaoUsuarioLogado;

                        alteracoesPropriedade.Add((Entidade)novaAlteracao);
                    }
                }
            }
            return alteracoesPropriedade;
        }

        private IAlteracaoPropriedade RetornarUtlimaAlteracao(Entidade entidade,
                                                              NotificarAlteracaoPropriedadeAttribute atributo)
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

    }
}
