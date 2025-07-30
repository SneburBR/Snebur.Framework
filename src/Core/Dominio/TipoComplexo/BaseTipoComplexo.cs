using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Snebur.Dominio;

[IgnorarClasseTS]
[IgnorarGlobalizacao]
public abstract class BaseTipoComplexo : BaseDominio, ICloneable
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [NaoMapear]
    [IgnorarGlobalizacao]
    [XmlIgnore]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    [OcultarColuna]
    [PropriedadeProtegida]
    internal string? __NomePropriedadeEntidade { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [NaoMapear]
    [IgnorarGlobalizacao]
    [XmlIgnore]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    [OcultarColuna]
    [PropriedadeProtegida]
    internal Entidade? __Entidade { get; set; }

    private List<PropertyInfo> PropriedadesMapeadas { get; }

    [NaoMapear]
    [IgnorarGlobalizacao]
    [XmlIgnore]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    [OcultarColuna]
    public bool IsCongelado { get; private set; }

    public BaseTipoComplexo() : base()
    {
        var tipo = this.GetType();

        this.PropriedadesMapeadas = tipo.GetProperties().Where(x => x.DeclaringType == tipo &&
                                    x.GetGetMethod() != null &&
                                    x.GetSetMethod() != null &&
                                    x.GetSetMethod()?.IsPublic == true &&
                                    x.GetGetMethod()?.IsPublic == true &&
                                    x.GetCustomAttribute<NaoMapearAttribute>() == null).ToList();
    }
     
    protected internal override void SetProperty<T>(
        T? antigoValor,
        T? novoValor,
        [CallerMemberName] string nomePropriedade = "")
        where T : default
    {
        if (this.IsCongelado && !Util.SaoIgual(antigoValor, novoValor))
        {
            throw new Erro("Não é possível alterar valores das propriedades quando um objeto está congelado");
        }

        if (this.__Entidade?.__IsControladorPropriedadesAlteradaAtivo == true)
        {
            var caminhoPropriedade = $"{this.__NomePropriedadeEntidade}_{nomePropriedade}";

            string? nomePropriedadeTipoComplexo = nomePropriedade;

            this.__Entidade.SetProperty(
                antigoValor,
                novoValor,
                caminhoPropriedade,
                this.__NomePropriedadeEntidade,
                nomePropriedadeTipoComplexo);

            throw new NotImplementedException("This need to be check");

        }
        base.NotificarPropriedadeAlterada(nomePropriedade);
    }

    internal void NotificarTodasPropriedadesAlteradas(BaseTipoComplexo? objetoAntigo)
    {
        foreach (var propriedade in this.PropriedadesMapeadas)
        {
            var novoValor = propriedade.GetValue(this);
            var antigoValor = propriedade.GetValue(objetoAntigo);
            var caminhoPropriedade = $"{this.__NomePropriedadeEntidade}_{propriedade.Name}";
            this.__Entidade?.Set(antigoValor, novoValor, caminhoPropriedade);
        }
    }

    internal protected abstract BaseTipoComplexo BaseClone();

    object ICloneable.Clone()
    {
        return this.BaseClone();
    }

    public T Clone<T>() where T : BaseTipoComplexo
    {
        return this.Clone() as T ??
            throw new InvalidOperationException("Clone não implementado.");
    }

    public BaseTipoComplexo Clone()
    {
        return (this as ICloneable)?.Clone() as BaseTipoComplexo
            ?? throw new InvalidOperationException("Clone não implementado.");
    }

    public void Congelar()
    {
        this.IsCongelado = true;
    }
}