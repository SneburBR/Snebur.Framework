﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Snebur.Dominio;
using Snebur.Utilidade;

namespace Snebur.AcessoDados.Estrutura
{
    internal partial class EstruturaBancoDados
    {
        internal DicionarioEstrutura<EstruturaEntidade> EstruturasEntidade { get; }

        internal DicionarioEstrutura<Type> TiposEntidade { get; }

        internal DicionarioEstrutura<PropertyInfo> TodasPropriedades { get; }

        //internal List<Assembly> Assemblies { get; set; } = new List<Assembly>();
        internal Assembly AssemblyEntidade { get; private set; }

        internal List<string> Alertas { get; } = new List<string>();

        internal Type TipoEntidadeArquivo { get; }

        internal Type TipoEntidadeImagem { get; }

        internal Type TipoUsuario { get; }

        internal Type TipoSessaoUsuario { get; }

        internal Type TipoIpInformacao { get; }

        internal Type TipoHistoricoManutencao { get;  }

        internal Type TipoEntidadeNotificaoPropriedadeAlteradaGenerica { get; }

        internal TiposSeguranca TiposSeguranca { get; }

        //internal BaseContextoDados Contexto { get; }

        internal Type TipoContexto { get; }

        #region  Construtor 

        internal EstruturaBancoDados(Type tipoContexto, bool isBancoDadosNaoGerenciavel)
        {
            this.TipoContexto = tipoContexto;
            this.EstruturasEntidade = new DicionarioEstrutura<EstruturaEntidade>();
            this.TiposEntidade = new DicionarioEstrutura<Type>();
            this.MontarEstruturaBancoDados(this.TipoContexto, isBancoDadosNaoGerenciavel);

            this.TipoEntidadeArquivo = this.RetornarTipoEntidadeArquivo();
            this.TipoEntidadeImagem = this.RetornarTipoEntidadeImagem();

            if (!isBancoDadosNaoGerenciavel)
            {
                this.TipoUsuario = this.RetornarTipoUsuario();
                this.TipoSessaoUsuario = this.RetornarTipoSessaoUsuario();
                this.TipoIpInformacao = this.RetornarTipoIpInformacao();
                this.TipoHistoricoManutencao = this.RetornarTipoHistoricoManutenacao();
                this.TipoEntidadeNotificaoPropriedadeAlteradaGenerica = this.RetornarTipoEntidadeNotificaoPropriedadeAlteradaGenerica();
                this.TiposSeguranca = new TiposSeguranca(this);
            }
         
        }
        #endregion

        #region Metodos internos

        internal EstruturaEntidade RetornarEstruturaEntidade(string chave)
        {
            if (this.EstruturasEntidade.ContainsKey(chave))
            {
                return this.EstruturasEntidade[chave];
            }
            var estruturaEntidade = this.EstruturasEntidade.Where(x => x.Key.Equals(chave, StringComparison.InvariantCultureIgnoreCase)).Single().Value;
            return estruturaEntidade;
        }
        #endregion

        #region  Metodos privados

