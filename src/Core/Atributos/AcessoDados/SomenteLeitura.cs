using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class SomenteLeituraAttribute : BaseAtributoDominio
    {
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public bool IsNotificarSeguranca { get; protected set; }

        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]        
        public virtual OpcoesSomenteLeitura OpcoesSomenteLeitura => new OpcoesSomenteLeitura(true, this.IsNotificarSeguranca);
    }

    public class OpcoesSomenteLeitura
    {
        public bool IsSomenteLeitura { get; } = true;
        public bool IsNotificarSeguranca { get; private set; }
        public OpcoesSomenteLeitura(bool isSomenteLeitura, bool isNotificarSeguranca = true)
        {
            this.IsSomenteLeitura = isSomenteLeitura;
            this.IsNotificarSeguranca = isNotificarSeguranca;
        }
    }
}