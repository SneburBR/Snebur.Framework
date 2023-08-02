using Snebur.AcessoDados.Estrutura;
using Snebur.AcessoDados.Seguranca;
using Snebur.Dominio;
using Snebur.Reflexao;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class AjudanteDataSetMapeamento
    {
        internal static List<IdTipoEntidade> MapearIdTipoEntidade(DataTable dataTable)
        {
            var lenRows = dataTable.Rows.Count;
            var resultado = new List<IdTipoEntidade>();

            var existeCampoFiltro = dataTable.Columns.Count == 3;

            for (var i = 0; i < lenRows; i++)
            {
                var linha = dataTable.Rows[i];
                var idTipoEntidade = new IdTipoEntidade
                {
                    Id = Convert.ToInt64(linha[0]),
                    __NomeTipoEntidade = linha[1].ToString(),
                };

                if (existeCampoFiltro)
                {
                    idTipoEntidade.CampoFiltro = Convert.ToInt64(linha[2]);
                }
                resultado.Add(idTipoEntidade);
            }
            return resultado;
        }

        internal static ListaEntidades<Entidade> MapearDataTable(IEstruturaConsultaSeguranca estruturaConsulta, DataTable dataTable, MapeamentoEntidade mapeamento)
        {
            var entidades = new ListaEntidades<Entidade>();

            var estruturasColuna = new List<EstruturaColuna>();
            foreach (var estruturaCampoApelido in mapeamento.EstruturaEntidadeApelido.EstruturasCampoApelido)
            {
                var nomeColuna = estruturaCampoApelido.Apelido.Substring(1, estruturaCampoApelido.Apelido.Length - 2);

                if (dataTable.Columns.Contains(nomeColuna))
                {
                    var coluna = dataTable.Columns[nomeColuna];
                    var posicao = dataTable.Columns.IndexOf(coluna);
                    estruturasColuna.Add(new EstruturaColuna(posicao, coluna, estruturaCampoApelido));
                }
                else
                {
                    if (estruturaConsulta.PropriedadesAbertas.Count == 0)
                    {
                        throw new Erro(String.Format("Não foi encontrado na coluna no mapeamento do DataSet {0} ", nomeColuna));
                    }
                }
            }
            var totalLinhas = dataTable.Rows.Count;
            var lenColunas = estruturasColuna.Count;

            for (var i = 0; i < totalLinhas; i++)
            {
                var linha = dataTable.Rows[i];

                var entidade = (IEntidadeInterna)Activator.CreateInstance(mapeamento.TipoEntidade);
                //Campos

                for (var j = 0; j < lenColunas; j++)
                {
                    var estruturaColuna = estruturasColuna[j];
                    var valorDB = linha[estruturaColuna.Posicao];
                  
                    var valorPropriedade = AjudanteDataSetMapeamento.RetornarValorConvertido(valorDB, estruturaColuna);
                    if (estruturaColuna.IsTipoComplexo)
                    {
                        var tipoComplexo = estruturaColuna.EstruturaCampo.PropriedadeTipoComplexo.GetValue(entidade);
                        if (tipoComplexo == null)
                        {
                            throw new Erro($"O a propriedade {estruturaColuna.EstruturaCampo.PropriedadeTipoComplexo.Name} <{estruturaColuna.EstruturaCampo.PropriedadeTipoComplexo.PropertyType.Name}> na entidade {mapeamento.TipoEntidade.Name}  não foi instanciada");
                        }
                        estruturaColuna.Propriedade.SetValue(tipoComplexo, valorPropriedade);
                    }
                    else
                    {
                        
                        estruturaColuna.Propriedade.SetValue(entidade, valorPropriedade);
                    }
                }
                //Teste - o momento extado de começar a rastrear as propriedade alteradas

                entidade.AtribuirPropriedadesAbertas(estruturaConsulta.PropriedadesAbertas);
                entidade.AtribuirPropriedadesAutorizadas(estruturaConsulta.PropriedadesAutorizadas);
                entidade.AtivarControladorPropriedadeAlterada();

                entidades.Add(entidade as IEntidade);
            }
            return entidades;
        }

 

        internal static object RetornarValorConvertido(object valorDB, 
                                                       EstruturaColuna estruturaColuna)
        {
            if (ReflexaoUtil.TipoIgualOuHerda(valorDB.GetType(), estruturaColuna.Propriedade.PropertyType))
            {
                return valorDB;
            }
            else
            {
                if ((valorDB == null) || valorDB == DBNull.Value)
                {
                    return estruturaColuna.ValorPadrao;
                }
                else
                {
                    if(valorDB is DateTime dateTime)
                    {
                        if(dateTime.Kind == DateTimeKind.Unspecified)
                        {
                            var kind = estruturaColuna.EstruturaCampo.DateTimeKind ??
                                       estruturaColuna.EstruturaCampo.EstruturaEntidade.EstruturaBancoDados.DateTimeKindPadrao;
                            return DateTime.SpecifyKind(dateTime, kind);
                        }
                        return dateTime;
                    }
                    return ConverterUtil.Converter(valorDB, estruturaColuna.Tipo);
                }
            }
        }
    }

    internal class EstruturaColuna
    {

        internal int Posicao { get; set; }
        internal DataColumn Coluna { get; set; }
        internal Type Tipo { get; set; }
        internal EnumTipoPrimario TipoPrimario { get; set; }
        internal PropertyInfo Propriedade { get; set; }
        internal EstruturaCampo EstruturaCampo { get; set; }
        internal EstruturaCampoApelido EstruturaCampoApelido { get; set; }
        internal bool IsTipoComplexo { get; set; }
        internal bool AceitaNulo { get; set; }
        internal bool IsDataHora { get; set; }
        internal bool IsNumerico { get; set; }
        internal bool IsString { get; set; }
        internal bool IsBoolean { get; set; }

        internal object ValorPadrao { get; set; }
        // internal bool Nullable { get; set; }

        public EstruturaColuna(int posicao, DataColumn coluna, EstruturaCampoApelido estruturaCampoApelido)
        {
            this.Posicao = posicao;
            this.Coluna = coluna;
            this.EstruturaCampoApelido = estruturaCampoApelido;
            this.EstruturaCampo = estruturaCampoApelido.EstruturaCampo;
            this.Propriedade = estruturaCampoApelido.EstruturaCampo.Propriedade;
            this.AceitaNulo = !this.Propriedade.PropertyType.IsValueType || ReflexaoUtil.IsTipoNullable(this.Propriedade.PropertyType);
            this.Tipo = ReflexaoUtil.RetornarTipoSemNullable(this.Propriedade.PropertyType);
            this.TipoPrimario = ReflexaoUtil.RetornarTipoPrimarioEnum(this.Propriedade.PropertyType);
            this.ValorPadrao = ReflexaoUtil.RetornarValorPadraoPropriedade(this.Propriedade);
            this.IsTipoComplexo = this.EstruturaCampo.IsTipoComplexo;
        }
    }
}