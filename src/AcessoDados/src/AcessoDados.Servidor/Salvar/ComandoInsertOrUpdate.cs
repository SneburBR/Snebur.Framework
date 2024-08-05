using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio;
using Snebur.Linq;
using Snebur.Servicos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class ComandoInsertOrUpdate : Comando, IComandoUpdate
    {
        internal override bool IsAdiconarParametrosChavePrimaria => false;
        public Dictionary<string, PropriedadeAlterada> PropriedadesAlterada { get; private set; }

        internal ComandoInsertOrUpdate(EntidadeAlterada entidadeAlterada,
                                       EstruturaEntidade estruturaEntidade ) : base(entidadeAlterada, estruturaEntidade)
        {
            this.PropriedadesAlterada = entidadeAlterada.RetornarPropriedadesAlteradas();

            this.EstruturasCampoParametro.Clear();

            this.EstruturasCampoParametro.Add(this.EstruturaEntidade.EstruturaCampoChavePrimaria);
            this.EstruturasCampoParametro.AddRange(this.EstruturaEntidade.EstruturasCampos.Values);

            this.SqlCommando = this.RetornarSqlCommando();
        }

        private string RetornarSqlCommando()
        {
            if (this.EstruturaEntidade.IsChavePrimariaAutoIncrimento)
            {
                throw new Exception("Não é permitido inserir ou atualizar uma entidade com chave primária auto incremento");
            }

            var estrutraChavePrimaria = this.EstruturaEntidade.EstruturaCampoChavePrimaria;
            var estruturasCamposAlterados = this.RetornarEstruturasCamposAlterados();
            var camposUpdate = estruturasCamposAlterados.Select(x => $" {x.NomeCampoSensivel} = {x.NomeParametroOuValorFuncaoServidor} ").ToList();
            var camposInsert = this.EstruturasCampoParametro.Select(x => x.NomeCampoSensivel).ToList();
            var parametrosInsert = this.EstruturasCampoParametro.Select(x => x.NomeParametroOuValorFuncaoServidor).ToList();

            var sb = new StringBuilderSql();

            if(estruturasCamposAlterados.Count> 0)
            {
                sb.AppendLine($" UPDATE [{this.EstruturaEntidade.Schema}].[{this.EstruturaEntidade.NomeTabela}]  ");
                sb.AppendLine($"    SET");
                sb.AppendLine($"\t\t\t{String.Join(",\r\n\t\t\t", camposUpdate)}  ");
                sb.AppendLine($"    WHERE {estrutraChavePrimaria.NomeCampoSensivel} = {estrutraChavePrimaria.NomeParametro}");
                sb.AppendLine(" IF @@ROWCOUNT = 0 ");
            }
            else
            {
                sb.AppendLine($" IF NOT EXISTS (SELECT 1 FROM  [{this.EstruturaEntidade.Schema}].[{this.EstruturaEntidade.NomeTabela}] WHERE {estrutraChavePrimaria.NomeCampoSensivel} = {estrutraChavePrimaria.NomeParametro} ) ");
            }
            
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
        private List<EstruturaCampo> RetornarEstruturasCamposAlterados()
        {
             var propriedadesAlterada = this.EntidadeAlterada.RetornarPropriedadesAlteradas();
            var estruturasCamposAlterados = this.EstruturaEntidade.EstruturasCampos.
                                                            Where(x => propriedadesAlterada.Keys.Contains(x.Key)).
                                                            Select(x => x.Value).ToList();



            var estruturasCamposSomenteLeitura = estruturasCamposAlterados.Where(x => x.OpcoesSomenteLeitura.IsSomenteLeitura).ToList();

            if (estruturasCamposSomenteLeitura.Count > 0)
            {
                foreach (var estruturaCampoSomenteLeitura in estruturasCamposSomenteLeitura)
                {
                    if (!this.EntidadeAlterada.Contexto.IsPodeSobreEscrever(estruturaCampoSomenteLeitura))
                    {
                        estruturasCamposAlterados.Remove(estruturaCampoSomenteLeitura);
                        var mensagem = $"Não é autorizado alterar valores das propriedades somente leitura" +
                                        $" '{estruturaCampoSomenteLeitura.Propriedade.Name}' na entidade '{estruturaCampoSomenteLeitura.EstruturaEntidade.TipoEntidade.Name}'";

                        if (DebugUtil.IsAttached)
                        {
                            Trace.TraceWarning(mensagem);
                        }

                        if (estruturaCampoSomenteLeitura.OpcoesSomenteLeitura.IsNotificarSeguranca)
                        {
                            LogUtil.ErroAsync(new ErroSeguranca(mensagem, EnumTipoLogSeguranca.AlterarandoPropriedadeSomenteLeitura));
                        }
                    }
                }
            }
            return estruturasCamposAlterados;
        }
    }
}