using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;

namespace Snebur.Comunicacao.Mensageiro
{
    public class DicionarioConexaoSessaoUsuario<TConexao> : BaseDicionarioConexao<TConexao, TConexao>, IIdentificador
                                                            where TConexao : ConexaoMensageiro
    {
        public Dictionary<Guid, TConexao> Conexoes => this.Dicionario;

        public override TConexao[] TodasConexoes {

            get
            {
                lock (this.BloqueioDicionario)
                {
                    return this.Conexoes.Values.ToArray();
                }
            }
        }

        public DicionarioConexaoSessaoUsuario(Guid identificador) : base(identificador)
        {

        }

        internal void AdicionaConexao(TConexao conexao)
        {
            if (!this.Conexoes.ContainsKey(conexao.identificadorUnicoConexao))
            {
                lock (this.BloqueioDicionario)
                {
                    if (!this.Conexoes.ContainsKey(conexao.identificadorUnicoConexao))
                    {
                        this.Conexoes.Add(conexao.identificadorUnicoConexao, conexao);
                    }
                }
            }
        }

        internal void RemoverConexao(TConexao conexao)
        {
            if (this.Conexoes.ContainsKey(conexao.identificadorUnicoConexao))
            {
                lock (this.BloqueioDicionario)
                {
                    if (this.Conexoes.ContainsKey(conexao.identificadorUnicoConexao))
                    {
                        this.Conexoes.Remove(conexao.identificadorUnicoConexao);
                    }
                }
            }
        }


    }

}
