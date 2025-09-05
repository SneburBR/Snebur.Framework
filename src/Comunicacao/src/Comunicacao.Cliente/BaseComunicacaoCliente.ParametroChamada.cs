using System.Linq;
using System.Reflection;

namespace Snebur.Comunicacao;

public abstract partial class BaseComunicacaoCliente
{

    protected ParametroChamada RetornarParametroChamada(ParameterInfo parametro, object? valor)
    {

        Type tipo = parametro.ParameterType;

        if (tipo.IsEnum)
        {
            return this.RetornarPrametroChamadaEnum(parametro, valor);
        }

        if (ReflexaoUtil.TipoRetornaTipoPrimario(tipo))
        {
            return this.RetornarParametroChamadaTipoPrimario(parametro, valor);
        }

        if (ReflexaoUtil.IsTipoIgualOuHerda(tipo, (typeof(BaseDominio))))
        {
            return this.RetornarParametroChamadaBaseDominio(parametro, valor);
        }

        if (ReflexaoUtil.IsTipoRetornaColecao(tipo))
        {
            var tipoItemLista = tipo.GetGenericArguments().Single();

            if (ReflexaoUtil.IsTipoIgualOuHerda(tipoItemLista, typeof(BaseDominio)) ||
                ReflexaoUtil.IsTipoIgualOuHerda(tipoItemLista, typeof(IEntidade)) ||
                ReflexaoUtil.IsTipoImplementaInterface(tipoItemLista, typeof(IEntidade)))
            {
                return this.RetornarParametroChamadaListaBaseDominio(parametro, valor);
            }

            if (tipoItemLista.IsEnum)
            {
                return this.RetornarParametroChamadaListaEnum(parametro, valor);
            }

            if (ReflexaoUtil.TipoRetornaTipoPrimario(tipoItemLista))
            {
                return this.RetornarParametroChamadaListaTipoPrimario(parametro, valor);
            }

            throw new NotSupportedException(string.Format("O tipo do item da lista nao é suportado '{0}'", tipoItemLista.Name));
        }
        throw new ErroNaoSuportado(string.Format("O tipo não é suportado {0}", tipo.Name));
    }

    private ParametroChamadaEnum RetornarPrametroChamadaEnum(ParameterInfo parametro, object? valor)
    {
        var tipo = parametro.ParameterType;
        Guard.NotNull(tipo.AssemblyQualifiedName);
        var parametroChamadaTipoPrimarioEnum = new ParametroChamadaEnum
        {
            Nome = parametro.Name,
            NomeTipoParametro = parametro.ParameterType.Name,
            NomeTipoEnum = tipo.Name,
            Valor = Convert.ToInt32(valor),
            AssemblyQualifiedName = FormatacaoUtil.FormatarTipoQualificado(tipo.AssemblyQualifiedName)
        };
        return parametroChamadaTipoPrimarioEnum;
    }

    private ParametroChamadaTipoPrimario RetornarParametroChamadaTipoPrimario(ParameterInfo parametro, object? valor)
    {
        var tipo = parametro.ParameterType;
        var tipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(tipo);
        Guard.NotNull(tipo.AssemblyQualifiedName);
        var parametroChamadaTipoPrimario = new ParametroChamadaTipoPrimario
        {
            Nome = parametro.Name,
            NomeTipoParametro = EnumUtil.RetornarDescricao(tipoPrimarioEnum),
            TipoPrimarioEnum = tipoPrimarioEnum,
            Valor = valor,
            AssemblyQualifiedName = FormatacaoUtil.FormatarTipoQualificado(tipo.AssemblyQualifiedName)
        };
        return parametroChamadaTipoPrimario;
    }

    private ParametroChamadaBaseDominio RetornarParametroChamadaBaseDominio(ParameterInfo parametro, object? valor)
    {
        var tipo = parametro.ParameterType;
        Guard.NotNull(tipo.AssemblyQualifiedName);
        var parametroChamadaBaseDominio = new ParametroChamadaBaseDominio
        {
            Nome = parametro.Name,
            BaseDominio = valor as BaseDominio,
            NomeTipoParametro = tipo.Name,
            AssemblyQualifiedName = FormatacaoUtil.FormatarTipoQualificado(tipo.AssemblyQualifiedName)
        };
        return parametroChamadaBaseDominio;
    }

    private ParametroChamadaListaBaseDominio RetornarParametroChamadaListaBaseDominio(
        ParameterInfo parametro,
        object? valor)
    {
        var basesDominio = ConverterUtil.ToList<BaseDominio?>(valor);
        var tipoBaseDominio = ReflexaoUtil.RetornarTipoGenericoColecao(parametro.ParameterType);
        var parametroChamadaListaBaseDominio = new ParametroChamadaListaBaseDominio
        {
            Nome = parametro.Name,
            NomeTipoParametro = "Lista",
            AssemblyQualifiedName = tipoBaseDominio.AssemblyQualifiedName,
            NomeTipoBaseDominio = tipoBaseDominio.Name,
            NomeNamespaceTipoBaseDominio = tipoBaseDominio.Namespace,
            BasesDominio = basesDominio
        };
        return parametroChamadaListaBaseDominio;
    }

    private ParametroChamadaListaEnum RetornarParametroChamadaListaEnum(ParameterInfo parametro, object? valor)
    {
        var tipo = ReflexaoUtil.RetornarTipoGenericoColecao(parametro.ParameterType);
        Guard.NotNull(tipo.AssemblyQualifiedName);
        var parametroChamadaListaEnum = new ParametroChamadaListaEnum
        {
            Nome = parametro.Name,
            NomeTipoEnum = tipo.Name,
            NamespaceEnum = tipo.Namespace,
            AssemblyQualifiedName = FormatacaoUtil.FormatarTipoQualificado(tipo.AssemblyQualifiedName),
            NomeTipoParametro = "Enum"
        };

        var valores = new List<int>();
        if (valor is ICollection collection)
        {
            foreach (var item in collection)
            {
                valores.Add(Convert.ToInt32(item));
            }
        }
        parametroChamadaListaEnum.Valores = valores;
        return parametroChamadaListaEnum;
    }

    private ParametroChamadaListaTipoPrimario RetornarParametroChamadaListaTipoPrimario(
        ParameterInfo parametro,
        object? valor)
    {
        var tipo = ReflexaoUtil.RetornarTipoGenericoColecao(parametro.ParameterType);
        Guard.NotNull(tipo.AssemblyQualifiedName);
        var parametroChamadaListaBaseDominio = new ParametroChamadaListaTipoPrimario
        {
            Nome = parametro.Name,
            TipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(tipo),
            AssemblyQualifiedName = FormatacaoUtil.FormatarTipoQualificado(tipo.AssemblyQualifiedName),
            NomeTipoParametro = "Lista",
        };

        var lista = new List<object?>();
        if (valor is ICollection collection)
        {
            foreach (var item in collection)
            {
                lista.Add((object?)item);
            }
        }
        parametroChamadaListaBaseDominio.Lista = lista;
        return parametroChamadaListaBaseDominio;
    }
}
