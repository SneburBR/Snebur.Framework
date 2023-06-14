using Snebur.AcessoDados.Estrutura;
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
        internal Queue<EntidadeAlterada> Fila { get; private set; }
        internal BaseContextoDados Contexto { get; private set; }
        internal BaseConexao Conexao { get; set; }
        internal bool IsNotificarAlteracaoPropriedade { get; }
        internal EnumOpcaoSalvar OpcaoSalvar { get; }

        private int ContadorParametro;

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
            this.Fila = new Queue<EntidadeAlterada>();
            this.Contexto = contexto;
            this.Conexao = this.Contexto.Conexao;
            this.EntidadesAlteradas = this.RetornarEntidadesAlteradas(entidades);
            this.Fila = FilaEntidadeAlterada.RetornarFila(this.EntidadesAlteradas);

            if (opcaoDeletar != EnumOpcaoSalvar.Salvar)
            {
                this.Fila = new Queue<EntidadeAlterada>(this.Fila.Reverse());
            }
        }

        internal Resultado Salvar(bool ignorarValidacao = false)
        {
            if (this.OpcaoSalvar == EnumOpcaoSalvar.Salvar && !ignorarValidacao)
            {
                var entidades = Enumerable.Reverse(this.Fila).Select(x => x.Entidade).ToList();
                var errosValidacao = ValidarEntidades.Validar(this.Contexto, entidades.ToList());
                if (errosValidacao.Count > 0)
                {
                    return this.RetornarResultadoSalvarErrosValidacao(errosValidacao);
                }
            }

            if (this.Contexto.IsExisteTransacao)
            {
                return this.SalvarTransacao();
            }
            else
            {
                return this.SalvarNormal();
            }
        }

        private Resultado SalvarNormal()
        {
            var comandosExecutados = new List<string>();
            var entidadesAltearas = new List<EntidadeAlterada>();
            var linhasAfetadas = 0;
            try
            {
                if (this.Fila.Count > 0)
                {
                    //$######### TESTE
                    foreach (var entidadeAlterada in this.Fila.ToList())
                    {
                        entidadeAlterada.Comandos = entidadeAlterada.RetornarCommandos();
                    }

                    using (var conexao = this.Conexao.RetornarNovaConexao())
                    {
                        conexao.Open();
                        try
                        {
                            using (var transacao = conexao.BeginTransaction(ConfiguracaoAcessoDados.IsolamentoLevelSalvarPadrao))
                            {
                                try
                                {
                                    while (this.Fila.Count > 0)
                                    {
                                        var entidadeAlterada = this.Fila.Dequeue();
                                        //var comandos = entidadeAlterada.RetornarCommandos();
                                        foreach (var comando in entidadeAlterada.Comandos)
                                        {
                                            try
                                            {
                                                linhasAfetadas += this.ExecutarCommando(conexao, transacao, entidadeAlterada, comando);
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
                                        entidadeAlterada.RetornarCommandos();
                                        entidadesAltearas.Add(entidadeAlterada);
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
                            conexao.Close();
                        }
                    }
                }
                return this.RetornarResultado(entidadesAltearas);

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

        private Resultado SalvarTransacao()
        {
            var comandosExecutados = new List<string>();
            var entidadesAltearas = new List<EntidadeAlterada>();
            var linhasAfetadas = 0;

            var conexao = this.Contexto.ConexaoAtual;
            var transacao = this.Contexto.TransacaoAtual;


            if (this.Fila.Count > 0)
            {

                //$######### TESTE
                foreach (var entidadeAlterada in this.Fila.ToList())
                {
                    entidadeAlterada.Comandos = entidadeAlterada.RetornarCommandos();
                }


                while (this.Fila.Count > 0)
                {
                    var entidadeAlterada = this.Fila.Dequeue();
                    //var comandos = entidadeAlterada.RetornarCommandos();
                    foreach (var comando in entidadeAlterada.Comandos)
                    {
                        try
                        {
                            linhasAfetadas += this.ExecutarCommando(conexao, transacao, entidadeAlterada, comando);
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

        internal int ExecutarCommando(DbConnection conexao, DbTransaction transacao, EntidadeAlterada entidadeAlterada, Comando comando)
        {
            using (var cmd = this.Conexao.RetornarNovoComando(comando.SqlCommando, null, conexao, transacao))
            {
                var parametros = comando.RetornarParametros();
                foreach (var parametro in parametros)
                {
                    if (!parametro.EstruturaCampo.IsValorFuncaoServidor)
                    {
                        this.ContadorParametro = +1;
                        var dbParametro = this.Conexao.RetornarNovoParametro(parametro.EstruturaCampo, parametro.Valor);
                        cmd.Parameters.Add(dbParametro);
                    }
                }

                DepuracaoUtil.EscreverSaida(this.Contexto, cmd.Parameters.OfType<DbParameter>().ToList(), cmd.CommandText);

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
        //Excluir

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

        private List<EntidadeAlterada> RetornarEntidadesAlteradas(HashSet<Entidade> entidades)
        {
            var entidadesNormalizada = this.RetornarEntidadesNormalizadas(entidades);
            var entidadesAlterada = new List<EntidadeAlterada>();
            foreach (var entidade in entidadesNormalizada)
            {
                if (entidade != null)
                {
                    var estruturaEntidade = this.Contexto.EstruturaBancoDados.EstruturasEntidade[entidade.GetType().Name];
                    var entidadeAlterada = new EntidadeAlterada(this.Contexto, entidade, estruturaEntidade, this.OpcaoSalvar);
                    entidadesAlterada.Add(entidadeAlterada);
                    AtualizarValorPadrao.Atualizar(entidadeAlterada, this.Contexto);
                }
            }
            return NormalizarEntidadeAlterada.RetornarEntidadesAlteradaNormalizada(this.Contexto, entidadesAlterada);

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


                    if (this.Contexto.EstruturaBancoDados.TipoEntidadeNotificaoPropriedadeAlteradaGenerica != null)
                    {
                        //Propriedade alteradas GENERICAS
                        var entidadesNotificacaoPropriedadeAlteradaGenerica = this.RetornarEntidadesAlteracaoPropriedadeGenericas(entidadesNormalizada);
                        entidadesNotificacaoPropriedadeAlteradaGenerica = NormalizarEntidade.RetornarEntidadesNormalizada(this.Contexto, entidadesNotificacaoPropriedadeAlteradaGenerica);
                        entidadesNormalizada.AddRange(entidadesNotificacaoPropriedadeAlteradaGenerica);
                    }
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
            this.Fila?.Clear();
            this.EntidadesAlteradas?.Clear();

            this.Fila = null;
            this.EntidadesAlteradas = null;
            this.Contexto = null;
            this.Conexao = null;
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