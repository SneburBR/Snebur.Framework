using Snebur.Reflexao;
using System.IO;
using System.Xml.Serialization;
using Snebur.Linq;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

#if NET6_0_OR_GREATER
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace Snebur.Comunicacao;

public class Requisicao : IDisposable
{
    private string? _jsonRequisacao;

    #region Propriedades
    public CredencialServico CredencialServico { get; }
    public string IdentificadorProprietario { get; }
    public string NomeManipulador { get; }
    [XmlIgnore, JsonIgnore]
    public HttpContext HttpContext { get; }
    public string CaminhoAplicacao { get; }

    [MemberNotNullWhen(true,
        nameof(ContratoChamada),
        nameof(Cabecalho),
        nameof(CredencialUsuario),
        nameof(InformacaoSessaoUsuario),
        nameof(Operacao))]
    public bool IsRequsicaoValida { get; private set; }
    public string? Operacao { get; private set; }
    public Cabecalho? Cabecalho { get; private set; }
    public ContratoChamada? ContratoChamada { get; private set; }
    public CredencialUsuario? CredencialUsuario { get; private set; }
    public CredencialUsuario? CredencialAvalisata { get; private set; }
    public InformacaoSessao? InformacaoSessaoUsuario { get; private set; }

    public Guid? IdentificadorSessaoUsuario { get; set; }
    public DateTime DataHoraChamada { get; private set; }
    public Dictionary<string, object?> Parametros { get; } = new Dictionary<string, object?>();

    public bool IsSerializarJavascript { get; set; }
    public EnumTipoSerializacao TipoSerializacao => this.IsSerializarJavascript ? EnumTipoSerializacao.Javascript :
                                                                                  EnumTipoSerializacao.DotNet;

    #endregion

    #region Construtor

    public Requisicao(HttpContext httpContext,
                      CredencialServico credencialServico,
                      string? identificadorProprietario,
                      string nomeManipulador)
    {
        var caminhoAplicacao = httpContext.Items[ConstantesItensRequsicao.CAMINHO_APLICACAO]?.ToString();
        if (!Directory.Exists(caminhoAplicacao))
        {
            throw new DirectoryNotFoundException($"Caminho da aplicação não encontrado {this.CaminhoAplicacao}");
        }
        Guard.NotNull(identificadorProprietario);

        this.CaminhoAplicacao = caminhoAplicacao;
        this.HttpContext = httpContext;
        this.CredencialServico = credencialServico;
        this.IdentificadorProprietario = identificadorProprietario;
        this.NomeManipulador = nomeManipulador;
    }

#if NET6_0_OR_GREATER
    public async Task ProcessarRequisicaoAsync()
    {
        var httpContext = this.HttpContext;
        using (var streamRequisicao = await this.RetornarInputStreamBufferizado(httpContext))
        {
            this.ProcessarRequisicao(streamRequisicao);
        }
    }

    private async Task<MemoryStream> RetornarInputStreamBufferizado(HttpContext context)
    {
        try
        {
            var resultado = new MemoryStream();
            var streamOrigem = context.Request.Body;
            while (true)
            {
                var buffer = new byte[1024];
                var bytesRead = await streamOrigem.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }
                await resultado.WriteAsync(buffer, 0, bytesRead);
            }
            resultado.Position = 0;
            return resultado;

        }
        catch
        {
            throw new ErroReceberStreamCliente("Erro ao receber a stream buffered, a conexão foi fechada pelo cliente");
        }
    }

#else
    public void ExtrairDadosRequisicao()
    {
        var httpContext = this.HttpContext;
        using (var streamRequisicao = this.RetornarInputStreamBufferizado(httpContext))
        {
            this.ExtrairDadosRequisicao(streamRequisicao);
        }
    }

    private MemoryStream RetornarInputStreamBufferizado(HttpContext context)
    {
        try
        {
            return StreamUtil.RetornarMemoryStreamBuferizada(context.Request.GetBufferedInputStream());
        }
        catch
        {
            throw new ErroReceberStreamCliente("Erro ao receber a stream buffered, a conexão foi fechada pelo cliente");
        }
    }


