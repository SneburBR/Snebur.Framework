//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Snebur;
//using Snebur.Utilidade;
//using Snebur.Dominio;
//using Snebur.AcessoDados.Estrutura;

//namespace Snebur.AcessoDados.Servidor.Salvar
//{
//    internal class DeletarEntidades: IDisposable
//    {
//        internal List<EntidadeAlterada> EntidadesAlteradas { get; set; }

//        internal Queue<EntidadeAlterada> Fila { get; set; }

//        internal ContextoDados Contexto { get; set; }

//        internal BaseConexao Conexao { get; set; }

//        internal DeletarEntidades(ContextoDados contexto, List<Entidade> entidades)
//        {
//            this.Fila = new Queue<EntidadeAlterada>();
//            this.Contexto = contexto;
//            this.Conexao = this.Contexto.Conexao;
//            this.EntidadesAlteradas = this.RetornarEntidadesAlteradas(entidades);
//            this.Fila = FilaEntidadeAlterada.RetornarFila(this.EntidadesAlteradas);
//        }

//        private List<EntidadeAlterada> RetornarEntidadesAlteradas(List<Entidade> entidades)
//        {
//            var entidadesAlteradas = new List<EntidadeAlterada>();
//            foreach(var entidade in entidades)
//            {
//                var estrutura = this.Contexto.EstruturaBancoDados.EstruturasEntidade[entidade.GetType().Name];
//                entidadesAlteradas.Add(new EntidadeAlterada(entidade, estrutura, true));
//            }
//            return entidadesAlteradas;
//        }

//        #region IDisposable

//        public void Dispose()
//        {

//        }

//        #endregion
//    }
//}