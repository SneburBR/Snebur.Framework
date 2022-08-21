/*eslint-disable*/
//Data : segunda-feira, 15 de agosto de 2022
//Hora : 06:15:27
//@Namespace: Snebur.ServicoArquivo
//@PrioridadeDominio: 2
//@Globalizar: False
//@Dominios dependentes: []

namespace Snebur.ServicoArquivo
{
    export class ConstantesServicoArquivo
    {
        public static readonly NOME_ARQUIVO_BAIXAR_ARQUIVO : string = "BaixarArquivo";
        public static readonly NOME_ARQUIVO_ENVIAR_ARQUIVO : string = "EnviarArquivo";
        public static readonly NOME_ARQUIVO_BAIXAR_VERSAO : string = "BaixarVersao";
        public static readonly ID_ARQUIVO : string = "IdArquivo";
        public static readonly PARTE_ATUAL : string = "ParteAtual";
        public static readonly TOTAL_PARTES : string = "TotalPartes";
        public static readonly TOTAL_BYTES : string = "TotalBytes";
        public static readonly TAMANHO_PACOTE : string = "TamanhoPacote";
        public static readonly TAMANHO_PACOTE_ATUAL : string = "TamanhoPacoteAtual";
        public static readonly CHECKSUM_ARQUIVO : string = "ChecksumArquivo";
        public static readonly CHECKSUM_PACOTE : string = "ChecksumPacote";
        public static readonly IDENTIFICADOR_SESSAO_USUARIO : string = "IdentificadorSessaoUsuario";
        public static readonly IDENTIFICADOR_USUARIO : string = "IdentificadorUsuario";
        public static readonly SENHA : string = "Senha";
        public static readonly ID_SESSAO_USUARIO : string = "IdSessaoUsuario";
        public static readonly ASEMMBLY_QUALIFIED_NAME : string = "AssemblyQualifiedName";
        public static readonly NOME_TIPO_ARQUIVO : string = "NomeTipoArquivo";
        public static readonly IDENTIFICADOR_CACHE : string = "IdentificadorCache";
    }
    export class ConstantesServicoFonte
    {
        public static readonly NOME_FORMATO_FONTE : string = "formato_fonte";
        public static readonly NOME_ARQUVIVO_FONTE : string = "fonte";
    }
    export class ConstantesServicoImagem
    {
        public static readonly ID_IMAGEM : string = "IdImagem";
        public static readonly TAMANHO_IMAGEM : string = "TamanhoImagem";
        public static readonly IMAGEM_PENDENTE : string = "ImagemPendente";
        public static readonly NOME_ARQUIVO_BAIXAR_IMAGEM : string = "BaixarImagem";
        public static readonly NOME_ARQUIVO_ENVIAR_IMAGEM : string = "EnviarImagem";
        public static readonly NOME_ARQUIVO_VIZUALIZAR_IMAGEM : string = "imagem";
        public static readonly NOME_ARQUIVO_FONTE : string = "Fonte";
        public static readonly FORMATO_IMAGEM : string = "FormatoImagem";
    }
}