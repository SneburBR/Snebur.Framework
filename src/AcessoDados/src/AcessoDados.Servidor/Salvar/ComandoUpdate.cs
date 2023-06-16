using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio;
using Snebur.Dominio.Interface;
using Snebur.Linq;
using Snebur.Servicos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class ComandoUpdate : Comando
    {
        internal override bool IsAdiconarParametrosChavePrimaria => true;
        public Dictionary<string, PropriedadeAlterada> PropriedadesAlterada { get; private set; }

        internal bool ExisteAtualizacao { get => this.EstruturasCampoParametro.Count > 0; }

        internal ComandoUpdate(EntidadeAlterada entidadeAlterada,
                               EstruturaEntidade estruturaEntidade ) : base(entidadeAlterada, estruturaEntidade)
        {
            
            this.PropriedadesAlterada = this.RetornarPropriedadesAlterada();

            this.EstruturasCampoParametro.AddRange(this.RetornarEstrutasCamposAlterados());

            if (this.EstruturaEntidade.IsSomenteLeitura)
            {
                if (!this.EstruturasCampoParametro.All(x => x.IsAutorizarAlteracaoPropriedade))
                {
                    throw new ErroSeguranca("Não é autorizado atualizar uma entidade somente leitura, somente os campos autorizados", 
                                            EnumTipoLogSeguranca.AlterarandoEntidadeSomenteLeitura);
                }
            }

            if (this.EstruturasCampoParametro.Count > 0)
            {
                this.SqlCommando = this.RetornarSqlCommando();
            }
        }

        private Dictionary<string, PropriedadeAlterada> RetornarPropriedadesAlterada()
        {
            return this.EntidadeAlterada.Entidade.RetornarPropriedadesAlteradas() ??
                   new Dictionary<string, PropriedadeAlterada>();
        }

        private List<EstruturaCampo> RetornarEstrutasCamposAlterados()
        {
            if (this.Entidade.Id == 0)
            {
                return this.EstruturaEntidade.EstruturasCampos.Values.ToList();
            }

            var estruturasCamposAlterados = this.EstruturaEntidade.EstruturasCampos.
                                                            Where(x => this.PropriedadesAlterada.Keys.Contains(x.Key)).
                                                            Select(x => x.Value).ToList();

            

            var estruturasCamposSomenteLeitura = estruturasCamposAlterados.Where(x => x.OpcoesSomenteLeitura.IsSomenteLeitura).ToList();

            if (estruturasCamposSomenteLeitura.Count > 0)
            {
                foreach(var estruturaCampoSomenteLeitura in estruturasCamposSomenteLeitura)
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

        private string RetornarSqlCommando()
        {
            var camposAtualizar = this.EstruturasCampoParametro.Select(x => $" {x.NomeCampoSensivel} = {x.NomeParametroOuValorFuncaoServidor} ").ToList();
            var estrutraChavePrimaria = this.EstruturaEntidade.EstruturaCampoChavePrimaria;
            var sb = new StringBuilderSql();
            sb.AppendFormat($" UpDaTe [{this.EstruturaEntidade.Schema}].[{this.EstruturaEntidade.NomeTabela}]  " );
            sb.Append($" SET {String.Join(",", camposAtualizar)}  ");
            sb.Append($" WHERE {estrutraChavePrimaria.NomeCampoSensivel} = {estrutraChavePrimaria.NomeParametro}");
            return sb.ToString();
        }

        
        //this.EstruturasCampoParametro.AddRange(this.EstruturaEntidade.EstruturasCampos.Values);
    }
}