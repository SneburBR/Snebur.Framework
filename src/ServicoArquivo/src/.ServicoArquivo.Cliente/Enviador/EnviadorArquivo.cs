using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using Snebur.Seguranca;

namespace Snebur.ServicoArquivo.Cliente
{
    public class EnviadorArquivo : BaseEnviadorArquivo<IArquivo>
    {
        private Stream StreamArquivo { get; }

        public EnviadorArquivo(string urlServicoArquivo, IArquivo arquivo, Stream streamImagem) :
                               this(urlServicoArquivo, arquivo, streamImagem,
                                                                AplicacaoSnebur.Atual.CredencialUsuario,
                                                                AplicacaoSnebur.Atual.IdentificadorSessaoUsuario,
                                                                AplicacaoSnebur.Atual.IdentificadorProprietario)
        {

        }

        public EnviadorArquivo(string urlServicoArquivo, IArquivo arquivo, Stream streamArquivo,
                              CredencialUsuario credencialUsuario, Guid IdentificadorSessaoUsuario, string IdentificadorProprietario) :
                              base(urlServicoArquivo, arquivo, credencialUsuario, IdentificadorSessaoUsuario, IdentificadorProprietario)
        {

            this.StreamArquivo = streamArquivo;
        }
         
        protected override Stream RetornarStreamArquivo()
        {
            return this.StreamArquivo;
        }
    }
}
