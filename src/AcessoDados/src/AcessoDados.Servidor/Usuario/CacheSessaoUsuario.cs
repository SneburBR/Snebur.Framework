using Snebur.Seguranca;
using System.Timers;

namespace Snebur.AcessoDados;

public partial class CacheSessaoUsuario : IDisposable
{
    private BaseContextoDados _contexto;

    //implementar classe de Cache para sessaoUsuario para melhorar o desenpenho, solução paliativa, uusuario o contexto pra criar sql e colocar cache
    //private const string PARAMETRO_IDENTIFICADOR_SESSAO_USUARIO = "@IdentificadorSessaoUsuario ";
    //private const string PARAMETRO_IDENTIFICADOR_USUARIO = "@IdentificadorUsuario ";

    //private const string SQL_DATA_HORA_ULTIMA_ATUALIZACAO = "  UPDATE usuario.SessaoUsuario SET DataHoraUltimoAcesso = GETUTCDATE() WHERE IdentificadorSessaoUsuario = " + PARAMETRO_IDENTIFICADOR_SESSAO_USUARIO;
    //private const string SQL_VERFICAR_CREDENCIAL = "  SELECT e2.Senha  from usuario.Identificacao as e1 INNER JOIN usuario.Usuario as e2 on  e1.Id = e2.Id WHERE e1.Identificador = " + PARAMETRO_IDENTIFICADOR_USUARIO;
    //private const string SQL_ESTADO_SESSAO_USUARIO = "  select Status from usuario.SessaoUsuario WHERE IdentificadorSessaoUsuario = @IdentificadorSessaoUsuario ";

    //private const string SQL_ATUALIZAR_ESTADO_SESSAO_USUARIO = "  UPDATE usuario.SessaoUsuario SET DataHoraUltimoAcesso = GETUTCDATE() WHERE IdentificadorSessaoUsuario = @IdentificadorSessaoUsuario";

    private static readonly double TIMEOUT_REMOVER_CACHE = DebugUtil.IsAttached ? TimeSpan.FromMinutes(5).TotalMilliseconds :
                                                                                  TimeSpan.FromHours(2).TotalMilliseconds;

    private static readonly double TIMEOUT_ATUALIZAR_STATUS_SESSAO = DebugUtil.IsAttached ? TimeSpan.FromMinutes(10).TotalMilliseconds :
                                                                                            TimeSpan.FromMinutes(5).TotalMilliseconds;

    public ISessaoUsuario SessaoUsuario { get; private set; }
    public IUsuario Usuario { get; private set; }
    public EnumStatusSessaoUsuario StatusSessaoUsuario { get; private set; }

    private DateTime DataHoraUltimoAcesso { get; set; }
    private Guid IdentificadorSessaoUsuario { get; }
    internal BaseContextoDados Contexto
    {
        get
        {
            if (this._contexto.IsDispensado == true)
            {
                throw new Exception($"CacheSessaoUsuario: O contexto de dados {this._contexto} foi dispensado");
            }
            return this._contexto;
        }
        set
        {
            if (value.IsDispensado)
            {
                throw new Exception($"CacheSessaoUsuario: O contexto de dados {value} foi dispensado");
            }
            this._contexto = value;
        }
    }

    private Credencial Credencial { get; }
    private InformacaoSessao? InformacaoSessaoUsuario { get; }
    private AjudanteSessaoUsuarioInterno AjudanteSessaoUsuario { get; }
    private bool IsNoticacaoStatusPendente { get; set; }

    private Timer TimerAtualizarStatus = new Timer(TIMEOUT_ATUALIZAR_STATUS_SESSAO);
    public object BloqueioInicializar = new object();
    public bool IsInicializado = false;

    public CacheSessaoUsuario(
        BaseContextoDados contexto,
        Credencial credencial,
        Guid identificadorSessaoUsuario,
        InformacaoSessao? informacaoSessaoUsuario)
    {
        this._contexto = contexto;
        this.IdentificadorSessaoUsuario = identificadorSessaoUsuario;
        this.Contexto = contexto;
        this.Credencial = credencial;
        this.InformacaoSessaoUsuario = informacaoSessaoUsuario;
        this.AjudanteSessaoUsuario = AjudanteSessaoUsuarioInterno.RetornarAjudanteUsuario(contexto);
        this.TimerAtualizarStatus.Stop();

        this.Inicializar();
        Guard.NotNull(this.Usuario);
        Guard.NotNull(this.SessaoUsuario);
    }

