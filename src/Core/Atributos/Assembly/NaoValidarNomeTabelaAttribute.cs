﻿using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class NaoValidarNomeTabelaAttribute : Attribute
    {

    }
}
