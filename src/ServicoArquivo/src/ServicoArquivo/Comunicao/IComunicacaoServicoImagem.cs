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
    public interface IComunicacaoServicoImagem : IComunicacaoServicoArquivo
    {
        bool ExisteImagem(long idImagem, EnumTamanhoImagem tamanhoImagem);

        bool NotificarFimEnvioImagem(long idImagem, long totalBytes, EnumTamanhoImagem tamanhoImagem, string checksum);
    }
}
