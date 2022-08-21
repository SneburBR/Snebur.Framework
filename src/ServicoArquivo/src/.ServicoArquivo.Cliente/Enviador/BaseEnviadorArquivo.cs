using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Snebur.Comunicacao;
using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Utilidade;

namespace Snebur.ServicoArquivo.Cliente
{
    public abstract class BaseEnviadorArquivo<TArquivo> : IDisposable where TArquivo : IArquivo
    {
        private const int TAMANHO_BUFFER_PADRAO = 64 * 1024;
        private const int MAXIMO_TENTATIVA_ENVIAR_PACOTE = 10;
        private const int MAXIMO_TENTATIVA_ENVIAR_ARQUIVO = 10;

        public CredencialUsuario CredencialUsuario { get; }
        private Guid IdentificadorSessaoUsuario { get; }
        public string IdentificadorProprietario { get; }

        public TArquivo Arquivo { get; }
        public int ParteAtual { get; private set; }
        public long TotalBytes { get; private set; }
        public int TotalPartes { get; private set; }
        public int TamanhoPacote { get; set; } = TAMANHO_BUFFER_PADRAO;
        private Stream StreamArquivo { get; set; }
        public string ChecksumArquivo { get; private set; }
        public string ChecksumPacoteAtual { get; private set; }
        public string UrlServicoArquivo { get; private set; }
        public int TentativaEnviarPacoteAtual { get; private set; }
        public int TentativaEnviarArquivo { get; private set; }

        public Exception Erro { get; private set; }
        public ResultadoServicoArquivo Resultado { get; private set; }

        public event ProgressoEventHandler ProgressoAlterado;
        public event EventHandler<ErrorEventArgs> EnvioFinalizado;

        public BaseEnviadorArquivo(string urlServicoArquivo, TArquivo arquivo) :
                                   this(urlServicoArquivo, arquivo, AplicacaoSnebur.Atual.CredencialUsuario,
                                                                    AplicacaoSnebur.Atual.IdentificadorSessaoUsuario,
                                                                    AplicacaoSnebur.Atual.IdentificadorProprietario)
        {

        }

        public BaseEnviadorArquivo(string urlServicoArquivo, TArquivo arquivo,
                                  CredencialUsuario credencialUsuario, Guid IdentificadorSessaoUsuario, string IdentificadorProprietario)
        {
            this.Arquivo = arquivo;
            this.UrlServicoArquivo = urlServicoArquivo;
            this.CredencialUsuario = credencialUsuario;
            this.IdentificadorSessaoUsuario = IdentificadorSessaoUsuario;
            this.IdentificadorProprietario = IdentificadorProprietario;

            ValidacaoUtil.ValidarReferenciaNulaOuVazia(nameof(this.UrlServicoArquivo), this.UrlServicoArquivo);
        }

        public void Enviar()
        {
            this.IniciarEnvio();
        }

        public void EnviarAsync()
        {
            Task.Factory.StartNew(this.IniciarEnvio);
        }

        public Task EnviarAwait()
        {
            return Task.Factory.StartNew(this.IniciarEnvio);
        }

        private void IniciarEnvio()
        {
            this.StreamArquivo = this.RetornarStreamArquivo();
            if (!this.StreamArquivo.CanSeek)
            {
                throw new Erro("Não é possivel mover o ponteido da stream");
            }

            this.ParteAtual = 1;
            this.TotalBytes = this.StreamArquivo.Length;
            this.TotalPartes = (int)Math.Ceiling((this.TotalBytes / (double)this.TamanhoPacote));
            this.ChecksumArquivo = ChecksumUtil.RetornarChecksum(this.StreamArquivo);
            this.EnviarPacoteAtual();
        }

        #region Enviar pacote

        private void EnviarProixmoPacote()
        {
            if (this.ParteAtual < this.TotalPartes)
            {
                this.ParteAtual += 1;
                this.EnviarPacoteAtual();
            }
            else
            {
                this.FinalizarEnvio();
            }
        }

        private void EnviarPacoteAtual()
        {
            var resultado = this.TentarEnviarPacote();
            if (resultado.IsSucesso)
            {
                this.Resultado = resultado;
                this.NotificarProgressoEnvio();
                this.EnviarProixmoPacote();
            }
            else
            {
                this.RecuperarEnvio(resultado);
            }
        }

