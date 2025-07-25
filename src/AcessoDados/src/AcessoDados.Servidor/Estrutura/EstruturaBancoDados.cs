﻿using Snebur.AcessoDados.Mapeamento;
using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Snebur.AcessoDados.Estrutura
{
    internal partial class EstruturaBancoDados
    {
        public object _bloqueio = new object();

        internal bool IsEstruturasEntidadeMontada { get; private set; }
        internal DicionarioEstrutura<EstruturaEntidade> EstruturasEntidade { get; } = new DicionarioEstrutura<EstruturaEntidade>();
        internal DicionarioEstrutura<Type> TiposEntidade { get; } = new DicionarioEstrutura<Type>();
        internal DicionarioEstrutura<PropertyInfo> TodasPropriedades { get; } = new DicionarioEstrutura<PropertyInfo>();
        internal Dictionary<Type, IntercepetadorModel> Interceptadores { get; } = new Dictionary<Type, IntercepetadorModel>();
        internal Assembly AssemblyEntidades { get; private set; }
        internal List<string> Alertas { get; } = new List<string>();
        internal Type TipoEntidadeArquivo { get; }
        internal Type TipoEntidadeImagem { get; }
        internal Type TipoUsuario { get; }
        internal Type TipoSessaoUsuario { get; }
        internal Type TipoIpInformacao { get; }
        internal Type TipoHistoricoManutencao { get; }
        internal Type TipoEntidadeNotificaoPropriedadeAlteradaGenerica { get; }
        internal TiposSeguranca TiposSeguranca { get; }
        internal Type TipoContexto { get; }

        internal DateTimeKind DateTimeKindPadrao { get; }

        internal bool IsDateTimeUtc => this.DateTimeKindPadrao == DateTimeKind.Utc; 

        internal int IdNamespace { get; }

        #region  Construtor 

        internal EstruturaBancoDados(BaseContextoDados contextoDados,
                                     BancoDadosSuporta sqlSuporte)
        {
            this.TipoContexto = contextoDados.GetType();
            this.PopularInterceptadores();
            this.MontarEstruturaBancoDados(this.TipoContexto, sqlSuporte);
            this.TipoEntidadeArquivo = this.RetornarTipoEntidadeArquivo();
            this.TipoEntidadeImagem = this.RetornarTipoEntidadeImagem();
            this.TipoEntidadeNotificaoPropriedadeAlteradaGenerica = this.RetornarTipoEntidadeNotificaoPropriedadeAlteradaGenerica(contextoDados);
            this.TipoHistoricoManutencao = this.RetornarTipoHistoricoManutenacao();
            this.TiposSeguranca = new TiposSeguranca(this);

            if (sqlSuporte.IsSessaoUsuarioContextoAtual)
            {
                this.TipoUsuario = this.RetornarTipoUsuario();
                this.TipoSessaoUsuario = this.RetornarTipoSessaoUsuario();
                this.TipoIpInformacao = this.RetornarTipoIpInformacao();
            }
            this.DateTimeKindPadrao = sqlSuporte.IsDataHoraUtc ? DateTimeKind.Utc : DateTimeKind.Local;
            this.IdNamespace = contextoDados.IdNamespace;

        }
        #endregion

        internal Type RetornarTipoConsultaImplementaInterface<TIInterface>(bool ignorarNaoEncontrado = false)
        {
            var tipoInterface = typeof(TIInterface);
            var tiposInterface = new List<Type>();
            var tiposEntidades = this.TiposEntidade.Values.ToList();
            var len = tiposEntidades.Count;

            for (var i = 0; i < len; i++)
            {
                var tipoEntidade = tiposEntidades[i];
                if (ReflexaoUtil.IsTipoImplementaInterface(tipoEntidade, tipoInterface, false))
                {
                    while (ReflexaoUtil.IsTipoImplementaInterface(tipoEntidade.BaseType, tipoInterface, false))
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

        #region Métodos internos

        internal EstruturaEntidade RetornarEstruturaEntidade(string chave, bool isValidar = true)
        {
            if (this.EstruturasEntidade.ContainsKey(chave))
            {
                return this.EstruturasEntidade[chave];
            }

            var estruturasEntidade = this.EstruturasEntidade.Where(x => x.Key.Equals(chave, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (estruturasEntidade.Count == 1)
            {
                return estruturasEntidade.Single().Value;
            }

            if (isValidar)
            {
                throw new Erro($"Não foi encontrado a estrutura da entidade {chave} no contexto {this.TipoContexto.Name}");
            }
            return null;
        }
        #endregion

        #region  Métodos privados

        private void MontarEstruturaBancoDados(Type tipoContexto,
                                              BancoDadosSuporta sqlSuporte)
        {
            lock (this._bloqueio)
            {
                if (this.IsEstruturasEntidadeMontada)
                {
                    throw new Exception("A estrutura do banco dados já está montada");
                }

                Debug.WriteLine("Montando estrutura do BancoDados ");

                //var propriedadesConsulta = ReflectionUtils.RetornarPropriedades(contexto.GetType()).
                //                                            Where(x => x.PropertyType.IsSubclassOf(typeof(ConsultaEntidade))).
                //                                            Select(x => x.PropertyType).ToList();

                this.MontarEstuturasEntidades(tipoContexto, sqlSuporte);
                this.AssociarEstruturasEntidades();

                if (DebugUtil.IsAttached)
                {
                    this.AnalisarAlertasEstruturaEntidade();
                }

            }
        }

        private void MontarEstuturasEntidades(Type tipoContexto, BancoDadosSuporta sqlSuporte)
        {
            var tiposPopriedadeConsulta = ContextoDadosUtil.RetornarPropriedadesIConsultaEntidade(tipoContexto).
                                                                Select(x => x.PropertyType).ToList();

            var tiposEntidadeConsulta = tiposPopriedadeConsulta.Select(x => x.GetGenericArguments().Single()).ToList();

            var assemblies = tiposEntidadeConsulta.Select(x => x.Assembly).Distinct().ToList();
            if (assemblies.Count > 1)
            {
                throw new Erro("O numero de assemblies entidades não é suportado");
            }
            this.AssemblyEntidades = assemblies.Single();

            foreach (var tipoEntidade in tiposEntidadeConsulta)
            {
                if (!tipoEntidade.IsSubclassOf(typeof(Entidade)))
                {
                    throw new Erro(String.Format("Tipo {0} genérico da consulta {1} não herda de BaseEntidade ", tipoEntidade.Name, tipoEntidade.Name));
                }
                if (tipoEntidade.BaseType.IsSubclassOf(typeof(Entidade)))
                {
                    throw new Erro($"O tipo base {tipoEntidade.BaseType.Name} da entidade {tipoEntidade.Name} não foi mapeado. " +
                                   $"Mapeie sempre o tipo mais abstrato da entidade. Você pode configurar um atalho ");
                }
                this.MontarEstruturaEntidade(tipoEntidade, sqlSuporte);
            }

            this.IsEstruturasEntidadeMontada = true;
        }

        private void MontarEstruturaEntidade(Type tipoEntidade, BancoDadosSuporta sqlSuporte)
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
            this.EstruturasEntidade.Add(tipoEntidade.Name, new EstruturaEntidade(this,
                                                                                 tipoEntidade,
                                                                                 estruturaEntidadeBase,
                                                                                 sqlSuporte));

            var tiposEspecializado = tipoEntidade.Assembly.GetTypes().Where(x => x.BaseType == tipoEntidade).ToList();

            foreach (var tipoEspecializado in tiposEspecializado)
            {
                this.MontarEstruturaEntidade(tipoEspecializado, sqlSuporte);
            }
        }

        private void AssociarEstruturasEntidades()
        {
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
                !x.Propriedade.Name.StartsWith("__") && !x.Propriedade.Name.EndsWith("_Id") &&
                x.Propriedade.Name.Contains("_"));
                foreach (var estrutura in camposInvalidos)
                {
                    this.Alertas.Add($" O nome da propriedade  {estrutura.Propriedade.Name} é invalido '{estruturaEntidade.TipoEntidade.Name}', não utilizar underline '_', somente em chave estrangeira ");
                }
                estruturaEntidade.AssociarEstruturaRalacaos();
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
            //Deixando as estrutura na ordem da Hierarquia
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

            var tipoEntidadeEspecializadas = this.AssemblyEntidades.GetTypes().
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

            var tipoEntidadeEspecializadas = this.AssemblyEntidades.GetTypes().
                                               Where(x => x.BaseType == tipoEntidadeBaseNivel).ToList();

            if (tipoEntidadeEspecializadas.Count > 0)
            {
                //Separando o níveis - agrupando - 
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

        private void PopularInterceptadores()
        {
            var tipos = this.TipoContexto.Assembly.GetLoadableTypes().Where(x => typeof(IInterceptador).IsAssignableFrom(x)).ToList();
            if(tipos.Count> 0)
            {
                var grupos = tipos.GroupBy(x => getTipoEntidade(x));
                foreach (var grupo in grupos)
                {
                    var tipoEntidade = grupo.Key;
                    if (!tipoEntidade.IsSubclassOf(typeof(Entidade)))
                    {
                        throw new Exception($"O tipo {tipoEntidade.Name} não é uma entidade");
                    }
                    var interceptorModel = new IntercepetadorModel(tipoEntidade, grupo.ToArray());
                    this.Interceptadores.Add(tipoEntidade, interceptorModel);
                }

                Type getTipoEntidade(Type type)
                {
                    var tipoInterceptorGenerico = type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IInterceptador<,>)).FirstOrDefault();
                    if (tipoInterceptorGenerico != null)
                    {
                        return tipoInterceptorGenerico.GetGenericArguments().LastOrDefault();
                    }

                    throw new Exception($"O tipo {type.Name} não implementa a interface IInterceptador<,>");
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

        private Type RetornarTipoEntidadeNotificaoPropriedadeAlteradaGenerica(BaseContextoDados contextoDados)
        {
            if(contextoDados.IsContextoSessaoUsuario)
            {
                return this.RetornarTipoConsultaImplementaInterface<IAlteracaoPropriedadeGenerica>(true);
            }
            return contextoDados.ContextoSessaoUsuario.EstruturaBancoDados.TipoEntidadeNotificaoPropriedadeAlteradaGenerica;
        }

        private Type RetornarTipoEntidadeArquivo()
        {
            return this.RetornarTipoConsultaImplementaInterface<IArquivo>(true);
        }

        private Type RetornarTipoEntidadeImagem()
        {
            return this.RetornarTipoConsultaImplementaInterface<IImagem>(true);
        }

        #endregion
    }
}