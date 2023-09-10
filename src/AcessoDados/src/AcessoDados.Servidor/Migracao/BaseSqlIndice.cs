using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio.Atributos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snebur.AcessoDados
{
    internal abstract class BaseSqlIndice : SqlMigracao
    {
        internal bool IsUnico { get; }
        internal List<PropriedadeIndexar> PropriedadesIndexar { get; }
        internal List<FiltroPropriedadeIndexar> PropriedadesFiltros { get; }
        private List<CampoFiltro> CamposFiltros { get; } = new List<CampoFiltro>();

        internal BaseSqlIndice(EstruturaEntidade estruturaEntidade,
                              List<PropriedadeIndexar> propriedadesIndexar,
                              List<FiltroPropriedadeIndexar> filtros,
                              bool isUnico) : base(estruturaEntidade,
                              propriedadesIndexar.Select(x => x.Propriedade).ToList())
        {
            if (filtros?.Count > 0 && !isUnico)
            {
                throw new Erro("Filtros são exclusivo para validações de único");
            }

            this.PropriedadesIndexar = propriedadesIndexar;
            this.IsUnico = isUnico;

            if (this.IsUnico)
            {
                this.PropriedadesFiltros = filtros;
                this.PopularCamposFiltros();
            }
        }

        private void PopularCamposFiltros()
        {
            //if (this.EstruturaEntidade.IsImplementaInterfaceIDeletado)
            //{
            //    var estruturaCampoIsDeletado = this.EstruturaEntidade.EstruturaCampoDelatado;
            //    var expressao = $" ( {estruturaCampoIsDeletado.NomeCampo} = 0 )";
            //    this.CamposFiltros.Add(new CampoFiltro {
            //        Campo = estruturaCampoIsDeletado.NomeCampo,
            //        Expressao = expressao 
            //    });
            //}

            foreach (var propriedadeIndexar in this.PropriedadesIndexar.Where(x => !x.IsPermitirNulo))
            {
                var estruturaCampo = this.RetornarEstruturaCampo(propriedadeIndexar.Propriedade);
                if (estruturaCampo.IsAceitaNulo)
                {
                    var expressao = $" ( {estruturaCampo.NomeCampo} IS NOT NULL)";
                    this.CamposFiltros.Add(new CampoFiltro { 
                        Campo = estruturaCampo.NomeCampo, 
                        Expressao = expressao 
                    });
                }
            }
             
            if (this.PropriedadesFiltros?.Count > 0)
            {
                foreach (var propriedadeFiltro in this.PropriedadesFiltros)
                {
                    var estruturaCampo = this.RetornarEstruturaCampo(propriedadeFiltro.Propriedade);
                    var expressao = this.RetornarFiltroEspressaoPropriedade(estruturaCampo, propriedadeFiltro);
                    this.CamposFiltros.Add(new CampoFiltro { Campo = estruturaCampo.NomeCampo, Expressao = expressao });
                }
            }
        }

        protected override string RetornarSql_PostgreSQL()
        {
            var nomeIndice = this.RetornarNomeIndice();
            var sb = new StringBuilder();

            var campos = String.Join(",", this.Campos.Select(x => String.Format("\"{0}\" ASC NULLS FIRST", x)));

            sb.Append("CREATE ");

            if (this.IsUnico)
            {
                sb.Append(" UNIQUE ");
            }
            sb.Append(String.Format(" INDEX  IF NOT EXISTS {0} ON \"{1}\"({2}) ;", nomeIndice, this.NomeTabela, campos));

            return sb.ToString();
        }

        protected override string RetornarSql_SqlServer()
        {
            var nomeIndice = this.RetornarNomeIndice();
            var sb = new StringBuilder();
            sb.Append(String.Format(" IF NOT EXISTS (select * from sys.indexes where object_id = OBJECT_ID(N'[{0}].[{1}]') AND name = N'{2}') ", this.Schema, this.NomeTabela, nomeIndice));
            sb.Append("CREATE ");

            if (this.IsUnico)
            {
                sb.Append(" UNIQUE ");
            }
            sb.Append(" NONCLUSTERED INDEX ");
            sb.Append(String.Format("[{0}]", nomeIndice));
            sb.Append(String.Format(" ON  [{0}].[{1}] ", this.Schema, this.NomeTabela));
            sb.AppendLine();
            sb.Append(" ( ");
            sb.Append(String.Join(",", this.Campos.Select(x => String.Format(" [{0}] ASC", x))));
            sb.Append(" ) ");
            sb.AppendLine();

            if (this.CamposFiltros.Count > 0)
            {
                sb.AppendLine(" WHERE ");
                sb.Append(" ( ");
                sb.Append(String.Join(" AND ", this.CamposFiltros.Select(x => x.Expressao)));
                sb.Append(" ) ");

            }
            if (!String.IsNullOrWhiteSpace(this.GrupoArquivoIndices))
            {
                sb.Append($" ON [{this.GrupoArquivoIndices}]");
            }

            return sb.ToString();
        }

        private string RetornarNomeIndice()
        {
            var prefixo = (this.IsUnico) ? "UNQ" : "IX";
            var campos = this.ConcatenarCampos();
            var nomeIndice = $"{prefixo}_{this.Schema}_{this.NomeTabela}_{campos}";
            if (nomeIndice.Length > 128)
            {
                var hash = nomeIndice.GetHashCode().ToString();
                nomeIndice = nomeIndice.Substring(0, 128 - hash.Length) + hash;
            }
            return nomeIndice;
        }

        private string ConcatenarCampos()
        {
            if (this.CamposFiltros.Count > 0)
            {
                return String.Join("_", this.Campos.Union(this.CamposFiltros.Select(x => x.Campo)).Distinct());
            }
            return String.Join("_", this.Campos);
        }

        private string RetornarFiltroEspressaoPropriedade(EstruturaCampo estruturaCampo,
                                                          FiltroPropriedadeIndexar propriedadeFiltro)
        {
            var operador = propriedadeFiltro.OperadoprString;
            if(propriedadeFiltro.Valor == "null" || propriedadeFiltro.Valor == null)
            {
                return $" {estruturaCampo.NomeCampo} IS NULL ";
            }
            return $" {estruturaCampo.NomeCampo} {operador} {propriedadeFiltro.Valor} ";
        }

        private class CampoFiltro
        {
            public string Campo { get; set; }
            public string Expressao { get; set; }
        }
    }
}