#endif

    private void ProcessarRequisicao(MemoryStream streamRequisicao)
    {
        var json = PacoteUtil.DescompactarPacote(streamRequisicao);
        if (DebugUtil.IsAttached)
        {
            this._jsonRequisacao = json;
        }

        var contratoChamada = JsonUtil.Deserializar<ContratoChamada>(json, EnumTipoSerializacao.DotNet);
        Guard.NotNull(contratoChamada);

        this.ContratoChamada = contratoChamada;
        this.Cabecalho = contratoChamada.Cabecalho;
        this.InformacaoSessaoUsuario = contratoChamada.InformacaoSessao;
        this.IdentificadorSessaoUsuario = contratoChamada.IdentificadorSessaoUsuario;
        this.CredencialUsuario = contratoChamada.Cabecalho?.CredencialUsuario;
        this.CredencialAvalisata = contratoChamada.Cabecalho?.CredencialAvalista;
        this.Operacao = this.ContratoChamada.Operacao;
        this.DataHoraChamada = this.ContratoChamada.DataHora;

        this.AdicionarItensrequisicaoAtual();

        this.IsSerializarJavascript = (this.InformacaoSessaoUsuario?.TipoAplicacao == EnumTipoAplicacao.Typescript);

        foreach (var parametro in this.ContratoChamada.Parametros)
        {
            if (String.IsNullOrWhiteSpace(parametro.Nome))
            {
                throw new Erro("Nome do parâmetro não informado na chamada");
            }

            var nomeParametro = NormalizacaoUtil.NormalizarNomeParametro(parametro.Nome);
            if (this.Parametros.ContainsKey(nomeParametro))
            {
                throw new Erro($"Parâmetro '{nomeParametro}' duplicado na chamada");
            }
            this.Parametros.Add(NormalizacaoUtil.NormalizarNomeParametro(parametro.Nome), this.RetornarValorParametroChamada(parametro));
        }

        if (this.IdentificadorSessaoUsuario == Guid.Empty)
        {
            throw new Erro("Identificador da sessão do usuário não foi definido");
        }
        var isRequsicaoValida = this.CheckIsRequiscaoValida();
        if (!isRequsicaoValida)
        {
            Debugger.Break();
        }
        this.IsRequsicaoValida = isRequsicaoValida;
    }

    #endregion

    #region Métodos

    private bool CheckIsRequiscaoValida()
    {
        var cabecalho = this.Cabecalho;
        if (cabecalho is null)
            return false;

        var credencialServicoRequesicao = cabecalho.CredencialServico;
        if (credencialServicoRequesicao is null)
            return false;

        if (ValidacaoUtil.IsNullOrEmpy(this.IdentificadorSessaoUsuario))
            return false;

        if (string.IsNullOrEmpty(this.Operacao))
            return false;

        if (this.InformacaoSessaoUsuario is null)
            return false;

        return (this.CredencialServico.IdentificadorUsuario == credencialServicoRequesicao.IdentificadorUsuario &&
                this.CredencialServico.Senha == credencialServicoRequesicao.Senha);
    }

    #endregion

    #region Métodos privados

    private object? RetornarValorParametroChamada(ParametroChamada parametro)
    {
        switch (parametro)
        {
            case ParametroChamadaNulo parametroChamadaNulo:

                return null;

            case ParametroChamadaBaseDominio parametroBaseDominio:

                return parametroBaseDominio.BaseDominio;

            case ParametroChamadaEnum parametroChamadaEnum:

                return parametroChamadaEnum.Valor;

            case ParametroChamadaTipoPrimario parametroChamadaTipoPrimario:
                var tipoPrimarioEnum = parametroChamadaTipoPrimario.TipoPrimarioEnum;

                if (tipoPrimarioEnum == EnumTipoPrimario.EnumValor)
                {
                    throw new InvalidOperationException("Usar ParametroChamadaEnum");
                }
                return ConverterUtil.ConverterTipoPrimario(parametroChamadaTipoPrimario.Valor, tipoPrimarioEnum);

            case ParametroChamadaListaBaseDominio parametroChamadaListaBaseDominio:

                var basesDominio = parametroChamadaListaBaseDominio.BasesDominio;

                Guard.NotNullOrWhiteSpace(parametro.AssemblyQualifiedName);

                var tipoBaseDominio = Type.GetType(parametro.AssemblyQualifiedName);
                if (tipoBaseDominio is null)
                {
                    throw new InvalidOperationException($"Tipo '{parametro.AssemblyQualifiedName}' não encontrado");
                }

                var tipoLista = typeof(List<>).MakeGenericType(tipoBaseDominio);
                var lista = Activator.CreateInstance(tipoLista) as IList
                    ?? throw new InvalidOperationException("Não foi possível criar a lista de base de domínio");

                foreach (var baseDominio in parametroChamadaListaBaseDominio.BasesDominio)
                {
                    lista.Add(baseDominio);
                }
                return lista;

            case ParametroChamadaListaEnum parametroListaEnum:

                Guard.NotNullOrWhiteSpace(parametroListaEnum.AssemblyQualifiedName);
                var tipoEnum = Type.GetType(parametroListaEnum.AssemblyQualifiedName);

                if (tipoEnum is null)
                {
                    throw new InvalidOperationException($"Tipo '{parametroListaEnum.AssemblyQualifiedName}' não encontrado");
                }

                var tipoListaEnum = typeof(List<>).MakeGenericType(tipoEnum);
                var listaEnum = Activator.CreateInstance(tipoListaEnum) as IList
                    ?? throw new InvalidOperationException("Não foi possível criar a lista de enum");

                foreach (var valorEnum in parametroListaEnum.Valores)
                {
                    listaEnum.Add(valorEnum);
                }
                return listaEnum;

            case ParametroChamadaListaTipoPrimario parametroChamadaListaTipoPrimario:

                var tipoListaPrimarioEnum = parametroChamadaListaTipoPrimario.TipoPrimarioEnum;
                var tipoPrimario = ReflexaoUtil.RetornarTipoPrimario(tipoListaPrimarioEnum);
                var tipoListaPrimario = typeof(List<>).MakeGenericType(tipoPrimario);

                var listaPrimario = Activator.CreateInstance(tipoListaPrimario) as IList
                    ?? throw new InvalidOperationException("Não foi possível criar a lista de tipo primário");

                if (parametroChamadaListaTipoPrimario.Lista != null)
                {
                    foreach (var valor in parametroChamadaListaTipoPrimario.Lista)
                    {
                        listaPrimario.Add(ConverterUtil.ConverterTipoPrimario(valor, tipoListaPrimarioEnum));
                    }
                }
                return listaPrimario;

            default:

                throw new Erro("Parâmetro não suportado ");
        }
    }

    #endregion

    private void AdicionarItensrequisicaoAtual()
    {
        var context = this.HttpContext;
        if (context is null)
        {
            return;
        }

        lock (context.Items.SyncLock())
        {
            context.AdicionrItem(ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL, this.InformacaoSessaoUsuario);
            context.AdicionrItem(ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO, this.CredencialUsuario);
            context.AdicionrItem(ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_PROPRIETARIO, this.IdentificadorProprietario);

            if (this.CredencialAvalisata != null)
            {
                context.AdicionrItem(ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO_AVALISTA, this.CredencialUsuario);
            }
        }
    }

    private void RemoverItensRequisicaoAtual()
    {
        var context = this.HttpContext;
        if (context is null)
        {
            return;
        }

        lock (context.Items.SyncLock())
        {
            context.RemoverItem(ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL);
            context.RemoverItem(ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO);
            context.RemoverItem(ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO_AVALISTA);
            context.RemoverItem(ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_PROPRIETARIO);
        }
    }

    #region IDisposable

    public void Dispose()
    {
        this.RemoverItensRequisicaoAtual();
    }

    public void Processar()
    {
        throw new NotImplementedException();
    }

    #endregion
}
