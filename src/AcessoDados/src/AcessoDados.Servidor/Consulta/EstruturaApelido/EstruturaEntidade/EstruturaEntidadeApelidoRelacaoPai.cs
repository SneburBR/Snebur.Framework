﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class EstruturaEntidadeApelidoRelacaoPai : EstruturaEntidadeApelidoRelacao
    {

        internal EstruturaRelacao EstruturaRelacaoPai { get { return this.EstruturaRelacao; } }

        internal EstruturaCampoApelido EstruturaCampoChaveEstrangeiraMapeado { get; set; }

        internal EstruturaEntidadeApelidoRelacaoPai(BaseMapeamentoConsulta mapeamentoConsulta,
                                                    string apelidoEntidadeMapeada,EstruturaEntidade estruturaEntidade,
                                                    EstruturaRelacao estruturaRelacao,
                                                    EstruturaCampoApelido estruturaCampoChaveEstrangeiraMapeado) :
                                                    base(mapeamentoConsulta,
                                                        apelidoEntidadeMapeada, 
                                                        estruturaEntidade, 
                                                        estruturaRelacao)
        {
            this.EstruturaCampoChaveEstrangeiraMapeado = estruturaCampoChaveEstrangeiraMapeado;
        }
    }
}