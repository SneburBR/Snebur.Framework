using Snebur.AcessoDados.Estrutura;
using Snebur.AcessoDados.Seguranca;
using Snebur.Reflexao;
using System.Data;

namespace Snebur.AcessoDados.Mapeamento;

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

    internal static List<Entidade> MapearDataTable(IEstruturaConsultaSeguranca estruturaConsulta,
                                                  DataTable dataTable, MapeamentoEntidade mapeamento)
    {
        var entidades = new List<Entidade>();
        var estruturasColuna = new List<EstruturaColuna>();
        foreach (var estruturaCampoApelido in mapeamento.EstruturaEntidadeApelido.EstruturasCampoApelido)
        {
            var nomeColuna = estruturaCampoApelido.Apelido.Substring(1, estruturaCampoApelido.Apelido.Length - 2);

            if (dataTable.Columns.Contains(nomeColuna))
            {
                var coluna = dataTable.Columns[nomeColuna]
                    ?? throw new Erro($"Não foi encontrado na coluna no mapeamento do DataSet {nomeColuna} ");

                var posicao = dataTable.Columns.IndexOf(coluna);
                estruturasColuna.Add(new EstruturaColuna(posicao, coluna, estruturaCampoApelido));
            }
            else
            {
                if (estruturaConsulta.PropriedadesAbertas?.Count <= 0)
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

            var entidade = Activator.CreateInstance(mapeamento.TipoEntidade) as IEntidadeInterna
                ?? throw new Erro($"Erro ao instanciar a entidade do tipo {mapeamento.TipoEntidade.Name}");

            entidade.NotifyIsNotNewEntity();
            //Campos

            for (var j = 0; j < lenColunas; j++)
            {
                var estruturaColuna = estruturasColuna[j];
                var valorDB = linha[estruturaColuna.Posicao];

                var valorPropriedade = RetornarValorConvertido(valorDB, estruturaColuna);
                var estruturaCampo = estruturaColuna.EstruturaCampo;
                if (estruturaCampo.IsTipoComplexo)
                {
                    var tipoComplexo = estruturaCampo.PropriedadeTipoComplexo.GetValue(entidade);
                    if (tipoComplexo == null)
                    {
                        throw new Erro($"O a propriedade {estruturaCampo.PropriedadeTipoComplexo.Name} <{estruturaCampo.PropriedadeTipoComplexo.PropertyType.Name}> na entidade {mapeamento.TipoEntidade.Name}  não foi instanciada");
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

    internal static object? RetornarValorConvertido(object valorDB,
                                                   EstruturaColuna estruturaColuna)
    {
        return RetornarValorConvertidoInterno(valorDB, estruturaColuna);
        //return estruturaColuna.Normalizar(valor);
    }

    private static object? RetornarValorConvertidoInterno(object valorDB,
                                                         EstruturaColuna estruturaColuna)
    {
        if (ReflexaoUtil.IsTipoIgualOuHerda(valorDB.GetType(), estruturaColuna.Propriedade.PropertyType))
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
                if (valorDB is DateTime dateTime)
                {
                    if (dateTime.Kind == DateTimeKind.Unspecified)
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
    internal int Posicao { get; }
    internal DataColumn Coluna { get; }
    internal Type Tipo { get; }
    internal EnumTipoPrimario TipoPrimario { get; }
    internal PropertyInfo Propriedade { get; }
    internal EstruturaCampo EstruturaCampo { get; }
    internal EstruturaCampoApelido EstruturaCampoApelido { get; }
    internal bool IsTipoComplexo { get; }
    internal bool AceitaNulo { get; }
    internal bool IsDataHora { get; }
    internal bool IsNumerico { get; }
    internal bool IsString { get; }
    internal bool IsBoolean { get; }

    internal object? ValorPadrao { get; }
    // internal bool Nullable { get; set; }

    public EstruturaColuna(
        int posicao,
        DataColumn coluna,
        EstruturaCampoApelido estruturaCampoApelido)
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

    internal object Normalizar(object valor)
    {
        return this.EstruturaCampo.Normalizar(valor);
    }
}