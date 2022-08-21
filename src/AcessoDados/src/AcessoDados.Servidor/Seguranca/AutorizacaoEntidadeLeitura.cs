using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Seguranca
{
    internal class AutorizacaoEntidadeLeitura : AutorizacaoEntidade
    {
        internal AutorizacaoEntidadeLeitura(string nomeTipoEntidade, EnumOperacao operacao, EstruturaConsulta estruturaConsulta) : 
                                            base(nomeTipoEntidade, operacao)
        {
        }
    }
}