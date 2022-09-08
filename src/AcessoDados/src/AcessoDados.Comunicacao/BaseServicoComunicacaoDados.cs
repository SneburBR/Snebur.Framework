using Snebur.Comunicacao;
using Snebur.Servicos;
using Snebur.Utilidade;
using System;
using System.Data;
using System.Reflection;
using System.Text;

namespace Snebur.AcessoDados.Comunicacao
{
    public abstract class BaseServicoComunicacaoDados<TContextoDados> : BaseComunicacaoServidor, IDisposable where TContextoDados : BaseContextoDados
    {
        public TContextoDados ContextoDados { get; private set; }
        public bool IsServicoTransacionadoDB { get; protected set; } = true;
        public bool IsPermitirIdentificadorProprietarioGlobal { get; protected set; } = false;
        public IsolationLevel IsolamentoTransacao { get; protected set; } = ConfiguracaoAcessoDados.IsolamentoLevelSalvarPadrao;

        private Exception ErroInicializacao;

        private bool IsSessaoUsuarioValida
        {
            get
            {
                return this.ErroInicializacao == null &&
                       this.ContextoDados?.IsSessaoUsuarioAtiva == true;
            }
        }

        public BaseServicoComunicacaoDados()
        {

        }

        protected override void Inicializar(Requisicao requisicao)
        {
            try
            {
                this.ContextoDados = this.RetornarNovoContextoInterno();
                if (this.IsServicoTransacionadoDB)
                {
                    this.ContextoDados.IniciarNovaTransacao(this.IsolamentoTransacao);
                }

                if (this.ContextoDados.IsIdentificadorProprietarioGlobal &&
                  !this.IsPermitirIdentificadorProprietarioGlobal)
                {
                    throw new ErroSeguranca("Identificador global não autorizado",
                                            EnumTipoLogSeguranca.IdentificadorProprietarioGlobalNaoAutorizado);
                }

            }
            catch (Exception ex)
            {
                this.ErroInicializacao = ex;
            }
        }

        public void Inicializar(TContextoDados contexto)
        {
            this.ContextoDados = contexto;
        }

        protected override object RetornarResultadoOperacao(Requisicao requisicao,
                                                            MethodInfo metodoOperacao,
                                                            object[] parametros)
        {

            try
            {
                if (this.IsSessaoUsuarioValida)
                {
                    var resultado = base.RetornarResultadoOperacao(requisicao, metodoOperacao, parametros);
                    if (this.ContextoDados.IsExisteTransacao)
                    {
                        this.ContextoDados.Commit();
                    }
                    return resultado;
                }

                if (this.ErroInicializacao != null)
                {
                    throw this.ErroInicializacao;
                }
            }
            catch (Exception ex)
            {
                if (this.ContextoDados?.IsExisteTransacao == true)
                {
                    this.ContextoDados.Rollback();
                }

                if (!this.IsErroSessaoInvalida(ex))
                {
                    var detalhesParametros = this.RetornarDetalhesParametros(metodoOperacao, parametros);
                    var mensagemErro = $"Erro ao executar método {metodoOperacao.Name} no serviço {this.GetType().Name}\r\n{detalhesParametros}";
                    throw new ErroComunicacao(mensagemErro, ex);
                }

            }

            var estado = this.ContextoDados?.SessaoUsuarioLogado?.Estado ?? Snebur.Dominio.EnumEstadoSessaoUsuario.Desconhecida;
            var identificadorSessaoUsuario = this.ContextoDados?.SessaoUsuarioLogado?.IdentificadorSessaoUsuario ?? Guid.Empty;
            return new ResultadoSessaoUsuarioInvalida(estado, identificadorSessaoUsuario);

        }

        private bool IsErroSessaoInvalida(Exception ex)
        {
            return ErroUtil.IsTipo<ErroSessaoUsuarioExpirada>(ex) ||
                   ErroUtil.IsTipo<ErroSessaoUsuarioInvalida>(ex);
        }

        private string RetornarDetalhesParametros(MethodInfo metodoOperacao, object[] valoresParametros)
        {
            var sb = new StringBuilder();
            var parametros = metodoOperacao.GetParameters();
            if (parametros.Length != valoresParametros.Length)
            {
                sb.AppendLine($"O numero de parâmetros ({parametros.Length}) do método é diferente dos valores {valoresParametros.Length}");
            }

            for (var i = 0; i < valoresParametros.Length; i++)
            {
                if (parametros.Length < i)
                {
                    var valorParametro = valoresParametros[i];
                    var parametro = parametros[i];
                    var valor = valorParametro?.ToString() ?? "null";
                    sb.AppendLine($"Nome do parâmetro: {parametro.Name}; Tipo: {parametro.ParameterType.Name}; Valor : {valor}");
                }
            }
            return sb.ToString();
        }

        protected override object NormalizarResultadoOperacao(object resultadoOperacao)
        {
#if DEBUG
            if (DebugUtil.IsAttached && false)
            {
                if (resultadoOperacao is Resultado resultadoTipoado)
                {
                    resultadoTipoado.Comandos.AddRange(this.ContextoDados.Comandos);
                }

            }
#endif
            return resultadoOperacao;
        }

        #region Métodos abstratos

        protected abstract TContextoDados RetornarNovoContextoInterno();

        #endregion

        public override void Dispose()
        {
            base.Dispose();

            this.ContextoDados?.Dispose();
            this.ContextoDados = null;
        }
    }
}
