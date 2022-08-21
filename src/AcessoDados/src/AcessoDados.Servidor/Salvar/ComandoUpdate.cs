﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.AcessoDados.Estrutura;
using Snebur.Servicos;
using Snebur.Linq;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class ComandoUpdate : Comando
    {
        public Dictionary<string, PropriedadeAlterada> PropriedadesAlterada { get; private set; }

        internal bool ExisteAtualizacao { get => this.EstruturasCampoParametro.Count > 0; }

        internal ComandoUpdate(EntidadeAlterada entidadeAlterada, EstruturaEntidade estruturaEntidade) : base(entidadeAlterada, estruturaEntidade)
        {

            this.PropriedadesAlterada = this.RetornarPropriedadesAlterada();
              
            this.EstruturasCampoParametro.AddRange(this.RetornarEstrutasCamposAlterados());

            if (this.EstruturaEntidade.IsSomenteLeitura)
            {
                if (!this.EstruturasCampoParametro.All(x => x.IsAutorizarAlteracaoPropriedade))
                {
                    throw new ErroSeguranca("Não é autorizado atualizar uma entidade somente leitura, somente os campos autorizados", Servicos.EnumTipoLogSeguranca.AlterarandoEntidadeSomenteLeitura);
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
                //throw new ArgumentNullException(nameof(this.PropriedadesAlterada));
            }

            var estruturasCamposAlterados = this.EstruturaEntidade.EstruturasCampos.
                                                            Where(x => this.PropriedadesAlterada.Keys.Contains(x.Key)).
                                                            Select(x => x.Value).ToList();

            var estruturasCamposSomenteLeitura = estruturasCamposAlterados.Where(x => x.IsSomenteLeitura).ToList();

            if (estruturasCamposSomenteLeitura.Count > 0)
            {
                var nomesPropriedade = String.Join(",", estruturasCamposSomenteLeitura.Select(x => x.Propriedade.Name));
                var mensagem = $"Não é autorizado alterar valores das propriedades somente leitura '{nomesPropriedade}' na entidade '{this.EstruturaEntidade.TipoEntidade.Name}'";

                estruturasCamposAlterados.RemoveRange(estruturasCamposSomenteLeitura);

                LogUtil.ErroAsync(new ErroSeguranca(mensagem, EnumTipoLogSeguranca.AlterarandoPropriedadeSomenteLeitura));
            }


            //foreach(var estruturaTipoCompleto in this.EstruturaEntidade.EstruturasTipoComplexao.Values)
            //{
            //    estruturasCamposAlterados.AddRange(estruturaTipoCompleto.EstruturasCampo.Values);
            //}
            //

            return estruturasCamposAlterados;
        }

        private string RetornarSqlCommando()
        {
            var camposAtualizar = this.EstruturasCampoParametro.Select(x => String.Format(" {0} = {1} ", x.NomeCampoSensivel, x.NomeParametroOuValorFuncaoServidor)).ToList();

            var sb = new StringBuilderSql();
            sb.AppendFormat(" UpDaTe [{0}].[{1}] SET ", this.EstruturaEntidade.Schema, this.EstruturaEntidade.NomeTabela);
            sb.Append(String.Format(" {0}  ", String.Join(",", camposAtualizar)));
            sb.Append(String.Format(" WHERE {0} = {1} ", this.EstruturaEntidade.EstruturaCampoChavePrimaria.NomeCampoSensivel, this.EntidadeAlterada.Entidade.Id));
            return sb.ToString();
        }
        //this.EstruturasCampoParametro.AddRange(this.EstruturaEntidade.EstruturasCampos.Values);
    }
}