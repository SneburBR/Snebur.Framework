﻿namespace Snebur.Dominio.Atributos;

[IgnorarAtributoTS]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class NaoMapearInternoAttribute : Attribute
{

}
