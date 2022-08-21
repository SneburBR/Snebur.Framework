using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.AcessoDados.Admin
{
    public class ContextoDadosUtil
    {
        public static IContextoDadosSeguranca RetornarContextoDados()
        {
            //var contextoMonitor = typeof(ContextoMonitor);

            var nomeQualificadoTipoContexto = $"{Configuracao.NomeAssemblyContexto}.{Configuracao.NomeTipoContexto}, {Configuracao.NomeAssemblyContexto}";

            var tipoContexto = Type.GetType(nomeQualificadoTipoContexto);
            if (tipoContexto == null)
            {
                throw new Exception($"Não foi encontrado o tipo do contexto {tipoContexto}, o caminho qualificado {nomeQualificadoTipoContexto}");
            }

            try
            {
                var contexto = Activator.CreateInstance(tipoContexto) as IContextoDadosSeguranca;
                if (contexto == null)
                {
                    throw new Exception("O contexto não foi definido");
                }
                return contexto;
            }
            catch (Exception ex)
            {
                throw new Erro(String.Format("Não foi possivel criar um instancia do contexto dados. {0}", tipoContexto.Name), ex);
            }
        }
    }
}

