using Snebur.Net;

namespace Snebur.Comunicacao;

public abstract class BaseChamadaServico
{
    private const int MAXIMO_TENTATIVA = 3;
    private string NomeManipulador { get; }
    public ContratoChamada ContratoChamada { get; }
    public string UrlServico { get; }
    public Type TipoRetorno { get; }
    public Dictionary<string, string>? ParametrosCabecalhoAdicionais { get; }

    public BaseChamadaServico(
        string nomeManipualador,
        ContratoChamada constratoChamada,
        string urlServico,
        Type tipoRetorno,
        Dictionary<string, string>? parametrosCabecalhoAdicionais)
    {
        this.NomeManipulador = nomeManipualador;
        this.ContratoChamada = constratoChamada;
        this.UrlServico = urlServico;
        this.TipoRetorno = tipoRetorno;
        this.ParametrosCabecalhoAdicionais = parametrosCabecalhoAdicionais;
    }

    protected object? RetornarValorChamada()
    {
        var resultadoChamada = this.RetornarResultadoChamada();
        switch (resultadoChamada)
        {
            case ResultadoChamadaTipoPrimario resultadoTipoPrimario:

                return ConverterUtil.ConverterTipoPrimario(
                    resultadoTipoPrimario.Valor,
                    resultadoTipoPrimario.TipoPrimarioEnum);

            case ResultadoChamadaEnum resultadoChamadaEnum:

                return resultadoChamadaEnum.Valor;

            case ResultadoChamadaBaseDominio resultadoChamadaBaseDominio:

                return resultadoChamadaBaseDominio.BaseDominio;

            case ResultadoChamadaLista resultadoChamadaLista:

                return this.RetornarResultadoChamadaLista(resultadoChamadaLista);

            case ResultadoChamadaVazio resultadoChamadaVazio:

                return null;

            case ResultadoSessaoUsuarioInvalida resultadoSessaoUsuarioInvalida:

                return resultadoSessaoUsuarioInvalida;

            case ResultadoChamadaErro resultadoChamadaErro:

                throw new ErroNaoSuportado($"O  erro no resultado da chamada  {resultadoChamada?.GetType().Name ?? "null"} \r\n " +
                                           $" {resultadoChamadaErro.MensagemErro} ");

            default:

                throw new ErroNaoSuportado($"O resultado da chamada não é suportado {resultadoChamada?.GetType().Name ?? "null"}");
        }
    }

    private IList RetornarResultadoChamadaLista(ResultadoChamadaLista resultadoLista)
    {
        Guard.NotNullOrWhiteSpace(resultadoLista.AssemblyQualifiedName);

        var tipoItem = Type.GetType(resultadoLista.AssemblyQualifiedName);

        if (tipoItem is null)
        {
            throw new ErroNaoSuportado($"O tipo do item da lista não foi encontrado {resultadoLista.AssemblyQualifiedName}");
        }

        var tipoLista = typeof(List<>).MakeGenericType(tipoItem);
        var listaTipada = (IList?)Activator.CreateInstance(tipoLista);
        if (listaTipada is null)
        {
            throw new ErroNaoSuportado($"Não foi possível criar a lista do tipo {tipoLista.FullName}");
        }

        var lista = this.RetornarListaResultadoChamadaLista(resultadoLista);
        foreach (var item in lista)
        {
            listaTipada.Add(item);
        }
        return listaTipada;
    }

    private ICollection RetornarListaResultadoChamadaLista(ResultadoChamadaLista resultadoLista)
    {
        if (resultadoLista is ResultadoChamadaListaBaseDominio listaBasedominio)
        {
            return listaBasedominio.BasesDominio;
        }

        if (resultadoLista is ResultadoChamadaListaTipoPrimario listaTipoPimario)
        {
            return listaTipoPimario.Valores;
        }

        if (resultadoLista is ResultadoChamadaListaEnum resultadoListaEnum)
        {
            return resultadoListaEnum.Valores;
        }
        throw new ErroNaoSuportado(String.Format("O resultado da chamada não é suportado"));
    }

