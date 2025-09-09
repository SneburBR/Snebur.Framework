using Snebur.AcessoDados.Estrutura;
using System.Data;

namespace Snebur.AcessoDados.Servidor.Salvar;

internal class BaseEnitdadesAlteracaoPropriedade
{
    protected readonly BaseContextoDados Contexto;

    public BaseEnitdadesAlteracaoPropriedade(BaseContextoDados contexto)
    {
        this.Contexto = contexto;
    }

    protected BaseConexao Conexao => this.Contexto.Conexao;

    protected (bool IsExisteAlteracao, object? valorAntigo) IsExisteAlteracaoPropriedade(Entidade entidade,
                                                                                       IEstruturaAlteracaoPropriedade estruturaAlteracaoPropriedade)
    {
        if (entidade.Id == 0)
        {
            return (estruturaAlteracaoPropriedade.IsNotificarNovoCadastro, null);
        }

        var propriedadesAlteradas = entidade.__PropriedadesAlteradas;
        if (propriedadesAlteradas?.Count > 0)
        {
            if (estruturaAlteracaoPropriedade.IsTipoComplexo)
            {
                //var campos = estruturaAlteracaoPropriedade.EstruturaTipoComplexo.EstruturasCampo.Values.Select(x => x.NomeCampo);
                var tupleEstruturaPropriedadeAlterada = estruturaAlteracaoPropriedade
                    .EstruturaTipoComplexo
                    .EstruturasCampo.Values
                    .Where(x => propriedadesAlteradas.ContainsKey(x.NomeCampo))
                    .Select(x => (x, propriedadesAlteradas[x.NomeCampo]))
                    .ToList();

                if (tupleEstruturaPropriedadeAlterada.Count > 0)
                {
                    return this.IsExisteAlteracaoTipoComplexo(entidade,
                                                              estruturaAlteracaoPropriedade,
                                                              tupleEstruturaPropriedadeAlterada);
                }
            }
            else
            {

                if (propriedadesAlteradas.TryGetValue(estruturaAlteracaoPropriedade.Propriedade.Name,
                                                      out var propriedadeAlterada))
                {

                    return this.IsExisteAlteracaoTipoPrimario(entidade,
                                                              estruturaAlteracaoPropriedade,
                                                              propriedadeAlterada);
                }
            }
        }
        return (false, null);
    }

    protected (bool IsExisteAlteracao, object valorAntigo) IsExisteAlteracaoTipoComplexo(
        Entidade entidade,
        IEstruturaAlteracaoPropriedade estruturaAlteracaoPropriedade,
        List<(EstruturaCampo, PropriedadeAlterada)> estruturasCampos)
    {
        if (estruturaAlteracaoPropriedade.IsVerificarAlteracaoBanco)
        {
            return this.IsExisteAlteracaoTipoComplexoBanco(entidade, estruturaAlteracaoPropriedade);
        }

        if (!estruturaAlteracaoPropriedade.IsTipoComplexo)
        {
            throw new Exception($"A estrutura propriedade {estruturaAlteracaoPropriedade.Propriedade.Name} não é do tipo complexo");
        }

        var estruturaTipoComplexo = estruturaAlteracaoPropriedade.EstruturaTipoComplexo;
        var novoValorTipoComplexo = estruturaTipoComplexo.Propriedade.GetValue(entidade) as BaseTipoComplexo
            ?? throw new InvalidOperationException("Não foi possível criar a instância do tipo complexo.");

        var valorAntigo = novoValorTipoComplexo.Clone();

        foreach (var (estruturaCampo, propriedadeAlterada) in estruturasCampos)
        {
            var valorTipado = ConverterUtil.Converter(propriedadeAlterada.AntigoValor, estruturaCampo.Propriedade.PropertyType);
            estruturaCampo.Propriedade.SetValue(valorAntigo, valorTipado);
        }

        var isExisteAlteracao = !novoValorTipoComplexo.Equals(valorAntigo);
        return (isExisteAlteracao, valorAntigo);
    }

    private (bool IsExisteAlteracao, object valorAntigo) IsExisteAlteracaoTipoComplexoBanco(
        Entidade entidade,
        IEstruturaAlteracaoPropriedade estruturaAlteracaoPropriedade)
    {
        if (!estruturaAlteracaoPropriedade.IsTipoComplexo)
        {
            throw new Exception($"A estrutura propriedade {estruturaAlteracaoPropriedade.Propriedade.Name} não é do tipo complexo");
        }

        var estruturaEntidade = estruturaAlteracaoPropriedade.EstruturaEntidade;
        var estruturaTipoComplexo = estruturaAlteracaoPropriedade.EstruturaTipoComplexo;
        var campos = estruturaTipoComplexo.EstruturasCampo.Select(x => x.Value.NomeCampoSensivel);
        var sqlCampos = String.Join(", ", campos);

        var sqlValorAtual = new StringBuilderSql();
        sqlValorAtual.Append($" SELECT [Id], {sqlCampos} FROM [{estruturaEntidade.Schema}].[{estruturaEntidade.NomeTabela}] WHERE [{estruturaEntidade.EstruturaCampoChavePrimaria.NomeCampo}] = {entidade.Id}");

        var dataTable = this.Conexao.RetornarDataTable(sqlValorAtual.ToString(), null);
        var valorAtualTipoComplexo = this.RetornarValorTipoComplexo(estruturaTipoComplexo, dataTable);

        var novoValorTipoComplexo = estruturaTipoComplexo.Propriedade.GetValue(entidade) as BaseTipoComplexo
            ?? throw new InvalidOperationException("Não foi possível criar a instância do tipo complexo.");

        var isExisteAlteradao = !Util.SaoIgual(valorAtualTipoComplexo, novoValorTipoComplexo);
        return (isExisteAlteradao, valorAtualTipoComplexo);
    }

