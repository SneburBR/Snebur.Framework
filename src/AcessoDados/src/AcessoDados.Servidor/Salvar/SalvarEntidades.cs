using Snebur.Dominio;
using Snebur.Linq;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal partial class SalvarEntidades : IDisposable
    {
        internal List<EntidadeAlterada> EntidadesAlteradas { get; private set; }
        internal List<EntidadeAlterada> EntidadesAlteradasAlteracoesGenericas { get; private set; }
        internal Queue<EntidadeAlterada> FilaEntidades { get; private set; }
        internal Queue<EntidadeAlterada> FilaAlteracoesGenericas { get; private set; }

        internal BaseContextoDados Contexto { get; private set; }
        internal bool IsNotificarAlteracaoPropriedade { get; }
        internal EnumOpcaoSalvar OpcaoSalvar { get; }

        internal SalvarEntidades(BaseContextoDados contexto,
                                 HashSet<Entidade> entidades,
                                 EnumOpcaoSalvar opcaoDeletar,
                                 bool isNotificarAlteracaoPropriedade)
        {
            foreach (var entidade in entidades)
            {
                entidade.AtivarControladorPropriedadeAlterada();
            }

            this.OpcaoSalvar = opcaoDeletar;
            this.IsNotificarAlteracaoPropriedade = isNotificarAlteracaoPropriedade;
            this.FilaEntidades = new Queue<EntidadeAlterada>();
            this.Contexto = contexto;

            var entidadesNormalizada = this.RetornarEntidadesNormalizadas(entidades);
            this.EntidadesAlteradas = this.RetornarEntidadesAlteradas(entidadesNormalizada);
            this.EntidadesAlteradasAlteracoesGenericas = this.RetornarEntidadesAlteracoesPropriedadeGenericas(entidadesNormalizada);

            this.FilaEntidades = FilaEntidadeAlterada.RetornarFila(this.EntidadesAlteradas);
            this.FilaAlteracoesGenericas = FilaEntidadeAlterada.RetornarFila(this.EntidadesAlteradasAlteracoesGenericas);

            if (opcaoDeletar == EnumOpcaoSalvar.Deletar ||
                opcaoDeletar == EnumOpcaoSalvar.DeletarRegistro)
            {
                this.FilaEntidades = new Queue<EntidadeAlterada>(this.FilaEntidades.Reverse());
            }
        }

        internal Resultado Salvar(bool ignorarValidacao = false)
        {
            if (this.OpcaoSalvar == EnumOpcaoSalvar.Salvar && !ignorarValidacao)
            {
                var entidades = Enumerable.Reverse(this.FilaEntidades).Select(x => x.Entidade).ToList();
                var errosValidacao = ValidarEntidades.Validar(this.Contexto, entidades.ToList());
                if (errosValidacao.Count > 0)
                {
                    return this.RetornarResultadoSalvarErrosValidacao(errosValidacao);
                }
            }

            var conexao = this.Contexto.Conexao;
            var fila = this.FilaEntidades;
            var resultado = this.SalvarEntidadesInterno(conexao, fila);
            if (resultado.IsSucesso && this.FilaAlteracoesGenericas.Count > 0)
            {
                this.SalvarAlteracoesGenericas();
            }
            return resultado;
        }

        private Resultado SalvarEntidadesInterno(BaseConexao conexao,
                                                  Queue<EntidadeAlterada> fila)
        {
            if (this.Contexto.IsExisteTransacao)
            {
                return this.SalvarTransacao(conexao, fila);
            }
            else
            {
                return this.SalvarNormal(conexao, fila, false);
            }
        }

        private void SalvarAlteracoesGenericas()
        {
            var conexao = this.Contexto.ContextoSessaoUsuario.Conexao;
            var fila = this.FilaAlteracoesGenericas;
            this.SalvarNormal(conexao, fila, true);
        }

        private Resultado SalvarNormal(BaseConexao conexao,
                                       Queue<EntidadeAlterada> fila,
                                       bool isIgnorarErro = false)
        {

            var linhasAfetadas = 0;
            var comandosExecutados = new List<string>();
            var entidadesAlteradas = new List<EntidadeAlterada>();
            var erros = new List<Exception>();
            try
            {
                if (fila.Count > 0)
                {
                    //$######### TESTE
                    foreach (var entidadeAlterada in fila.ToList())
                    {
                        entidadeAlterada.Comandos = entidadeAlterada.RetornarCommandos();
                    }

                    using (var dbConnection = conexao.RetornarNovaConexao())
                    {
                        dbConnection.Open();
                        try
                        {
                            using (var transacao = dbConnection.BeginTransaction(ConfiguracaoAcessoDados.IsolamentoLevelSalvarPadrao))
                            {
                                try
                                {
                                    while (fila.Count > 0)
                                    {
                                        var entidadeAlterada = fila.Dequeue();
                                        //var comandos = entidadeAlterada.RetornarCommandos();
                                        foreach (var comando in entidadeAlterada.Comandos)
                                        {
                                            try
                                            {
                                                linhasAfetadas += this.ExecutarCommando(conexao,
                                                                                        dbConnection,
                                                                                        transacao,
                                                                                        entidadeAlterada,
                                                                                        comando);
                                            }
                                            catch (Exception erro)
                                            {
                                                if (!isIgnorarErro)
                                                {
                                                    throw new ErroConsultaSql(comando.SqlCommando, erro);
                                                }
                                            }
                                            finally
                                            {
                                                comando.Dispose();
                                            }
                                        }
                                        if (entidadeAlterada.Entidade.Id == 0)
                                        {
                                            throw new Erro($"A entidade não foi salvar {entidadeAlterada.Entidade.__CaminhoTipo}");
                                        }
                                        entidadeAlterada.RetornarCommandos();
                                        entidadesAlteradas.Add(entidadeAlterada);
                                    }
                                    transacao.Commit();
                                    //return this.RetornarResultado(entidadesAltearas);
                                }
                                catch
                                {
                                    transacao.Rollback();
                                    this.EntidadesAlteradas.ForEach(x => x.Rollback());
                                    throw;
                                }
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            dbConnection.Close();
                        }
                    }
                }
                return this.RetornarResultado(entidadesAlteradas);

            }
            catch (Exception erro)
            {
                LogUtil.ErroAsync(erro);
                return this.RetornarResultadoErro(erro);
            }
            finally
            {

            }
        }

        private Resultado SalvarTransacao(BaseConexao conexao,
                                          Queue<EntidadeAlterada> fila)
        {
            var comandosExecutados = new List<string>();
            var entidadesAltearas = new List<EntidadeAlterada>();
            var linhasAfetadas = 0;

            var dbConnection = this.Contexto.ConexaoAtual;
            var transacao = this.Contexto.TransacaoAtual;

            if (fila.Count > 0)
            {
                //$######### TESTE
                foreach (var entidadeAlterada in fila.ToList())
                {
                    entidadeAlterada.Comandos = entidadeAlterada.RetornarCommandos();
                }

                while (fila.Count > 0)
                {
                    var entidadeAlterada = fila.Dequeue();
                    //var comandos = entidadeAlterada.RetornarCommandos();
                    foreach (var comando in entidadeAlterada.Comandos)
                    {
                        try
                        {
                            linhasAfetadas += this.ExecutarCommando(conexao,
                                                                    dbConnection,
                                                                    transacao,
                                                                    entidadeAlterada,
                                                                    comando);
                        }
                        catch (Exception erro)
                        {
                            throw new ErroConsultaSql(comando.SqlCommando, erro);
                        }
                        finally
                        {
                            comando.Dispose();
                        }
                    }
                    if (entidadeAlterada.Entidade.Id == 0)
                    {
                        throw new Erro($"A entidade não foi salvar {entidadeAlterada.Entidade.__CaminhoTipo}");
                    }
                    entidadesAltearas.Add(entidadeAlterada);
                }

                //return this.RetornarResultado(entidadesAltearas);

            }

            return this.RetornarResultado(entidadesAltearas);
        }

        internal int ExecutarCommando(BaseConexao conexao,
                                      DbConnection dbConnection,
                                      DbTransaction transacao,
                                      EntidadeAlterada entidadeAlterada,
                                      Comando comando)
        {
            using (var cmd = conexao.RetornarNovoComando(comando.SqlCommando, null, dbConnection, transacao))
            {
                var parametros = comando.RetornarParametros();
                foreach (var parametro in parametros)
                {
                    if (!parametro.EstruturaCampo.IsValorFuncaoServidor)
                    {
                        var dbParametro = conexao.RetornarNovoParametro(parametro.EstruturaCampo,
                                                                        parametro.EstruturaCampo.NomeParametro,
                                                                        parametro.Valor);
                        cmd.Parameters.Add(dbParametro);
                    }
                }

                DepuracaoUtil.EscreverSaida(this.Contexto,
                                            cmd.Parameters,
                                            cmd.CommandText);

                switch (comando)
                {
                    case ComandoInsert comandoInsert:

                        if (comandoInsert.IsRecuperarUltimoId)
                        {

                            var valorId = cmd.ExecuteScalar();
                            var id = Convert.ToInt64(valorId);
                            if (id == 0)
                            {
                                throw new Exception(" ultimo id invalido");
                            }
                            entidadeAlterada.SetId(id);

                            return 0;
                        }
                        return cmd.ExecuteNonQuery();

                    case ComandoInsertOrUpdate comandoInsertOrUpdate:

                        return cmd.ExecuteNonQuery();

                    case ComandoUpdate comandoUpdate:
                    case ComandoDelete comandoDelete:

                        return cmd.ExecuteNonQuery();

                    //case ComandoUltimoId comandoUltimoId:

                    //    var valorId = cmd.ExecuteScalar();
                    //    var id = Convert.ToInt64(valorId);
                    //    entidadeAlterada.Entidade.Id = id;
                    //    return 0;

                    case ComandoCampoComputado comandoCampoComputado:

                        var valorComputado = cmd.ExecuteScalar();
                        var campoComputado = new CampoComputado(comandoCampoComputado.EstruturaCampo, valorComputado);
                        entidadeAlterada.AtualizarPropriedadeCampoComputado(campoComputado);
                        entidadeAlterada.CamposComputado.Add(campoComputado);
                        return 0;
                    default:

                        throw new ErroNaoSuportado("Comando não suportado");

                }
            }
        }

        #region Resultado 

        private Resultado RetornarResultado(List<EntidadeAlterada> entidadesAlterada)
        {
            if (this.OpcaoSalvar == EnumOpcaoSalvar.Salvar)
            {
                return this.RetornarResultadoSalvar(entidadesAlterada);
            }
            return this.RetornarResultadoDeletar(entidadesAlterada);
        }

        //Erro
        private Resultado RetornarResultadoErro(Exception erro)
        {
            if (this.OpcaoSalvar == EnumOpcaoSalvar.Salvar)
            {
                return this.RetornarResultadoErroSalvar(erro);
            }
            else
            {
                return this.RetornarResultadoErroDeletar(erro);
            }
        }

        //Salvar
        private ResultadoSalvar RetornarResultadoSalvar(List<EntidadeAlterada> entidadesAlterada)
        {
            var resultadoSalvar = new ResultadoSalvar
            {
                IsSucesso = true
            };

            foreach (var entidadeAlterada in entidadesAlterada)
            {
                //throw new NotImplementedException();
                var identificadorUnico = entidadeAlterada.Entidade.RetornarIdentificadorReferencia();
                var entidadeSalva = new EntidadeSalva
                {
                    IdentificadorUnicoEntidade = identificadorUnico,
                    Id = entidadeAlterada.Entidade.Id,
                    CaminhoTipoEntidadeSalva = entidadeAlterada.Entidade.__CaminhoTipo
                };

                if (entidadeAlterada.Entidade.__IsNewEntity)
                {
                    (entidadeAlterada.Entidade as IEntidadeInterna).NotifyIsNotNewEntity();
                }

                foreach (var campoComputado in entidadeAlterada.CamposComputado)
                {
                    var nomePropriedade = campoComputado.EstruturaCampo.Propriedade.Name;
                    var propriedadeComputada = new PropriedadeComputada
                    {
                        NomePropriedade = nomePropriedade,
                        Valor = campoComputado.Valor
                    };
                    entidadeSalva.PropriedadesComputada.Add(propriedadeComputada);
                }
                foreach (var propriedadeAlterada in entidadeAlterada.PropriedadesAtualizadas)
                {
                    var nomePropriedade = propriedadeAlterada.Name;
                    var valorAlterado = propriedadeAlterada.GetValue(entidadeAlterada.Entidade);
                    var propriedadeComputada = new PropriedadeComputada
                    {
                        NomePropriedade = nomePropriedade,
                        Valor = valorAlterado
                    };
                    entidadeSalva.PropriedadesComputada.Add(propriedadeComputada);
                }
                resultadoSalvar.EntidadesSalvas.Add(entidadeSalva);
            }
            return resultadoSalvar;
        }

        private ResultadoSalvar RetornarResultadoSalvarErrosValidacao(List<Snebur.Dominio.ErroValidacao> erros)
        {
            var resultado = new ResultadoSalvar
            {
                IsSucesso = false,
                ErrosValidacao = erros,
                MensagemErro = ErroValidacao.RetornarMensagemErro(erros),
                Erro = new ErroValidacao(erros)

            };
            return resultado;
        }

        private ResultadoSalvar RetornarResultadoErroSalvar(Exception erro)
        {
            var resultado = new ResultadoSalvar
            {
                IsSucesso = false,
                MensagemErro = ErroUtil.RetornarDescricaoDetalhadaErro(erro),
                Erro = erro
            };
            return resultado;
        }
        //Deletar

        private ResultadoDeletar RetornarResultadoDeletar(List<EntidadeAlterada> entidadesAlterada)
        {
            var resultado = new ResultadoDeletar();
            resultado.IsSucesso = true;
            return resultado;
        }

        private ResultadoDeletar RetornarResultadoErroDeletar(Exception erro)
        {
            var resultado = new ResultadoDeletar
            {
                IsSucesso = false,
                MensagemErro = ErroUtil.RetornarDescricaoDetalhadaErro(erro),
                Erro = erro
            };
            return resultado;
        }
        #endregion

        #region Alterações das propriedades
        private HashSet<Entidade> RetornarEntidadesAlteracaoPropriedade(HashSet<Entidade> entidades)
        {
            return new EntidadesAlteracaoPropriedade(this.Contexto).
                                                    RetornarEntidadesAlteracaoPropriedade(entidades);
        }

        private HashSet<Entidade> RetornarEntidadesAlteracaoPropriedadeGenericas(HashSet<Entidade> entidades)
        {
            return new EntidadesAlteracaoPropriedadeGenerica(this.Contexto).
                                                             RetornarEntidadesAlteracaoPropriedadeGenericas(entidades);
        }
        #endregion

        #region Métodos privados

        private List<EntidadeAlterada> RetornarEntidadesAlteradas(HashSet<Entidade> entidadesNormalizada)
        {

            var entidadesAlterada = new List<EntidadeAlterada>();
            foreach (var entidade in entidadesNormalizada)
            {
                if (entidade != null)
                {
                    var estruturaEntidade = this.Contexto.EstruturaBancoDados.EstruturasEntidade[entidade.GetType().Name];
                    var entidadeAlterada = new EntidadeAlterada(this.Contexto,
                                                                entidade,
                                                                estruturaEntidade,
                                                                this.OpcaoSalvar);
                    entidadesAlterada.Add(entidadeAlterada);
                    AtualizarValorPadrao.Atualizar(entidadeAlterada, this.Contexto);
                }
            }
            return NormalizarEntidadeAlterada.RetornarEntidadesAlteradaNormalizada(this.Contexto, entidadesAlterada);
        }

        private List<EntidadeAlterada> RetornarEntidadesAlteracoesPropriedadeGenericas(HashSet<Entidade> entidadesNormalizada)
        {
            var contextoSessaoUsuario = this.Contexto.ContextoSessaoUsuario;
            var entidadesNotificacaoPropriedadeAlteradaGenerica = this.RetornarEntidadesAlteracaoPropriedadeGenericas(entidadesNormalizada);
            entidadesNotificacaoPropriedadeAlteradaGenerica = NormalizarEntidade.RetornarEntidadesNormalizada(contextoSessaoUsuario, entidadesNotificacaoPropriedadeAlteradaGenerica);
            var entidadesAlterada = new List<EntidadeAlterada>();

            foreach (var entidade in entidadesNotificacaoPropriedadeAlteradaGenerica)
            {
                if (entidade != null)
                {
                    var estruturaEntidade = contextoSessaoUsuario.EstruturaBancoDados.EstruturasEntidade[entidade.GetType().Name];
                    var entidadeAlterada = new EntidadeAlterada(contextoSessaoUsuario,
                                                                entidade,
                                                                estruturaEntidade,
                                                                this.OpcaoSalvar);
                    entidadesAlterada.Add(entidadeAlterada);
                    AtualizarValorPadrao.Atualizar(entidadeAlterada, contextoSessaoUsuario);
                }
            }
            return NormalizarEntidadeAlterada.RetornarEntidadesAlteradaNormalizada(contextoSessaoUsuario, entidadesAlterada);
        }

        private HashSet<Entidade> RetornarEntidadesNormalizadas(HashSet<Entidade> entidades)
        {
            if (this.OpcaoSalvar == EnumOpcaoSalvar.Salvar)
            {
                var entidadesNormalizada = NormalizarEntidade.RetornarEntidadesNormalizada(this.Contexto, entidades);
                if (this.IsNotificarAlteracaoPropriedade)
                {

                    //Propriedade alteradas
                    var entidadesNotificacaoPropriedadeAlterada = this.RetornarEntidadesAlteracaoPropriedade(entidadesNormalizada);
                    entidadesNotificacaoPropriedadeAlterada = NormalizarEntidade.RetornarEntidadesNormalizada(this.Contexto, entidadesNotificacaoPropriedadeAlterada);
                    entidadesNormalizada.AddRange(entidadesNotificacaoPropriedadeAlterada);
                }
                ///AtualizarValorPadrao.Atualizar(entidades, this.Contexto);
                return entidadesNormalizada;
            }
            return entidades;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            this.FilaEntidades?.Clear();
            this.FilaAlteracoesGenericas?.Clear();
            this.EntidadesAlteradas?.Clear();
            this.EntidadesAlteradasAlteracoesGenericas?.Clear();

            this.FilaEntidades = null;
            this.FilaAlteracoesGenericas = null;
            this.EntidadesAlteradas = null;
            this.EntidadesAlteradasAlteracoesGenericas = null;
            this.Contexto = null;
        }
        #endregion
    }

    public enum EnumOpcaoSalvar
    {
        Salvar = 0,
        Deletar = 1,
        DeletarRegistro = 2
    }
}