    private ResultadoChamada RetornarResultadoChamada(int tentativa = 0)
    {
        var cabecalho = this.ContratoChamada.Cabecalho;

        Guard.NotNull(cabecalho);
        Guard.NotNull(cabecalho.CredencialUsuario?.IdentificadorUsuario);
        Guard.NotNull(cabecalho.CredencialServico?.IdentificadorUsuario);

        var nomeAssembly = AplicacaoSnebur.AtualRequired.NomeAplicacao;
        var identificadorAplicacao = AplicacaoSnebur.AtualRequired.IdentificadorAplicacao;
        var conteudo = this.RetornarConteudoCompactado();
        var token = Token.RetornarNovoToken();
        var nomeArquivo = Md5Util.RetornarHash(token);

        var urlServico = UriUtil.CombinarCaminhos(this.UrlServico, nomeArquivo);

        //var requisicaoHttp = HttpWebRequest.Create(urlServico);
        var requisicaoHttp = HttpWebRequestProxy.Create(urlServico);

        var identificadorUsuario = CriptografiaUtil.Criptografar(token, cabecalho.CredencialUsuario.IdentificadorUsuario);
        var identifcadorProprietario = cabecalho.IdentificadorProprietario;

        requisicaoHttp.Headers[ConstantesCabecalho.IDENTIFICADOR_USUARIO] = identificadorUsuario;
        requisicaoHttp.Headers[ConstantesCabecalho.SENHA] = CriptografiaUtil.Criptografar(token, cabecalho.CredencialServico.Senha);
        requisicaoHttp.Headers[ConstantesCabecalho.NOME_ASSEMBLY_APLICACAO] = nomeAssembly;
        requisicaoHttp.Headers[ConstantesCabecalho.IDENTIFICADOR_APLICACAO] = identificadorAplicacao;

        if (!String.IsNullOrEmpty(identifcadorProprietario))
        {
            requisicaoHttp.Headers[ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO] = identifcadorProprietario;
        }
        else
        {
            var aplicacao = AplicacaoSnebur.AtualRequired;
            if (aplicacao.IsAplicacaoAspNet && aplicacao.AspNetRequired.IsPossuiRequisicaoAspNetAtiva)
            {
                //var identificadorProprietario = AplicacaoSneburAspNet.AtualAspNet?.HttpContext?.Request.Headers[ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO];
                var identificadorProprietario = aplicacao.AspNetRequired.RetornarValueCabecalho(ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO);
                if (!String.IsNullOrEmpty(identificadorProprietario))
                {
                    requisicaoHttp.Headers[ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO] = identificadorProprietario;
                }
            }
        }

        if (this.ParametrosCabecalhoAdicionais != null)
        {
            foreach (var parametroAdicional in this.ParametrosCabecalhoAdicionais)
            {
                requisicaoHttp.Headers[parametroAdicional.Key] = parametroAdicional.Value;
            }
        }
        requisicaoHttp.ContentType = "application/octet-stream";
        requisicaoHttp.ContentLength = conteudo.Length;
        requisicaoHttp.Method = "POST";

        requisicaoHttp.Headers.Add(ParametrosComunicacao.TOKEN, WebUtility.UrlEncode(token));
        requisicaoHttp.Headers.Add(ParametrosComunicacao.MANIPULADOR, this.NomeManipulador);
        requisicaoHttp.Timeout = (int)TimeSpan.FromMinutes(2).TotalMilliseconds;

#if DEBUG
        if (DebugUtil.IsAttached)
        {
            requisicaoHttp.Timeout = (int)TimeSpan.FromHours(1).TotalMilliseconds;
        }
#endif
        try
        {
            using (var streamRequisicao = requisicaoHttp.GetRequestStream())
            {
                streamRequisicao.Write(conteudo, 0, conteudo.Length);
            }

            using (var resposta = requisicaoHttp.GetResponse())
            {
                if (!resposta.IsStatusCodeOk)
                {
                    throw new ErroComunicacao("Falha de comunicação com servidor");
                }

                using (var streamResposta = resposta.GetResponseStream())
                {
                    InternetUtil.FecharMensagemSemInternet();
                    Guard.NotNull(streamResposta);

                    var jsonResposta = PacoteUtil.DescompactarPacote(streamResposta);
                    Guard.NotNull(jsonResposta);

                    var resultado = JsonUtil.Deserializar<ResultadoChamada>(jsonResposta, EnumTipoSerializacao.DotNet);
                    Guard.NotNull(resultado);
                    return resultado;
                }
            }
        }
        catch (Exception)
        {
            if (!RedeUtil.InternetConectada())
            {
                //tentativa = 0;
                InternetUtil.AguardarRestabelecerInternet();
            }

            if (AplicacaoSnebur.AtualRequired.TipoAplicacao == EnumTipoAplicacao.DotNet_WebService ||
                tentativa > MAXIMO_TENTATIVA)
            {
                throw;
            }
            return this.RetornarResultadoChamada(tentativa + 1);
        }
    }

    private byte[] RetornarConteudoCompactado()
    {
        var jsonString = JsonUtil.Serializar(this.ContratoChamada, EnumTipoSerializacao.DotNet);
        return PacoteUtil.CompactarPacote(jsonString);
    }
}