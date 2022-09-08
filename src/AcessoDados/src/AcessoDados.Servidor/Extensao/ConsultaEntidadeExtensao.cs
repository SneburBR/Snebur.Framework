namespace Snebur.AcessoDados
{
    public static class ConsultaEntidadeExtensao
    {
        public static string RetornarSql(this BaseConsultaEntidade consultaEntidade)
        {
            return ((BaseContextoDados)consultaEntidade.ContextoDados).RetornarSql(consultaEntidade.EstruturaConsulta);
        }
    }
}
