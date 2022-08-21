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
//    internal class ComandoUltimoId : Comando
//    {

//        internal ComandoUltimoId(EntidadeAlterada entidadeAlterada, EstruturaEntidade estruturaEntidade) : base(entidadeAlterada, estruturaEntidade)
//        {
//            this.SqlCommando = this.RetornarSqlComando();
//        }

//        private string RetornarSqlComando()
//        {
//            //

//            switch (ConfiguracaoAcessoDados.TipoBancoDadosEnum)
//            {
//                case (EnumTipoBancoDados.SQL_SERVER):

//                    //não usar @@IDENTITY - pode retornar um id de entidade criado por um Trigger

//                    //return " SELECT  SCOPE_IDENTITY();";
//                    var mensagem = " Não usar @@IDENTITY - pode retornar um id de entidade criado por um Trigger \r\n" +
//                                   " colocar o somando junto    SELECT  SCOPE_IDENTITY() com comando insert, " +
//                                   " \r\n pois quando tem parametros no comando  \r\n" +
//                                   " o  SELECT  SCOPE_IDENTITY(); retorna null NULL ";

//                    throw new Exception(mensagem);

//                    return " SELECT @@IDENTITY;";

//                case (EnumTipoBancoDados.PostgreSQL):

//                    //return "SELECT LASTVAL() ": - pode retornar um id de entidade criado por um Trigger
//                    var estruturaEntidadeChavePrimaria = this.RetornarEstruturaEntidadeChavePrimariaAutoIncremento();
//                    return String.Format(" SELECT CURRVAL(pg_get_serial_sequence('\"{0}\"', '{1}') ); ", estruturaEntidadeChavePrimaria.NomeTabela, estruturaEntidadeChavePrimaria.EstruturaCampoChavePrimaria.NomeCampo);

//                case (EnumTipoBancoDados.PostgreSQLImob):

//                    //return String.Format(" SELECT CURRVAL(pg_get_serial_sequence('\"{0}\"', '{1}') ); ", this.EstruturaEntidade.NomeTabela, this.EstruturaEntidade.EstruturaCampoChavePrimaria.NomeCampo);
//                    throw new ErroNaoImplementado();

//                default:

//                    throw new ErroNaoSuportado("O tipo do banco de dados não é suportado");
//            }
//        }

//        private EstruturaEntidade RetornarEstruturaEntidadeChavePrimariaAutoIncremento()
//        {
//            var estruturaEntidadeAtual = this.EstruturaEntidade;
//            while (estruturaEntidadeAtual != null)
//            {
//                if (estruturaEntidadeAtual.IsChavePrimariaAutoIncrimento)
//                {
//                    return estruturaEntidadeAtual;
//                }
//                estruturaEntidadeAtual = estruturaEntidadeAtual.EstruturaEntidadeBase;
//            }
//            throw new Erro("A estrutura entidade chave primeira auto incremento não foi encontrada");
//        }
//    }
//}