using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Snebur.Dominio;

namespace Snebur.Comunicacao.Mensageiro
{
    public abstract class BaseDicionarioConexao<TDicionarioConexao, TConexao> where TDicionarioConexao : IIdentificador where TConexao: ConexaoMensageiro
    {
        protected Dictionary<Guid, TDicionarioConexao> Dicionario { get; } = new Dictionary<Guid, TDicionarioConexao>();

        public Guid Identificador { get; }

        public abstract TConexao[] TodasConexoes { get; }

        public object BloqueioDicionario => ((ICollection)this.Dicionario).SyncRoot;

        public TDicionarioConexao RetornarDicionarioConexoes(Guid identificador)
        {
            if (this.Dicionario.TryGetValue(identificador, out TDicionarioConexao conexao))
            {
                return conexao;
            }

            lock (this.BloqueioDicionario)
            {
                if (!this.Dicionario.ContainsKey(identificador))
                {
                    var instancia = Activator.CreateInstance<TDicionarioConexao>();
                    this.Dicionario.Add(identificador, instancia);
                }
            }
            return this.RetornarDicionarioConexoes(identificador);


        }

        public BaseDicionarioConexao(Guid identificador)
        {
            this.Identificador = identificador;
        }
    }
}