using Newtonsoft.Json.Linq;
using Snebur.AcessoDados.Estrutura;
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
            //private BaseContextoDados _contexto;
            private IUsuario _usuarioAnonimo;

            #region Propriedades privadas

            //internal BaseContextoDados Contexto
            //{
            //    get
            //    {
            //        if (this._contexto.IsDispensado)
            //        {
            //            throw new Exception($"O contexto de dados {this._contexto} foi dispensado");
            //        }
            //        return this._contexto;
            //    }
            //    private set
            //    {
            //        if (value.IsDispensado)
            //        {
            //            throw new Exception($"O contexto de dados {value} foi dispensado");
            //        }
            //        this._contexto = value;
            //    }
            //}

            private Type TipoUsuario { get; set; }

            private Type TipoSessaoUsuario { get; set; }

            private Type TipoIpInformacao { get; set; }

            private List<IUsuario> Usuarios { get; set; }
            internal IUsuario UsuarioAnonimo { get; }

            #endregion

            #region Construtor

            private AjudanteSessaoUsuarioInterno(BaseContextoDados contexto,
                                                 EstruturaBancoDados estruturaBancoDados,
                                                 List<IUsuario> usuariosSistema)
            {
                this.Usuarios = usuariosSistema;
                this.TipoUsuario = estruturaBancoDados.TipoUsuario;
                this.TipoSessaoUsuario = estruturaBancoDados.TipoSessaoUsuario;
                this.TipoIpInformacao = estruturaBancoDados.TipoIpInformacao;
                this._usuarioAnonimo = this.RetornarUsuario(contexto, CredencialAnonimo.Anonimo);

                //this.PropriedadeIdentificadorUsuario = this.RetornarPropriedadeIdentificadorUsuario();
                //this.PropriedadeIdentificadorSessaoUsuario = this.RetornarPropriedadeIdentificadorSessaoUsuario();
                //this.PropriedadeSenha = this.RetornarPropriedadeSenha();
                //this.PropriedadeRelacaoUsuario = this.RetornarPropriedadeRelacaoUsuario();
                //this.PropriedadeIp = this.RetornarPropriedadeIp();

                this.SalvarUsuariosSistemas(contexto);
            }

            #endregion

            #region Métodos públicos

            internal EnumStatusSessaoUsuario RetornarStatusSessaoUsuario(BaseContextoDados contexto,
                                                                         Guid identificadorSessaoUsuario)
            {
                var consulta = contexto.RetornarConsulta<ISessaoUsuario>(this.TipoSessaoUsuario);
                consulta.Where(x => x.IdentificadorSessaoUsuario == identificadorSessaoUsuario)
                        .AbrirPropriedade(x => x.Status)
                        .AbrirPropriedade(x => x.IdentificadorAplicacao);
                                          

                var sessaoUsuario = consulta.SingleOrDefault();
                if (sessaoUsuario != null)
                {
                    return sessaoUsuario.Status;
                }
                return EnumStatusSessaoUsuario.IdentificadorSessaoUsuarioInexistente;
            }

            internal bool IsValidarCredencialSessaoUsuario(BaseContextoDados contexto,
                                                           ISessaoUsuario sessaoUsuario,
                                                           Credencial credencial)
            {
                var usuario = this.RetornarUsuario(contexto, credencial);
                if (usuario != null)
                {
                    return true;
                }
                sessaoUsuario.Status = EnumStatusSessaoUsuario.SenhaAlterada;
                (contexto as IContextoDadosSemNotificar).SalvarInternoSemNotificacao(sessaoUsuario);
                return false;
            }

            internal void NotificarSessaoUsuarioAtiva(BaseContextoDados contexto,
                                                      IUsuario usuario,
                                                      ISessaoUsuario sessaoUsuario)
            {
                //var usuarioClone = usuario.CloneSomenteId<IUsuario>();
                //var sessaoUsuarioClone = sessaoUsuario.CloneSomenteId<ISessaoUsuario>();
                (contexto as IContextoDadosSemNotificar).NotificarSessaoUsuarioAtiva(usuario, sessaoUsuario);

            }

            internal IUsuario RetornarUsuario(BaseContextoDados contexto,
                                              Credencial credencial)
            {
                var identificadorUsuario = credencial.IdentificadorUsuario;

                var consultaUsuario = contexto.RetornarConsulta<IUsuario>(this.TipoUsuario);
                consultaUsuario = consultaUsuario.Where(x => x.IdentificadorUsuario == identificadorUsuario);
                var usuario = consultaUsuario.SingleOrDefault();
                if (usuario != null)
                {
                    if (!credencial.Validar(usuario))
                    {
                        var msgErro = $"AjudanteSessaoUsuarioInterno.RetornarUsuario. Credencial inválida {usuario.Senha} <> {credencial.Senha}";
                        LogUtil.ErroAsync(new Exception(msgErro));
                        return null;
                    }
                    return usuario;
                }
                if (credencial is CredencialUsuario credencialUsuario)
                {
                    return AplicacaoSnebur.Atual.RetornarUsuario(contexto, credencialUsuario);
                }
                return null;

            }

            internal ISessaoUsuario RetornarSessaoUsuario(BaseContextoDados contexto,
                                                          IUsuario usuario,
                                                          Guid identificadorSessaoUsuario,
                                                          InformacaoSessao informacaoSessaoUsuario)
            {
                if (Guid.Empty == identificadorSessaoUsuario)
                {
                    throw new ErroSessaoUsuarioInvalida("O identificador da sessão do usuário não foi definido");
                    //throw new ErroNaoDefinido(String.Format("O identificador da sessão do usuario não foi definido"));
                }

                var consulta = contexto.RetornarConsulta<ISessaoUsuario>(this.TipoSessaoUsuario);
                consulta.Where(x => x.IdentificadorSessaoUsuario == identificadorSessaoUsuario);

                consulta.AbrirPropriedade(x => x.IdentificadorSessaoUsuario).
                         AbrirPropriedade(x => x.Status).
                         AbrirPropriedade(x => x.IdentificadorProprietario).
                         AbrirPropriedade(x => x.IdentificadorAplicacao).
                         AbrirPropriedade(x => x.StatusServicoArquivo).
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
                        //throw new ErroNaoDefinido(String.Format("A informação da sessão do usuário é requerido para criar uma nova sessão"));
                    }

                    sessaoUsuario = this.RetornarNovaSessaoUsuario(contexto,
                                                                   usuario,
                                                                   identificadorSessaoUsuario,
                                                                   informacaoSessaoUsuario);
                }
                else
                {

                    if (this.IsSessaoUsuarioDiferente(contexto, sessaoUsuario, usuario))
                    {
                        sessaoUsuario.Status = EnumStatusSessaoUsuario.UsuarioDiferente;

                        var cloneSessao = (sessaoUsuario as Entidade).CloneSomenteId<Entidade>(true) as ISessaoUsuario;
                        cloneSessao.Status = EnumStatusSessaoUsuario.UsuarioDiferente;

                        ((IContextoDadosSemNotificar)contexto).SalvarInternoSemNotificacao(cloneSessao);

                        var mensagem = $"O usuário {usuario.Nome} ({usuario.Id}) não pertence da sessão do usuário do identificador" +
                                       $" {identificadorSessaoUsuario} ";

                        LogUtil.SegurancaAsync(mensagem, EnumTipoLogSeguranca.UsuarioDiferenteSessao);

                        throw new ErroSessaoUsuarioInvalida(mensagem);

                    }
                }
                if (sessaoUsuario.Status == EnumStatusSessaoUsuario.Nova)
                {
                    sessaoUsuario.Status = EnumStatusSessaoUsuario.Ativo;
                }
                return sessaoUsuario;
            }

            private bool IsSessaoUsuarioDiferente(BaseContextoDados contexto,
                                                  ISessaoUsuario sessaoUsuario,
                                                  IUsuario usuario)
            {

                if (sessaoUsuario.Usuario_Id != usuario.Id)
                {
                    var credenciasGlobal = contexto.RetornarCredenciaisGlobais();
                    var credencialUsuario = usuario.RetornarCredencial();

                    if (usuario.IsAnonimo || credenciasGlobal.Contains(credencialUsuario))
                    {
                        return false;
                    }

                    var usuarioSessao = contexto.RetornarConsulta<IUsuario>(this.TipoUsuario).
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

            private ISessaoUsuario RetornarSessaoUsuario(BaseContextoDados contexto,
                                                         Guid identificadorSessaoUsuario)
            {
                var sessaoUsuario = contexto.RetornarConsulta<ISessaoUsuario>(this.TipoSessaoUsuario).
                                             Where(x => x.IdentificadorSessaoUsuario == identificadorSessaoUsuario).
                                             AbrirRelacao(x => x.Usuario).SingleOrDefault();

                if (sessaoUsuario == null)
                {
                    throw new Erro(String.Format("Não foi encontrado a sessão usuario do identificador {0}", identificadorSessaoUsuario.ToString()));
                }
                sessaoUsuario.DataHoraUltimoAcesso = contexto.RetornarDataHora();
                return sessaoUsuario;
            }

            private IIPInformacao RetornarIpInformacao(BaseContextoDados contexto, int tentativa = 0)
            {
                var ip = AplicacaoSnebur.Atual.AspNet.IpRequisicao;
                var ipInformacao = this.RetornarIpInformacao(contexto, ip);
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
                        ipInformacao = this.RetornarIpInformacao(contexto, dadosIpInformacao.IP);
                        if (ipInformacao != null)
                        {
                            return ipInformacao;
                        }
                    }
                    AutoMapearUtil.Mapear(dadosIpInformacao, novoIpInformacao);
                    contexto.Salvar(novoIpInformacao as Entidade);
                    return novoIpInformacao;
                }
                catch (Exception)
                {
                    if (tentativa > 2)
                    {
                        throw;
                    }
                    Thread.Sleep(200);
                    return this.RetornarIpInformacao(contexto, tentativa += 1);
                }

            }

            private IIPInformacao RetornarIpInformacao(BaseContextoDados contexto,
                                                       string ip)
            {
                if (String.IsNullOrEmpty(ip))
                {
                    return null;
                }

                var consultaIpInformacao = contexto.RetornarConsulta<IIPInformacaoEntidade>(this.TipoIpInformacao);
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

            private void SalvarUsuariosSistemas(BaseContextoDados contexto)
            {
                var usuariosSalvar = new List<Entidade>();
                foreach (var usuario in this.Usuarios)
                {
                    var consulta = contexto.RetornarConsulta<IUsuario>(this.TipoUsuario);
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
                    (contexto as IContextoDadosSemNotificar).SalvarInternoSemNotificacao(entidades, true);
                }
            }

            private ISessaoUsuario RetornarNovaSessaoUsuario(BaseContextoDados contexto,
                                                             IUsuario usuario,
                                                             Guid identificadorSessaoUsuario,
                                                             InformacaoSessao informacaoSessaoUsuario)
            {
                var sessaoUsuario = (ISessaoUsuario)Activator.CreateInstance(this.TipoSessaoUsuario);
                sessaoUsuario.IdentificadorSessaoUsuario = identificadorSessaoUsuario;
                sessaoUsuario.IdentificadorAplicacao = informacaoSessaoUsuario.IdentificadorAplicacao;

                if (String.IsNullOrWhiteSpace(informacaoSessaoUsuario.IdentificadorAplicacao))
                {
                    throw new ErroSessaoUsuarioExpirada(EnumStatusSessaoUsuario.Cancelada,
                        sessaoUsuario.IdentificadorSessaoUsuario,
                        $"A sessão foi finalizada para ela ainda não existe o identificador da sessão {sessaoUsuario.IdentificadorSessaoUsuario}, " +
                        $"e não possui informações suficiente para inicializar uma nova sessão ");
                }

                var ipInformacao = this.RetornarIpInformacao(contexto);

                sessaoUsuario.Usuario = usuario;
                sessaoUsuario.Status = EnumStatusSessaoUsuario.Nova;

                AutoMapearUtil.Mapear(informacaoSessaoUsuario, sessaoUsuario);
                sessaoUsuario.IdentificadorProprietario = contexto.IdentificadorProprietario;
                sessaoUsuario.IPInformacao = ipInformacao;
                sessaoUsuario.IP = ipInformacao.IP;

                (contexto as IContextoDadosSemNotificar).SalvarInternoSemNotificacao((Entidade)sessaoUsuario);
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
                var propriedadesRelacaoUsuario = propriedades.Where(x => ReflexaoUtil.IsTipoIgualOuHerda(x.PropertyType, this.TipoUsuario));
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