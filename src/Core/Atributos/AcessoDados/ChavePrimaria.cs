using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ChavePrimariaAttribute : BaseAtributoDominio
    {
    }
}