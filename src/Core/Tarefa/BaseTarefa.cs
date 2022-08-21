using Snebur.Utilidade;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;

namespace Snebur.Tarefa
{
    public abstract class BaseTarefa : ITarefa, INotifyPropertyChanged
    {
        /// <summary>
        /// Tempo maixmo ocioso
        /// </summary>
        private static TimeSpan TIMEOUT_PADRAO = TimeSpan.FromSeconds(45);

        #region Propriedades

        private double _progresso;
        private EnumEstadoTarefa _estadoTarefa { get; set; }

        public double Progresso
        {
            get
            {
                return this._progresso;
            }
            set
            {
                this._progresso = value;
                this.NotificarPropriedadeAlterada(nameof(this.Progresso));
            }
        }

        public EnumEstadoTarefa Estado
        {
            get
            {
                return this._estadoTarefa;
            }
            set
            {
                this._estadoTarefa = value;
                this.NotificarPropriedadeAlterada(nameof(this.Estado));
                this.NotificarEstadoTarefaAlterado();
            }
        }

        public Guid Identificador { get; }

        public TimeSpan Timeout { get; set; }
        public bool AtivarProgresso { get; set; }
        public DateTime DataHoraUltimaAtividade { get; set; }
        public Exception Erro { get; set; }

        protected Action<ResultadoTarefaFinalizadaEventArgs> CallbackTarefaConcluida;

        #endregion

        #region Eventos

        public event EventHandler<ProgressoAlteradoEventArgs> ProgressoAlterado;
        public event EventHandler<EstadoTarefaAlteradoEventArgs> EstadoTarefaAlterado;
        //public event EventHandler<Exception> EventoErro;

        #endregion

        public static readonly object _bloqueio = new object();

        private Timer TimerAnalizarTimeout { get; set; }

        private TimeSpan IntervalorAnalisarTimeout { get; set; } = BaseTarefa.TIMEOUT_PADRAO;

        #region Construtor

        public BaseTarefa()
        {
            this.Identificador = Guid.NewGuid();
            this._estadoTarefa = EnumEstadoTarefa.Aguardando;
            this.Timeout = TIMEOUT_PADRAO;
            this.DataHoraUltimaAtividade = DateTime.Now;
        }
        #endregion

        #region Métodos publicos

        public virtual void IniciarAsync(Action<ResultadoTarefaFinalizadaEventArgs> callback)
        {
            this.CallbackTarefaConcluida = callback;
            ThreadUtil.ExecutarAsync(this.Iniciar, true);
        }

        private void Iniciar()
        {
            this.TimerAnalizarTimeout = new Timer((int)this.IntervalorAnalisarTimeout.TotalMilliseconds);
            this.TimerAnalizarTimeout.Start();

            this.Estado = EnumEstadoTarefa.Executando;
            this.DataHoraUltimaAtividade = DateTime.Now;
            this.ExecutarInterno();
        }

        public void FinalizarTarefa(Exception erro)
        {
            lock (_bloqueio)
            {
                this.TimerAnalizarTimeout?.Stop();
                this.Estado = (erro != null) ? EnumEstadoTarefa.Erro : EnumEstadoTarefa.Concluida;
                if (this.CallbackTarefaConcluida != null)
                {
                    var callback = this.CallbackTarefaConcluida;
                    this.CallbackTarefaConcluida = null;

                    var resultado = new ResultadoTarefaFinalizadaEventArgs(this, erro);
                    callback(resultado);
                }
            }
        }

        public void NotificarProgresso(double progresso)
        {
            this.DataHoraUltimaAtividade = DateTime.Now;
            this.Progresso = progresso;
            if (progresso < 0) progresso = 0;
            if (progresso > 100) progresso = 100;

            if (this.ProgressoAlterado != null)
            {
                var progressoEventArgs = new ProgressoAlteradoEventArgs(this, progresso);
                Task.Factory.StartNew((Action)(() =>
                {
                    this.ProgressoAlterado(this, progressoEventArgs);
                }));
            }
        }

        private void NotificarEstadoTarefaAlterado()
        {
            this.DataHoraUltimaAtividade = DateTime.Now;
            if (this.EstadoTarefaAlterado != null)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Métodos privados

        #endregion

        #region Métodos notificar eventos

        protected void NotificarProgressoAlterado(double progresso)
        {
            var args = new ProgressoAlteradoEventArgs(this, progresso);
            this.NotificarProgressoAlterado(progresso);
        }

        protected void NotificarProgressoAlterado(ProgressoAlteradoEventArgs progressoEventArgs)
        {
            this.Progresso = progressoEventArgs.Progresso;

            if (this.ProgressoAlterado != null)
            {
                Task.Factory.StartNew(() =>
                {
                    try { this.ProgressoAlterado(this, progressoEventArgs); } catch { };
                });
            }
        }
        #endregion

        #region Métodos abstratodos

        protected void IniciarPausamento()
        {
            this.Estado = EnumEstadoTarefa.Pausando;
            this.DataHoraUltimaAtividade = DateTime.Now;
            this.TimerAnalizarTimeout.Stop();
        }

        protected void IniciarCancelamento()
        {
            this.Estado = EnumEstadoTarefa.Cancelando;
            this.DataHoraUltimaAtividade = DateTime.Now;
            this.TimerAnalizarTimeout.Stop();
        }

        protected void IniciarContinuar()
        {
            this.Estado = EnumEstadoTarefa.Executando;
            this.DataHoraUltimaAtividade = DateTime.Now;
            this.TimerAnalizarTimeout.Start();
        }

        internal abstract void ExecutarInterno();

        public void PausarAsync(Action callbackConcluido)
        {
            this.IniciarPausamento();
            Task.Factory.StartNew(() =>
            {
                this.Pausar(() =>
                {
                    this.Estado = EnumEstadoTarefa.Pausada;
                    callbackConcluido?.Invoke();
                });
            });
        }
        public void CancelarAsync(Action callbackConcluido)
        {
            this.IniciarCancelamento();
            Task.Factory.StartNew(() =>
            {
                this.Cancelar(() =>
                {
                    this.Estado = EnumEstadoTarefa.Cancelada;
                    callbackConcluido?.Invoke();
                });
            });
        }

        public void ContinuarAsync()
        {
            this.IniciarContinuar();
            Task.Factory.StartNew(this.Continuar);
        }

        protected abstract void Pausar(Action callbackConcluido);

        protected abstract void Cancelar(Action callbackConcluido);

        protected abstract void Continuar();

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotificarPropriedadeAlterada([CallerMemberName] string nomePropriedade = "")
        {
            if (String.IsNullOrEmpty(nomePropriedade))
            {
                throw new ErroNaoDefinido(String.Format("O {0} não foi definida", nameof(nomePropriedade)));
            }
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nomePropriedade));
        }
        #endregion
    }
}