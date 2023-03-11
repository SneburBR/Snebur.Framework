using Snebur.Seguranca;
using Snebur.Utilidade;
using System;
#if NET7_0
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif  


namespace Snebur.ServicoArquivo
{

    public partial class CabecalhoServicoArquivo : Cabecalho, IInformacaoRepositorioArquivo
    {
        public long IdArquivo { get;  }
        public string IdentificadorProprietario { get;   }
        public Guid IdentificadorSessaoUsuario { get;   }
        public string IdentificadorUsuario { get;   }
        public string Senha { get; }
        //public long IdSessaoUsuario { get; set; }
        public string CheckSumArquivo { get;  }

        public string CheckSumPacote { get;  }
        public string Ip { get;  }
        public string AssemblyQualifiedName { get;  }

        public double ParteAtual { get; }
        public double TotalPartes { get; }
        public int TamanhoPacote { get;  }
        public long TotalBytes { get;   }

        public bool EnviarArquivo { get; }

        public string NomeTipoArquivo { get; }

        public CredencialUsuario CredencialRequisicao => new CredencialUsuario(this.IdentificadorUsuario, this.Senha);

        public CabecalhoServicoArquivo(HttpContext context, bool enviarArquivo = false) : base(context)
        {
            this.EnviarArquivo = enviarArquivo;
            this.IdentificadorProprietario = context.Request.Headers[ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO];

            this.IdArquivo = this.RetornarLong(ConstantesServicoArquivo.ID_ARQUIVO);
            this.ParteAtual = this.RetornarDouble(ConstantesServicoArquivo.PARTE_ATUAL);
            this.TotalPartes = this.RetornarDouble(ConstantesServicoArquivo.TOTAL_PARTES);
            this.TotalBytes = this.RetornarLong(ConstantesServicoArquivo.TOTAL_BYTES);

            this.TamanhoPacote = this.RetornarInteger(ConstantesServicoArquivo.TAMANHO_PACOTE);
            this.Ip = AplicacaoSnebur.Atual.IP;
            this.CheckSumArquivo = this.RetornarString(ConstantesServicoArquivo.CHECKSUM_ARQUIVO);
            this.CheckSumPacote = this.RetornarString(ConstantesServicoArquivo.CHECKSUM_PACOTE);

            this.IdentificadorSessaoUsuario = this.RetornarGuid(ConstantesCabecalho.IDENTIFICADOR_SESSAO_USUARIO);
            this.IdentificadorUsuario = this.RetornarString(ConstantesCabecalho.IDENTIFICADOR_USUARIO);
            this.Senha = this.RetornarString(ConstantesCabecalho.SENHA);
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
                   !String.IsNullOrWhiteSpace(this.AssemblyQualifiedName) &&
                    GuidUtil.GuidValido(this.IdentificadorSessaoUsuario) &&
                   (!this.EnviarArquivo ||
                   (this.EnviarArquivo && this.IdArquivo > 0 && 
                                          this.ParteAtual > 0 && 
                                          this.TamanhoPacote > 0 && 
                                          this.TotalPartes > 0 &&
                                          !String.IsNullOrWhiteSpace(this.CheckSumArquivo)));
        }
    }
}
