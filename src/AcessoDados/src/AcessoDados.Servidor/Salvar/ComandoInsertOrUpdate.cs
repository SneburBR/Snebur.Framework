using Snebur.AcessoDados.Estrutura;
using System;
using System.Linq;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class ComandoInsertOrUpdate : Comando
    {
        internal override bool IsAdiconarParametrosChavePrimaria => false;
        internal bool IsRecuperarUltimoId { get; }

        internal ComandoInsertOrUpdate(EntidadeAlterada entidadeAlterada,
                                       EstruturaEntidade estruturaEntidade,
                                       bool isRecuperarUltimoId) : base(entidadeAlterada, estruturaEntidade)
        {
            this.IsRecuperarUltimoId = isRecuperarUltimoId;
            this.SqlCommando = this.RetornarSqlCommando();
        }

        private string RetornarSqlCommando()
        {
            if (this.EstruturaEntidade.IsChavePrimariaAutoIncrimento)
            {
                throw new Exception("Não é permitido inserir ou atualizar uma entidade com chave primária auto incremento");
            }

            this.EstruturasCampoParametro.AddRange(this.EstruturaEntidade.EstruturasCampos.Values);

            var estrutraChavePrimaria = this.EstruturaEntidade.EstruturaCampoChavePrimaria;



            var camposUpdate = this.EstruturasCampoParametro.Select(x => $" {x.NomeCampoSensivel} = {x.NomeParametroOuValorFuncaoServidor} ").ToList();

            this.EstruturasCampoParametro.Insert(0, estrutraChavePrimaria);

            var camposInsert = this.EstruturasCampoParametro.Select(x => x.NomeCampoSensivel).ToList();
            var parametrosInsert = this.EstruturasCampoParametro.Select(x => x.NomeParametroOuValorFuncaoServidor).ToList();

            var sb = new StringBuilderSql();

            sb.AppendLine($" UPDATE [{this.EstruturaEntidade.Schema}].[{this.EstruturaEntidade.NomeTabela}]  ");
            sb.AppendLine($"    SET");
            sb.AppendLine($"\t\t\t{String.Join(",\r\n\t\t\t", camposUpdate)}  ");
            sb.AppendLine($"    WHERE {estrutraChavePrimaria.NomeCampoSensivel} = {estrutraChavePrimaria.NomeParametro}");


            sb.AppendLine(" IF @@ROWCOUNT = 0 ");
            sb.AppendLine(" BEGIN ");

            sb.AppendLine($"     INSERT INTO [{this.EstruturaEntidade.Schema}].[{this.EstruturaEntidade.NomeTabela}] ");
            sb.AppendLine($"\t\t( ");
            sb.AppendLine($"\t\t\t{String.Join(",\r\n\t\t\t", camposInsert)} ");
            sb.AppendLine("\t\t) ");
            sb.AppendLine("     VALUES ");
            sb.AppendLine($"\t\t(");
            sb.AppendLine($"\t\t\t {String.Join(",\r\n\t\t\t", parametrosInsert)} ");
            sb.AppendLine($"\t\t) ; ");

            sb.AppendLine(" END ");

            return sb.ToString();
        }
    }
}