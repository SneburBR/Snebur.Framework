namespace Snebur.AcessoDados.Estrutura
{
    internal class NivelEstruturaEntidadeEspecializada
    {

        internal Dictionary<string, EstruturaEntidade> EstruturasEntidade { get; set; }

        internal int Nivel { get; set; }

        internal Type TipoEntidadeBase { get; set; }

        internal NivelEstruturaEntidadeEspecializada(int nivel, Type tipoEntidadeBase, Dictionary<string, EstruturaEntidade> estruturasEntidade)
        {
            this.Nivel = nivel;
            this.EstruturasEntidade = estruturasEntidade;
            this.TipoEntidadeBase = tipoEntidadeBase;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1} ({2}) - {3}", this.Nivel, this.TipoEntidadeBase.Name, this.EstruturasEntidade.Count, base.ToString());
        }
    }
}