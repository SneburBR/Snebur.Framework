using Snebur.AcessoDados.Estrutura;
using System;
using System.Linq;

namespace Snebur.AcessoDados.Mapeamento
{
    internal abstract partial class BaseMapeamentoEntidade
    {
        private EstruturaEntidadeApelido RetornarEstruturaEntidadeApelidoImob(EstruturaEntidade estruturaEntidade,
                                                                              string caminhoBancoParcial,
                                                                              string caminhoPropriedadeParcial,
                                                                              EstruturaEntidadeApelido estruturaEntidadeApelidoOrigem = null,
                                                                              EstruturaRelacao estruturaRelacao = null,
                                                                              EstruturaCampoApelido estruturaCampoChaveEstrangeiraMapeado = null)
        {
            var apelidoEntidade = (estruturaEntidade == this.EstruturaEntidade) ? caminhoBancoParcial : String.Format("{0}_{1}", caminhoBancoParcial, estruturaEntidade.TipoEntidade.Name);
            var estruturaCampoChavePrimaria = estruturaEntidade.EstruturaCampoChavePrimaria;

            if (estruturaEntidadeApelidoOrigem == null)
            {
                estruturaEntidadeApelidoOrigem = this.RetornarNovaEstruturaEntidadeApelido(apelidoEntidade, estruturaEntidade, estruturaRelacao, estruturaCampoChaveEstrangeiraMapeado);

                var estruturaCampoApelidoChavePrimaria = this.RetornarEstruturaCampoApelido(estruturaCampoChavePrimaria, caminhoPropriedadeParcial, estruturaEntidadeApelidoOrigem);
                estruturaEntidadeApelidoOrigem.EstruturaCampoApelidoChavePrimaria = estruturaCampoApelidoChavePrimaria;
                estruturaEntidadeApelidoOrigem.EstruturasCampoApelido.Add(estruturaCampoApelidoChavePrimaria);
                this.TodasEstruturaCampoApelidoMapeado.Add(estruturaCampoApelidoChavePrimaria.CaminhoPropriedade, estruturaCampoApelidoChavePrimaria);

                this.EstruturaCampoApelidoChavePrimaria = estruturaCampoApelidoChavePrimaria;
            }
            var estruturasCampo = estruturaEntidade.EstruturasCampos.Select(x => x.Value).ToList();
            foreach (var estruturaCampo in estruturasCampo)
            {
                var estruturaCampoApelido = this.RetornarEstruturaCampoApelido(estruturaCampo, caminhoPropriedadeParcial, estruturaEntidadeApelidoOrigem);

                this.TodasEstruturaCampoApelidoMapeado.Add(estruturaCampoApelido.CaminhoPropriedade, estruturaCampoApelido);
                estruturaEntidadeApelidoOrigem.EstruturasCampoApelido.Add(estruturaCampoApelido);
            }
            if (estruturaEntidade.EstruturaEntidadeBase != null)
            {
                return this.RetornarEstruturaEntidadeApelidoImob(estruturaEntidade.EstruturaEntidadeBase, caminhoBancoParcial, caminhoPropriedadeParcial, estruturaEntidadeApelidoOrigem);
            }
            return estruturaEntidadeApelidoOrigem;
        }
    }
}