        private ResultadoServicoArquivo TentarEnviarPacote()
        {
            try
            {
                return this.EnviarPacoteInterno();
            }
            catch (Exception ex)
            {
                if (this.TentativaEnviarPacoteAtual > MAXIMO_TENTATIVA_ENVIAR_ARQUIVO)
                {
                    throw new Erro($"Não foi possivel enviar pacote {this.UrlServicoArquivo} Arquivo: {this.Arquivo.Id}", ex);
                }
                this.TentativaEnviarPacoteAtual += 1;
                return this.EnviarPacoteInterno();
            }
        }

        private ResultadoServicoArquivo EnviarPacoteInterno()
        {
            var pacote = this.RetornarPacote();
            var token = Token.RetornarNovoToken();

            this.ChecksumPacoteAtual = ChecksumUtil.RetornarChecksum(pacote);

            var parametros = this.RetornarParametros(pacote.Length);
            var urlEnviarImagem = ServicoImagemClienteUtil.RetornarEnderecoEnviarImagem(this.UrlServicoArquivo);
            var requisicao = (HttpWebRequest)WebRequest.Create(urlEnviarImagem);

            requisicao.Headers.Add(ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO, this.IdentificadorProprietario.ToString()); ; ;
            foreach (var item in parametros)
            {
                requisicao.Headers.Add(item.Key, Base64Util.Encode(item.Value));
            }
            requisicao.Headers.Add(ParametrosComunicacao.TOKEN, HttpUtility.UrlEncode(token));

            requisicao.Timeout = Int32.MaxValue;
            requisicao.Proxy = null;
            requisicao.ContentType = "application/octet-stream";
            requisicao.ContentLength = pacote.Length;
            requisicao.Method = "POST";

            using (var streamRequisicao = requisicao.GetRequestStream())
            {
                streamRequisicao.Write(pacote, 0, pacote.Length);
            }

            using (var resposta = (HttpWebResponse)requisicao.GetResponse())
            {
                using (var streamResposta = resposta.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(streamResposta, Encoding.UTF8))
                    {
                        var json = streamReader.ReadToEnd();
                        var resultado = JsonUtil.Deserializar<ResultadoServicoArquivo>(json, false);
                        return resultado;
                    }
                }
            }
        }

        private byte[] RetornarPacote()
        {
            var posicao = (this.ParteAtual - 1) * this.TamanhoPacote;
            var tamanhoPacote = this.RetornarTamanhoPacote(posicao);
            this.StreamArquivo.Seek(posicao, SeekOrigin.Begin);
            var pacote = new byte[tamanhoPacote];
            var tamanhoPacoteAtual = this.RetornarTamanhoPacote(posicao);
            this.StreamArquivo.Read(pacote, 0, tamanhoPacoteAtual);
            return pacote;
        }

        private int RetornarTamanhoPacote(long posicaoAtual)
        {
            var fim = (posicaoAtual + this.TamanhoPacote);
            if (fim < this.TotalBytes)
            {
                return this.TamanhoPacote;
            }
            return (int)(this.TotalBytes - posicaoAtual);
        }

        protected virtual Dictionary<string, string> RetornarParametros(int tamanhoPacoteAtual)
        {
            return new Dictionary<string, string>
            {
                { ConstantesServicoArquivo.ID_ARQUIVO, this.Arquivo.Id.ToString() },
                { ConstantesServicoArquivo.IDENTIFICADOR_SESSAO_USUARIO, this.IdentificadorSessaoUsuario.ToString() },
                { ConstantesServicoArquivo.TAMANHO_PACOTE, this.TamanhoPacote.ToString() },
                //{ ConstantesServicoArquivo.TAMANHO_PACOTE_ATUAL, tamanhoPacoteAtual.ToString() },
                { ConstantesServicoArquivo.CHECKSUM_ARQUIVO, this.ChecksumArquivo },
                { ConstantesServicoArquivo.CHECKSUM_PACOTE, this.ChecksumPacoteAtual },
                { ConstantesServicoArquivo.TOTAL_PARTES, this.TotalPartes.ToString() },
                { ConstantesServicoArquivo.PARTE_ATUAL, this.ParteAtual.ToString() },
                { ConstantesServicoArquivo.TOTAL_BYTES, this.TotalBytes.ToString() },
                { ConstantesServicoArquivo.ASEMMBLY_QUALIFIED_NAME, this.Arquivo.GetType().AssemblyQualifiedName },
                { ConstantesServicoArquivo.IDENTIFICADOR_USUARIO, this.CredencialUsuario.IdentificadorUsuario },
                { ConstantesServicoArquivo.SENHA, this.CredencialUsuario.Senha },
                { ConstantesServicoArquivo.NOME_TIPO_ARQUIVO, this.Arquivo.GetType().Name }

            };

        }

