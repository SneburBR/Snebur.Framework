using Snebur.Dominio.Atributos;

namespace Snebur.ServicoArquivo
{
    [ConstantesTS]
    public class ConstantesServicoArquivo
    {
        public const string NOME_ARQUIVO_BAIXAR_ARQUIVO = "BaixarArquivo";

        public const string NOME_ARQUIVO_ENVIAR_ARQUIVO = "EnviarArquivo";

        public const string NOME_ARQUIVO_BAIXAR_VERSAO = "BaixarVersao";

        public const string ID_ARQUIVO = "IdArquivo";

        public const string PARTE_ATUAL = "ParteAtual";

        public const string TOTAL_PARTES = "TotalPartes";

        public const string TOTAL_BYTES = "TotalBytes";

        public const string TAMANHO_PACOTE = "TamanhoPacote";

        public const string TAMANHO_PACOTE_ATUAL = "TamanhoPacoteAtual";

        public const string CHECKSUM_ARQUIVO = "ChecksumArquivo";

        public const string CHECKSUM_PACOTE = "ChecksumPacote";

        public const string IDENTIFICADOR_SESSAO_USUARIO = "IdentificadorSessaoUsuario";

        public const string IDENTIFICADOR_USUARIO = "IdentificadorUsuario";

        public const string SENHA = "Senha";

        public const string ID_SESSAO_USUARIO = "IdSessaoUsuario";

        public const string ASEMMBLY_QUALIFIED_NAME = "AssemblyQualifiedName";

        public const string NOME_TIPO_ARQUIVO = "NomeTipoArquivo";

        public const string IDENTIFICADOR_CACHE = "IdentificadorCache";
    }

    [ConstantesTS]
    public class ConstantesServicoFonte
    {
        public const string NOME_FORMATO_FONTE = "formato_fonte";
        public const string NOME_ARQUVIVO_FONTE = "fonte";
    }
}