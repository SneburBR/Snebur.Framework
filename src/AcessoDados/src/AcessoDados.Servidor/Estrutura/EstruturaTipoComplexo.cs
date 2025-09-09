namespace Snebur.AcessoDados.Estrutura;

internal class EstruturaTipoComplexo : EstruturaPropriedade
{

    internal DicionarioEstrutura<EstruturaCampo> EstruturasCampo { get; set; }

    internal EstruturaTipoComplexo(PropertyInfo propriedade, EstruturaEntidade estruturaEntidade) : base(propriedade, estruturaEntidade)
    {
        this.EstruturasCampo = this.RetornarEstruturasCampo();
        this.IsAceitaNulo = false;
    }

    private DicionarioEstrutura<EstruturaCampo> RetornarEstruturasCampo()
    {
        var estruturasCampos = new DicionarioEstrutura<EstruturaCampo>();
        var tipoNormalizado = this.RetornarTipoNormalizado(this.Tipo);
        var propriedadesCampo = AjudanteEstruturaBancoDados.RetornarPropriedadesCampos(tipoNormalizado);
        foreach (var propriedadeCampo in propriedadesCampo)
        {
            estruturasCampos.Add(propriedadeCampo.Name, new EstruturaCampo(this.Propriedade, propriedadeCampo, this.EstruturaEntidade));
        }
        return estruturasCampos;
    }

    private Type RetornarTipoNormalizado(Type tipo)
    {
        if (tipo.BaseType?.IsGenericType == true)
        {
            if (tipo.BaseType.GetGenericTypeDefinition() == typeof(BaseListaTipoComplexo<>))
            {
                return tipo.BaseType;
            }
            throw new Exception("O tipo não é suportado");
        }
        return tipo;

    }
}