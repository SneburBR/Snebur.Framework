using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RelacaoUmUmAttribute : RelacaoChaveEstrangeiraAttribute, IIgnorarAlerta
    {
        public bool IgnorarAlerta { get; set; }

        public EnumTipoExclusaoRelacao TipoExclusao { get; set; } = EnumTipoExclusaoRelacao.NaoExcluir;

        public RelacaoUmUmAttribute()
        {
            throw new ErroNaoImplementado("RelacaoUmUmAttribute");
        }
    }
}