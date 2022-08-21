using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Seguranca
{
    internal class AutorizacaoEntidadeSalvar : AutorizacaoEntidade
    {
        public List<Entidade> Entidades { get; }

        internal AutorizacaoEntidadeSalvar(string nomeTipoEntidade, EnumOperacao operacao, List<Entidade> entidades) : 
                                          base(nomeTipoEntidade, operacao)
        {
            this.Entidades = entidades;
        }
    }
}