namespace Snebur.Comunicacao.Dominio.ServicoDados
{
    public class Salvar
    {
        public Entidade? Entidade { get; set; }
    }

    public class EntidadeSalvar
    {
        public Entidade? Entidade { get; set; }

        public bool Arvore { get; set; }
    }
}