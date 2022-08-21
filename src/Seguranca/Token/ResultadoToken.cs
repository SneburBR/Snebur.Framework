using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Servicos;
using Snebur.Utilidade;

namespace Snebur.Seguranca
{
    public partial class ResultadoToken
    {

        private const int TEMPO_MAXIMO_PADRAO_DIAS = 1;
        public EnumEstadoToken Estado { get; set; }
        public DateTime DataHora { get; set; }

        public ResultadoToken(Guid chave, DateTime dataHora, TimeSpan tempoExpirar, TimeSpan? tempoMaximo = null)
        {
            if (!tempoMaximo.HasValue)
            {
                tempoMaximo = TimeSpan.FromDays(TEMPO_MAXIMO_PADRAO_DIAS).Add(tempoExpirar);
            }
            this.DataHora = dataHora;
            var dataLimite = dataHora.Add(tempoExpirar);
            var agoraUtc = DateTime.UtcNow.Add(AplicacaoSnebur.Atual.DiferencaDataHoraUtcServidor);

            if (chave != Token.CHAVE)
            {
                this.Estado = EnumEstadoToken.ChaveInvalida;
            }
            else
            {
                this.Estado = agoraUtc < dataLimite ? EnumEstadoToken.Valido : EnumEstadoToken.Expirado;
                if (this.Estado == EnumEstadoToken.Valido)
                {
                    var diferenca = dataLimite - agoraUtc;
                    if (diferenca > tempoMaximo)
                    {
                        this.Estado = EnumEstadoToken.ExpiradoDataFuturaUltrapassada;
                    }
                }
            }
        }

        public EnumTipoLogSeguranca RetornarTipoLogReguranca()
        {
            switch (this.Estado)
            {
                case (EnumEstadoToken.Valido):

                    throw new Exception("Não é suportado para estado válido");

                case (EnumEstadoToken.ChaveInvalida):

                    return EnumTipoLogSeguranca.TokenChaveInvalida;

                case (EnumEstadoToken.Expirado):

                    return EnumTipoLogSeguranca.TokenExpirado;

                case (EnumEstadoToken.Invalido):

                    return EnumTipoLogSeguranca.TokenInvalido;

                case (EnumEstadoToken.ExpiradoDataFuturaUltrapassada):

                    return EnumTipoLogSeguranca.TokenExpiradoDataFuturaUltrapassada;

                default:

                    throw new Exception("Estado do token não suportado ");
            }
        }
    }

    public partial class ResultadoToken<T> : ResultadoToken where T : struct
    {
        public Tuple<T> Itens { get; set; }

        public ResultadoToken(Guid chave, DateTime dataHora, TimeSpan tempoExpirar, T item) : base(chave, dataHora, tempoExpirar)
        {
            this.Itens = new Tuple<T>(item);
        }
    }

    public partial class ResultadoToken<T1, T2> : ResultadoToken where T1 : struct
                                                                 where T2 : struct
    {
        public Tuple<T1, T2> Itens { get; set; }

        public ResultadoToken(Guid chave, DateTime dataHora, TimeSpan tempoExpirar, T1 item1, T2 item2) : base(chave, dataHora, tempoExpirar)
        {
            this.Itens = new Tuple<T1, T2>(item1, item2);
        }
    }

    public partial class ResultadoToken<T1, T2, T3> : ResultadoToken where T1 : struct
                                                             where T2 : struct
                                                             where T3 : struct
    {
        public Tuple<T1, T2, T3> Itens { get; set; }

        public ResultadoToken(Guid chave, DateTime dataHora, TimeSpan tempoExpirar, T1 item1, T2 item2, T3 item3) : base(chave, dataHora, tempoExpirar)
        {
            this.Itens = new Tuple<T1, T2, T3>(item1, item2, item3);
        }
    }

    public partial class ResultadoToken<T1, T2, T3, T4> : ResultadoToken where T1 : struct
                                                                 where T2 : struct
                                                                 where T3 : struct
                                                                 where T4 : struct
    {
        public Tuple<T1, T2, T3, T4> Itens { get; set; }

        public ResultadoToken(Guid chave, DateTime dataHora, TimeSpan tempoExpirar, T1 item1, T2 item2, T3 item3, T4 item4) : base(chave, dataHora, tempoExpirar)
        {
            this.Itens = new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }
    }
    public partial class ResultadoToken<T1, T2, T3, T4, T5> : ResultadoToken where T1 : struct
                                                                     where T2 : struct
                                                                     where T3 : struct
                                                                     where T4 : struct
                                                                     where T5 : struct
    {
        public Tuple<T1, T2, T3, T4, T5> Itens { get; set; }

        public ResultadoToken(Guid chave, DateTime dataHora, TimeSpan tempoExpirar, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) : base(chave, dataHora, tempoExpirar)
        {
            this.Itens = new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        }
    }


}
