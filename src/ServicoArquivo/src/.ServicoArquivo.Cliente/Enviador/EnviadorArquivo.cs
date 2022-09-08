using Snebur.Dominio;
using Snebur.Seguranca;
using System;
using System.IO;

namespace Snebur.ServicoArquivo.Cliente
{
    public class EnviadorArquivo : BaseEnviadorArquivo<IArquivo>
    {
        private Stream StreamArquivo { get; }

        public EnviadorArquivo(string urlServicoArquivo, 
                               IArquivo arquivo, 
                               Stream streamImagem) : this(urlServicoArquivo, 
                                                           arquivo, 
                                                           streamImagem,
                                                           AplicacaoSnebur.Atual.CredencialUsuario,
                                                           AplicacaoSnebur.Atual.IdentificadorSessaoUsuario,
                                                           AplicacaoSnebur.Atual.IdentificadorProprietario)
        {

        }

        public EnviadorArquivo(string urlServicoArquivo, 
                              IArquivo arquivo, 
                              Stream streamArquivo,
                              CredencialUsuario credencialUsuario,
                              Guid IdentificadorSessaoUsuario, 
                              string IdentificadorProprietario) : base(urlServicoArquivo,
                                                                       arquivo,
                                                                       credencialUsuario, 
                                                                       IdentificadorSessaoUsuario, 
                                                                       IdentificadorProprietario)
        {

            this.StreamArquivo = streamArquivo;
        }

        protected override Stream RetornarStreamArquivo()
        {
            return this.StreamArquivo;
        }
        protected override string RetornarUrlEnviarArquivo()
        {
            return ServicoArquivoClienteUtil.RetornarEnderecoEnviarArquivo(this.UrlServicoArquivo);
        }
    }
}
