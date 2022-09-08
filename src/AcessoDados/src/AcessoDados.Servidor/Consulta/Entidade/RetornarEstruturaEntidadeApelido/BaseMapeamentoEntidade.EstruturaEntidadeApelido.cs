using Snebur.AcessoDados.Estrutura;
using System;
using System.Linq;

namespace Snebur.AcessoDados.Mapeamento
{
    internal abstract partial class BaseMapeamentoEntidade
    {

        private EstruturaEntidadeApelido RetornarEstruturaEntidadeApelido(EstruturaEntidade estruturaEntidade,
                                                                         string caminhoBancoParcial,
                                                                         string caminhoPropriedadeParcial,
                                                                         EstruturaEntidadeApelido estruturaEntidadeApelidoOrigem = null,
                                                                         EstruturaRelacao estruturaRelacao = null,
                                                                         EstruturaCampoApelido estruturaCampoChaveEstrangeiraApelido = null)
        {
            if (ConfiguracaoAcessoDados.TipoBancoDadosEnum == EnumTipoBancoDados.PostgreSQLImob)
            {
                return this.RetornarEstruturaEntidadeApelidoImob(estruturaEntidade, caminhoBancoParcial, caminhoPropriedadeParcial, estruturaEntidadeApelidoOrigem, estruturaRelacao, estruturaCampoChaveEstrangeiraApelido);
            }
            else
            {
                return this.RetornarEstruturaEntidadeApelidoPadrao(estruturaEntidade, caminhoBancoParcial, caminhoPropriedadeParcial, estruturaEntidadeApelidoOrigem, estruturaRelacao, estruturaCampoChaveEstrangeiraApelido);
            }
        }

        private EstruturaEntidadeApelido RetornarEstruturaEntidadeApelidoPadrao(EstruturaEntidade estruturaEntidade,
                                                                               string caminhoBancoParcial,
                                                                               string caminhoPropriedadeParcial,
                                                                               EstruturaEntidadeApelido estruturaEntidadeApelidoOrigem = null,
                                                                               EstruturaRelacao estruturaRelacao = null,
                                                                               EstruturaCampoApelido estruturaCampoChaveEstrangeiraApelido = null)
        {
            EstruturaEntidadeApelido estruturaEntidadeApelido = null;

            var apelidoEntidade = (estruturaEntidade == this.EstruturaEntidade) ? caminhoBancoParcial : String.Format("{0}_{1}", caminhoBancoParcial, estruturaEntidade.TipoEntidade.Name);
            var estruturaCampoChavePrimaria = estruturaEntidade.EstruturaCampoChavePrimaria;
            // var estruturaCampoChavePrimariaMapeado = this.RetornarEstruturaCampoMapeado(estruturaCampoChavePrimaria, caminhoPropriedadeParcial, apelidoEntidade);

            if (estruturaEntidadeApelidoOrigem == null)
            {
                estruturaEntidadeApelidoOrigem = this.RetornarNovaEstruturaEntidadeApelido(apelidoEntidade, estruturaEntidade, estruturaRelacao, estruturaCampoChaveEstrangeiraApelido);

                var estruturaCampoApelidoChavePrimaria = this.RetornarEstruturaCampoApelido(estruturaCampoChavePrimaria, caminhoPropriedadeParcial, estruturaEntidadeApelidoOrigem);

                estruturaEntidadeApelidoOrigem.EstruturaCampoApelidoChavePrimaria = estruturaCampoApelidoChavePrimaria;
                estruturaEntidadeApelidoOrigem.EstruturasCampoApelido.Add(estruturaCampoApelidoChavePrimaria);
                this.TodasEstruturaCampoApelidoMapeado.Add(estruturaCampoApelidoChavePrimaria.CaminhoPropriedade, estruturaCampoApelidoChavePrimaria);
                estruturaEntidadeApelido = estruturaEntidadeApelidoOrigem;

                this.EstruturaCampoApelidoChavePrimaria = estruturaCampoApelidoChavePrimaria;
            }
            else
            {
                var estruturaEntidadeMapeadaBase = new EstruturaEntidadeApelidoBase(this.MapeamentoConsulta,
                                                                                    apelidoEntidade,
                                                                                    estruturaEntidadeApelidoOrigem,
                                                                                    estruturaEntidade);

                estruturaEntidadeMapeadaBase.EstruturaCampoApelidoChavePrimaria = this.RetornarEstruturaCampoApelido(estruturaCampoChavePrimaria, caminhoPropriedadeParcial, estruturaEntidadeMapeadaBase);

                estruturaEntidadeApelidoOrigem.EstruturasEntidadeMapeadaBase.Add(estruturaEntidadeMapeadaBase);

                estruturaEntidadeApelido = estruturaEntidadeMapeadaBase;
            }
            var estruturasCampo = estruturaEntidade.EstruturasCampos.Values.ToList();
            //if (this.Contexto.IsSuportaOffsetFetch)
            //{
            //    if (!this.TipoEntidade.IsAbstract)
            //    {
            //        estruturasCampo.Remove(this.EstruturaEntidade.EstruturaCampoNomeTipoEntidade);
            //    }

            //}
            foreach (var estruturaCampo in estruturasCampo)
            {

                var estruturaCampoApelido = this.RetornarEstruturaCampoApelido(estruturaCampo, caminhoPropriedadeParcial, estruturaEntidadeApelido);
                if (estruturaCampoApelido.EstruturaCampo.IsTipoComplexo)
                {
                    this.TodasEstruturaCampoApelidoMapeado.Add(estruturaCampoApelido.ChavePropriedadeAdicional, estruturaCampoApelido);
                }
                this.TodasEstruturaCampoApelidoMapeado.Add(estruturaCampoApelido.CaminhoPropriedade, estruturaCampoApelido);
                estruturaEntidadeApelidoOrigem.EstruturasCampoApelido.Add(estruturaCampoApelido);
            }
            if (estruturaEntidade.EstruturaEntidadeBase != null)
            {
                return this.RetornarEstruturaEntidadeApelido(estruturaEntidade.EstruturaEntidadeBase, caminhoBancoParcial, caminhoPropriedadeParcial, estruturaEntidadeApelidoOrigem);
            }
            return estruturaEntidadeApelidoOrigem;
        }

