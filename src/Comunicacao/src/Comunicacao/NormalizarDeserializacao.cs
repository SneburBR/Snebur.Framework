using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using Snebur.Utilidade;
using System.Collections;

namespace Snebur.Comunicacao
{

    public class NormalizarDeserializacao : IDisposable
    {

        public object Objeto { get; set; }

        public Dictionary<Guid, BaseDominio> ObjetosAnalisados { get; set; }
        public Dictionary<Guid, BaseDominio> BasesDominios { get; set; }

        public NormalizarDeserializacao(object objeto)
        {

            this.Objeto = objeto;

            this.ObjetosAnalisados = new Dictionary<Guid, BaseDominio>();
            this.BasesDominios = new Dictionary<Guid, BaseDominio>();
        }


        public void Normalizar()
        {
            this.AnalisarBaseDominio(this.Objeto);
            this.ObjetosAnalisados.Clear();
            this.SubstituirBasesDominioReferencia(this.Objeto);
            this.ObjetosAnalisados.Clear();
        }


        public void AnalisarBaseDominio(object objeto)
        {

            if (objeto != null)
            {
                if (objeto.GetType().IsSubclassOf(typeof(BaseDominio)))
                {
                    var baseDominio = (BaseDominio)objeto;

                    if (!this.ObjetosAnalisados.ContainsKey(baseDominio.__IdentificadorUnico))
                    {
                        this.ObjetosAnalisados.Add(baseDominio.__IdentificadorUnico, baseDominio);

                        if (!baseDominio.__BaseDominioReferencia)
                        {
                            if (this.BasesDominios.ContainsKey(baseDominio.__IdentificadorUnico ))
                            {
                                throw new Erro(" Possivel erro do algoritmo, a bases negocio soh pode existe um vez, as demais precisão ser referencias ");
                            }
                            this.BasesDominios.Add(baseDominio.__IdentificadorUnico , baseDominio);
                        }


                        var propriedades = SerializacaoUtil.RetornarPropriedades(objeto.GetType()).ToList();

                        foreach (var propriedade in propriedades)
                        {

                            var valorPropriedade = ReflexaoUtil.RetornarValorPropriedade(objeto, propriedade);

                            if (valorPropriedade != null)
                            {

                                if (ReflexaoUtil.TipoRetornaColecao(valorPropriedade.GetType()))
                                {
                                    var colecao = valorPropriedade as IEnumerable;
                                    foreach (var item in colecao)
                                    {
                                        this.AnalisarBaseDominio(item);
                                    }
                                }
                                else
                                {
                                    this.AnalisarBaseDominio(valorPropriedade);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SubstituirBasesDominioReferencia(object objeto)
        {
            if (objeto != null && objeto is BaseDominio)
            {

                var baseDominio = (BaseDominio)objeto;


                if (!this.ObjetosAnalisados.ContainsKey(baseDominio.__IdentificadorUnico))
                {
                    this.ObjetosAnalisados.Add(baseDominio.__IdentificadorUnico, baseDominio);


                    if (objeto.GetType().IsSubclassOf(typeof(BaseDominio)))
                    {
                        var propriedades = SerializacaoUtil.RetornarPropriedades(objeto.GetType());

                        foreach (var propriedade in propriedades)
                        {
                            var valorPropriedade = ReflexaoUtil.RetornarValorPropriedade(objeto, propriedade);

                            if (valorPropriedade != null)
                            {

                                if (valorPropriedade is BaseDominio)
                                {
                                    var bodeDominio = (BaseDominio)valorPropriedade;

                                    if (bodeDominio.__BaseDominioReferencia)
                                    {
                                        if (!this.BasesDominios.ContainsKey(bodeDominio.__IdentificadorReferencia.Value))
                                        {
                                            throw new Erro("Não foi encontrada uma BaseDominio para a Referencia da mesma. Isso se deve uma referencia circular na arvore.");
                                        }

                                        ReflexaoUtil.AtribuirValorPropriedade(objeto, propriedade, this.BasesDominios[bodeDominio.__IdentificadorReferencia.Value]);
                                    }
                                }

                                if (ReflexaoUtil.TipoRetornaColecao(valorPropriedade.GetType()))
                                {
                                    var colecao = (IEnumerable)valorPropriedade;
                                    foreach (var item in colecao)
                                    {
                                        this.SubstituirBasesDominioReferencia(item);
                                    }
                                }
                                else
                                {
                                    this.SubstituirBasesDominioReferencia(valorPropriedade);
                                }
                            }
                        }
                    }
                }
            }
        }

        #region IDisposable

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {

                if (disposing)
                {
                    this.BasesDominios.Clear();
                    this.ObjetosAnalisados.Clear();

                    this.BasesDominios = null;
                    this.ObjetosAnalisados = null;
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