    private BaseTipoComplexo RetornarValorTipoComplexo(EstruturaTipoComplexo estruturaTipoComplexo,
                                                       DataTable dataTable)
    {
        var tipoComplexo = Activator.CreateInstance(estruturaTipoComplexo.Tipo) as BaseTipoComplexo ??
                            throw new InvalidOperationException("Não foi possível criar a instância do tipo complexo.");
        if (dataTable.Rows.Count == 1)
        {
            var linha = dataTable.Rows[0];
            foreach (var estrturaCampo in estruturaTipoComplexo.EstruturasCampo.Values)
            {
                if (dataTable.Columns.Contains(estrturaCampo.NomeCampo))
                {
                    var valorProprieade = linha[estrturaCampo.NomeCampo];
                    var valorPrpriedadeTipado = ConverterUtil.Converter(valorProprieade, estrturaCampo.Propriedade.PropertyType);
                    estrturaCampo.Propriedade.SetValue(tipoComplexo, valorPrpriedadeTipado);
                }
            }
        }
        return tipoComplexo;
    }

    private (bool IsExisteAlteracao, object? valorAntigo) IsExisteAlteracaoTipoPrimario(Entidade entidade,
                                                                                       IEstruturaAlteracaoPropriedade estruturaAlteracaoPropriedade,
                                                                                       PropriedadeAlterada propriedadeAlterada)
    {
        if (propriedadeAlterada.AntigoValor == null &&
           estruturaAlteracaoPropriedade.IsIgnorarValorAntigoNull)
        {
            return (false, null);
        }

        if (estruturaAlteracaoPropriedade.IsVerificarAlteracaoBanco)
        {
            return this.IsExisteAlteracaoTipoPrimarioBanco(entidade,
                                                           estruturaAlteracaoPropriedade);

        }
        var novoValor = estruturaAlteracaoPropriedade.Propriedade.GetValue(entidade);
        var valorAntigoTipado = ConverterUtil.Converter(propriedadeAlterada.AntigoValor,
                                                        estruturaAlteracaoPropriedade.Propriedade.PropertyType);

        var isExisteAlteracao = Util.SaoIgual(novoValor, valorAntigoTipado) == false;
        return (isExisteAlteracao, valorAntigoTipado);
    }

    private (bool IsExisteAlteracao, object? valorAntigo) IsExisteAlteracaoTipoPrimarioBanco(Entidade entidade,
                                                                                            IEstruturaAlteracaoPropriedade estruturaAlteracaoPropriedade)
    {
        var estruturaEntidade = estruturaAlteracaoPropriedade.EstruturaEntidade;
        var estruturaCampo = estruturaAlteracaoPropriedade.EstruturaCampo;

        Guard.NotNull(estruturaCampo);

        var sqlValorAtual = new StringBuilderSql();
        sqlValorAtual.Append($" SELECT [{estruturaCampo.NomeCampo}] FROM [{estruturaEntidade.Schema}].[{estruturaEntidade.NomeTabela}] WHERE [{estruturaEntidade.EstruturaCampoChavePrimaria.NomeCampo}] = {entidade.Id}");

        var valorBancoAtual = this.Conexao.RetornarValorScalar(sqlValorAtual.ToString(), null);
        var valorBancoAtualTipado = ConverterUtil.Converter(valorBancoAtual, estruturaCampo.Propriedade.PropertyType);
        var novoValor = estruturaCampo.Propriedade.GetValue(entidade);
        var isExisteAlteracao = this.IsExisteAlteracaoPropriedade(estruturaCampo, novoValor, valorBancoAtualTipado);
        return (isExisteAlteracao, valorBancoAtualTipado);
    }

    private bool IsExisteAlteracaoPropriedade(
        EstruturaCampo estruturaCampo,
        object? novoValor,
        object? valorBancoAtualTipado)
    {
        if (estruturaCampo.Propriedade.PropertyType.IsEnum)
        {
            return Convert.ToInt32(novoValor) != Convert.ToInt32(valorBancoAtualTipado);
        }
        else
        {
            return !Util.SaoIgual(novoValor, valorBancoAtualTipado);
        }
    }
}