        private EstruturaCampoApelido RetornarEstruturaCampoApelido(EstruturaCampo estruturaCampo, string caminhoPropriedadeParcial, EstruturaEntidadeApelido estruturaEntidadeApelido)
        {
            var ponto = String.IsNullOrWhiteSpace(caminhoPropriedadeParcial) ? "" : ".";
            var caminhoPropriedade = String.Format("{0}{1}{2}", caminhoPropriedadeParcial, ponto, estruturaCampo.RetornarCaminhoPropriedade());
            var apelidoCampo = String.Format("{0}_{1}", estruturaEntidadeApelido.ApelidoOriginal, estruturaCampo.RetornarCaminhoPropriedade());
            var caminhoBanco = String.Format("{0}.[{1}]", estruturaEntidadeApelido.Apelido, estruturaCampo.NomeCampo);

            return new EstruturaCampoApelido(this.MapeamentoConsulta, caminhoPropriedade, estruturaCampo, estruturaEntidadeApelido, apelidoCampo, caminhoBanco);
        }

        private EstruturaEntidadeApelido RetornarNovaEstruturaEntidadeApelido(string apelidoEntidadeMapeada,
                                                                              EstruturaEntidade estruturaEntidade,
                                                                              EstruturaRelacao estruturaRelacao,
                                                                              EstruturaCampoApelido estruturaCampoChaveEstrangeiraMapeado)
        {
            if (estruturaRelacao == null)
            {
                return new EstruturaEntidadeApelido(this.MapeamentoConsulta,
                                                    apelidoEntidadeMapeada,
                                                    estruturaEntidade);
            }
            if (estruturaRelacao is EstruturaRelacaoPai)
            {
                return new EstruturaEntidadeApelidoRelacaoPai(this.MapeamentoConsulta,
                                                              apelidoEntidadeMapeada,
                                                              estruturaEntidade,
                                                              estruturaRelacao,
                                                              estruturaCampoChaveEstrangeiraMapeado);
            }
            throw new NotImplementedException();
        }
    }
}