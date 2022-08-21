using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using Snebur.Utilidades;
using System.Collections;

namespace Snebur.Json.Servidor
{

    public class ResultadoSalvarConstrutor : IDisposable
    {

        public List<BaseEntidade> Entidades { get; set; }

        public List<EntidadeSalva> EntidadesSalvas { get; set; }

        public Dictionary<Guid, BaseEntidade> EntidadesAnalisada { get; set; }


        public ResultadoSalvarConstrutor(List<BaseEntidade> entidades)
        {
            this.Entidades = entidades;
        }

        public ResultadoSalvar RetoranrResultadoSalvar()
        {

            foreach (var entidade in this.Entidades)
            {
                this.AdicionarEntidade(entidade);
            }


            ResultadoSalvar resultado = new ResultadoSalvar();
            resultado.EntidadesSalvas = this.EntidadesSalvas;
            resultado.Sucesso = true;
            return resultado;

        }

        private void AdicionarEntidade(BaseEntidade entidade)
        {

            if (entidade != null)
            {

                if (!this.EntidadesAnalisada.ContainsKey(entidade.IdentificadorUnicoObjeto))
                {
                    this.EntidadesAnalisada.Add(entidade.IdentificadorUnicoObjeto, entidade);


                    var baseNegocioSalvada = new EntidadeSalva();
                    baseNegocioSalvada.Id = entidade.Id;
                    baseNegocioSalvada.IdentificadorUnicoObjeto = entidade.IdentificadorUnicoObjeto;
                    this.EntidadesSalvas.Add(baseNegocioSalvada);

                    var propriedades = ReflectionUtils.RetornarPropriedades(entidade.GetType());

                    foreach (var propriedade in propriedades)
                    {
                        if (propriedade.CanRead)
                        {

                            if (propriedade.PropertyType.IsSubclassOf(typeof(BaseEntidade)))
                            {
                                BaseEntidade entidadeRelacao = (BaseEntidade)ReflectionUtils.RetornarValorPropriedade(entidade, propriedade);
                                this.AdicionarEntidade(entidadeRelacao);
                            }

                            if (ReflectionUtils.TipoRetornaColecao(propriedade.PropertyType))
                            {
                                var tipoColexao = propriedade.PropertyType.GetGenericArguments().Single();

                                if (object.ReferenceEquals(tipoColexao, typeof(BaseEntidade)) || tipoColexao.IsSubclassOf(typeof(BaseEntidade)))
                                {
                                    var lista = (IList)ReflectionUtils.RetornarValorPropriedade(entidade, propriedade);
                                    foreach (BaseEntidade item in lista)
                                    {
                                        this.AdicionarEntidade(item);
                                    }

                                }
                            }

                        }
                    }

                }


            }

        }

        #region "IDisposable Support"
        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Entidades.Clear();
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
