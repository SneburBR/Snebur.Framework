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
    internal class EstruturaEntidadeApelidoBase : EstruturaEntidadeApelido
    {

        internal EstruturaEntidadeApelido EstruturaEntidadeMapeadaPai { get; set; }

        internal EstruturaEntidadeApelidoBase(BaseMapeamentoConsulta mapeamentoConsutla,
                                              string apelidoEntidadeMapeada,
                                              EstruturaEntidadeApelido estruturaEntidadeMapeadaPai,
                                              EstruturaEntidade estruturaEntidade ) :
                                              base(mapeamentoConsutla,
                                                   apelidoEntidadeMapeada, 
                                                   estruturaEntidade)
        {
            this.EstruturaEntidadeMapeadaPai = estruturaEntidadeMapeadaPai;
        }

        internal string RetornarSqlUniaoEntidadeBaseBanco()
        {
            return String.Format("    {0} As {1} ON {2} = {3} ", this.CaminhoBanco, this.Apelido,
                                                                 this.EstruturaEntidadeMapeadaPai.EstruturaCampoApelidoChavePrimaria.CaminhoBanco,
                                                                 this.EstruturaCampoApelidoChavePrimaria.CaminhoBanco);
        }
    }
}