        private void MontarEstruturaBancoDados(Type tipoContexto, bool isBancoDadosNaoGerenciavel)
        {
            Debug.WriteLine("Montando estrutura do BancoDados ");

            //var propriedadesConsulta = ReflectionUtils.RetornarPropriedades(contexto.GetType()).
            //                                            Where(x => x.PropertyType.IsSubclassOf(typeof(ConsultaEntidade))).
            //                                            Select(x => x.PropertyType).ToList();

            var tiposPopriedadeConsulta = ContextoDadosUtil.RetornarPropriedadesIConsultaEntidade(tipoContexto).
                                                            Select(x => x.PropertyType).ToList();

            var tiposEntidadeConsulta = tiposPopriedadeConsulta.Select(x => x.GetGenericArguments().Single()).ToList();

            var assemblies = tiposEntidadeConsulta.Select(x => x.Assembly).Distinct().ToList();
            if (assemblies.Count > 1)
            {
                throw new Erro("O numero de assemblies entidades não é suportado");
            }
            this.AssemblyEntidade = assemblies.Single();

            foreach (var tipoEntidade in tiposEntidadeConsulta)
            {
                if (!tipoEntidade.IsSubclassOf(typeof(Entidade)))
                {
                    throw new Erro(String.Format("Tipo {0} genérico da consulta {1} não herda de BaseEntidade ", tipoEntidade.Name, tipoEntidade.Name));
                }
                this.MontarEstruturaEntidades(tipoEntidade, isBancoDadosNaoGerenciavel);
            }
            foreach (var estruturaEntidade in this.EstruturasEntidade.Values)
            {
                //if (this.Contexto.IsValidarNomeTabelaEntidade)
                //{
                //    if (!estruturaEntidade.NomeTabela.Equals(estruturaEntidade.TipoEntidade.Name, StringComparison.CurrentCultureIgnoreCase))
                //    {
                //        this.Alertas.Add($" Nome da tabela '{estruturaEntidade.NomeTabela}' é diferente do nome do tipo '{estruturaEntidade.TipoEntidade.Name}'");
                //    }
                //}
                this.AssociarEstruturasBase(estruturaEntidade);
                this.AssociarEstruturasEspecializadas(estruturaEntidade);
                this.AssociarNiveisEstruturasEspecializadas(estruturaEntidade, estruturaEntidade.TipoEntidade, 0);

                estruturaEntidade.PreencherRelacoes(this.EstruturasEntidade);
                estruturaEntidade.PreencherEstruturasAlteracaoPropriedade();
            }
            foreach (var estruturaEntidade in this.EstruturasEntidade.Values)
            {
                estruturaEntidade.AssociarEstruturaRalacaos();
            }

            foreach (var estruturaEntidade in this.EstruturasEntidade.Values)
            {
                var camposInvalidos = estruturaEntidade.EstruturasCampos.Values.Where(x => !x.IsTipoComplexo && !x.IsRelacaoChaveEstrangeira &&
                !x.Propriedade.Name.StartsWith("__") &&   !x.Propriedade.Name.EndsWith("_Id") &&
                x.Propriedade.Name.Contains("_"));
                foreach (var estrutura in camposInvalidos)
                {
                    this.Alertas.Add($" O nome da propriedade  {estrutura.Propriedade.Name} é invalido '{estruturaEntidade.TipoEntidade.Name}', não utilizar underline '_', somente em chave estrangeira ");
                }
                estruturaEntidade.AssociarEstruturaRalacaos();
            }
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.AnalisarAlertasEstruturaEntidade();
            }
        }

        private void MontarEstruturaEntidades(Type tipoEntidade, bool isBancoDadosNaoGerenciavel)
        {
            if (this.TiposEntidade.ContainsKey(tipoEntidade.Name))
            {
                throw new Erro(String.Format("Já existe o tipo {0} no dicionario, Tipo duplicado", tipoEntidade.Name));
            }
            this.TiposEntidade.Add(tipoEntidade.Name, tipoEntidade);

            EstruturaEntidade estruturaEntidadeBase = null;

            if (!AjudanteEstruturaBancoDados.TipoEntidadeBaseNaoMepeada(tipoEntidade.BaseType))
            {
                estruturaEntidadeBase = this.EstruturasEntidade[tipoEntidade.BaseType.Name];
            }
            this.EstruturasEntidade.Add(tipoEntidade.Name, new EstruturaEntidade(tipoEntidade, estruturaEntidadeBase, isBancoDadosNaoGerenciavel));

            var tiposEspecializado = tipoEntidade.Assembly.GetTypes().Where(x => x.BaseType == tipoEntidade).ToList();

            foreach (var tipoEspecializado in tiposEspecializado)
            {
                this.MontarEstruturaEntidades(tipoEspecializado, isBancoDadosNaoGerenciavel);
            }
        }

        private void AssociarEstruturasBase(EstruturaEntidade estruturaEntidade)
        {
            var tipoEntidadeBase = estruturaEntidade.TipoEntidade.BaseType;
            var estruturasEntidadeBase = new List<EstruturaEntidade>();

            while (!AjudanteEstruturaBancoDados.TipoEntidadeBaseNaoMepeada(tipoEntidadeBase))
            {
                var estruturaEntidadeBase = this.EstruturasEntidade[tipoEntidadeBase.Name];
                estruturasEntidadeBase.Add(estruturaEntidadeBase);
                tipoEntidadeBase = tipoEntidadeBase.BaseType;
            }
            //Deixando as estrutura na ordem da Hirarquia
            estruturasEntidadeBase.Reverse();

            foreach (var estruturaEntidadeBase in estruturasEntidadeBase)
            {
                estruturaEntidade.EstruturasEntidadeBase.Add(estruturaEntidadeBase.TipoEntidade.Name, estruturaEntidadeBase);
            }
        }

        private void AssociarEstruturasEspecializadas(EstruturaEntidade estruturaEntidade)
        {
            //var tipoEntidadeEspecializadas = this.Assemblies.SelectMany(x => x.GetTypes()).
            //                                                 Where(x => x.IsSubclassOf(estruturaEntidade.TipoEntidade)).ToList();

            var tipoEntidadeEspecializadas = this.AssemblyEntidade.GetTypes().
                                                  Where(x => x.IsSubclassOf(estruturaEntidade.TipoEntidade)).ToList();

            foreach (var tipoEntidade in tipoEntidadeEspecializadas)
            {
                var estruturaEntidadeEspecializada = this.EstruturasEntidade[tipoEntidade.Name];
                estruturaEntidade.EstruturasEntidadeEspecializada.Add(tipoEntidade.Name, estruturaEntidadeEspecializada);
            }
        }

