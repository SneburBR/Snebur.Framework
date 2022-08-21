using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using System.Collections;
using Snebur.Utilidade;

namespace Snebur.Comunicacao
{

    public class PrepararSerializacao : IDisposable
    {
        public Dictionary<Guid, object> ObjetosAnalisados { get; set; }
        public Dictionary<Guid, object> BasesDominio { get; set; }

        public PrepararSerializacao()
        {
            this.ObjetosAnalisados = new Dictionary<Guid, object>();
            this.BasesDominio = new Dictionary<Guid, object>();
        }

        private BaseDominio RetornarBaseDominioReferencia(BaseDominio entidade)
        {
            var baseDominioReferencia = (BaseDominio)Activator.CreateInstance(entidade.GetType());
            baseDominioReferencia.__BaseDominioReferencia = true;
            baseDominioReferencia.__IdentificadorReferencia = entidade.__IdentificadorUnico;
            return baseDominioReferencia;
        }

        public void Preparar(object objeto)
        {
            this.PrepararObjeto(objeto);
        }

        private void PrepararObjeto(object objeto)
        {
            if (objeto is BaseDominio)
            {
                var baseDominio = (BaseDominio)objeto;

                if (!this.ObjetosAnalisados.ContainsKey(baseDominio.__IdentificadorUnico))
                {

                    if ((baseDominio.__IdentificadorUnico == Guid.Empty))
                    {
                        throw new Exception("O Identificador do Objeto unico está vazio ");

                        //baseDominio.IdentificadorUnicoObjeto = Guid.NewGuid()

                        //If TypeOf baseDominio Is Entidade
                        //   DirectCast(baseDominio, Entidade).IdentificadorReferencia = baseDominio.IdentificadorUnicoObjeto
                        //End If

                    }

                    this.ObjetosAnalisados.Add(baseDominio.__IdentificadorUnico, objeto);


                    if (baseDominio.__BaseDominioReferencia)
                    {
                        throw new Exception("Entidade referencia, não suportado para serializacao. ");
                    }

                    if (!this.BasesDominio.ContainsKey(baseDominio.__IdentificadorUnico))
                    {
                        this.BasesDominio.Add(baseDominio.__IdentificadorUnico, objeto);
                    }

                    if (objeto.GetType().IsSubclassOf(typeof(BaseDominio)))
                    {
                        var propriedades = SerializacaoUtil.RetornarPropriedades(objeto.GetType());
                        foreach (var propriedade in propriedades)
                        {
                            var valorPropriedade = ReflexaoUtil.RetornarValorPropriedade(objeto, propriedade);

                            if (valorPropriedade != null)
                            {

                                if (valorPropriedade is BaseDominio && this.BasesDominio.ContainsKey(((BaseDominio)valorPropriedade).__IdentificadorReferencia.GetValueOrDefault()))
                                {
                                    var baseDominioReferencia = this.RetornarBaseDominioReferencia((BaseDominio)valorPropriedade);
                                    ReflexaoUtil.AtribuirValorPropriedade(objeto, propriedade, baseDominioReferencia);
                                }
                                else
                                {
                                    var tipo = valorPropriedade.GetType();
                                    if (ReflexaoUtil.TipoRetornaColecao(tipo))
                                    {
                                        var tipoItemLista = ReflexaoUtil.RetornarTipoGenericoColecao(tipo);
                                        if (ReflexaoUtil.TipoIgualOuHerda(tipoItemLista, typeof(BaseDominio)) ||
                                            ReflexaoUtil.TipoIgualOuHerda(tipoItemLista, typeof(IEntidade)))
                                        {
                                            var lista = this.RetornarLista((ICollection)valorPropriedade);
                                            for (int i = 0; i <= lista.Count - 1; i++)
                                            {
                                                var item = lista[i];
                                                if (item is BaseDominio _baseDominio && ( (_baseDominio.__BaseDominioReferencia && this.BasesDominio.ContainsKey(_baseDominio.__IdentificadorReferencia.Value)) ||
                                                                                         this.BasesDominio.ContainsKey(_baseDominio.__IdentificadorUnico)))
                                                {
                                                    lista[i] = this.RetornarBaseDominioReferencia((BaseDominio)item);
                                                }
                                                else
                                                {
                                                    this.PrepararObjeto(item);
                                                }
                                            }
                                        }
                                    }
                                    else if (tipo.IsSubclassOf(typeof(BaseDominio)))
                                    {
                                        this.PrepararObjeto(valorPropriedade);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private IList RetornarLista(ICollection colecao)
        {
            var tipoColecao = colecao.GetType();
            if (tipoColecao.IsGenericType)
            {
                if (tipoColecao.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    var dicionario = (IDictionary)colecao;
                    var lista = new List<object>();
                    foreach (var valor in dicionario.Values)
                    {
                        lista.Add(valor);
                    }
                    return lista;
                }
            }
            return (IList)colecao;

        }

        #region IDisposable

        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.ObjetosAnalisados.Clear();
                    this.BasesDominio.Clear();
                }
            }
            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
