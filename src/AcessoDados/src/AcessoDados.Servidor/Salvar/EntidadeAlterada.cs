using Microsoft.VisualBasic;
using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class EntidadeAlterada
    {
        #region Propriedades

        internal Entidade Entidade { get; }
        internal bool IsImplementaIDeletado { get; set; }
        internal EnumTipoAlteracao TipoAlteracao { get; }
        internal EstruturaEntidade EstruturaEntidade { get; }
        internal Dictionary<string, RelacaoChaveEstrageniraDependente> EntidadesRelacaoChaveEstrangeiraDepedente { get; set; }
        internal List<CampoComputado> CamposComputado { get; set; }
        internal List<PropertyInfo> PropriedadesAtualizadas { get; set; }
        internal BaseContextoDados Contexto { get; }
        public List<Comando> Comandos { get; internal set; }

        internal string IdentificadorEntidade => this.Entidade.__IdentificadorEntidade;
        #endregion

        private long _idRollback;

        internal EntidadeAlterada(BaseContextoDados contexto,
                                 Entidade entidade,
                                 EstruturaEntidade estruturaEntidade,
                                 EnumOpcaoSalvar opcaoSalvar)
        {
            this._idRollback = entidade.Id;
            this.CamposComputado = new List<CampoComputado>();
            this.PropriedadesAtualizadas = new List<PropertyInfo>();
            this.Contexto = contexto;
            this.Entidade = entidade;
            this.EstruturaEntidade = estruturaEntidade;

            if (opcaoSalvar != EnumOpcaoSalvar.Salvar)
            {
                if (!(entidade.Id > 0))
                {
                    throw new ErroOperacaoInvalida("Não é possível deletar uma entidade com id 0");
                }

                if (estruturaEntidade.IsImplementaInterfaceIDeletado &&  
                    !estruturaEntidade.IsDeletarRegistro &&
                    opcaoSalvar == EnumOpcaoSalvar.Deletar)
                {

                    this.IsImplementaIDeletado = true;

                    var entidadeDeletada = (IDeletado)entidade;

                    entidadeDeletada.IsDeletado = true;
                    entidadeDeletada.SessaoUsuarioDeletado_Id = this.Contexto.SessaoUsuarioLogado.Id;
                    entidadeDeletada.SessaoUsuarioDeletado = this.Contexto.SessaoUsuarioLogado;
                    entidadeDeletada.DataHoraDeletado = this.Contexto.SqlSuporte.IsDataHoraUtc ? DateTime.UtcNow : DateTime.Now;

                    if (estruturaEntidade.IsImplementaInterfaceIAtivo)
                    {
                        var entidadeAtivo = (IAtivo)entidadeDeletada;
                        entidadeAtivo.IsAtivo = false;
                    }

                    //if (estruturaEntidade.IsImplementaInterfaceIOrdenacao)
                    //{
                    //    var entidadeOrdenada = entidade as IOrdenacao;
                    //    entidadeOrdenada.Ordenacao = Double.MaxValue;
                    //}
                    this.AtualizarValorEntidadeDeletada();

                    this.TipoAlteracao = EnumTipoAlteracao.Atualizar;
                }
                else
                {
                    this.TipoAlteracao = EnumTipoAlteracao.Deletar;
                }
            }
            else
            {
                this.TipoAlteracao = (entidade.Id == 0) ? EnumTipoAlteracao.Nova : EnumTipoAlteracao.Atualizar;
            }
        }

        private void AtualizarValorEntidadeDeletada()
        {
            foreach (var propriedade in this.EstruturaEntidade.TipoEntidade.GetProperties())
            {
                var atributo = propriedade.GetCustomAttribute<ValorDeletadoConcatenarGuidAttribute>();
                if (atributo != null)
                {
                    if (propriedade.PropertyType != typeof(string))
                    {
                        throw new Erro($"O atributo {nameof(ValorDeletadoConcatenarGuidAttribute)} é suportado apenas para propriedades do tipo string ");
                    }
                    var maximoCaracteres = propriedade.GetCustomAttribute<ValidacaoTextoTamanhoAttribute>()?.MaximumLength ?? Int32.MaxValue;
                    maximoCaracteres = Math.Min(maximoCaracteres, 255);
                    var minGuid = Guid.NewGuid().ToString().Split('-').First();
                    var valorDeletado = String.Concat(minGuid, " - deletado ", propriedade.GetValue(this.Entidade)?.ToString()).RetornarPrimeirosCaracteres(maximoCaracteres);
                    propriedade.SetValue(this.Entidade, valorDeletado);
                }

                var atributoDeletado = propriedade.GetCustomAttribute<ValorDeletadoAttribute>();
                if (atributoDeletado != null)
                {
                    propriedade.SetValue(this.Entidade, atributoDeletado.Valor  );
                }
            }
        }

        internal void AtualizarEntidadesDepedentes()
        {
            this.EntidadesRelacaoChaveEstrangeiraDepedente = this.RetornarEntidadesRelacaoChaveEstrangeiraDepedente();
        }

        public List<Comando> RetornarCommandos()
        {
            switch (this.TipoAlteracao)
            {
                case (EnumTipoAlteracao.Nova):
                case (EnumTipoAlteracao.Atualizar):

                    return this.RetornarComandosSalvar();

                case (EnumTipoAlteracao.Deletar):

                    return this.RetornarComandosDeletar();

                default:

                    throw new ErroNaoSuportado("O tipo da alteração não é suportado");
            }
        }

        #region Salvar

        private List<Comando> RetornarComandosSalvar()
        {
            var comandos = new List<Comando>();
            var estruturasEntidade = this.RetornarEstruturasEntidadeSalvar();
            foreach (var estruturaEntidade in estruturasEntidade)
            {
                switch (this.TipoAlteracao)
                {
                    case EnumTipoAlteracao.Nova:

                        comandos.Add(new ComandoInsert(this, estruturaEntidade, estruturaEntidade.IsChavePrimariaAutoIncrimento));

                        //if (estruturaEntidade.IsChavePrimariaAutoIncrimento)
                        //{
                        //    comandos.Add(new ComandoUltimoId(this, this.EstruturaEntidade));
                        //}
                        break;

                    case EnumTipoAlteracao.Atualizar:

                        
                        var comandoUpdate = new ComandoUpdate(this,  estruturaEntidade);
                        if (comandoUpdate.ExisteAtualizacao)
                        {
                            comandos.Add(comandoUpdate);
                            this.AnalisarCamposComputados(estruturaEntidade, comandoUpdate);
                        }
                        break;

                    case EnumTipoAlteracao.Deletar:

                        throw new ErroOperacaoInvalida("Operação invalida");

                    default:

                        throw new Erro("O tipo de alteração não é suportado");
                }
                if (this.TipoAlteracao == EnumTipoAlteracao.Nova)
                {
                    foreach (var estruturaCampo in estruturaEntidade.EstruturasCamposComputadoBanco)
                    {
                        comandos.Add(new ComandoCampoComputado(this, estruturaEntidade, estruturaCampo));
                    }
                }
            }
            return comandos;
        }

        private void AnalisarCamposComputados(EstruturaEntidade estruturaEntidade, ComandoUpdate comandoUpdate)
        {
            foreach (var estruturaCampo in estruturaEntidade.EstruturasCamposComputadoServico)
            {
                if (comandoUpdate.PropriedadesAlterada.ContainsKey(estruturaCampo.Propriedade.Name))
                {
                    var valor = estruturaCampo.Propriedade.GetValue(this.Entidade);
                    this.CamposComputado.Add(new CampoComputado(estruturaCampo, valor));
                }
            }
        }

        private List<EstruturaEntidade> RetornarEstruturasEntidadeSalvar()
        {
            var estruturas = this.RetornarEstruturasEntidade();
            estruturas.Reverse();
            return estruturas;
        }
        #endregion

        #region Deletar

        private List<Comando> RetornarComandosDeletar()
        {
            var comandos = new List<Comando>();
            var estruturasEntidade = this.RetornarEstruturasEntidade();
            foreach (var estruturaEntidade in estruturasEntidade)
            {
                var comando = new ComandoDelete(this, estruturaEntidade);
                if (!String.IsNullOrWhiteSpace(comando.SqlCommando))
                {
                    comandos.Add(comando);
                }
            }
            return comandos;
        }
        #endregion

        private List<EstruturaEntidade> RetornarEstruturasEntidade()
        {
            var estruturasEntidade = new List<EstruturaEntidade>();
            estruturasEntidade.Add(this.EstruturaEntidade);

            var estruturaAtual = this.EstruturaEntidade;
            while (estruturaAtual.EstruturaEntidadeBase != null)
            {
                estruturasEntidade.Add(estruturaAtual.EstruturaEntidadeBase);
                estruturaAtual = estruturaAtual.EstruturaEntidadeBase;
            }
            return estruturasEntidade;
        }

        #region Métodos privados

        private Dictionary<string, RelacaoChaveEstrageniraDependente> RetornarEntidadesRelacaoChaveEstrangeiraDepedente()
        {
            var relacoesDepedente = new Dictionary<string, RelacaoChaveEstrageniraDependente>();
            var estruturasRelacaoChaveEstrangeira = this.EstruturaEntidade.TodasRelacoesChaveEstrangeira();

            var estruturas = new List<EstruturaRelacao>();
            foreach (var estruturaRelacaoChaveEstrangeira in estruturasRelacaoChaveEstrangeira)
            {
                var entidadeRelacao = (Entidade)estruturaRelacaoChaveEstrangeira.Propriedade.GetValue(this.Entidade);
                if (entidadeRelacao != null && !this.IsIgnorarRelacaoDepedenta(entidadeRelacao))
                {
                    if (!relacoesDepedente.ContainsKey(entidadeRelacao.__IdentificadorEntidade))
                    {
                        relacoesDepedente.Add(entidadeRelacao.__IdentificadorEntidade, new RelacaoChaveEstrageniraDependente(entidadeRelacao, estruturaRelacaoChaveEstrangeira));
                    }
                }
            }
            return relacoesDepedente;
        }

        private bool IsIgnorarRelacaoDepedenta(Entidade entidadeRelacao)
        {
            var estruturaEntidadeRelacao = this.Contexto.EstruturaBancoDados.EstruturasEntidade[entidadeRelacao.GetType().Name];
            return estruturaEntidadeRelacao.IsImplementaInterfaceISessaoUsuario;
        }

        internal void AtualizarPropriedadeCampoComputado(CampoComputado campoComputado)
        {
            var propriedade = campoComputado.EstruturaCampo.Propriedade;
            propriedade.SetValue(this.Entidade, campoComputado.Valor);
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", base.ToString(), this.Entidade.ToString());
        }


        internal void SetId(long id)
        {
            this.Entidade.Id = id;
        }
        internal void Rollback()
        {
            this.Entidade.Id = this._idRollback;
        }
        #endregion
    }
}