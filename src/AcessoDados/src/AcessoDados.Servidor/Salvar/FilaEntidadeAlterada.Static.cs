namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal partial class FilaEntidadeAlterada
    {
        public static Queue<EntidadeAlterada> RetornarFila(List<EntidadeAlterada> entidadesAlterada)
        {
            using (var construtor = new FilaEntidadeAlterada(entidadesAlterada))
            {
                return construtor.RetornarFila();
            }
        }
    }
}