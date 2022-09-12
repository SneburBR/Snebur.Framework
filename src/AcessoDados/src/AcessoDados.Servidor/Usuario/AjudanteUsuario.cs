using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Seguranca;
using Snebur.Servicos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Snebur.AcessoDados
{
    public partial class CacheSessaoUsuario
    {
        private partial class AjudanteSessaoUsuarioInterno : IDisposable
        {
            private IUsuario _usuarioAnonimo;

            #region Propriedades privadas

            private BaseContextoDados Contexto { get; set; }

            private IContextoDadosSemNotificar ContextoSalvar => this.Contexto as IContextoDadosSemNotificar;

            private BaseConexao Conexao { get; set; }

            private Type TipoUsuario { get; set; }

            private Type TipoSessaoUsuario { get; set; }

            private Type TipoIpInformacao { get; set; }


            private List<IUsuario> Usuarios { get; set; }

            #endregion

            #region Construtor

            private AjudanteSessaoUsuarioInterno(BaseContextoDados contexto, List<IUsuario> usuariosSistema)
            {
                this.Contexto = contexto;
                this.Conexao = this.Contexto.Conexao;
                this.Usuarios = usuariosSistema;
                this.TipoUsuario = this.Contexto.EstruturaBancoDados.TipoUsuario;
                this.TipoSessaoUsuario = this.Contexto.EstruturaBancoDados.TipoSessaoUsuario;
                this.TipoIpInformacao = this.Contexto.EstruturaBancoDados.TipoIpInformacao;

                //this.PropriedadeIdentificadorUsuario = this.RetornarPropriedadeIdentificadorUsuario();
                //this.PropriedadeIdentificadorSessaoUsuario = this.RetornarPropriedadeIdentificadorSessaoUsuario();
                //this.PropriedadeSenha = this.RetornarPropriedadeSenha();
                //this.PropriedadeRelacaoUsuario = this.RetornarPropriedadeRelacaoUsuario();
                //this.PropriedadeIp = this.RetornarPropriedadeIp();

                this.SalvarUsuariosSistemas();
            }

            #endregion

            internal IUsuario UsuarioAnonimo
            {
                get
                {
                    if (this._usuarioAnonimo == null)
                    {
                        this._usuarioAnonimo = this.RetornarUsuario(CredencialAnonimo.Anonimo);
                    }
                    return this._usuarioAnonimo;
                }
            }

            #region Métodos publicos

            internal EnumEstadoSessaoUsuario RetornarEstadoSessaoUsuario(Guid identificadorSessaoUsuario)
            {
                var consulta = this.Contexto.RetornarConsulta<ISessaoUsuario>(this.TipoSessaoUsuario);
                consulta.Where(x => x.IdentificadorSessaoUsuario == identificadorSessaoUsuario);
                consulta.AbrirPropriedade(x => x.Estado);

                var sessaoUsuario = consulta.SingleOrDefault();
                if (sessaoUsuario != null)
                {
                    return sessaoUsuario.Estado;
                }
                return EnumEstadoSessaoUsuario.IdentificadorSessaoUsuarioInexistente;
            }

            internal bool IsValidarCredencialSessaoUsuario(ISessaoUsuario sessaoUsuario, Credencial credencial)
            {
                var usuario = this.RetornarUsuario(credencial);
                if (usuario != null)
                {
                    return true;
                }
                sessaoUsuario.Estado = EnumEstadoSessaoUsuario.SenhaAlterada;
                this.ContextoSalvar.SalvarInternoSemNotificacao(sessaoUsuario);
                return false;
            }

            internal void NotificarSessaoUsuarioAtiva(IUsuario usuario, ISessaoUsuario sessaoUsuario)
            {
                //var usuarioClone = usuario.CloneSomenteId<IUsuario>();
                //var sessaoUsuarioClone = sessaoUsuario.CloneSomenteId<ISessaoUsuario>();
                var nowUtc = DateTime.UtcNow;
                usuario.DataHoraUltimoAcesso = nowUtc;
                sessaoUsuario.DataHoraUltimoAcesso = nowUtc;
                sessaoUsuario.Estado = EnumEstadoSessaoUsuario.Ativo;
                this.ContextoSalvar.SalvarInternoSemNotificacao(new IEntidade[] { usuario, sessaoUsuario }, false);
            }


            internal IUsuario RetornarUsuario(Credencial credencial)
            {
                var identificadorUsuario = credencial.IdentificadorUsuario;

                var consultaUsuario = this.Contexto.RetornarConsulta<IUsuario>(this.TipoUsuario);
                consultaUsuario = consultaUsuario.Where(x => x.IdentificadorUsuario == identificadorUsuario);

                var usuario = consultaUsuario.SingleOrDefault();
                if (usuario != null && !credencial.Validar(usuario))
                {
                    return null;
                }
                return usuario;
            }

            internal ISessaoUsuario RetornarSessaoUsuario(IUsuario usuario, Guid identificadorSessaoUsuario, InformacaoSessaoUsuario informacaoSessaoUsuario)
            {
                if (Guid.Empty == identificadorSessaoUsuario)
                {
                    throw new ErroSessaoUsuarioInvalida("O identificador da sessão do usuário não foi definido");
                    //throw new ErroNaoDefinido(String.Format("O identificador da sessão do usuario não foi definido"));
                }

                var consulta = this.Contexto.RetornarConsulta<ISessaoUsuario>(this.TipoSessaoUsuario);
                consulta.Where(x => x.IdentificadorSessaoUsuario == identificadorSessaoUsuario);

                consulta.AbrirPropriedade(x => x.IdentificadorSessaoUsuario).
                         AbrirPropriedade(x => x.Estado).
                         AbrirPropriedade(x => x.EstadoServicoArquivo).
                         AbrirPropriedade(x => x.Usuario_Id).
                         //AbrirPropriedade(x => x.DataHoraInicio).
                         AbrirPropriedade(x => x.DataHoraUltimoAcesso);


                //consulta.AbrirRelacao(x => x.Usuario);
                //var consulta = this.Contexto.RetornarEstruturaConsulta(this.TipoSessaoUsuario);
                //consulta.FiltroGrupoE.Filtros.Add(new FiltroPropriedade(this.PropriedadeIdentificadorSessaoUsuario, EnumOperadorFiltro.Igual, informacaoSessaoUsuario.IdentificadorSessaoUsuario));
                //consulta.RelacoesAberta.Add(this.PropriedadeRelacaoUsuario.Name, new RelacaoAbertaEntidade(this.PropriedadeRelacaoUsuario));
                //var sessaoUsuario = (ISessaoUsuario)this.Contexto.RetornarResultadoConsultaInterno(consulta).Entidades.SingleOrDefault();

                var sessaoUsuario = consulta.SingleOrDefault();
                if (sessaoUsuario == null)
                {
                    if (informacaoSessaoUsuario == null)
                    {
                        throw new ErroSessaoUsuarioInvalida("A informação da sessão do usuário é requerido para criar uma nova sessão");
                        //throw new ErroNaoDefinido(String.Format("A informação da sessão do usuario é requerido para criar uma nova sessão"));
                    }

                    sessaoUsuario = this.RetornarNovaSessaoUsuario(usuario, informacaoSessaoUsuario);
                }
                else
                {

                    if (this.IsSessaoUsuarioDiferente(sessaoUsuario, usuario))
                    {
                        sessaoUsuario.Estado = EnumEstadoSessaoUsuario.UsuarioDiferente;

                        var cloneSessao = (sessaoUsuario as Entidade).CloneSomenteId<Entidade>() as ISessaoUsuario;
                        cloneSessao.Estado = EnumEstadoSessaoUsuario.UsuarioDiferente;

                        ((IContextoDadosSemNotificar)this.Contexto).SalvarInternoSemNotificacao(cloneSessao);

                        var mensagem = $"O usuário {usuario.Nome} ({usuario.Id}) não pertence da sessão do usuário do identificador" +
                                       $" {informacaoSessaoUsuario.IdentificadorSessaoUsuario} ";

                        LogUtil.SegurancaAsync(mensagem, EnumTipoLogSeguranca.UsuarioDiferenteSessao);

                        throw new ErroSessaoUsuarioInvalida(mensagem);

                    }
                }
                if (sessaoUsuario.Estado == EnumEstadoSessaoUsuario.Nova)
                {
                    sessaoUsuario.Estado = EnumEstadoSessaoUsuario.Ativo;
                }
                return sessaoUsuario;
            }

            private bool IsSessaoUsuarioDiferente(ISessaoUsuario sessaoUsuario,
                                                  IUsuario usuario)
            {

                if (sessaoUsuario.Usuario_Id != usuario.Id)
                {
                    var credenciasGlobal = this.Contexto.RetornarCredenciaisGlobais();
                    var credencialUsuario = usuario.RetornarCredencial();

                    if (usuario.IsAnonimo || credenciasGlobal.Contains(credencialUsuario))
                    {
                        return false;
                    }


                    var usuarioSessao = this.Contexto.RetornarConsulta<IUsuario>(this.TipoUsuario).
                                          Where(x => x.Id == sessaoUsuario.Usuario_Id).
                                          AbrirPropriedade(x => x.Nome).
                                          AbrirPropriedade(x => x.IdentificadorUsuario).
                                          AbrirPropriedade(x => x.Senha).Single();

                    var credencialUsuarioSessao = usuarioSessao.RetornarCredencial();
                    if (usuarioSessao.IsAnonimo || credenciasGlobal.Contains(credencialUsuarioSessao))
                    {
                        return false;
                    }
                    return true;
                }
                return false;


            }

            private ISessaoUsuario RetornarSessaoUsuario(Guid identificadorSessaoUsuario)
            {
                var sessaoUsuario = this.Contexto.RetornarConsulta<ISessaoUsuario>(this.TipoSessaoUsuario).
                                                  Where(x => x.IdentificadorSessaoUsuario == identificadorSessaoUsuario).
                                                  AbrirRelacao(x => x.Usuario).SingleOrDefault();

                if (sessaoUsuario == null)
                {
                    throw new Erro(String.Format("Não foi encontrado a sessão usuario do identificador {0}", identificadorSessaoUsuario.ToString()));
                }
                sessaoUsuario.DataHoraUltimoAcesso = this.Contexto.RetornarDataHora();
                return sessaoUsuario;
            }

            private IIPInformacao RetornarIpInformacao(int tentativa = 0)
            {
                var ip = IpUtil.RetornarIpDaRequisicao();

                //if (!ValidacaoUtil.IsIp(ip))
                //{
                //    throw new Erro($"O {ip} não é valido");
                //}

                var ipInformacao = this.RetornarIpInformacao(ip);
                if (ipInformacao != null)
                {
                    return ipInformacao;
                }

                var novoIpInformacao = (IIPInformacao)Activator.CreateInstance(this.TipoIpInformacao);
                try
                {
                    var dadosIpInformacao = IpUtil.RetornarIPInformacao(ip);
                    if (dadosIpInformacao.IP != ip)
                    {
                        ipInformacao = this.RetornarIpInformacao(dadosIpInformacao.IP);
                        if (ipInformacao != null)
                        {
                            return ipInformacao;
                        }
                    }
                    AutoMapearUtil.Mapear(dadosIpInformacao, novoIpInformacao);
                    this.Contexto.Salvar(novoIpInformacao as Entidade);
                    return novoIpInformacao;
                }
                catch (Exception)
                {
                    if (tentativa > 2)
                    {
                        throw;
                    }
                    Thread.Sleep(200);
                    return this.RetornarIpInformacao(tentativa += 1);
                }

            }

            private IIPInformacao RetornarIpInformacao(string ip)
            {
                if (String.IsNullOrEmpty(ip))
                {
                    return null;
                }

                var consultaIpInformacao = this.Contexto.RetornarConsulta<IIPInformacaoEntidade>(this.TipoIpInformacao);
                consultaIpInformacao = consultaIpInformacao.Where(x => x.IP == ip);

                var ipInformacao = consultaIpInformacao.SingleOrDefault();
                if (ipInformacao != null)
                {
                    return (IIPInformacao)ipInformacao;
                }
                return null;
            }

            #endregion

            #region Métodos privados

            private void SalvarUsuariosSistemas()
            {
                var usuariosSalvar = new List<Entidade>();
                foreach (var usuario in this.Usuarios)
                {
                    var consulta = this.Contexto.RetornarConsulta<IUsuario>(this.TipoUsuario);
                    consulta = consulta.Where(x => x.IdentificadorUsuario == usuario.IdentificadorUsuario);

                    var existeUsuario = consulta.SingleOrDefault() != null;
                    if (!existeUsuario)
                    {
                        usuariosSalvar.Add((Entidade)usuario);
                    }
                }
                if (usuariosSalvar.Count > 0)
                {
                    var entidades = usuariosSalvar.ToArray<IEntidade>();
                    (this.Contexto as IContextoDadosSemNotificar).SalvarInternoSemNotificacao(entidades, true);
                }
            }

            private ISessaoUsuario RetornarNovaSessaoUsuario(IUsuario usuario,
                                                             InformacaoSessaoUsuario informacaoSessaoUsuario)
            {
                var sessaoUsuario = (ISessaoUsuario)Activator.CreateInstance(this.TipoSessaoUsuario);
                sessaoUsuario.IdentificadorSessaoUsuario = informacaoSessaoUsuario.IdentificadorSessaoUsuario;
                sessaoUsuario.IdentificadorAplicacao = informacaoSessaoUsuario.IdentificadorAplicacao;

                if (String.IsNullOrWhiteSpace(informacaoSessaoUsuario.IdentificadorAplicacao))
                {
                    throw new ErroSessaoUsuarioExpirada(EnumEstadoSessaoUsuario.Cancelada,
                        sessaoUsuario.IdentificadorSessaoUsuario,
                        $"A sessão foi finalizada para ela ainda não existe o identificador da sessão {sessaoUsuario.IdentificadorSessaoUsuario}, " +
                        $"e não possui informações suficiente para inicializar uma nova sessão");
                }


                var ipInformacao = this.RetornarIpInformacao();

                sessaoUsuario.Usuario = usuario;
                sessaoUsuario.Estado = EnumEstadoSessaoUsuario.Nova;

                AutoMapearUtil.Mapear(informacaoSessaoUsuario, sessaoUsuario);
                sessaoUsuario.IdentificadorProprietario = this.Contexto.IdentificadorProprietario;
                sessaoUsuario.IPInformacao = ipInformacao;
                sessaoUsuario.IP = ipInformacao.IP;


                (this.Contexto as IContextoDadosSemNotificar).SalvarInternoSemNotificacao((Entidade)sessaoUsuario);
                return sessaoUsuario;
            }

            private PropertyInfo RetornarPropriedadeIdentificadorUsuario()
            {
                var nomePropriedade = ReflexaoUtil.RetornarNomePropriedade<IUsuario>(x => x.IdentificadorUsuario);
                return ReflexaoUtil.RetornarPropriedade(this.TipoUsuario, nomePropriedade, typeof(PropriedadeIdentificadorUsuarioAttribute));
            }

            private PropertyInfo RetornarPropriedadeSenha()
            {
                var nomePropriedade = ReflexaoUtil.RetornarNomePropriedade<IUsuario>(x => x.Senha);
                return ReflexaoUtil.RetornarPropriedade(this.TipoUsuario, nomePropriedade, typeof(PropriedadeSenhaAttribute));
            }

            private PropertyInfo RetornarPropriedadeIdentificadorSessaoUsuario()
            {
                var nomePropriedade = ReflexaoUtil.RetornarNomePropriedade<IIdentificadorSessaoUsuario>(x => x.IdentificadorSessaoUsuario);
                return ReflexaoUtil.RetornarPropriedade(this.TipoSessaoUsuario, nomePropriedade, false);
            }

            private PropertyInfo RetornarPropriedadeIp()
            {
                var nomePropriedade = ReflexaoUtil.RetornarNomePropriedade<IIPInformacao>(x => x.IP);
                return ReflexaoUtil.RetornarPropriedade(this.TipoIpInformacao, nomePropriedade, false);
            }

            private PropertyInfo RetornarPropriedadeRelacaoUsuario()
            {
                var propriedades = ReflexaoUtil.RetornarPropriedades(this.TipoSessaoUsuario);
                var propriedadesRelacaoUsuario = propriedades.Where(x => ReflexaoUtil.TipoIgualOuHerda(x.PropertyType, this.TipoUsuario));
                if (propriedadesRelacaoUsuario.Count() == 0)
                {
                    throw new Erro(String.Format("Nenhum propriedade relacao usuario foi encontrado na entidade Sessão do usuario {0]", this.TipoSessaoUsuario.Name));
                }
                if (propriedadesRelacaoUsuario.Count() > 1)
                {
                    throw new Erro(String.Format("Mais de um propriedade relacao usuario foi encontrado na entidade Sessão do usuario {0]", this.TipoSessaoUsuario.Name));
                }
                return propriedadesRelacaoUsuario.Single();
            }
            #endregion

            #region IDisposable

            public void Dispose()
            {
            }
            #endregion
        }
    }
}