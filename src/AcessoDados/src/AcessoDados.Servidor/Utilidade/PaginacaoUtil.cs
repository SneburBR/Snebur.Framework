namespace Snebur.AcessoDados;

//public class PaginacaoUtil
//{
//    public static PaginacaoConsulta RetornarPaginacaoConsulta(int totalRegistros, int registroPorPagina, int paginaAtual)
//    {
//        var paginacao = new PaginacaoConsulta();
//        paginacao.TotalRegistros = totalRegistros;
//        paginacao.PaginaAtual = paginaAtual;
//        paginacao.RegistrosPorPagina = registroPorPagina;
//        paginacao.TotalPaginas = PaginacaoUtil.RetornarTotalPaginas(totalRegistros, registroPorPagina);
//        return paginacao;
//    }

//    public static int RetornarTotalPaginas(int totalRegistros, int registroPorPagina)
//    {
//        var totalPaginas = totalRegistros / registroPorPagina;
//        if ((totalRegistros % registroPorPagina) > 0)
//        {
//            totalPaginas += 1;
//        }
//        return totalPaginas;
//    }
//}
