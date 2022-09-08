using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Snebur.AcessoDados.Mapeamento
{
    internal partial class MapeamentoConsulta :
                            BaseMapeamentoConsulta
    {

        private Dictionary<long, Entidade> Entidades { get; }

        public DicionarioEstrutura<MapeamentoConsultaRelacaoAberta> MapeamentosRelacaoAberta { get; }

        private long TotalRegistro { get; set; }

        internal MapeamentoConsulta(EstruturaConsulta estruturaConsulta,
                                   EstruturaBancoDados estruturaBancoDados,
                                   BaseConexao ConexaoDB, BaseContextoDados contexto) :
                                   base(estruturaConsulta, estruturaBancoDados, ConexaoDB, contexto)
        {
            this.Entidades = new Dictionary<long, Entidade>();
            this.MapeamentosRelacaoAberta = new DicionarioEstrutura<MapeamentoConsultaRelacaoAberta>();

            this.TipoEntidade = this.EstruturaConsulta.RetornarTipoEntidade();
            this.EstruturaEntidade = this.EstruturaBancoDados.EstruturasEntidade[this.TipoEntidade.Name];

            this.MontarMapamentosConsultaRelacaoAberta();
        }

        internal string RetornarSql()
        {
            if (this is MapeamentoConsultaRelacaoAbertaNn)
            {
                if (DebugUtil.IsAttached)
                {
                    throw new NotImplementedException("debugar linha por linha");
                }
            }
            using (var mapeamentoEntidade = new MapeamentoEntidade(this, this.EstruturaEntidade, this.EstruturaBancoDados, this.ConexaoDB, this.Contexto))
            {
                return mapeamentoEntidade.RetornarSql(true, true, new FiltroMapeamentoVazio());
            }
        }

        internal ResultadoConsulta RetornarResultadoConsulta()
        {
            //Agrupar as consultas

            var resultadoConsulta = new ResultadoConsulta();
            this.ExecutarConsulta(new FiltroMapeamentoVazio());

            resultadoConsulta.Entidades.AddRange(this.Entidades.Values);
            resultadoConsulta.Entidades.IsAberta = true;
            if (this.EstruturaConsulta.ContarRegistros)
            {
                resultadoConsulta.TotalRegistros = this.RetornarContagem(new FiltroMapeamentoVazio());
            }
            //if (this.EstruturaConsulta.PaginaAtual > 0)
            //{
            //    var totalRegistros = this.RetornarContagem(new FiltroMapeamentoVazio());
            //    resultadoConsulta.PaginacaoConsulta = PaginacaoUtil.RetornarPaginacaoConsulta(totalRegistros, this.EstruturaConsulta.Take, this.EstruturaConsulta.PaginaAtual);
            //}
            return resultadoConsulta;
        }

        internal Dictionary<long, Entidade> RetornarEntidades(BaseFiltroMapeamento filtroMapeamento)
        {
            this.ExecutarConsulta(filtroMapeamento);
            return this.Entidades;
        }

        private void ExecutarConsulta(BaseFiltroMapeamento filtroMapeamento)
        {
            if (!this.EstruturaEntidade.IsAbstrata && !this.EstruturaEntidade.IsPossuiEntidadesEspecializacao)
            {
                var entidades = this.RetornarEntidades(this.EstruturaEntidade, filtroMapeamento);
                foreach (var entidade in entidades)
                {
                    this.Entidades.Add(entidade.Id, entidade);
                }
            }
            else
            {
                using (var mapeamentoEntidade = new MapeamentoEntidade(this, this.EstruturaEntidade, this.EstruturaBancoDados, this.ConexaoDB, this.Contexto))
                {
                    var entidades = new ListaEntidades<Entidade>();
                    var idsTipoEntidade = this.RetornarIdsTipoEntidade(mapeamentoEntidade, filtroMapeamento);

                    //foi alterado pelo  motivo do filtro da indentificador da instancia
                    //uma vez o no  fo filtros do ids, o filtro do identificador da instancia por esta numa classe especializada
                    //foreach (var idTipoEntidade in idsTipoEntidade)
                    //{
                    //    this.Entidades.Add(idTipoEntidade.Id, null);
                    //}

                    var grupos = new List<GrupoIdTipoEntidade>();

                    foreach (var grupo in idsTipoEntidade.GroupBy(x => x.__NomeTipoEntidade))
                    {
                        var nomeTipoEntidade = grupo.Key;
                        if (String.IsNullOrEmpty(nomeTipoEntidade))
                        {
                            throw new Exception($"Não foi definido o nome tipo entidade {this.EstruturaEntidade.TipoEntidade.Name}");
                        }
                        var idsUnico = new SortedSet<long>(grupo.Select(x => x.Id));
                        grupos.Add(new GrupoIdTipoEntidade(idsUnico, nomeTipoEntidade));
                    }
                    foreach (var grupo in grupos)
                    {
                        var estruturaTipoEntidade = this.EstruturaBancoDados.RetornarEstruturaEntidade(grupo.NomeTipoEntidade);

                        var menorId = grupo.Ids.First();
                        var maiorId = grupo.Ids.Last();

                        if (DebugUtil.IsAttached)
                        {
                            if (menorId != grupo.Ids.Min())
                            {
                                throw new Exception("menorId != grupo.Ids.Min()");
                            }
                            if (maiorId != grupo.Ids.Max())
                            {
                                throw new Exception("menorId != grupo.Ids.Min()");
                            }
                        }
                        var filtroMapeamentoEntre = new FiltroMapeamentoEntre(filtroMapeamento,
                            menorId, maiorId, grupo.NomeTipoEntidade);

                        var filtroMapeamentoId = new FiltroMapeamentoIds(filtroMapeamento, grupo.Ids);

                        var entidadesEspecializada = this.RetornarEntidades(estruturaTipoEntidade, filtroMapeamentoId);

                        //if (entidadesEspecializada.Count != grupo.Ids.Count)
                        //{
                        //    throw new ErroOperacaoInvalida(String.Format("O numero de entidade retornada não confere {0} - {1}", entidadesEspecializada.Count, grupo.Ids.Count));
                        //}
                        foreach (var entidadeEspecializada in entidadesEspecializada)
                        {
                            //alterado 
                            this.Entidades.Add(entidadeEspecializada.Id, entidadeEspecializada);
                        }
                    }
                }
            }
            this.AbrirRelacoes();
        }

        private ListaEntidades<Entidade> RetornarEntidades(EstruturaEntidade estruturaEntidade, BaseFiltroMapeamento filtro)
        {
            using (var mapeamentoEntidade = new MapeamentoEntidade(this, estruturaEntidade, this.EstruturaBancoDados, this.ConexaoDB, this.Contexto))
            {
                return mapeamentoEntidade.RetornarEntidades(filtro);
            }
        }

        private List<IdTipoEntidade> RetornarIdsTipoEntidade(MapeamentoEntidade mapeamentoEntidade, BaseFiltroMapeamento filtroMapeamento)
        {
            if (filtroMapeamento is FiltroMapeamentoIds && ((FiltroMapeamentoIds)filtroMapeamento).Ids.Count > 500)
            {
                return this.RetornarIdsTipoEntidadeMuitosIds(mapeamentoEntidade, (FiltroMapeamentoIds)filtroMapeamento);
            }
            else
            {
                return mapeamentoEntidade.RetornarIdTipoEntidade(filtroMapeamento);
            }
        }

        private List<IdTipoEntidade> RetornarIdsTipoEntidadeMuitosIds(MapeamentoEntidade mapeamentoEntidade, FiltroMapeamentoIds filtroMapeamento)
        {
            var ids = filtroMapeamento.Ids;
            var menorId = ids.Min();
            var maiorId = ids.Max();

            var filtroEntre = new FiltroMapeamentoEntre(filtroMapeamento.EstruturaCampoFiltro, menorId, maiorId);
            var idsTipoEntidade = mapeamentoEntidade.RetornarIdTipoEntidade(filtroEntre);

            var resultado = new List<IdTipoEntidade>();
            foreach (var idTipoEntidade in idsTipoEntidade)
            {
                var valorId = (filtroMapeamento.EstruturaCampoFiltro == null) ? idTipoEntidade.Id : idTipoEntidade.CampoFiltro;

                if (ids.Contains(valorId))
                {
                    resultado.Add(idTipoEntidade);
                }
            }
            return resultado;
        }

        private int RetornarContagem(BaseFiltroMapeamento filtro)
        {
            using (var mapeamentoEntidade = new MapeamentoEntidade(this, this.EstruturaEntidade, this.EstruturaBancoDados, this.ConexaoDB, this.Contexto))
            {
                return mapeamentoEntidade.RetornarContagem(filtro);
            }
        }
        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();
            this.Entidades.Clear();
            //this.Entidades = null;
        }
        #endregion
    }
}