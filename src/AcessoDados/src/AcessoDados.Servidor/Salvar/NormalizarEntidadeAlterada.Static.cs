namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal partial class NormalizarEntidadeAlterada
    {
        internal static List<EntidadeAlterada> RetornarEntidadesAlteradaNormalizada(BaseContextoDados contexto, List<EntidadeAlterada> entidadesAlterada)
        {
            using (var normalizar = new NormalizarEntidadeAlterada(contexto, entidadesAlterada))
            {
                return normalizar.RetornarEntidadesAlteradaNormalizada();
            }
        }
    }
}
