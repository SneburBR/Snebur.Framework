//using System;
//using System.Diagnostics;
//using System.Linq;
//using System.Reflection;
//using Snebur.Utilidade;

//namespace Snebur.Dominio.Atributos
//{
//    [AttributeUsage(AttributeTargets.Property)]
//    public class ValidacaoRequeridoCondicionalAttribute : BaseAtributoDominio, IAtributoValidacao
//    {
//        //[MensagemValidacao]
//        //public static string MensagemValidacao { get; set; } = "O campo {0} deve ser preenchido.";

//        public string CaminhoManipuladorValidacao { get; }

//        [IgnorarPropriedadeTS]
//        public Type TipoManipuadorValidacao { get; }

//        public IValidacaoCondicional Manipulador { get; }

//        [IgnorarConstrutorTS()]
//        public ValidacaoRequeridoCondicionalAttribute(Type tipoValidador)
//        {
//            this.TipoManipuadorValidacao = tipoValidador;
//            this.CaminhoManipuladorValidacao = $"{tipoValidador.Namespace}.{tipoValidador.Name}";

//            if (!ReflexaoUtil.TipoImplementaInterface(this.TipoManipuadorValidacao, typeof(IValidacaoCondicional)))
//            {
//                var mensagem = $" o tipo {this.TipoManipuadorValidacao.Name} não implementa a interface {nameof(IValidacaoCondicional)} ";
//                Trace.Fail(mensagem);
//                throw new Exception(mensagem);
//            }

//            if (!this.TipoManipuadorValidacao.GetConstructors().Any(x => x.GetParameters().Count() == 0))
//            {
//                var mensagem = $" o tipo {this.TipoManipuadorValidacao.Name}  não possui um construtor vazio ";
//                Trace.Fail(mensagem);
//                throw new Exception(mensagem);
//            }
//            this.Manipulador = Activator.CreateInstance(this.TipoManipuadorValidacao) as IValidacaoCondicional;
//        }

//        public ValidacaoRequeridoCondicionalAttribute(string caminhoManipuladorValidacao)
//        {
//            var mensagem = "Esse constrtuor é excluiso do cliente Typescript, utilizar o construtor do tipo";
//            Trace.Fail(mensagem);
//            throw new Erro(mensagem);
//        }
//        public bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
//        {
//            return this.Manipulador.IsValido(propriedade, paiPropriedade, valorPropriedade);
//        }

//        public string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
//        {
//            return this.Manipulador.RetornarMensagemValidacao(propriedade, paiPropriedade, valorPropriedade);
//        }
//    }
//}