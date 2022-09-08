using Snebur.Dominio;
using Snebur.Seguranca;
using System;
using System.Collections.Generic;
using System.IO;

namespace Snebur.ServicoArquivo.Cliente
{
    public class EnviadorImagem : BaseEnviadorArquivo<IImagem>
    {
        public EnumTamanhoImagem TamanhoImagem { get; }
        private Stream StreamImagem { get; }

        public EnviadorImagem(string urlServicoArquivo, IImagem imagem, EnumTamanhoImagem tamanhoImagem, Stream streamImagem) :
                             this(urlServicoArquivo, imagem, tamanhoImagem, streamImagem,
                                                             AplicacaoSnebur.Atual.CredencialUsuario,
                                                             AplicacaoSnebur.Atual.IdentificadorSessaoUsuario,
                                                             AplicacaoSnebur.Atual.IdentificadorProprietario)
        {

        }

        public EnviadorImagem(string urlServicoArquivo, IImagem imagem, EnumTamanhoImagem tamanhoImagem, Stream streamImagem,
                              CredencialUsuario credencialUsuario, Guid IdentificadorSessaoUsuario, string IdentificadorProprietario) :
                              base(urlServicoArquivo, imagem, credencialUsuario, IdentificadorSessaoUsuario, IdentificadorProprietario)
        {

            this.TamanhoImagem = tamanhoImagem;
            this.StreamImagem = streamImagem;
        }

        protected override Dictionary<string, string> RetornarParametros(int tamanhoPacote)
        {
            var parametros = base.RetornarParametros(tamanhoPacote);
            parametros.Add(ConstantesServicoImagem.TAMANHO_IMAGEM, ((int)this.TamanhoImagem).ToString());
            parametros.Add(ConstantesServicoImagem.FORMATO_IMAGEM, ((int)this.Arquivo.FormatoImagem).ToString());

            return parametros;
        }

        protected override Stream RetornarStreamArquivo()
        {
            return this.StreamImagem;
        }

        protected override string RetornarUrlEnviarArquivo()
        {
            return ServicoImagemClienteUtil.RetornarEnderecoEnviarImagem(this.UrlServicoArquivo);
        }
    }
}
