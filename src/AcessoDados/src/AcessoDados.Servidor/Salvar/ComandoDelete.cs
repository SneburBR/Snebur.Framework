using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Servidor.Salvar;

internal class ComandoDelete : Comando
{
    internal override bool IsAdiconarParametrosChavePrimaria => true;

    internal ComandoDelete(EntidadeAlterada entidadeAlterada,
        EstruturaEntidade estruturaEntidade) : base(entidadeAlterada, estruturaEntidade)
    {

        if (this.EstruturaEntidade.IsSomenteLeitura)
        {
            throw new ErroSeguranca("Não é autorizado deletar uma entidade somente leitura", Servicos.EnumTipoLogSeguranca.AlterarandoEntidadeSomenteLeitura);
        }
    }

    protected override string RetornarSqlComando()
    {
        return this.RetornarSqlCommnadoDelete();
    }

    private string RetornarSqlCommnadoDelete()
    {
        var estruturaChavePrimaria = this.EstruturaEntidade.EstruturaCampoChavePrimaria;
        var sb = new StringBuilderSql();
        sb.Append($" DELETE FROM [{this.EstruturaEntidade.Schema}].[{this.EstruturaEntidade.NomeTabela}] WHERE {estruturaChavePrimaria.NomeCampoSensivel} = {estruturaChavePrimaria.NomeParametro}");
        return sb.ToString();
    }

    internal string RetornarSqlCommandoUpdate()
    {
        throw new ErroNaoImplementado();
    }
}
