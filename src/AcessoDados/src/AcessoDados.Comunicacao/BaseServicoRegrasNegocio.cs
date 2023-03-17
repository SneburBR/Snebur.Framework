using Snebur.Comunicacao;
using Snebur.Servicos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Snebur.AcessoDados.Comunicacao
{
    public abstract class BaseServicoRegrasNegocio<TContextoDados> : BaseServicoComunicacaoDados<TContextoDados>, IServicoRegrasNegocio where TContextoDados : BaseContextoDados
    {

        public object ChamarRegra(ChamadaRegraNegocio chamadaRegraNegocio, object[] parametros)
        {
            var tupleTipoMetodo = this.RetornarTipoMetodoDaRegraNegocio(chamadaRegraNegocio);
            var metodo = tupleTipoMetodo.Item2;
            if(metodo.Name == "AtualizarProjeto")
            {
                var tipo = tupleTipoMetodo.Item1;
            }
            var instancia = this.RetornarInstancia(metodo);
            return metodo.Invoke(instancia, parametros);
        }

        private object RetornarInstancia(MethodInfo metodo)
        {
            if (metodo.IsStatic)
            {
                return null;
            }
            var tipo = metodo.DeclaringType;
            if (tipo.GetConstructors().Any(x => x.GetParameters().Length == 0))
            {
                return Activator.CreateInstance(tipo);
            }

            if (tipo.GetConstructors().Any(x => x.GetParameters().Length == 1))
            {
                var construtor = tipo.GetConstructors().Single();
                var parametro = construtor.GetParameters().Single();
                if (parametro.ParameterType.IsSubclassOf(typeof(__BaseContextoDados)))
                {
                    return Activator.CreateInstance(tipo, (object)this.ContextoDados);
                }
            }
            throw new Erro($"Não foi possível instanciar class {tipo.Name}, pois nenhum construtor é suportado");
        }

        protected override object[] RetornarParametrosMetodoOperacao(MethodInfo operacao,
                                                                     Dictionary<string, object> parametros)
        {
            if (parametros.Count == 0)
            {
                throw new ErroSeguranca("Os parâmetros para chamada regra de negocio não pode ser vazio", EnumTipoLogSeguranca.ParametrosComunicacaoInvalidos);
            }

            var primeiraChave = parametros.ElementAt(0).Key;
            var primeiroParametro = parametros[primeiraChave];

            if (primeiroParametro is ChamadaRegraNegocio chamageRegraNegocio)
            {
                var metodo = this.RetornarTipoMetodoDaRegraNegocio(chamageRegraNegocio).Item2;
                parametros.Remove(primeiraChave);

                var parametroContexo = metodo.GetParameters().Where(x => x.ParameterType.IsSubclassOf(typeof(BaseContextoDados))).SingleOrDefault();
                if (parametroContexo != null && !parametros.ContainsKey(parametroContexo.Name))
                {
                    parametros.Add(parametroContexo.Name, this.ContextoDados);
                }

                var parametrosMetodoDaRegra = base.RetornarParametrosMetodoOperacao(metodo, parametros);
                var reotrno = new object[] { chamageRegraNegocio, parametrosMetodoDaRegra };
                return reotrno;
            }

            throw new ErroSeguranca("Os parametros para chamada regra de negocio não são suportado", EnumTipoLogSeguranca.ParametrosComunicacaoInvalidos);
        }

        private Tuple<Type, MethodInfo> RetornarTipoMetodoDaRegraNegocio(ChamadaRegraNegocio chamadaRegraNegocio)
        {
            var tipo = Type.GetType(chamadaRegraNegocio.AssemblyQualifiedName);
            if (tipo == null)
            {
                throw new ErroSeguranca($"O tipo '{chamadaRegraNegocio.AssemblyQualifiedName}' não foi encontrado", EnumTipoLogSeguranca.ParametrosComunicacaoInvalidos);
            }
            var metodo = tipo.GetMethod(chamadaRegraNegocio.NomeMetodo);
            if (metodo == null)
            {
                throw new ErroSeguranca($"O metodo da regra negorcio  '{chamadaRegraNegocio.NomeMetodo}' não foi encontrado em {tipo.Name}", EnumTipoLogSeguranca.ParametrosComunicacaoInvalidos);
            }
            return new Tuple<Type, MethodInfo>(tipo, metodo);
        }

        //protected override object[] NormalizarParametros(object[] parametros)
        //{
        //    var chamageRegraNegocio = (ChamadaRegraNegocio)parametros[0];
        //    var paratrosDaRegra = parametros.Skip(1).ToArray();
        //    var retorno = new object[] { chamageRegraNegocio, paratrosDaRegra };
        //    return retorno;
        //}

    }
}
