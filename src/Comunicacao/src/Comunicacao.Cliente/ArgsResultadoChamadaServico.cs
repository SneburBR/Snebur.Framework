using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;

namespace Snebur.Comunicacao
{
    public class ArgsResultadoChamadaServico : System.ComponentModel.AsyncCompletedEventArgs
    {

        public object Resultado { get; set; }

        public ArgsResultadoChamadaServico(Exception erro, object resultado, object userState) : base(erro, false, userState)
        {
            this.Resultado = resultado;
        }

    }
}
