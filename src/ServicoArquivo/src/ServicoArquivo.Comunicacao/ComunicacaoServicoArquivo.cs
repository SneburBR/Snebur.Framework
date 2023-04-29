using Snebur.AcessoDados;
using Snebur.Comunicacao;
using Snebur.Dominio;
using Snebur.Seguranca;
using System;

namespace Snebur.ServicoArquivo.Comunicacao
{
    public abstract class ComunicacaoServicoArquivo : BaseComunicacaoServidor, IComunicacaoServicoArquivo
    {
        #region IComunicacaoServicoArquivo

        public bool ExisteArquivo(long idArquivo)
        {
            using (var contexto = this.RetornarContextoDados())
            {
                var arquivo = contexto.RetornarConsulta<IArquivo>(contexto.TipoEntidadeArquivo).Where(x => x.Id == idArquivo).SingleOrDefault();
                if (arquivo != null)
                {
                    return arquivo.IsExisteArquivo;
                }
                return false;
            }
        }

        public bool ExisteIdArquivo(long idArquivo)
        {
            using (var contexto = this.RetornarContextoDados())
            {
                var arquivo = contexto.RetornarConsulta<IArquivo>(contexto.TipoEntidadeArquivo).
                                       Where(x => x.Id == idArquivo).SingleOrDefault();
                return arquivo != null;
            }
        }

        public bool NotificarInicioEnvioArquivo(long idArquivo)
        {
            using (var contexto = this.RetornarContextoDados())
            {
                var arquivo = contexto.RetornarConsulta<IArquivo>(contexto.TipoEntidadeArquivo).Where(x => x.Id == idArquivo).SingleOrDefault();
                if (arquivo == null)
                {
                    throw new ErroOperacaoInvalida(String.Format("Não foi encontrado o arquivo com ID : {0}", idArquivo));
                }
                if (arquivo.Status != EnumStatusArquivo.Enviando)
                {
                    arquivo.DataHoraInicioEnvio = DateTime.UtcNow;
                    arquivo.IsExisteArquivo = false;
                    arquivo.Status = EnumStatusArquivo.Enviando;
                    return contexto.Salvar(arquivo).IsSucesso;
                }
            }
            return true;
        }

        public bool NotificarFimEnvioArquivo(long idArquivo, long totalBytes, string checksum)
        {
            using (var contexto = this.RetornarContextoDados())
            {
                var arquivo = contexto.RetornarConsulta<IArquivo>(contexto.TipoEntidadeArquivo).Where(x => x.Id == idArquivo).SingleOrDefault();
                if (arquivo == null)
                {
                    throw new ErroOperacaoInvalida(String.Format("Não foi encontrado o arquivo com ID : {0}", idArquivo));
                }
                arquivo.DataHoraFimEnvio = DateTime.UtcNow;
                arquivo.Checksum = checksum;
                arquivo.Status = EnumStatusArquivo.EnvioConcluido;
                arquivo.TotalBytes = totalBytes;
                arquivo.IsExisteArquivo = true;
                return contexto.Salvar(arquivo).IsSucesso;
            }
        }

        public bool NotificarArquivoDeletado(long idArquivo)
        {
            using (var contexto = this.RetornarContextoDados())
            {
                var arquivo = contexto.RetornarConsulta<IArquivo>(contexto.TipoEntidadeArquivo).Where(x => x.Id == idArquivo).SingleOrDefault();
                if (arquivo == null)
                {
                    throw new ErroOperacaoInvalida(String.Format("Não foi encontrado o arquivo com ID : {0}", idArquivo));
                }
                arquivo.DataHoraArquivoDeletado = DateTime.UtcNow;
                arquivo.Status = EnumStatusArquivo.ArquivoDeletado;
                return contexto.Salvar(arquivo).IsSucesso;
            }
        }

        public bool NotificarProgresso(long idArquivo, double progresso)
        {
            using (var contexto = this.RetornarContextoDados())
            {
                var arquivo = contexto.RetornarConsulta<IArquivo>(contexto.TipoEntidadeArquivo).Where(x => x.Id == idArquivo).SingleOrDefault();
                if (arquivo == null)
                {
                    throw new ErroOperacaoInvalida(String.Format("Não foi encontrado o arquivo com ID : {0}", idArquivo));
                }
                arquivo.Progresso = progresso;
                return contexto.Salvar(arquivo).IsSucesso;
            }
        }
        #endregion

        public abstract BaseContextoDados RetornarContextoDados();

        #region Credencial 

        protected override CredencialServico CredencialServico
        {
            get
            {
                return CredencialComunicacaoServicoArquivo.ServicoArquivo;
            }
        }
        #endregion
    }
}