    private void Inicializar()
    {
        if (!this.IsInicializado)
        {
            lock (this.BloqueioInicializar)
            {
                try
                {
                    if (!this.IsInicializado)
                    {
                        this.InicializarInterno();
                        this.IsInicializado = true;
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.ErroAsync(ex);
                    this.IsInicializado = false;
                }
            }
        }
    }

    internal IUsuario? RetornarUsuarioAvalista(Credencial? credencialAvalista)
    {
        if (credencialAvalista is null)
        {
            return null;
        }
        return this.AjudanteSessaoUsuario.RetornarUsuario(this.Contexto, credencialAvalista);
    }

    internal IUsuario? UsuarioAnonimo
    {
        get
        {
            return this.AjudanteSessaoUsuario.UsuarioAnonimo;
        }
    }

    private void InicializarInterno()
    {
        Guard.NotNull(this.Credencial);

        var usuario = this.AjudanteSessaoUsuario.RetornarUsuario(this.Contexto, this.Credencial);
        if (usuario == null)
        {
            throw new ErroSessaoUsuarioInvalida($" O usuário em cache não foi encontrado: {this.Credencial.IdentificadorUsuario} -- {this.Credencial.Senha}. {this.Contexto.IdentificadorProprietario}");
            //this.Usuario = this.AjudanteSessaoUsuario.RetornarUsuario(CredencialAnonimo.Anonimo);
            //throw new Erro($"Não foi possível retornar o usuário para a credencial  {this.Credencial.IdentificadorUsuario}");
        }
        this.Usuario = usuario;
        this.SessaoUsuario = this.AjudanteSessaoUsuario
                                 .RetornarSessaoUsuario(this.Contexto,
                                                        this.Usuario,
                                                        this.IdentificadorSessaoUsuario,
                                                        this.InformacaoSessaoUsuario);

        this.NotificarSessaoUsuarioAtivaInterno();
        if (this.TimerAtualizarStatus == null)
        {
            this.TimerAtualizarStatus = new Timer(TIMEOUT_ATUALIZAR_STATUS_SESSAO);
        }

        this.DataHoraUltimoAcesso = DateTime.Now;

        var timer = this.TimerAtualizarStatus;
        timer.AutoReset = true;
        timer.Elapsed -= this.TimerAtualizarStatus_Elapsed;
        timer.Elapsed += this.TimerAtualizarStatus_Elapsed;
        timer.Reiniciar();
    }

    private void TimerAtualizarStatus_Elapsed(object? sender, ElapsedEventArgs e)
    {
        if (this.IsDispensado)
        {
            return;
        }

        var diferenca = DateTime.Now - this.DataHoraUltimoAcesso;
        if (diferenca.TotalMilliseconds > TIMEOUT_REMOVER_CACHE)
        {
            this.Dispose();
            GerenciadorCacheSessaoUsuario.Instancia.RemoverCacheSessaoUsuario(this.IdentificadorSessaoUsuario);
        }
        else
        {
            this.IsNoticacaoStatusPendente = true;
        }
    }
    public void NotificarSessaoAtivaAsync()
    {
        this.DataHoraUltimoAcesso = DateTime.Now;
        if (this.IsNoticacaoStatusPendente)
        {
            this.NotificarSessaoUsuarioAtivaInterno();
        }
    }
    private void NotificarSessaoUsuarioAtivaInterno()
    {
        var statusSessaoUsuario = this.RetornarStatusSessaoUsuario();
        if (statusSessaoUsuario == EnumStatusSessaoUsuario.Ativo ||
            statusSessaoUsuario == EnumStatusSessaoUsuario.Inativo ||
            statusSessaoUsuario == EnumStatusSessaoUsuario.Nova)
        {
            this.AjudanteSessaoUsuario.NotificarSessaoUsuarioAtiva(
                this.Contexto,
                this.Usuario,
                this.SessaoUsuario);
            statusSessaoUsuario = EnumStatusSessaoUsuario.Ativo;
        }
        this.StatusSessaoUsuario = statusSessaoUsuario;
        this.TimerAtualizarStatus?.Reiniciar();
    }

    private EnumStatusSessaoUsuario RetornarStatusSessaoUsuario()
    {
        if (this.IsValidarCredencialSessaoUsuario())
        {
            return this.AjudanteSessaoUsuario.RetornarStatusSessaoUsuario(this.Contexto, this.IdentificadorSessaoUsuario);
        }
        return EnumStatusSessaoUsuario.SenhaAlterada;
    }

    private bool IsValidarCredencialSessaoUsuario()
    {
        return this.AjudanteSessaoUsuario.IsValidarCredencialSessaoUsuario(this.Contexto,
                                                                           this.SessaoUsuario,
                                                                           this.Credencial);
    }

    public void FinalizarSessaoUsuario()
    {
        this.IsNoticacaoStatusPendente = true;
        this.NotificarSessaoAtivaAsync();
    }

    #region IDisposable Support

    private bool IsDispensado = false;
    private void Dispose(bool disposing)
    {
        if (!this.IsDispensado)
        {
            if (disposing)
            {
                this.TimerAtualizarStatus?.Dispose();
            }
            this.IsDispensado = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
    }
    #endregion
}