namespace Snebur.AcessoDados.Servidor.Salvar
{
    public interface IComandoUpdate
    {
        Dictionary<string, PropriedadeAlterada> PropriedadesAlterada { get;   }
    }
}