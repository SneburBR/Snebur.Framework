using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.Comunicacao
{
    public interface IServicoLogServicoArquivo : IBaseServico
    {
        Guid NotificarInicioEnvio(int totalArquivos, int totalBytes);

        bool NotificarProgressoEnvioArquivo(Guid identificadorLog, double progresso, double bytesEnvidos);

        bool NotificarFimEnvio(Guid identificadorLog, double totalBytesEnviado);
    }
}
