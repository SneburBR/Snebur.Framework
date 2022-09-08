using Snebur.Net;
using Snebur.Seguranca;
using Snebur.Utilidade;
using System;

namespace Snebur.ServicoArquivo
{

    public partial class CabecalhoServicoArquivo : Cabecalho, IInformacaoRepositorioArquivo
    {
        public long IdArquivo { get; set; }
        public Guid IdentificadorSessaoUsuario { get; set; }
        public string IdentificadorUsuario { get; set; }
        public string Senha { get; set; }
        //public long IdSessaoUsuario { get; set; }
        public string CheckSumArquivo { get; set; }

        public string CheckSumPacote { get; set; }
        public string Ip { get; set; }
        public string AssemblyQualifiedName { get; set; }

        public double ParteAtual { get; set; }
        public double TotalPartes { get; set; }
        public int TamanhoPacote { get; set; }
        public long TotalBytes { get; set; }

        public bool EnviarArquivo { get; }

        public string NomeTipoArquivo { get; }

        public CredencialUsuario CredencialRequisicao
        {
            get
            {
                return new CredencialUsuario(this.IdentificadorUsuario, this.Senha);
            }
        }

        public CabecalhoServicoArquivo(SnHttpContext context, bool enviarArquivo = false) : base(context)
        {
            this.EnviarArquivo = enviarArquivo;
            this.IdArquivo = this.RetornarLong(ConstantesServicoArquivo.ID_ARQUIVO);
            this.ParteAtual = this.RetornarDouble(ConstantesServicoArquivo.PARTE_ATUAL);
            this.TotalPartes = this.RetornarDouble(ConstantesServicoArquivo.TOTAL_PARTES);
            this.TotalBytes = this.RetornarLong(ConstantesServicoArquivo.TOTAL_BYTES);

            this.TamanhoPacote = this.RetornarInteger(ConstantesServicoArquivo.TAMANHO_PACOTE);
            this.Ip = IpUtilLocal.RetornarIp(context);
            this.CheckSumArquivo = this.RetornarString(ConstantesServicoArquivo.CHECKSUM_ARQUIVO);
            this.CheckSumPacote = this.RetornarString(ConstantesServicoArquivo.CHECKSUM_PACOTE);
            this.IdentificadorSessaoUsuario = this.RetornarGuid(ConstantesServicoArquivo.IDENTIFICADOR_SESSAO_USUARIO);
            this.IdentificadorUsuario = this.RetornarString(ConstantesServicoArquivo.IDENTIFICADOR_USUARIO);
            this.Senha = this.RetornarString(ConstantesServicoArquivo.SENHA);
            //this.IdSessaoUsuario = this.RetornarLong(ConstantesServicoArquivo.ID_SESSAO_USUARIO, context);
            this.AssemblyQualifiedName = this.RetornarString(ConstantesServicoArquivo.ASEMMBLY_QUALIFIED_NAME);
            this.NomeTipoArquivo = this.RetornarString(ConstantesServicoArquivo.NOME_TIPO_ARQUIVO);

            if (this.CheckSumArquivo != null)
            {
                this.CheckSumArquivo = this.CheckSumArquivo.Trim().ToLower();
            }
        }

        public bool IsCabecalhoValido()
        {
            return !String.IsNullOrWhiteSpace(this.IdentificadorUsuario) &&
                   !String.IsNullOrWhiteSpace(this.Senha) &&
                   !String.IsNullOrWhiteSpace(this.CheckSumArquivo) &&
                   !String.IsNullOrWhiteSpace(this.AssemblyQualifiedName) &&
                   GuidUtil.GuidValido(this.IdentificadorSessaoUsuario) &&
                   (!this.EnviarArquivo ||
                   (this.EnviarArquivo && this.IdArquivo > 0 && this.ParteAtual > 0 && this.TamanhoPacote > 0 && this.TotalPartes > 0));
        }
    }
}
