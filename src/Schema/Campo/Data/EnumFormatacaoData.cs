using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.ComponentModel;

namespace Snebur.Schema
{

    public enum EnumFormatacaoData
    {
        [Description("Desconhecido")]
        Desconhecido = 0,

        [Description("dd/mm/aaaa")]
        dd_mm_aaaa = 1,

        [Description("dd-mm-aaaa")]
        dd_t_mm_t_aaaa = 2,

        [Description("dd/mm/aa")]
        dd_mm_aa = 3,

        [Description("dd-mm-aa")]
        dd_t_mm_t_aa = 4,

        //segundos atraz  '1 minuto   '2 minutos  '1 dia   '2 dias '1 mes 2 meses '1 ano 2 anos
        [Description("TempoSemantico")]
        TempoSemantico = 5
    }
}