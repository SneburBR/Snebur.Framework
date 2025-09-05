namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal partial class NormalizarEntidade : IDisposable
    {
        internal HashSet<Entidade> Entidades { get; private set; }
        internal BaseContextoDados Contexto { get; private set; }
        internal Dictionary<string, Entidade> EntidadesNormalizadas { get; private set; } = new Dictionary<string, Entidade>();

        private NormalizarEntidade(BaseContextoDados contexto, 
                                   HashSet<Entidade> entidades)
        {
            if (contexto == null)
            {
                throw new ArgumentNullException(nameof(contexto));
            }

            this.Contexto = contexto;
            this.Entidades = entidades;
        }

        internal HashSet<Entidade> RetornarEntidadesNormalizada()
        {
            this.Normalizar();
            return this.EntidadesNormalizadas.Values.ToHashSet();
        }

        private void Normalizar()
        {
            foreach (var entidade in this.Entidades)
            {
                this.Normalizar(entidade);
            }
        }

        private void Normalizar(Entidade entidade)
        {
            if (!this.EntidadesNormalizadas.ContainsKey(entidade.__IdentificadorEntidade))
            {
                this.EntidadesNormalizadas.Add(entidade.__IdentificadorEntidade, entidade);

                var estruturaEntidade = this.Contexto.EstruturaBancoDados.RetornarEstruturaEntidade(entidade.GetType().Name, true);
                if (estruturaEntidade.IsImplementaInterfaceIAtividadeUsuario)
                {
                    var atividadeUsuario = (IAtividadeUsuario)entidade;
                    atividadeUsuario.Usuario = this.Contexto.UsuarioLogado;
                    atividadeUsuario.SessaoUsuario = this.Contexto.SessaoUsuarioLogado;
                }
                //if (estruturaEntidade.IsUsuario)
                //{
                //    throw new NotImplementedException();
                //}

                var estruturasRelacoesPai = estruturaEntidade.TodasRelacoesPai();
                var estruturasRelacoesUmUm = estruturaEntidade.TodasRelacoesUmUm();
                var estruturasRelacoesUmUmReversa = estruturaEntidade.TodasRelacoesUmUmReversa();
                var estruturasRelacoesFilhos = estruturaEntidade.TodasRelacoesFilhos;
                var estruturasRelacoesNn = estruturaEntidade.TodasRelacoesNn();

                foreach (var estruturaRelacaoPai in estruturasRelacoesPai)
                {
                    var entidadeRelacaoPai = (Entidade)estruturaRelacaoPai.Propriedade.GetValue(entidade);
                    if (entidadeRelacaoPai != null)
                    {
                        this.Normalizar(entidadeRelacaoPai);
                    }
                }
                foreach (var estruturaRelacaoUmUm in estruturasRelacoesUmUm)
                {
                    var entidadeRelacaoUmUm = (Entidade)estruturaRelacaoUmUm.Propriedade.GetValue(entidade);
                    if (entidadeRelacaoUmUm != null)
                    {
                        this.Normalizar(entidadeRelacaoUmUm);
                    }
                }
                foreach (var estruturaRelacaoUmUmReversa in estruturasRelacoesUmUmReversa)
                {
                    var entidadeRelacaoUmUmReversa = (Entidade)estruturaRelacaoUmUmReversa.Propriedade.GetValue(entidade);
                    if (entidadeRelacaoUmUmReversa != null)
                    {
                        this.Normalizar(entidadeRelacaoUmUmReversa);
                    }
                }
                foreach (var estruturaRelacaoFilhos in estruturasRelacoesFilhos)
                {
                    var entidadesFilho = (IListaEntidades)estruturaRelacaoFilhos.Propriedade.GetValue(entidade);
                    if (entidadesFilho.Count > 0)
                    {
                        foreach (Entidade entidadeFilho in entidadesFilho)
                        {
                            var relacaoPai = (IEntidade)estruturaRelacaoFilhos.EstruturaRelacaoPai.Propriedade.GetValue(entidadeFilho);
                            if (relacaoPai != null)
                            {
                                if (relacaoPai.Id != entidade.Id)
                                {
                                    var propriedadeRelacoaPai = estruturaRelacaoFilhos.EstruturaRelacaoPai.Propriedade;
                                    propriedadeRelacoaPai.SetValue(entidadeFilho, entidade);
                                }
                            }
                            else
                            {
                                var idRelacao = Convert.ToInt64(estruturaRelacaoFilhos.EstruturaCampoChaveEstrangeira.Propriedade.GetValue(entidadeFilho));
                                if (idRelacao != entidade.Id)
                                {
                                    var propriedadeRelacoaPai = estruturaRelacaoFilhos.EstruturaRelacaoPai.Propriedade;
                                    propriedadeRelacoaPai.SetValue(entidadeFilho, entidade);
                                }
                            }
                            this.Normalizar(entidadeFilho);
                        }
                    }

                    if (entidadesFilho.EntidadesRemovida.Count > 0)
                    {
                        //throw new ErroNaoImplementado();
                    }
                }
                foreach (var estruturaRelacaoNn in estruturasRelacoesNn)
                {
                    //var entidadesRelacaoNn = (IListaEntidades)estruturaRelacaoNn.Propriedade.GetValue(entidade);
                    //if (entidadesRelacaoNn.Count > 0)
                    //{
                    //    if (entidadesRelacaoNn.EntidadesRemovida.Count > 0)
                    //    {
                    //        throw new ErroNaoImplementado();
                    //    }

                    //    if (entidadesRelacaoNn.EntidadesAdicionada.Count > 0)
                    //    {
                    //        throw new ErroNaoImplementado();
                    //    }

                    //}
                }
            }
        }
        #region IDisposable

        public void Dispose()
        {
            this.Contexto = null;
            this.EntidadesNormalizadas.Clear();
            this.EntidadesNormalizadas = null;
            this.Entidades = null;
        }
        #endregion
    }
}