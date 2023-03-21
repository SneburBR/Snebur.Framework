﻿using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Snebur.Dominio
{
    [NaoCriarTabelaEntidade]
    [Plural("Entidades")]
    public abstract class Entidade : BaseDominio, IEntidade, IEntidadeInterna, IEquatable<Entidade>, INotifyPropertyChanged, INomeTipoEntidade, IDataErrorInfo
    {
        [OcultarColuna]
        public abstract long Id { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Indexar]
        [NaoMapearPostgreeSql]
        [IgnorarGlobalizacao]
        [ValidacaoTextoTamanho(255)]
        [OcultarColuna]
        [SomenteLeitura]
        [PropriedadeProtegida]
        public string __NomeTipoEntidade { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [NaoMapear]
        [IgnorarGlobalizacao]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        [PropriedadeProtegida]
        public string __IdentificadorEntidade
        {
            get
            {
                if (this.Id > 0)
                {
                    return $"{this.__NomeTipoEntidade}-{this.Id}";
                }
                else
                {
                    return $"{this.__NomeTipoEntidade}-(0){this.GetHashCode()}";
                    //return this.__IdentificadorUnico.ToString();
                }
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [NaoMapear]
        [IgnorarGlobalizacao]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        [PropriedadeProtegida]
        internal Type __TipoEntidade { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [NaoMapear]
        [IgnorarGlobalizacao]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        [PropriedadeProtegida]
        public override bool __IsExisteAlteracao
        {
            get
            {
                return this.RetornarIsExisteAlteracaoPropriedade();
            }
        }
        #region Construtor

        public Entidade() : base()
        {
            this.__TipoEntidade = this.GetType();
            this.__NomeTipoEntidade = this.__TipoEntidade.Name;

            var propriedadesTipoComplexo = this.__TipoEntidade.GetProperties().
                                                              Where(x => x.PropertyType.IsSubclassOf(typeof(BaseTipoComplexo))).
                                                              ToList();

            foreach (var propriedadeTipoComplexo in propriedadesTipoComplexo)
            {
                var tipoComplexo = this.RetornarValorPropriedadeTipoComplexo(propriedadeTipoComplexo);
                if (tipoComplexo != null)
                {
                    tipoComplexo.__NomePropriedadeEntidade = propriedadeTipoComplexo.Name;
                    tipoComplexo.__Entidade = this;
                }
                else
                {
                    var isValidarTipoComplexo = propriedadeTipoComplexo.GetCustomAttribute<IgnorarValidacaoTipoComplexo>() == null;
                    if (isValidarTipoComplexo)
                    {
                        throw new Exception($"  A propriedade {propriedadeTipoComplexo.Name} do tipo complexo {propriedadeTipoComplexo.PropertyType.Name} " +
                                            $"   não foi definida no campo privado na entidade '{propriedadeTipoComplexo.DeclaringType.Name}'. Atualize os projetos na  extensão Snebur");
                    }
                }
            }
        }

        private BaseTipoComplexo RetornarValorPropriedadeTipoComplexo(PropertyInfo propriedadeTipoComplexo)
        {
            try
            {
                return (BaseTipoComplexo)propriedadeTipoComplexo.GetValue(this);
            }
            catch (Exception ex)
            {
                throw new Exception($"A propriedade {propriedadeTipoComplexo.Name} do tipo complexo {propriedadeTipoComplexo.PropertyType.Name} " +
                                    $" não foi possível retornar o valor, na entidade '{propriedadeTipoComplexo.DeclaringType.Name}'. " +
                                    $"Analise e mensagem de erro interna", ex);
            }
        }
        #endregion

        #region  IEquatable 

        public bool Equals(Entidade entidade)
        {
            if (entidade != null)
            {
                if (Object.ReferenceEquals(entidade.__TipoEntidade, this.__TipoEntidade))
                {
                    return this.Id == entidade.Id;
                }
            }
            return false;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            if (this.Id > 0)
            {
                return String.Format("{0}-{1}", this.GetType().Name, this.Id).GetHashCode();
            }
            return base.GetHashCode();
            //return this.__IdentificadorUnico.GetHashCode();
        }
        #endregion

        #region Atribuir valor na propriedade

        internal protected virtual long RetornarValorPropriedadeChaveEstrangeira(long valor, Entidade relacao, [CallerMemberName] string nomePropriedade = "")
        {
            if (relacao != null)
            {
                return relacao.Id;
            }
            return this.RetornarValorPropriedade(valor, nomePropriedade);
        }

        internal protected virtual long? RetornarValorPropriedadeChaveEstrangeira(long? valor, Entidade relacao, [CallerMemberName] string nomePropriedade = "")
        {
            if (relacao != null)
            {
                return relacao.Id;
            }
            return this.RetornarValorPropriedade(valor, nomePropriedade);
        }

        internal protected override T RetornarValorPropriedade<T>(T valor, [CallerMemberName] string nomePropriedade = "")
        {
            if (this.__IsControladorPropriedadesAlteradaAtivo)
            {
                var entidade = (this as IEntidadeInterna);
                var propriedadesAberta = entidade.__PropriedadesAbertas;
                if (propriedadesAberta != null)
                {
                    if (!propriedadesAberta.Contains(nomePropriedade))
                    {
                        if (this.IsSerializando)
                        {
                            return default;
                        }
                        if (this.__TipoEntidade.GetProperty(nomePropriedade).PropertyType.IsSubclassOf(typeof(Entidade)))
                        {
                            return default;
                        }
                        throw new Exception($"A propriedade {nomePropriedade} não foi aberta na entidade {entidade.GetType().Name}");
                    }
                }
                var propriedadesAutorizadas = entidade.__PropriedadesAutorizadas;
                if (propriedadesAutorizadas != null)
                {
                    if (!propriedadesAutorizadas.Contains(nomePropriedade))
                    {
                        throw new Exception($"Não autorizado, acesso a propriedade {nomePropriedade}");
                    }
                }
            }
            return valor;
        }

        internal protected virtual void Set(object antigoValor, object novoValor, [CallerMemberName] string nomePropriedade = "")
        {
            base.NotificarValorPropriedadeAlterada(antigoValor, novoValor, nomePropriedade);
        }

        internal protected virtual long Get(long valor, Entidade entidade, [CallerMemberName] string nomePropriedade = "")
        {
            return this.RetornarValorPropriedadeChaveEstrangeira(valor, entidade, nomePropriedade);
        }

        internal protected virtual T Get<T>(T valor, [CallerMemberName] string nomePropriedade = "")
        {
            return this.RetornarValorPropriedade<T>(valor, nomePropriedade);
        }

        internal protected override void NotificarValorPropriedadeAlteradaTipoCompleto(BaseTipoComplexo antigoValor,
                                                                                       BaseTipoComplexo novoValor,
                                                                                       [CallerMemberName] string nomePropriedade = "")
        {
            if (antigoValor != null)
            {
                novoValor.__Entidade = null;
                novoValor.__NomePropriedadeEntidade = null;
                //antigoValor.PropertyChanged -= this.TipoComplexo_PropertyChanged;
            }
            novoValor.__Entidade = this;
            novoValor.__NomePropriedadeEntidade = nomePropriedade;
            //novoValor.PropertyChanged += this.TipoComplexo_PropertyChanged;
            //base.NotificarValorPropriedadeAlterada(antigoValor, novoValor, nomePropriedade);

            if (this.__IsControladorPropriedadesAlteradaAtivo)
            {
                novoValor.NotificarTodasPropriedadesAlteradas(antigoValor);
                this.__isExisteAlteracaoTipoCompleto = true;
            }
            this.NotificarPropriedadeAlterada(nomePropriedade);
        }

        internal protected virtual void NotificarValorPropriedadeAlteradaChaveEstrangeiraAlterada<T>(T antivoValor, T novoValor, [CallerMemberName] string nomePropriedade = "") 
        {
            this.NotificarValorPropriedadeAlterada(antivoValor, novoValor, nomePropriedade);

            if (this.__IsControladorPropriedadesAlteradaAtivo)
            {
                if (!Util.SaoIgual(antivoValor, novoValor))
                {
                    var propriedadeRelacao = this.__TipoEntidade.
                        GetProperties().
                        Where(x => x.GetCustomAttribute<ChaveEstrangeiraAttribute>()?.NomePropriedade == nomePropriedade).
                        FirstOrDefault();

                    var entidade = propriedadeRelacao.GetValue(this);
                    if (entidade is Entidade entidadeTipada)
                    {
                        if (!entidadeTipada.Id.Equals(novoValor))
                        {
                            propriedadeRelacao.SetValue(this, null);
                        }
                    }
                }
            }
        }

        //private void TipoComplexo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (this.RastrearPropriedadesAlterada)
        //    {
        //        if (!(e is PropriedadeTipoComplexaoAlteradaEventArgs))
        //        {
        //            throw new InvalidOperationException($"O argumento e não  é do tipo {nameof(PropriedadeTipoComplexaoAlteradaEventArgs)}");
        //        }
        //        var eTipado = (e as PropriedadeTipoComplexaoAlteradaEventArgs);

        //        var tipoComplexo = (sender as BaseTipoComplexo);
        //        var caminhoProprieade = $"{eTipado.NomePropriedadeEntidade}.{e.PropertyName}";

        //        throw new NotImplementedException();
        //    }

        //}

        #endregion

        #region Métodos públicos

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Dictionary<string, PropriedadeAlterada> RetornarPropriedadesAlteradas()
        {
            return this.__PropriedadesAlteradas;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<ErroValidacao> RetornarErrosValidacao(object contextoDados)
        {
            return ValidarEntidades.Validar(contextoDados, this);
        }

        public TEntidade CloneSomenteId<TEntidade>(Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade, IEntidade
        {
            var entidadeClonada = (TEntidade)Activator.CreateInstance(this.__TipoEntidade);
            entidadeClonada.__IsClonado = true;

            if (expressoesPropriedade != null)
            {
                foreach (var expressaPropriedade in expressoesPropriedade)
                {
                    var propriedade = ExpressaoUtil.RetornarPropriedade(expressaPropriedade);
                    if (propriedade.DeclaringType != typeof(Entidade))
                    {
                        propriedade.TrySetValue(entidadeClonada, propriedade.GetValue(this), true);
                    }
                }
            }
            entidadeClonada.Id = this.Id;
            entidadeClonada.AtivarControladorPropriedadeAlterada();

            return entidadeClonada;
        }


        public TEntidade CloneSomenteId<TEntidade>(bool incluirTiposPrimariosETipoCompleto = true) where TEntidade : Entidade, IEntidade
        {
            var entidadeClonada = (TEntidade)Activator.CreateInstance(this.__TipoEntidade);
            entidadeClonada.__IsClonado = true;

            if (incluirTiposPrimariosETipoCompleto)
            {
                var propriedades = this.__TipoEntidade.GetProperties(ReflexaoUtil.BindingFlags).
                                                                Where(x => x.GetGetMethod() != null && x.GetGetMethod().IsPublic &&
                                                                          x.GetSetMethod() != null && x.GetSetMethod().IsPublic &&
                                                                          (ReflexaoUtil.PropriedadeRetornaTipoPrimario(x, true) ||
                                                                           ReflexaoUtil.PropriedadeRetornaTipoComplexo(x, true)));

                foreach (var propriedade in propriedades)
                {
                    if (propriedade.DeclaringType != typeof(Entidade))
                    {
                        propriedade.TrySetValue(entidadeClonada, propriedade.GetValue(this), true);
                    }
                }
            }


            entidadeClonada.Id = this.Id;
            entidadeClonada.__NomeTipoEntidade = this.__NomeTipoEntidade;
            entidadeClonada.AtivarControladorPropriedadeAlterada();

            return entidadeClonada;
        }

        public void LimparRelacoes()
        {
            this.LimparRelacoesInterno(false, true);
        }

        public void LimparColecoes()
        {
            this.LimparRelacoesInterno(true, false);
        }

        public void LimparRelacoesColecoes()
        {
            this.LimparRelacoesInterno(true, true);
        }

        public TEntidade LimparRelacoes<TEntidade>(bool isRelacaoEntidade = true, bool isColecaoEntidade = true) where TEntidade : Entidade, IEntidade
        {
            var entidade = (TEntidade)Activator.CreateInstance(this.__TipoEntidade);
            entidade.Id = this.Id;

            AutoMapearUtil.Mapear(this, entidade, true, ((PropertyInfo PropriedadeOrigem, PropertyInfo PropriedadeDestino) arg) =>
            {
                var propriedadeOrigem = arg.PropriedadeOrigem;
                var propriedadeDestino = arg.PropriedadeDestino;

                if (ReflexaoUtil.IsTipoEntidade(propriedadeDestino.PropertyType))
                {
                    return isRelacaoEntidade;
                }
                if (ReflexaoUtil.TipoRetornaColecaoEntidade(propriedadeDestino.PropertyType))
                {
                    return isColecaoEntidade;
                }
                return false;
            });

            entidade.AtivarControladorPropriedadeAlterada();
            return entidade;
        }

        private void LimparRelacoesInterno(bool isColecao, bool isRelacaoEntidade)
        {
            var proriedades = ReflexaoUtil.RetornarPropriedades(this.__TipoEntidade, false);
            foreach (var propriedade in proriedades)
            {
                if (propriedade.DeclaringType?.IsSubclassOf(typeof(Entidade)) ?? false)
                {
                    if (isRelacaoEntidade && ReflexaoUtil.IsTipoEntidade(propriedade.PropertyType))
                    {
                        propriedade.SetValue(this, null);
                    }
                    if (isColecao && ReflexaoUtil.TipoRetornaColecaoEntidade(propriedade.PropertyType))
                    {
                        var colecao = propriedade.GetValue(this);
                        if (colecao is IList colecaoTipada)
                        {
                            colecaoTipada.Clear();
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            var propriedadesDescricao = EntidadeUtil.RetornarPropridadesDescricao(this.GetType());
            var descricoes = new List<string>();
            if (propriedadesDescricao.Count > 0)
            {
                foreach (var propriedadeDescricao in propriedadesDescricao)
                {
                    var descricao = ReflexaoUtil.RetornarValorPropriedade(this, propriedadeDescricao)?.ToString();

                    if (!String.IsNullOrWhiteSpace(descricao))
                    {
                        descricoes.Add(descricao);
                    }
                }
                var descricaoCompleta = String.Join(", ", descricoes);
                return $"{this.__TipoEntidade.Name} ({this.Id}) {descricaoCompleta}";
            }
            return $"{this.__TipoEntidade.Name} ({this.Id})";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && Object.ReferenceEquals(this.GetType(), obj.GetType()) && this.Id > 0 && this.Id == ((Entidade)obj).Id)
            {
                return true;
            }
            return base.Equals(obj);
        }
        #endregion

        #region Métodos privados 

        private bool __isExisteAlteracaoTipoCompleto;

        private bool RetornarIsExisteAlteracaoPropriedade()
        {
            if (this.Id == 0)
            {
                return true;
            }
            if (this.__isExisteAlteracaoTipoCompleto)
            {
                return true;
            }
            if (!this.__IsControladorPropriedadesAlteradaAtivo)
            {
                return false;
            }
            var existeAlteraPropriedades = base.__IsExisteAlteracao;
            if (existeAlteraPropriedades)
            {
                return true;
            }
            var propriedadesRelacoesNn = this.GetType().GetProperties().Where(x => x.GetCustomAttribute<RelacaoNnAttribute>() != null);
            foreach (var propriedade in propriedadesRelacoesNn)
            {
                var lista = propriedade.GetValue(this) as IListaEntidades;
                if (lista.EntidadesRemovida.Count > 0)
                {
                    return true;
                }
            }
            var propriedadesTipoCompleso = this.GetType().GetProperties().Where(x => x.PropertyType.IsSubclassOf(typeof(BaseTipoComplexo))).ToList();
            foreach (var propriedade in propriedadesTipoCompleso)
            {
                if (this.__propriedadesAbertas == null || this.__propriedadesAbertas.Contains(propriedade.Name))
                {
                    var tipoCompleto = propriedade.GetValue(this) as BaseTipoComplexo;
                    if (tipoCompleto != null)
                    {
                        if (tipoCompleto.__IsExisteAlteracao)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion

        #region IEntidadeInterna 

        private HashSet<string> __propriedadesAbertas;
        private HashSet<string> __propriedadesAutorizadas;

        [PropriedadeProtegida]
        HashSet<string> IEntidadeInterna.__PropriedadesAbertas { get => this.__propriedadesAbertas; }

        [PropriedadeProtegida]
        HashSet<string> IEntidadeInterna.__PropriedadesAutorizadas { get => this.__propriedadesAutorizadas; }

        void IEntidadeInterna.AtribuirPropriedadesAbertas(HashSet<string> propriedadesAberta)
        {
            //if (this.__ControlarPropriedadesAlterada || this.__propriedadesAbertas != null)
            //{
            //    throw new ErroOperacaoInvalida(" o método é de uso interno AtribuirPropriedadesAberta ");
            //}
            if (propriedadesAberta?.Count > 0)
            {
                this.__propriedadesAbertas = propriedadesAberta;
            }
        }
        void IEntidadeInterna.AtribuirPropriedadesAutorizadas(HashSet<string> propriedadesAutorizadas)
        {
            if (this.__IsControladorPropriedadesAlteradaAtivo || this.__propriedadesAutorizadas != null)
            {
                throw new ErroOperacaoInvalida(" o método é de uso interno AtribuirPropriedadesAutorizadas ");
            }
            if (propriedadesAutorizadas?.Count > 0)
            {
                this.__propriedadesAutorizadas = propriedadesAutorizadas;
            }
        }
        #endregion

        #region  Validação - IDataErrorInfo

        public bool IsExistePedencia
        {
            get
            {
                return this.RetornarValidacoes().Count > 0;
            }
        }
        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                var validacoes = this.RetornarValidacoes();
                var pendencias = validacoes.Where(x => x.MemberNames.Contains(columnName)).ToList();
                if (pendencias.Count == 0)
                {
                    return String.Empty;
                }
                else
                {
                    return String.Join(Environment.NewLine, pendencias.Select(x => x.ErrorMessage));
                }
            }
        }
        string IDataErrorInfo.Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public List<ValidationResult> RetornarValidacoes()
        {
            var resultado = new List<ValidationResult>();
            Validator.TryValidateObject(this, new ValidationContext(this, null, null), resultado, true);
            return resultado;
        }
        //public IEntidade CloneSomenteId()
        //{
        //    return CloneSomenteId<IEntidade>();
        //}

        //public TEntidade CloneSomenteId<TEntidade>() where TEntidade : IEntidade
        //{
        //    var clone = (TEntidade)Activator.CreateInstance(this.__TipoEntidade);
        //    clone.Id = this.Id;
        //    (clone as Entidade).AtivarControladorPropriedadeAlterada();
        //    return clone;
        //}

        #endregion

        #region Operadores

        public static bool operator ==(Entidade entidade1, Entidade entidade2)
        {
            if (!(entidade1 is null) && !(entidade2 is null))
            {
                return entidade1.Equals(entidade2);
            }
            if (entidade1 is null && entidade2 is null)
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(Entidade entidade1, Entidade entidade2)
        {
            if (!(entidade1 is null) && !(entidade2 is null))
            {
                return !entidade1.Equals(entidade2);
            }
            if (entidade1 is null ^ entidade2 is null)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region IAtivo

        internal protected bool RetornarValorPropriedadeIsAtivo(bool valor, [CallerMemberName] string nomePropriedade = "")
        {
            var isAtivo = this.RetornarValorPropriedade(valor, nomePropriedade);
            if (this is IDeletado endidadeDeletado)
            {
                return isAtivo && !endidadeDeletado.IsDeletado;
            }
            return isAtivo;
        }

        internal protected void NotificarValorPropriedadeAlteradaIsAtivo(bool antigoValor, bool novoValor, [CallerMemberName] string nomePropriedade = "")
        {
            if (novoValor)
            {
                if (this is IDeletado deletado && deletado.IsDeletado)
                {
                    throw new Erro("Um entidade deletada não pode ser ativada");
                }
            }
            base.NotificarValorPropriedadeAlterada(antigoValor, novoValor, nomePropriedade);
        }
        #endregion
    }
}