namespace Snebur.AcessoDados.Ajudantes;

internal partial class AjudanteFiltroPropriedade
{

    public static FiltroPropriedade RetornarFiltroPropriedadeVardadeiro(EstruturaConsulta estruturaConsulta, MemberExpression expressao)
    {
        var valorPropriedade = true;
        var operadorFiltro = EnumOperadorFiltro.Igual;
        return AjudanteFiltroPropriedade.RetornarFiltroPropriedade(estruturaConsulta, expressao, valorPropriedade, operadorFiltro);
    }
}