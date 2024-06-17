using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Dominio
{
   
    public static class ConstantesItensRequsicao
    {
        public const string CAMINHO_APLICACAO = "CaminhoAplicacao";
        public const string CHAVE_INFORMACAO_SESSAO_ATUAL = "INFORMACAO_SESSAO_USUARO_ATUAL";
        public const string CHAVE_CREDENCIAL_USUARIO = "CREDENCIAL_USUARIO";
        public const string CHAVE_CREDENCIAL_USUARIO_AVALISTA = "CREDENCIAL_USUARIO_AVALISTA";
        public const string CHAVE_IDENTIFICADOR_PROPRIETARIO = "IDENTIFICADOR_PROPRIETARIO";
    }
    public class InformacaoSessaoUsuario : InformacaoSessao, IIdentificadorSessaoUsuario
    {
        public InformacaoSessaoUsuario()
        {
            this.IdentificadorSessaoUsuario = Guid.NewGuid();
        }
        public Guid IdentificadorSessaoUsuario { get; set; }
    }
}
