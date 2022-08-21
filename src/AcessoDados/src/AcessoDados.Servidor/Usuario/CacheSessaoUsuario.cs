using System;
using System.Timers;
using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Utilidade;

namespace Snebur.AcessoDados
{
    public partial class CacheSessaoUsuario : IDisposable
    {
        //implementar classe de Cache para sessaoUsuario para melhorar o desenpenho, solução paliativa, uusuario o contexto pra criar sql e colocar cache
        //private const string PARAMETRO_IDENTIFICADOR_SESSAO_USUARIO = "@IdentificadorSessaoUsuario ";
        //private const string PARAMETRO_IDENTIFICADOR_USUARIO = "@IdentificadorUsuario ";

        //private const string SQL_DATA_HORA_ULTIMA_ATUALIZACAO = "  UPDATE usuario.SessaoUsuario SET DataHoraUltimoAcesso = GETUTCDATE() WHERE IdentificadorSessaoUsuario = " + PARAMETRO_IDENTIFICADOR_SESSAO_USUARIO;
        //private const string SQL_VERFICAR_CREDENCIAL = "  SELECT e2.Senha  from usuario.Identificacao as e1 INNER JOIN usuario.Usuario as e2 on  e1.Id = e2.Id WHERE e1.Identificador = " + PARAMETRO_IDENTIFICADOR_USUARIO;
        //private const string SQL_ESTADO_SESSAO_USUARIO = "  select Estado from usuario.SessaoUsuario WHERE IdentificadorSessaoUsuario = @IdentificadorSessaoUsuario ";

        //private const string SQL_ATUALIZAR_ESTADO_SESSAO_USUARIO = "  UPDATE usuario.SessaoUsuario SET DataHoraUltimoAcesso = GETUTCDATE() WHERE IdentificadorSessaoUsuario = @IdentificadorSessaoUsuario";

        private static readonly double TIMEOUT_REMOVER_CACHE = TimeSpan.FromHours(2).TotalMilliseconds;
        private static readonly double TIMEOUT_ATUALIZAR_ESTADO_SESSAO = TimeSpan.FromMinutes(1).TotalMilliseconds;

        public ISessaoUsuario SessaoUsuario { get; private set; }
        public IUsuario Usuario { get; private set; }
        public EnumEstadoSessaoUsuario EstadoSessaoUsuario { get; private set; }

        private DateTime DataHoraUltimoAcesso { get; set; }
        private Guid IdentificadorSessaoUsuario { get; }
        public BaseContextoDados Contexto { get; }

        private Credencial Credencial { get; }
        private InformacaoSessaoUsuario InformacaoSessaoUsuario { get; }
        private AjudanteSessaoUsuarioInterno AjudanteSessaoUsuario { get; }
        private BaseConexao AjudanteConexao { get; }

        private bool IsNoticacaoEstadoPendente { get; set; }

        private Timer TimerAtualizarEstado = new Timer(CacheSessaoUsuario.TIMEOUT_ATUALIZAR_ESTADO_SESSAO);
        public object BloqueioInicializar = new object();
        public bool IsInicializado = false;

        private CacheSessaoUsuario(BaseContextoDados contexto, 
                                   Credencial credencial, 
                                   Guid identificadorSessaoUsuario, 
                                   InformacaoSessaoUsuario informacaoSessaoUsuario)
        {
            this.IdentificadorSessaoUsuario = identificadorSessaoUsuario;
            this.Contexto = contexto;
            this.Credencial = credencial;
            this.InformacaoSessaoUsuario = informacaoSessaoUsuario;
            this.AjudanteSessaoUsuario = AjudanteSessaoUsuarioInterno.RetornarAjudanteUsuario(contexto);
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
                throw new ErroSessaoUsuarioInvalida("O usuário em cache não foi encontrado");
                //this.Usuario = this.AjudanteSessaoUsuario.RetornarUsuario(CredencialAnonimo.Anonimo);
                //throw new Erro($"Não foi possível retornar o usuário para a credencial  {this.Credencial.IdentificadorUsuario}");
            }
            this.SessaoUsuario = this.AjudanteSessaoUsuario.RetornarSessaoUsuario(this.Usuario, this.IdentificadorSessaoUsuario, this.InformacaoSessaoUsuario);
          
            this.NotificarSessaoUsuarioAtivaInterno();
            this.TimerAtualizarEstado = new Timer(CacheSessaoUsuario.TIMEOUT_ATUALIZAR_ESTADO_SESSAO);
            this.TimerAtualizarEstado.AutoReset = true;
            this.DataHoraUltimoAcesso = DateTime.Now;
            this.TimerAtualizarEstado.Elapsed += this.TimerAtualizarEstado_Elapsed;
            this.TimerAtualizarEstado.Start();
        }

        private void TimerAtualizarEstado_Elapsed(object sender, ElapsedEventArgs e)
        {
            var diferenca = DateTime.Now - this.DataHoraUltimoAcesso;
            if (diferenca.TotalMilliseconds > TIMEOUT_REMOVER_CACHE)
            {
                CacheSessaoUsuario.RemoverCacheSessaoUsuario(this.IdentificadorSessaoUsuario);
                this.Dispose();
            }
            else
            {
                this.IsNoticacaoEstadoPendente = true;
            }
        }
        public void NotificarSessaoAtivaAsync()
        {
            this.DataHoraUltimoAcesso = DateTime.Now;
            if (this.IsNoticacaoEstadoPendente)
            {
                this.NotificarSessaoUsuarioAtivaInterno();
            }
        }
        private void NotificarSessaoUsuarioAtivaInterno()
        {
            var estadoSessaoUsuario = this.RetornarEstadoSessaoUsuario();
            if (estadoSessaoUsuario == EnumEstadoSessaoUsuario.Ativo ||
                estadoSessaoUsuario == EnumEstadoSessaoUsuario.Inativo ||
                estadoSessaoUsuario == EnumEstadoSessaoUsuario.Nova)
            {
                this.AjudanteSessaoUsuario.NotificarSessaoUsuarioAtiva(this.Usuario, this.SessaoUsuario);
                estadoSessaoUsuario = EnumEstadoSessaoUsuario.Ativo;
            }
            this.EstadoSessaoUsuario = estadoSessaoUsuario;
            this.TimerAtualizarEstado.Reiniciar();
        }

        private EnumEstadoSessaoUsuario RetornarEstadoSessaoUsuario()
        {
            if (this.IsValidarCredencialSessaoUsuario())
            {
                return this.AjudanteSessaoUsuario.RetornarEstadoSessaoUsuario(this.IdentificadorSessaoUsuario);
            }
            return EnumEstadoSessaoUsuario.SenhaAlterada;
        }

        private bool IsValidarCredencialSessaoUsuario()
        {
            return this.AjudanteSessaoUsuario.IsValidarCredencialSessaoUsuario(this.SessaoUsuario, this.Credencial);
        }

        public void FinalizarSessaoUsuario()
        {
            this.IsNoticacaoEstadoPendente = true;
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
                    this.TimerAtualizarEstado.Dispose();
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