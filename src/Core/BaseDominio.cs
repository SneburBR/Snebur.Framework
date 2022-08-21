using Newtonsoft.Json;
using Snebur.Dominio.Atributos;
using Snebur.Serializacao;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Snebur.Dominio
{
    [Plural("BasesDominio")]
    public abstract class BaseDominio : IBaseDominio, IBaseDominioReferencia, INotifyPropertyChanged
    {
        //private Guid? __identificadorReferenciaInterno;
        private bool _isSerializando;
        private bool __isControleProprieadesDestativado;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [IgnorarGlobalizacao]
        [NaoMapear]
        [IgnorarPropriedadeTSReflexao]
        [PropriedadeProtegida]
        public string __CaminhoTipo { get; internal protected set; }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[IgnorarGlobalizacao]
        //[NaoMapear]
        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        //[PropriedadeProtegida]
        //public string __AssemblyQualifiedName { get; internal protected set; }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[IgnorarGlobalizacao]
        //[NaoMapear]
        //[OcultarColuna]
        //[PropriedadeProtegida]
        //public bool __BaseDominioReferencia { get;  set; }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[IgnorarGlobalizacao]
        //[NaoMapear]
        //[IgnorarPropriedadeTSReflexao]
        //[PropriedadeProtegida]
        //public Guid __IdentificadorUnico { get; set; }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[IgnorarGlobalizacao]
        //[NaoMapear]
        //[IgnorarPropriedadeTSReflexao]
        //[PropriedadeProtegida]
        //public Guid? __IdentificadorReferencia
        //{
        //    get
        //    {
        //        if (__BaseDominioReferencia)
        //        {
        //            return this.__identificadorReferenciaInterno;
        //        }
        //        return null;
        //    }
        //    set
        //    {
        //        this.__identificadorReferenciaInterno = value;
        //    }
        //}

        //private bool IsEntidade { get; }

        //Usar AssemblyQualifiedName para obeter o tipo

        #region Obsoleto

        //[IgnorarGlobalizacao]
        //[NaoMapear]
        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        //public string NamespaceJS { get; set; }

        //[IgnorarGlobalizacao]
        //[NaoMapear]
        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        //public string NamespaceServidor { get; set; }

        #endregion

        #region Propriedade Alterada

        [NaoMapear]
        [IgnorarGlobalizacao]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        [PropriedadeProtegida]
        public virtual bool __IsExisteAlteracao
        {
            get
            {
                return (this.__PropriedadesAlteradas?.Count ?? 0) > 0;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [OcultarColuna]
        [IgnorarGlobalizacao]
        [NaoMapear]
        [PropriedadeProtegida]
        [SomenteLeitura]
        public Dictionary<string, PropriedadeAlterada> __PropriedadesAlteradas { get; set; } = null;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        internal protected bool __IsControladorPropriedadesAlteradaAtivo { get; private set; } = false;

        [XmlIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        internal bool IsSerializando
        {
            get => this._isSerializando;
            set
            {
                if (value)
                {
                    this.DestivarControladorPropriedadeAlterada();
                }
                else
                {
                    if (this.__isControleProprieadesDestativado)
                    {
                        this.AtivarControladorPropriedadeAlterada();
                    }
                }
                this._isSerializando = value;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        internal protected bool __IsClonado { get; set; }
        #endregion

        public BaseDominio()
        {
            var tipo = this.GetType();
            this.__CaminhoTipo = String.Format("{0}.{1}", tipo.Namespace, tipo.Name);
            //this.__AssemblyQualifiedName = $"{tipo.FullName}, {tipo.Assembly.GetName().Name}";
            this.__IdentificadorUnico = Guid.NewGuid();
            //this.__IdentificadorReferencia = this.__IdentificadorUnico;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AtivarControladorPropriedadeAlterada()
        {
            if (this.__PropriedadesAlteradas == null)
            {
                this.__PropriedadesAlteradas = new Dictionary<string, PropriedadeAlterada>();
            }
            this.__IsControladorPropriedadesAlteradaAtivo = true;
            this.__isControleProprieadesDestativado = false;
        }

        private void DestivarControladorPropriedadeAlterada()
        {
            if (this.__IsControladorPropriedadesAlteradaAtivo)
            {
                if (this.__PropriedadesAlteradas?.Count == 0)
                {
                    this.__PropriedadesAlteradas = null;
                }
                this.__isControleProprieadesDestativado = true;
                this.__IsControladorPropriedadesAlteradaAtivo = false;
            }
        }
        //para server valor null na chave estrangeira de chaveEstrangeira_Id = null
        internal protected virtual void NotificarValorPropriedadeAlteradaRelacao(object antigoValor, object novoValor, [CallerMemberName] string nomePropriedade = "")
        {
            this.NotificarValorPropriedadeAlterada(antigoValor, novoValor, nomePropriedade);
            if (this.__IsControladorPropriedadesAlteradaAtivo && this is Entidade)
            {
                if (!Util.SaoIgual(antigoValor, novoValor))
                {
                    var tipoEntidade = this.GetType();
                    var propriedade = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nomePropriedade);
                    var propriedadeChaveEstrageira = EntidadeUtil.RetornarPropriedadeChaveEstrangeira(tipoEntidade, propriedade);

                    if (novoValor is Entidade entidade)
                    {
                        var antigoValorChaveEstrangeira = Convert.ToInt64(propriedadeChaveEstrageira.GetValue(this));
                        var novoValorChaveEstrangeira = entidade.Id;
                        if (antigoValorChaveEstrangeira != novoValorChaveEstrangeira || novoValorChaveEstrangeira == 0)
                        {
                            if (novoValorChaveEstrangeira > 0)
                            {
                                propriedadeChaveEstrageira.SetValue(this, novoValorChaveEstrangeira);
                            }
                            this.NotificarValorPropriedadeAlteradaChaveEstrangeira(antigoValorChaveEstrangeira, novoValorChaveEstrangeira, propriedadeChaveEstrageira.Name);
                        }
                    }
                }
            }
        }

        private void NotificarValorPropriedadeAlteradaChaveEstrangeira(long antigoValorChaveEstrangeira, long novoValorChaveEstrangeira, string nomePropriedade)
        {
            if (this.__IsControladorPropriedadesAlteradaAtivo)
            {
                if (!this.__PropriedadesAlteradas.ContainsKey(nomePropriedade))
                {
                    lock ((this.__PropriedadesAlteradas as ICollection).SyncRoot)
                    {
                        if (!this.__PropriedadesAlteradas.ContainsKey(nomePropriedade))
                        {
                            this.__PropriedadesAlteradas.Add(nomePropriedade, new PropriedadeAlterada(nomePropriedade, antigoValorChaveEstrangeira, novoValorChaveEstrangeira));
                        }
                    }
                }
                this.__PropriedadesAlteradas[nomePropriedade].NovoValor = novoValorChaveEstrangeira;
            }
            this.NotificarPropriedadeAlterada(nomePropriedade);
        }

        internal protected virtual void NotificarValorPropriedadeAlterada(object antigoValor, object novoValor, [CallerMemberName] string nomePropriedade = "")
        {
            if (this.__IsControladorPropriedadesAlteradaAtivo)
            {
                if (this.__PropriedadesAlteradas.TryGetValue(nomePropriedade, out PropriedadeAlterada propriedadeAlterada))
                {
                    if (Util.SaoIgual(propriedadeAlterada.AntigoValor, novoValor) && !this.__IsClonado)
                    {
                        this.__PropriedadesAlteradas.Remove(nomePropriedade);
                    }
                    else
                    {
                        propriedadeAlterada.NovoValor = novoValor;
                    }
                }
                else
                {
                    if (!Util.SaoIgual(antigoValor, novoValor) || this.__IsClonado)
                    {
                        lock ((this.__PropriedadesAlteradas as ICollection).SyncRoot)
                        {
                            if (!this.__PropriedadesAlteradas.ContainsKey(nomePropriedade))
                            {
                                this.__PropriedadesAlteradas.Add(nomePropriedade, new PropriedadeAlterada(nomePropriedade, antigoValor, novoValor));
                            }
                        }
                    }
                }
            }
            this.NotificarPropriedadeAlterada(nomePropriedade);
        }

        internal protected virtual void NotificarValorPropriedadeAlteradaTipoCompleto(BaseTipoComplexo antigoValor, BaseTipoComplexo novoValor, [CallerMemberName] string nomePropriedade = "")
        {
            this.NotificarValorPropriedadeAlterada(antigoValor, novoValor, nomePropriedade);
        }

        internal protected virtual T RetornarValorPropriedade<T>(T valor, [CallerMemberName] string nomePropriedade = "")
        {
            return valor;
        }
        #region INotifyPropertyChanged

        [NaoMapear]
        [IgnorarGlobalizacao]
        [XmlIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        [OcultarColuna]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool __AtivarNotificacaoPropriedadeAlterada { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        protected virtual void NotificarPropriedadeAlterada([CallerMemberName] string nomePropriedade = "")
        {
            if (this.__AtivarNotificacaoPropriedadeAlterada)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nomePropriedade));
            }
        }
        #endregion

        #region IBaseDominioReferencia

        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty]
        private Guid __IdentificadorUnico;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty]
        private Guid? __IdentificadorReferencia;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty]
        private bool? __IsBaseDominioReferencia;

        public Guid RetornarIdentificadorReferencia()
        {
            if (this.__IsBaseDominioReferencia.GetValueOrDefault())
            {
                return this.__IdentificadorReferencia.Value;
            }
            return this.__IdentificadorUnico;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [IgnorarGlobalizacao]
        [NaoMapear]
        [IgnorarPropriedadeTSReflexao]
        [PropriedadeProtegida]
        Guid IBaseDominioReferencia.__IdentificadorUnico => this.__IdentificadorUnico;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [IgnorarGlobalizacao]
        [NaoMapear]
        [IgnorarPropriedadeTSReflexao]
        [PropriedadeProtegida]
        Guid? IBaseDominioReferencia.__IdentificadorReferencia { get => this.__IdentificadorReferencia; set => this.__IdentificadorReferencia = value; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [IgnorarGlobalizacao]
        [NaoMapear]
        [IgnorarPropriedadeTSReflexao]
        [PropriedadeProtegida]
        bool? IBaseDominioReferencia.__IsBaseDominioReferencia { get => this.__IsBaseDominioReferencia; set => this.__IsBaseDominioReferencia = value; }
        bool IBaseDominioReferencia.IsSerializando { get => this.IsSerializando; set => this.IsSerializando = value; }
        //void IBaseDominioReferencia.LimparRefencia()
        //{
        //    this.__IdentificadorUnico = null;
        //}

        #endregion

    }
}