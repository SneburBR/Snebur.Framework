using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;

namespace Snebur.Json.Servidor
{

    public class ConsultaLinqWhereStringConstrutor : IDisposable
    {

        //http://dynamiclinq.azurewebsites.net/GettingStarted -

        public Consulta Consulta { get; set; }
        public Dictionary<string, object> Parametros { get; set; }
        public int ContadorParametro { get; set; }

        public ConsultaLinqWhereStringConstrutor(Consulta consulta)
        {
            this.Consulta = consulta;
        }

        public string RetornarLinqWhereString()
        {

            FiltroGropoE grupoE = new FiltroGropoE();
            grupoE.Filtros.AddRange(this.Consulta.Filtros);
            return this.RetornarFiltroGrupo(grupoE, string.Empty);

        }

        public object[] RetornarValoresParametros()
        {
            return this.Parametros.Select(x => x.Value).ToArray();
        }

        public string RetornarFiltroGrupo(BaseFiltroGrupo filtroGrupo, string caminhoRelacao)
        {

            List<string> expressoes = new List<string>();
            foreach (var filtro in filtroGrupo.Filtros)
            {

                if (filtro is BaseFiltroGrupo)
                {
                    expressoes.Add(this.RetornarFiltroGrupo((BaseFiltroGrupo)filtro, caminhoRelacao));
                }
                else if (filtro is FiltroRelecao)
                {
                    expressoes.Add(this.RetornarFiltroRelacao((FiltroRelecao)filtro, caminhoRelacao));
                }
                else if (filtro is FiltroPropriedade)
                {
                    expressoes.Add(this.RetornarFiltroPropriedade((FiltroPropriedade)filtro, caminhoRelacao));
                }
                else
                {
                    throw new Exception(string.Format(" Filtro não suportado {0}", filtro.GetType().Name));
                }
            }

            var juntar = string.Join(this.RetornarOperadorGrupo(filtroGrupo), expressoes);
            var resultado = string.Format("( {0} )", juntar);

            if (filtroGrupo is FiltroGrupoNAO)
            {
                resultado = string.Format(" NOT ( {0} ) ", resultado);

            }
            return resultado;

        }

        private string RetornarOperadorGrupo(BaseFiltroGrupo filtro)
        {

            switch (filtro.GetType().Name)
            {
                case nameof(FiltroGropoE):
                case nameof(FiltroGrupoNAO):
                    return " AND ";
                case nameof(FiltroGrupoOU):
                    return " OR ";
                default:
                    throw new Exception(string.Format("filtro grupo não suportado '{0}'", filtro.GetType().Name));
            }

        }

        private string RetornarFiltroRelacao(FiltroRelecao filtroRelacao, string caminhoRelacao)
        {
            throw new NotImplementedException();
        }

        #region " Parametros "

        private static object bloqueio = new object();

        private object RetornarValorParametro(string valor, EnumTipoPrimario tipoPrimaerioEnum)
        {


            lock (bloqueio)
            {
                switch (tipoPrimaerioEnum)
                {
                    case EnumTipoPrimario.String:
                        return string.Concat(valor, string.Empty);
                    case EnumTipoPrimario.Integer:
                        return Convert.ToInt32(valor);
                    case EnumTipoPrimario.Long:
                        return Convert.ToInt64(valor);
                    case EnumTipoPrimario.Decimal:
                        return Convert.ToDecimal(valor);
                    case EnumTipoPrimario.EnumValor:
                        return Convert.ToInt32(valor);
                    case EnumTipoPrimario.DateTime:
                        return this.RetornarDataHora(valor);
                    case EnumTipoPrimario.Boolean:
                        return Convert.ToBoolean(valor);
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        private DateTime RetornarDataHora(string datahora)
        {
            string parteData = null;
            string parteHora = null;
            var partes = datahora.Split(" ".ToArray());

            if (partes.Length == 1)
            {
                parteData = string.Format("{0}/{1}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                parteHora = "00:00.0";
            }
            else if (partes.Length == 2)
            {
                parteData = partes[0];
                parteHora = partes[1];
            }
            else
            {
                throw new Exception("");
            }

            return this.RetornarDataHora(parteData, parteHora);
        }

        private DateTime RetornarDataHora(string dataString, string horaString)
        {

            var partes = dataString.Split("/".ToCharArray());

            var dia = int.Parse(partes[0]);
            var mes = int.Parse(partes[1]);
            var ano = int.Parse(partes[2]);

            var partesHora = horaString.Split(":".ToArray());

            int segundos = 0;
            int milegundos = 0;
            dynamic horas = int.Parse(partesHora[0]);
            dynamic minutos = int.Parse(partesHora[1]);


            if (partesHora.Length > 2)
            {
                dynamic parteSegundos = partesHora[2].Split(".".ToArray());
                segundos = parteSegundos(0);

                if ((parteSegundos.Length > 1))
                {
                    milegundos = parteSegundos(1);
                }

            }

            return new DateTime(ano, mes, dia, horas, minutos, segundos, milegundos);
        }

        private int RetornarIndexParametro()
        {
            this.ContadorParametro += 1;
            return this.ContadorParametro;
        }

        #endregion

        #region " Expressao Propriedade "

        private string RetornarFiltroPropriedade(FiltroPropriedade filtro, string caminhoRelacao)
        {

            dynamic parametro = string.Format("@{0}", this.RetornarIndexParametro());
            this.Parametros.Add(parametro, this.RetornarValorParametro(filtro.Valor, filtro.TipoPropriedadeEnum));

            return string.Format(" {0}{1}({2}) ", this.RetornarCaminhoPropriedade(caminhoRelacao, filtro.NomePropriedade), this.RetornarOperadorExpressa(filtro.Operador), parametro);

        }

        private string RetornarCaminhoPropriedade(string caminhoRelacao, string nomePropriedade)
        {

            if ((caminhoRelacao.Length > 0))
            {
                throw new NotImplementedException();
            }
            else
            {
                return nomePropriedade;
            }

        }

        private string RetornarOperadorExpressa(EnumOperadorFiltro operador)
        {

            switch (operador)
            {
                case EnumOperadorFiltro.Igual:
                    return " = ";
                case EnumOperadorFiltro.Maior:
                    return " > ";
                case EnumOperadorFiltro.MaiorIgual:
                    return " >= ";
                case EnumOperadorFiltro.Menor:
                    return " < ";
                case EnumOperadorFiltro.MenorIgual:
                    return " <= ";
                case EnumOperadorFiltro.Diferente:
                    return " <> ";
                case EnumOperadorFiltro.Possui:
                    return ".Contains";
                case EnumOperadorFiltro.IniciaCom:
                    return ".StartsWith";
                case EnumOperadorFiltro.TerminarCom:
                    return ".EndsWith";
                default:
                    throw new NotImplementedException();
            }

        }

        #endregion
        
        #region IDisposable

        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Parametros.Clear();
                }
            }
            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

    }
}
