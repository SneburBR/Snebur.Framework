using Snebur.Dominio;

namespace Snebur.Comunicacao
{
    public class ResultadoValidarCodigoRecuperarSenha : BaseResultadoRecuperarSenha
    {
        public bool IsUsuarioEncontrado { get; set; }
        public EnumEstadoCodigoRecuperarSenha Estado { get; set; }
        public int TempoEsperar { get; set; }
    }
}
namespace Snebur.Dominio
{
}