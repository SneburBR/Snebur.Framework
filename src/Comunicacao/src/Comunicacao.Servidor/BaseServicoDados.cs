using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using Snebur.Utilidades;

namespace Snebur.Json.Servidor
{

    public abstract class BaseServicoDados : BaseServicoJson
    {



        #region " Consultar "

        public virtual ResultadoConsulta Consultar(Consulta consulta)
        {
            throw new NotImplementedException();
             /*
            using (dynamic contexto = this.RetornarNovoContexto())
            {

              
               
                dynamic tipoConsulta; // = contexto.TiposBasesNegocio.Where(x => x.Name == consulta.NomeTipoBaseNegocio).SingleOrDefault();
                if ((tipoConsulta == null))
                {
                    throw new Exception(string.Format("O tipo da consultado não foi encontrado '{0}'", consulta.NomeTipoBaseNegocio));
                }

                dynamic query = contexto.RetornarConsultaNoTracking<BaseNegocio>(tipoConsulta, consulta.RelacoesAberta);

                //Filtros

                if (consulta.Filtros.Count > 0)
                {
                    using (ConsultaLinqWhereStringConstrutor construtorLinq = new ConsultaLinqWhereStringConstrutor(consulta))
                    {
                        var queryString = construtorLinq.RetornarLinqWhereString();
                        var parametros = construtorLinq.RetornarValoresParametros();
                        query = query.Where(queryString, parametros);
                    }

                }

                //ISincronizacao
                if (!consulta.MostrarDeletados && ReflectionUtils.TipoImplementaInterface(tipoConsulta, typeof(ISincronizacao)))
                {
                    query = query.Where(" Deletado = False ");
                }

                //IAtivacao
                if (!consulta.MostrarInativos && ReflectionUtils.TipoImplementaInterface(tipoConsulta, typeof(IAtivacao)))
                {
                    query = query.Where(" Inativo = False ");
                }

                //'Ordenacao

                //ISequencia

                bool existeOrdenacao = !string.IsNullOrWhiteSpace(consulta.Ordenacao);

                if (existeOrdenacao)
                {
                    query = query.OrderBy(consulta.Ordenacao);
                }

                if (consulta.OrdernarSequencia && ReflectionUtils.TipoImplementaInterface(tipoConsulta, typeof(ISequencia)))
                {
                    query = query.OrderBy("Sequencia");
                    existeOrdenacao = true;
                }

                //If consulta.Ordenacoes.Count > 0 Then

                //    existeOrdenacao = True

                //    For Each ordenacao In consulta.Ordenacoes

                //        If ordenacao.Ordem = EnumOrdenacao.Crescente Then

                //            query = query.OrderBy(ordenacao.CaminhoPropriedade)

                //        ElseIf ordenacao.Ordem = EnumOrdenacao.Decrescente Then

                //            query = query.OrderBy(String.Format("{0} DESC ", ordenacao.CaminhoPropriedade))

                //        Else
                //            Throw New Exception("A Ordem não foi definida")
                //        End If

                //    Next

                //End If

                if (!existeOrdenacao)
                {
                    query = query.OrderBy("Id");
                }


                //Paginação
                PaginacaoConsulta paginacao = null;

                if (consulta.PaginacaoConsulta != null)
                {
                    paginacao = consulta.PaginacaoConsulta;
                    paginacao.TotalRegistros = query.Count();
                    paginacao.TotalPaginas = this.RetornarTotalPaginas(paginacao.TotalRegistros, paginacao.RegistrosPorPagina);


                    //'Skip
                    dynamic pular = (consulta.PaginacaoConsulta.PaginaAtual - 1) * consulta.PaginacaoConsulta.RegistrosPorPagina;
                    if ((pular > 0) && (paginacao.TotalRegistros > pular))
                    {
                        query = query.Skip(pular);
                    }

                    //'Take
                    query = query.Take(consulta.PaginacaoConsulta.RegistrosPorPagina);

                }

                //Resposta
                List<BaseNegocio> basesNegocio = query.ToList();
                ResultadoConsulta resultadoConsulta = new ResultadoConsulta();
                resultadoConsulta.BasesNegocio = basesNegocio;
                resultadoConsulta.PaginacaoConsulta = paginacao;
                return resultadoConsulta;

            }
            */
        }

        private int RetornarTotalPaginas(int totalRegistro, int registroPorPagina)
        {
            var totalPaginas = Math.Truncate(Convert.ToDecimal(totalRegistro / registroPorPagina));
            if ((totalRegistro % registroPorPagina) > 0)
            {
                totalPaginas += 1;
            }
            return (int)totalPaginas;
        }


        #endregion

        #region " Salvar "

        public virtual ResultadoSalvar Salvar(List<BaseEntidade> basesNegocio, bool naoEntrarArvore)
        {


            try
            {
                using (dynamic contexto = this.RetornarNovoContexto())
                {
                    contexto.Salvar(basesNegocio);
                }

                ResultadoSalvar resultado = default(ResultadoSalvar);
                using (var c = new ResultadoSalvarConstrutor(basesNegocio))
                {
                    resultado = c.RetoranrResultadoSalvar();
                }
                return resultado;
            }
            catch (Exception ex)
            {
                ResultadoSalvar resultado = new ResultadoSalvar();
                resultado.Sucesso = false;
                resultado.MensagemErro = ex.Message;
                ErroUtils.Notificar(ex);
                return resultado;
            }
        }

        #endregion

        #region " Excluir "

        public virtual ResultadoExcluir Excluir(BaseEntidade entidade, string relacoesExcluir, string relacoesNn)
        {


            try
            {
                using (dynamic contexto = this.RetornarNovoContexto())
                {
                    contexto.Excluir(entidade, relacoesExcluir, relacoesNn);
                    contexto.SaveChanges();
                }

                ResultadoExcluir r = new ResultadoExcluir();
                r.Sucesso = true;
                return r;


            }
            catch (Exception ex)
            {
                ResultadoExcluir r = new ResultadoExcluir();
                r.Sucesso = false;
                r.MensagemErro = ex.Message;
                ErroUtils.Notificar(ex);
                return r;
            }

        }


        #endregion

        public object RetornarNovoContexto()
        {

            throw new NotImplementedException();

            //BaseContexto contexto = this.RetornarInstanciaContexto();

            //if (contexto.Usuario == null)
            //{
            //    //Isso sera útil, quando sistema contexto possuir um Usuario
            //    //Throw New Exception("O Usuario não foi definidio")

            //}
            //return contexto;

        }

        protected abstract object RetornarInstanciaContexto();

        //Protected MustOverride Function RetornarTipoUsuario() As Type



    }

}
