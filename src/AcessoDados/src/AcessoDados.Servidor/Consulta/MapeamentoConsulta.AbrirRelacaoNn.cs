namespace Snebur.AcessoDados.Mapeamento
{
    internal partial class MapeamentoConsulta
    {

        private void AbrirRelacaoNn(MapeamentoConsultaRelacaoAbertaNn mapeamento)
        {
            var estruturaIds = this.RetornarEstruturaIdEntidadeIdChaveEstrangeira(mapeamento);
            var idsChaveEstrangeiraFilho = new SortedSet<long>(estruturaIds.SelectMany(x => x.Value));

            if (idsChaveEstrangeiraFilho.Count == 0)
            {
                idsChaveEstrangeiraFilho.Add(0L);
            }
             
            var filtro = new FiltroMapeamentoIds(mapeamento.EstruturaEntidade.EstruturaCampoChavePrimaria, 
                                                 idsChaveEstrangeiraFilho);
            var entidadesNn = mapeamento.RetornarEntidades(filtro);

            this.MapearRelacaoAbertaNn(entidadesNn, mapeamento, estruturaIds);
        }

        private void MapearRelacaoAbertaNn(Dictionary<long, Entidade> entidadesFilho,
                                           MapeamentoConsultaRelacaoAbertaNn mapeamentoNn,
                                           Dictionary<long, SortedSet<long>> estruturaIds)
        {
            var gruposEntidadeFilhos = new Dictionary<long, List<Entidade>>();
            var propriedadeRelacaoNn = mapeamentoNn.EstruturaRelacaoNn.Propriedade;

            //if (DebugUtil.IsAttached)
            //{
            //    var entidadesPossuiPropriedade = this.Entidades.Values.Where(x => ReflexaoUtil.TipoPossuiPropriedade(x.GetType(), propriedadeRelacaoNn)).ToList();
            //    if (entidadesPossuiPropriedade.Count != this.Entidades.Count)
            //    {
            //        var xxx = "testar";
            //    }
            //}

            foreach (var entidade in this.Entidades)
            {
                if (ReflexaoUtil.TipoPossuiPropriedade(entidade.Value.GetType(), propriedadeRelacaoNn))
                {
                    gruposEntidadeFilhos.Add(entidade.Key, new List<Entidade>());
                }
            }

            var tipoEntidadeFilho = ReflexaoUtil.RetornarTipoGenericoColecao(propriedadeRelacaoNn.PropertyType);
            var estruturaCampoChaveEstrangeiraFilho = mapeamentoNn.EstruturaRelacaoNn.EstruturaCampoChaveEstrangeiraFilho;
            //var propriedadeChaveEstrangeira = estruturaCampoChaveEstrangeiraFilho.Propriedade;

            foreach (var item in estruturaIds)
            {
                var idEntidade = item.Key;
                var idsEntidadeFilhos = item.Value;

                foreach (var idEntidadeFilho in idsEntidadeFilhos)
                {
                    var entidadeFilho = entidadesFilho[idEntidadeFilho];
                    gruposEntidadeFilhos[idEntidade].Add(entidadeFilho);
                }
            }

            foreach (var grupoEntidadeFilhos in gruposEntidadeFilhos)
            {
                var entidade = this.Entidades[grupoEntidadeFilhos.Key];
                var tipoListaEntidade = typeof(ListaEntidades<>).MakeGenericType(tipoEntidadeFilho);
                var listaEntidadeFilhos = (IListaEntidades)Activator.CreateInstance(tipoListaEntidade);
                listaEntidadeFilhos.AdicionarEntidades(grupoEntidadeFilhos.Value);
                listaEntidadeFilhos.IsAberta = true;
                propriedadeRelacaoNn.SetValue(entidade, listaEntidadeFilhos);

            }
        }

        private Dictionary<long, SortedSet<long>> RetornarEstruturaIdEntidadeIdChaveEstrangeira(MapeamentoConsultaRelacaoAbertaNn mapeamento)
        {
            var resultado = new Dictionary<long, SortedSet<long>>();
            if (this.Entidades.Count == 0)
            {
                return resultado;
            }

            var idsChavePrimara = new SortedSet<long>(this.Entidades.Keys);
            var estruturaCampoChaveEstrangeiraNn = mapeamento.EstruturaRelacaoNn.EstruturaCampoChaveEstrangeiraPai;
            var estruturaCampoChaveEstrangeiraFilho = mapeamento.EstruturaRelacaoNn.EstruturaCampoChaveEstrangeiraFilho;

            var propriedadeEntidade = estruturaCampoChaveEstrangeiraNn.Propriedade;
            var propriedadeChaveEstrangeiraFilho = estruturaCampoChaveEstrangeiraFilho.Propriedade;
             
            var filtro = new FiltroMapeamentoIds(estruturaCampoChaveEstrangeiraNn,
                                                 idsChavePrimara);

            var tipoEntidade = mapeamento.TipoEntidade;
            var estruturaEntidadeNn = mapeamento.EstruturaRelacaoNn.EstruturaEntidadeRelacaoNn;

   
            foreach (var idChavePrimaria in idsChavePrimara)
            {
                resultado.Add(idChavePrimaria, new SortedSet<long>());
            }
            using (var mapeamentoEntidade = new MapeamentoEntidade(mapeamento, estruturaEntidadeNn, this.EstruturaBancoDados, this.ConexaoDB, this.Contexto))
            {
                var entidadesNn = mapeamentoEntidade.RetornarEntidades(filtro);
                foreach (var entidadeNn in entidadesNn)
                {
                    var idEntidade = (long)propriedadeEntidade.GetValue(entidadeNn);
                    var idEntidadeChaveEstrangeira = (long)propriedadeChaveEstrangeiraFilho.GetValue(entidadeNn);
                    resultado[idEntidade].Add(idEntidadeChaveEstrangeira);
                }
            }
            return resultado;
        }
    }

    public class EstruturaEntidadesFilhoRalacaoNn
    {

        public long IdEntidade { get; set; }

        public SortedSet<long> IdsChaveEstrangeira { get; set; }
    }
}