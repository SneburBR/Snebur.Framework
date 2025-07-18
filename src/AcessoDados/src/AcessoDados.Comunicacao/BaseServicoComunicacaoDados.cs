using Snebur.Comunicacao;
using Snebur.Servicos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Snebur.AcessoDados.Comunicacao
{
    public abstract class BaseServicoComunicacaoDados<TContextoDados> : BaseComunicacaoServidor, IDisposable where TContextoDados : BaseContextoDados
    {
        private bool _isServicoTransacionadoDB = true;
        private Exception _erroInicializacao;

        public TContextoDados ContextoDados { get; private set; }
        public IsolationLevel IsolamentoTransacao { get; protected set; } = ConfiguracaoAcessoDados.IsolamentoLevelSalvarPadrao;
        public virtual bool IsPermitirIdentificadorProprietarioGlobal { get; protected set; } = false;

        protected List<Action> ExecutarDepoisCommit { get; } = new List<Action>();

        public string CaminhoAplicacao { get; private set; }

        public bool IsServicoTransacionadoDB
        {
            get => this.IsManterCache == false && this._isServicoTransacionadoDB;
            protected set
            {
                if (this.IsManterCache && value)
                {
                    var erro = new Erro("Não é possível ativar IsServicoTransacionadoDB quando IsManterCache está ativado");
                    if (Debugger.IsAttached)
                    {
                        throw erro;
                    }
                    LogUtil.ErroAsync(erro);
                }
                this._isServicoTransacionadoDB = value;
            }
        }

        private bool IsSessaoUsuarioValida
        {
            get
            {
                return this._erroInicializacao == null &&
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
                    !this.ContextoDados.IsUsuarioLogadoAutorizadoIdentificadorProprietarioGlobal())
                {
                    throw new ErroSeguranca("Identificador global não autorizado",
                                            EnumTipoLogSeguranca.IdentificadorProprietarioGlobalNaoAutorizado);
                }
                this.CaminhoAplicacao = requisicao.CaminhoAplicacao;

            }
            catch (Exception ex)
            {
                this._erroInicializacao = ex;
            }
        }

        public void Inicializar(TContextoDados contexto, bool isPodeDispensarServico)
        {
            this.ContextoDados = contexto;
            this._isPodeDispensarServico = isPodeDispensarServico;
        }

        protected override object RetornarResultadoOperacao(MethodInfo metodoOperacao,
                                                            object[] parametros)
        {
            try
            {
                if (this.IsSessaoUsuarioValida)
                {
                    var resultado = base.RetornarResultadoOperacao(metodoOperacao, parametros);
                    if (this.ContextoDados.IsExisteTransacao)
                    {
                        this.ContextoDados.Commit();
                    }
                    this.ExecutarDepoisCommit.ForEach(acao => acao());
                    return resultado;
                }

                if (this._erroInicializacao != null)
                {
                    throw this._erroInicializacao;
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
                    var mensagemErro = $"Erro ao executar método {metodoOperacao.Name} no serviço {this.GetType().Name} \r\n{detalhesParametros}";

                    throw new ErroComunicacao(mensagemErro, ex);
                }
            }

            var status = this.ContextoDados?.SessaoUsuarioLogado?.Status ?? Snebur.Dominio.EnumStatusSessaoUsuario.Desconhecida;
            var identificadorSessaoUsuario = this.ContextoDados?.SessaoUsuarioLogado?.IdentificadorSessaoUsuario ?? Guid.Empty;
            return new ResultadoSessaoUsuarioInvalida(status,
                                                      identificadorSessaoUsuario,
                                                      "Sessão do usuário inválida");
        }

        private bool IsErroSessaoInvalida(Exception ex)
        {
            return ErroUtil.IsErroSessaoInvalida(ex);
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
            if (this._isPodeDispensarServico)
            {
                base.Dispose();
                this.ContextoDados?.Dispose();
                this.ContextoDados = null;
            }
        }
    }

}
