using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento;

internal class EstruturaEntidadeApelidoBase : EstruturaEntidadeApelido
{

    internal EstruturaEntidadeApelido EstruturaEntidadeMapeadaPai { get; set; }

    internal EstruturaEntidadeApelidoBase(BaseMapeamentoConsulta mapeamentoConsutla,
                                          string apelidoEntidadeMapeada,
                                          EstruturaEntidadeApelido estruturaEntidadeMapeadaPai,
                                          EstruturaEntidade estruturaEntidade) :
                                          base(mapeamentoConsutla,
                                               apelidoEntidadeMapeada,
                                               estruturaEntidade)
    {
        Guard.NotNull(estruturaEntidadeMapeadaPai.EstruturaCampoApelidoChavePrimaria);
        this.EstruturaEntidadeMapeadaPai = estruturaEntidadeMapeadaPai;
    }

    internal string RetornarSqlUniaoEntidadeBaseBanco()
    {
        Guard.NotNull(this.EstruturaCampoApelidoChavePrimaria);
        Guard.NotNull(this.EstruturaEntidadeMapeadaPai.EstruturaCampoApelidoChavePrimaria);

        //return String.Format("    {0} As {1} ON {2} = {3} ", this.CaminhoBanco, this.Apelido,
        //                                                     this.EstruturaEntidadeMapeadaPai.EstruturaCampoApelidoChavePrimaria.CaminhoBanco,
        //                                                     this.EstruturaCampoApelidoChavePrimaria.CaminhoBanco);

        return $"{this.CaminhoBanco} As {this.Apelido} ON {this.EstruturaEntidadeMapeadaPai.EstruturaCampoApelidoChavePrimaria.CaminhoBanco} = {this.EstruturaCampoApelidoChavePrimaria.CaminhoBanco} ";

    }
}