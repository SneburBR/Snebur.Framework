using Snebur.Utilidade;
using System;
using System.Linq;
using System.Reflection;

namespace Snebur
{
    public abstract partial class AplicacaoSnebur
    {
        private readonly static object _bloqueio = new object();

        internal static AplicacaoSnebur _aplicacao;
        public static AplicacaoSnebur Atual
        {
            get => AplicacaoSnebur._aplicacao;
            //get => LazyUtil.RetornarValorLazyComBloqueio(ref _aplicacao, AplicacaoSnebur.RetornarAplicacaoAtual);
            private set
            {
                lock (_bloqueio)
                {
                    if(_aplicacao != null)
                    {
                        throw new Exception("Já existe uma instancia da aplicação.");
                    }
                    _aplicacao = value;
                }
            }
        }

        public static T AtualTipada<T>() where T : AplicacaoSnebur
        {
            return (T)Atual;
        }

        private static AplicacaoSnebur RetornarAplicacaoAtual()
        {
            //var assemblyEntrada = ReflexaoUtil.AssemblyEntrada;
            //if (assemblyEntrada != null)
            //{
            //    var tipoAplicacao = assemblyEntrada.GetAccessibleTypes().Where(x => x.IsSubclassOf(typeof(AplicacaoSnebur))).SingleOrDefault();
            //    if (tipoAplicacao != null)
            //    {
            //        return (AplicacaoSnebur)Activator.CreateInstance(tipoAplicacao);
            //    }
            //}

            throw new Exception("A aplicação atual não foi inicializada, a deve ser inicializar no StartUp da aplicação");
#if NetCore

#else

#endif
            //return new AplicacaoSneburInterna();
        }


    }


}