        private void AssociarNiveisEstruturasEspecializadas(EstruturaEntidade estruturaEntidade, Type tipoEntidadeBaseNivel, int nivel)
        {
            //var tipoEntidadeEspecializadas = this.Assemblies.SelectMany(x => x.GetTypes()).
            //                                                 Where(x => x.BaseType == tipoEntidadeBaseNivel).ToList();

            var tipoEntidadeEspecializadas = this.AssemblyEntidade.GetTypes().
                                               Where(x => x.BaseType == tipoEntidadeBaseNivel).ToList();

            if (tipoEntidadeEspecializadas.Count > 0)
            {
                //Separando o nivels - agrupando - 
                var estruturasEntidadeEspecializadasDoNivel = new Dictionary<string, EstruturaEntidade>();
                foreach (var tipoEntidadeEspecializada in tipoEntidadeEspecializadas)
                {
                    var estruturaEntidadeEspecializada = this.EstruturasEntidade[tipoEntidadeEspecializada.Name];
                    estruturasEntidadeEspecializadasDoNivel.Add(tipoEntidadeEspecializada.Name, estruturaEntidadeEspecializada);
                }
                var nivelEstruturaEntidadeEspecializada = new NivelEstruturaEntidadeEspecializada(nivel, tipoEntidadeBaseNivel, estruturasEntidadeEspecializadasDoNivel);

                var chave = String.Format("{0}-{1}", nivel.ToString(), tipoEntidadeBaseNivel.Name);

                estruturaEntidade.NiveisEstruturasEntidadesEspecializada.Add(chave, nivelEstruturaEntidadeEspecializada);

                nivel += 1;

                foreach (EstruturaEntidade estruturaEntidadeEspecializada in estruturasEntidadeEspecializadasDoNivel.Select(x => x.Value).ToList())
                {
                    this.AssociarNiveisEstruturasEspecializadas(estruturaEntidade, estruturaEntidadeEspecializada.TipoEntidade, nivel);
                }
            }
        }
        #endregion

        #region Tipos 

        private Type RetornarTipoUsuario()
        {
            return this.RetornarTipoConsultaImplementaInterface<IUsuario>();
        }

        private Type RetornarTipoHistoricoManutenacao()
        {
            return this.RetornarTipoConsultaImplementaInterface<IHistoricoManutencao>(true);
        }

        private Type RetornarTipoSessaoUsuario()
        {
            return this.RetornarTipoConsultaImplementaInterface<ISessaoUsuario>();
        }

        private Type RetornarTipoIpInformacao()
        {
            return this.RetornarTipoConsultaImplementaInterface<IIPInformacaoEntidade>();
        }

        private Type RetornarTipoEntidadeNotificaoPropriedadeAlteradaGenerica()
        {
            return this.RetornarTipoConsultaImplementaInterface<IAlteracaoPropriedadeGenerica>(true);
        }

        private Type RetornarTipoEntidadeArquivo()
        {
            return this.RetornarTipoConsultaImplementaInterface<IArquivo>(true);
        }

        private Type RetornarTipoEntidadeImagem()
        {
            return this.RetornarTipoConsultaImplementaInterface<IImagem>(true);
        }

        internal Type RetornarTipoConsultaImplementaInterface<TIInterface>(bool ignorarNaoEncontrado = false)
        {
            var tipoInterface = typeof(TIInterface);
            var tiposInterface = new List<Type>();
            var tiposEntidades = this.TiposEntidade.Values.ToList();
            var len = tiposEntidades.Count;

            for (var i = 0; i < len; i++)
            {
                var tipoEntidade = tiposEntidades[i];
                if (ReflexaoUtil.TipoImplementaInterface(tipoEntidade, tipoInterface, false))
                {
                    while (ReflexaoUtil.TipoImplementaInterface(tipoEntidade.BaseType, tipoInterface, false))
                    {
                        tipoEntidade = tipoEntidade.BaseType;
                    }
                    tiposInterface.Add(tipoEntidade);
                }
            }
            tiposInterface = tiposInterface.Distinct().ToList();
            if (tiposInterface.Count == 0)
            {
                if (!ignorarNaoEncontrado)
                {
                    throw new ErroOperacaoInvalida(String.Format("Não existe nenhum tipo que implementa a interface {0}", typeof(TIInterface).Name));
                }
                return null;
            }
            if (tiposInterface.Count > 1)
            {
                throw new ErroOperacaoInvalida(String.Format("Existe mais de um tipo e implementa a interface {0}", typeof(IUsuario).Name));
            }
            return tiposInterface.Single();
        }
        #endregion
    }
}