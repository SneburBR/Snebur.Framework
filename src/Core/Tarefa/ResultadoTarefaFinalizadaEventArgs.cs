using System;

namespace Snebur.Tarefa
{
    public class ResultadoTarefaFinalizadaEventArgs : EventArgs
    {
        public ITarefa Tarefa { get; set; }
        public Exception Erro { get; set; }
        public bool Cancelada { get; set; }
        public EnumEstadoTarefa Estado { get; set; }

        public ResultadoTarefaFinalizadaEventArgs(BaseTarefa tarefa, Exception erro)
        {
            this.Tarefa = tarefa;
            this.Erro = erro;
            this.Estado = this.Tarefa.Estado;
        }
    }
}