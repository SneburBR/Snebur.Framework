using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;

namespace Snebur.ServicoArquivo
{
    [IgnorarInterfaceTS]
    public interface IComunicacaoServicoArquivo
    {
        bool ExisteIdArquivo(long idArquivo);

        bool ExisteArquivo(long idArquivo);

        bool NotificarInicioEnvioArquivo(long idArquivo);

        bool NotificarFimEnvioArquivo(long idArquivo, long totalBytes, string checksum);

        bool NotificarArquivoDeletado(long idArquivo);

        bool NotificarProgresso(long idArquivo, double progresso);
    }
}
