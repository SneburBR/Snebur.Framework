using Snebur.Dominio.Atributos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Snebur.Dominio
{
    [IgnorarClasseTS]
    [IgnorarGlobalizacao]
    public abstract class BaseTipoComplexo : BaseDominio, ICloneable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [NaoMapear]
        [IgnorarGlobalizacao]
        [XmlIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        [OcultarColuna]
        [PropriedadeProtegida]
        internal string __NomePropriedadeEntidade { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [NaoMapear]
        [IgnorarGlobalizacao]
        [XmlIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        [OcultarColuna]
        [PropriedadeProtegida]
        internal Entidade __Entidade { get; set; }

        private List<PropertyInfo> PropriedadesMapeadas { get; }

        [NaoMapear]
        [IgnorarGlobalizacao]
        [XmlIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        [OcultarColuna]
        public bool IsCongelado { get; private set; }

        public BaseTipoComplexo() : base()
        {
            var tipo = this.GetType();

            this.PropriedadesMapeadas = tipo.GetProperties().Where(x => x.DeclaringType == tipo &&
                                        x.GetMethod != null &&
                                        x.SetMethod != null &&
                                        x.SetMethod.IsPublic &&
                                        x.GetMethod.IsPublic &&
                                        x.GetCustomAttribute<NaoMapearAttribute>() == null).ToList();
        }

        internal protected override void NotificarValorPropriedadeAlterada(object antigoValor, object novoValor,
                                                                           [CallerMemberName] string nomePropriedade = "")
        {
            if (this.IsCongelado && antigoValor != novoValor)
            {
                throw new Erro("Não é possivel alterar valores das propriedades quando um objeto está congelado");
            }
            if (this.__Entidade != null && this.__Entidade.__IsControladorPropriedadesAlteradaAtivo)
            {
                var caminhoPropriedade = $"{this.__NomePropriedadeEntidade}_{nomePropriedade}";
                this.__Entidade.NotificarValorPropriedadeAlterada(antigoValor, novoValor, caminhoPropriedade);
            }
            base.NotificarPropriedadeAlterada(nomePropriedade);
        }

        internal void NotificarTodasPropriedadesAlteradas(BaseTipoComplexo objetoAntigo)
        {
            foreach (var propriedade in this.PropriedadesMapeadas)
            {
                var novoValor = propriedade.GetValue(this);
                var antigoValor = propriedade.GetValue(objetoAntigo);
                var caminhoPropriedade = $"{this.__NomePropriedadeEntidade}_{propriedade.Name}";
                this.__Entidade.NotificarValorPropriedadeAlterada(antigoValor, novoValor, caminhoPropriedade);
            }
        }

        internal protected abstract BaseTipoComplexo BaseClone();

        object ICloneable.Clone()
        {
            return this.BaseClone();
        }

        public T Clone<T>() where T : BaseTipoComplexo
        {
            return this.Clone() as T;
        }

        public BaseTipoComplexo Clone()
        {
            return (this as ICloneable).Clone() as BaseTipoComplexo;
        }
        public void Congelar()
        {
            this.IsCongelado = true;
        }
    }
}