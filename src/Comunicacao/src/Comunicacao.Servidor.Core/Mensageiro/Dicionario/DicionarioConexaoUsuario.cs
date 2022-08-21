using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;

namespace Snebur.Comunicacao.Mensageiro
{
    public class DicionarioConexaoUsuario<TConexao> : BaseDicionarioConexao<DicionarioConexaoSessaoUsuario<TConexao>, TConexao>,
                                                      IIdentificador
                                                      where TConexao : ConexaoMensageiro
    {

        public Dictionary<Guid, DicionarioConexaoSessaoUsuario<TConexao>> Sessoes => this.Dicionario;

        public override TConexao[] TodasConexoes
        {
            get
            {
                lock (this.BloqueioDicionario)
                {
                    return this.Sessoes.SelectMany(x => x.Value.Conexoes.Values).ToArray();
                }
            }
        }

        public DicionarioConexaoUsuario(Guid identificador) : base(identificador)
        {

        }


    }
}