        #endregion

        #region Finalizar 

        private void FinalizarEnvio()
        {
            this.NotificarEnvioFinalizado();
        }

        #endregion

        #region Recuperacao

        private void RecuperarEnvio(ResultadoServicoArquivo resultado)
        {
            this.Erro = this.RetornarErro(resultado);
            this.Resultado = resultado;
            switch (resultado.TipoErroServicoArquivo)
            {
                case EnumTipoErroServicoArquivo.ChecksumPacoteDiferente:
                    this.TentativaEnviarPacoteAtual += 1;
                    this.RenviarEnviarPacoteAtual();
                    break;
                case EnumTipoErroServicoArquivo.ChecksumArquivoDiferente:
                case EnumTipoErroServicoArquivo.TotalBytesDiferente:
                case EnumTipoErroServicoArquivo.ArquivoTempEmUso:

                    this.TentativaEnviarArquivo += 1;
                    this.ReiniciarEnvioArquivo();
                    break;

                case EnumTipoErroServicoArquivo.Desconhecido:

                    throw new Erro("Erro desconhecido ao enviar imagem");


                default:

                    throw new Erro($"O {nameof(resultado.TipoErroServicoArquivo)} não suportado");
            }
        }

        private Exception RetornarErro(ResultadoServicoArquivo resultado)
        {
            switch (resultado.TipoErroServicoArquivo)
            {
                case EnumTipoErroServicoArquivo.ChecksumArquivoDiferente:

                    return new ErroChecksumArquivo();

                case EnumTipoErroServicoArquivo.ChecksumPacoteDiferente:

                    return new ErroChecksumPacote();

                case EnumTipoErroServicoArquivo.TotalBytesDiferente:

                    return new ErroTotalBytesDiferente();

                case EnumTipoErroServicoArquivo.ArquivoTempEmUso:

                    return new ErroArquivoEmUso();

                case EnumTipoErroServicoArquivo.Desconhecido:

                    return new Erro("Erro desconhecido");

                case EnumTipoErroServicoArquivo.ArquivoNaoEncontrado:

                    return new ErroArquivoNaoEncontrado();

                default:

                    throw new Erro($"Tipo do erro '{resultado.TipoErroServicoArquivo.ToString()}' servico arquivo não é suportado ");

            }
        }

        private void RenviarEnviarPacoteAtual()
        {
            if (this.TentativaEnviarPacoteAtual > MAXIMO_TENTATIVA_ENVIAR_PACOTE)
            {
                this.ReiniciarEnvioArquivo();
                return;
            }
            if (!RedeUtil.InternetConectada())
            {
                this.VerticarConexaoInternet();
            }
            this.EnviarPacoteAtual();
        }

        private void ReiniciarEnvioArquivo()
        {
            if (this.TentativaEnviarArquivo > MAXIMO_TENTATIVA_ENVIAR_ARQUIVO)
            {
                throw new Erro($"Não foi possivel enviar o arquivo id {this.Arquivo.Id} {this.Arquivo.NomeArquivo}", this.Erro);
            }

            if (!RedeUtil.InternetConectada())
            {
                this.VerticarConexaoInternet();
            }
            this.IniciarEnvio();
        }

        private void VerticarConexaoInternet()
        {
            InternetUtil.AguardarRestabelecerInternet();
        }

        #endregion

        #region Envetos

        private void NotificarProgressoEnvio()
        {
            var processoEnviado = (this.ParteAtual / (double)this.TotalPartes) * 100;
            this.ProgressoAlterado?.Invoke(this, new ProgressoEventArgs(processoEnviado));
        }

        private void NotificarEnvioFinalizado()
        {
            this.EnvioFinalizado?.Invoke(this, new ErrorEventArgs(this.Erro));
        }

        #endregion

        protected abstract Stream RetornarStreamArquivo();


        public void Dispose()
        {
            this.StreamArquivo?.Dispose();
            this.StreamArquivo = null;
        }

    }
}
