﻿using Snebur.Dominio.Atributos;
using System.Reflection;

namespace Snebur.AcessoDados
{
    public class RelacaoAbertaColecao : BaseRelacaoAberta
    {
        #region Campos Privados

        #endregion

        public EstruturaConsulta EstruturaConsulta { get; set; } = new EstruturaConsulta();

        public RelacaoAbertaColecao() : base()
        {
        }
        [IgnorarConstrutorTS]
        public RelacaoAbertaColecao(PropertyInfo propriedade) : base(propriedade)
        {
        }
    }
}