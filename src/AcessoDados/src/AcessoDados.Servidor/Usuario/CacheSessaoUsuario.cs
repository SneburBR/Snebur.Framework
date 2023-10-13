using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Utilidade;
using System;
using System.Timers;

namespace Snebur.AcessoDados
{
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

        private static readonly double TIMEOUT_ATUALIZAR_STATUS_SESSAO = DebugUtil.IsAttached ? TimeSpan.FromMinutes(1).TotalMilliseconds :
                                                                                                TimeSpan.FromMinutes(3).TotalMilliseconds;

        public ISessaoUsuario SessaoUsuario { get; private set; }
        public IUsuario Usuario { get; private set; }
        public EnumStatusSessaoUsuario StatusSessaoUsuario { get; private set; }

        private DateTime DataHoraUltimoAcesso { get; set; }
        private Guid IdentificadorSessaoUsuario { get; }
        internal BaseContextoDados Contexto
        {
            get
            {
                if (this._contexto.IsDispensado)
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
        private InformacaoSessaoUsuario InformacaoSessaoUsuario { get; }
        private AjudanteSessaoUsuarioInterno AjudanteSessaoUsuario { get; }
        private BaseConexao AjudanteConexao { get; }

        private bool IsNoticacaoStatusPendente { get; set; }

        private Timer TimerAtualizarStatus = new Timer(CacheSessaoUsuario.TIMEOUT_ATUALIZAR_STATUS_SESSAO);
        public object BloqueioInicializar = new object();
        public bool IsInicializado = false;

        public CacheSessaoUsuario(BaseContextoDados contexto,
                                  Credencial credencial,
                                  Guid identificadorSessaoUsuario,
                                  InformacaoSessaoUsuario informacaoSessaoUsuario)
        {
            this.IdentificadorSessaoUsuario = identificadorSessaoUsuario;
            this.Contexto = contexto;
            this.Credencial = credencial;
            this.InformacaoSessaoUsuario = informacaoSessaoUsuario;
            this.AjudanteSessaoUsuario = AjudanteSessaoUsuarioInterno.RetornarAjudanteUsuario(contexto);
            this.TimerAtualizarStatus.Stop();
        }

        internal void Inicializar()
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

        internal IUsuario RetornarUsuarioAvalista(Credencial credencialAvalista)
        {
            if (credencialAvalista == null)
            {
                return null;
            }
            return this.AjudanteSessaoUsuario.RetornarUsuario(credencialAvalista);
        }

        internal IUsuario UsuarioAnonimo
        {
            get
            {
                return this.AjudanteSessaoUsuario.UsuarioAnonimo;
            }
        }

        private void InicializarInterno()
        {
            ValidacaoUtil.ValidarReferenciaNula(this.Credencial, nameof(this.Credencial));

            this.Usuario = this.AjudanteSessaoUsuario.RetornarUsuario(this.Credencial);
            if (this.Usuario == null)
            {
                throw new ErroSessaoUsuarioInvalida($" O usuário em cache não foi encontrado: {this.Credencial.IdentificadorUsuario} -- {this.Credencial.Senha}. {this.AjudanteSessaoUsuario.Contexto.IdentificadorProprietario} {this.AjudanteSessaoUsuario.Contexto.Conexao.ConnectionString}");
                //this.Usuario = this.AjudanteSessaoUsuario.RetornarUsuario(CredencialAnonimo.Anonimo);
                //throw new Erro($"Não foi possível retornar o usuário para a credencial  {this.Credencial.IdentificadorUsuario}");
            }
            this.SessaoUsuario = this.AjudanteSessaoUsuario.RetornarSessaoUsuario(this.Usuario, this.IdentificadorSessaoUsuario, this.InformacaoSessaoUsuario);

            this.NotificarSessaoUsuarioAtivaInterno();
            if (this.TimerAtualizarStatus == null)
            {
                this.TimerAtualizarStatus = new Timer(CacheSessaoUsuario.TIMEOUT_ATUALIZAR_STATUS_SESSAO);
            }

            this.DataHoraUltimoAcesso = DateTime.Now;

            var timer = this.TimerAtualizarStatus;
            timer.AutoReset = true;
            timer.Elapsed -= this.TimerAtualizarStatus_Elapsed;
            timer.Elapsed += this.TimerAtualizarStatus_Elapsed;
            timer.Reiniciar();
        }

        private void TimerAtualizarStatus_Elapsed(object sender, ElapsedEventArgs e)
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
                this.NotificarSessaoUsuarioAtivaInterno( );
            }
        }
        private void NotificarSessaoUsuarioAtivaInterno( )
        {
            var statusSessaoUsuario = this.RetornarStatusSessaoUsuario();
            if (statusSessaoUsuario == EnumStatusSessaoUsuario.Ativo ||
                statusSessaoUsuario == EnumStatusSessaoUsuario.Inativo ||
                statusSessaoUsuario == EnumStatusSessaoUsuario.Nova)
            {
                this.AjudanteSessaoUsuario.NotificarSessaoUsuarioAtiva(this.Usuario, this.SessaoUsuario);
                statusSessaoUsuario = EnumStatusSessaoUsuario.Ativo;
            }
            this.StatusSessaoUsuario = statusSessaoUsuario;
            this.TimerAtualizarStatus?.Reiniciar();
        }

        private EnumStatusSessaoUsuario RetornarStatusSessaoUsuario()
        {
            if (this.IsValidarCredencialSessaoUsuario())
            {
                return this.AjudanteSessaoUsuario.RetornarStatusSessaoUsuario(this.IdentificadorSessaoUsuario);
            }
            return EnumStatusSessaoUsuario.SenhaAlterada;
        }

        private bool IsValidarCredencialSessaoUsuario()
        {
            return this.AjudanteSessaoUsuario.IsValidarCredencialSessaoUsuario(this.SessaoUsuario, this.Credencial);
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
                    this.TimerAtualizarStatus = null;
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
}