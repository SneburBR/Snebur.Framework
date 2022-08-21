//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Runtime.Serialization;
//using Snebur.Servicos;
//using Snebur.Utilidade;

//namespace Snebur.AcessoDados
//{
//    [Serializable]
//    public class ErroSeguranca : ErroAcessoDados
//    {

//        public ErroSeguranca(string mensagem,
//                             EnumTipoLogSeguranca tipoErroSeguranca,
//                             Exception erroInterno = null,
//                             [CallerMemberName] string nomeMetodo = "",
//                             [CallerFilePath] string caminhoArquivo = "",
//                             [CallerLineNumber] int linhaDoErro = 0) :
//                             base(mensagem,

//                                 erroInterno,
//                                 nomeMetodo,
//                                 caminhoArquivo,
//                                 linhaDoErro)
//        {
//            if (!System.Diagnostics.Debugger.IsAttached)
//            {
//                LogUtil.SegurancaAsync(mensagem, tipoErroSeguranca);
//            }
//        }

//        public static string RetornarMensagemErro(List<Snebur.Dominio.ErroValidacao> erros)
//        {
//            return String.Join("\n", erros.Select(x => x.Mensagem));
//        }
//        #region Serializacao 

//        public ErroSeguranca()
//        {
//        }

//        protected ErroSeguranca(SerializationInfo info, StreamingContext context) : base(info, context)
//        {
//        }
//        #endregion
//    }
//}