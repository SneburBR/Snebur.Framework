using System;

namespace Snebur.Seguranca
{
    public partial class ResultadoToken
    {
        protected ResultadoToken()
        {
            this.DataHora = DateTime.UtcNow;
        }

        public static ResultadoToken Invalido
        {
            get
            {
                return new ResultadoToken
                {
                    Estado = EnumStatusToken.Invalido
                };
            }
        }
    }

    public partial class ResultadoToken<T> : ResultadoToken where T : struct
    {

        protected ResultadoToken() : base()
        {

        }
        public new static ResultadoToken<T> Invalido
        {
            get
            {
                return new ResultadoToken<T>
                {
                    Estado = EnumStatusToken.Invalido
                };
            }
        }
    }

    public partial class ResultadoToken<T1, T2> : ResultadoToken where T1 : struct
                                                                 where T2 : struct
    {
        protected ResultadoToken() : base()
        {

        }
        public new static ResultadoToken<T1, T2> Invalido
        {
            get
            {
                return new ResultadoToken<T1, T2>
                {
                    Estado = EnumStatusToken.Invalido
                };
            }
        }
    }

    public partial class ResultadoToken<T1, T2, T3> : ResultadoToken where T1 : struct
                                                             where T2 : struct
                                                             where T3 : struct
    {
        protected ResultadoToken() : base()
        {

        }
        public new static ResultadoToken<T1, T2, T3> Invalido
        {
            get
            {
                return new ResultadoToken<T1, T2, T3>
                {
                    Estado = EnumStatusToken.Invalido
                };
            }
        }

    }

    public partial class ResultadoToken<T1, T2, T3, T4> : ResultadoToken where T1 : struct
                                                                 where T2 : struct
                                                                 where T3 : struct
                                                                 where T4 : struct
    {
        protected ResultadoToken() : base()
        {

        }
        public new static ResultadoToken<T1, T2, T3, T4> Invalido
        {
            get
            {
                return new ResultadoToken<T1, T2, T3, T4>
                {
                    Estado = EnumStatusToken.Invalido
                };
            }
        }
    }

    public partial class ResultadoToken<T1, T2, T3, T4, T5> : ResultadoToken where T1 : struct
                                                                             where T2 : struct
                                                                             where T3 : struct
                                                                             where T4 : struct
                                                                             where T5 : struct

    {
        protected ResultadoToken() : base()
        {

        }
        public new static ResultadoToken<T1, T2, T3, T4, T5> Invalido
        {
            get
            {
                return new ResultadoToken<T1, T2, T3, T4, T5>
                {
                    Estado = EnumStatusToken.Invalido
                };
            }
        }
    }

}
