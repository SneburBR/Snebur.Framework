using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RelacaoPaiAttribute : RelacaoChaveEstrangeiraAttribute, IIgnorarAlerta
    {
        public bool IgnorarAlerta { get; set; }

        public EnumTipoExclusaoRelacao TipoExclusao { get; set; } = EnumTipoExclusaoRelacao.NaoExcluir;

        public RelacaoPaiAttribute(EnumTipoExclusaoRelacao tipoExclusao = EnumTipoExclusaoRelacao.NaoExcluir)
        {
            this.TipoExclusao = tipoExclusao;
        }
    }
}