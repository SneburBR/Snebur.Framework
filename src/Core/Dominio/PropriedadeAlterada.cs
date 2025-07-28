
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System.Text.Json.Serialization;

namespace Snebur.Dominio
{
    [IgnorarClasseTS]
    public class PropriedadeAlterada : BaseDominio, IPropriedadeAlterada
    {
        public string NomePropriedade { get; set; } = string.Empty;
        public object? AntigoValor { get; set; }

        public object? NovoValor { get; set; }

        [IgnorarConstrutorTS]
        [JsonConstructor]
        public PropriedadeAlterada()
        {
        }

        public PropriedadeAlterada(
            string nomePropriedade,
            object? antigoValor,
            object? novoValor)
        {
            this.NomePropriedade = nomePropriedade;
            this.AntigoValor = antigoValor;
            this.NovoValor = novoValor;
        }

        internal static PropriedadeAlterada Create<T>(string nomePropriedade,
                                                     T? antigoValor,
                                                     T? novoValor,
                                                     string? nomePropriedadeEntidade,
                                                     string? nomePropriedadeTipoComplexo)
        {
            if (nomePropriedadeEntidade is not null &&
                nomePropriedadeTipoComplexo is not null)
            {
                return new PropriedadeAlteradaTipoComplexo(
                    nomePropriedade,
                    antigoValor,
                    novoValor,
                    nomePropriedadeEntidade,
                    nomePropriedadeTipoComplexo);
            }
            if (nomePropriedadeEntidade is not null ||
                nomePropriedadeTipoComplexo is not null)
            {
                throw new InvalidOperationException(
                    "Ambos nomePropriedadeEntidade e nomePropriedadeTipoComplexo devem ser nulos ou ambos devem ser preenchidos.");

            }

            return new PropriedadeAlterada(
                  nomePropriedade,
                  antigoValor,
                  novoValor);
        }

        internal static PropriedadeAlterada Create(string nomePropriedade,
                                                   long? antigoValor,
                                                   long? novoValor)
        {
            return new PropriedadeAlterada(nomePropriedade,
                                           antigoValor,
                                           novoValor);
        }
    }

    [IgnorarClasseTS]
    public class PropriedadeAlteradaTipoComplexo : PropriedadeAlterada
    {
        public string NomePropriedadeEntidade { get; set; } = string.Empty;
        public string NomePropriedadeTipoComplexo { get; set; } = string.Empty;

        [IgnorarConstrutorTS]
        [JsonConstructor]
        public PropriedadeAlteradaTipoComplexo()
        {

        }

        public PropriedadeAlteradaTipoComplexo(string nomePropriedade,
                                               object? antigoValor,
                                               object? novoValor,
                                               string nomePropriedadeEntidade,
                                               string propriedadeAlteradaTipoComplexo) :
                                               base(nomePropriedade, antigoValor, novoValor)
        {
            Guard.NotNull(nomePropriedadeEntidade);
            Guard.NotNull(propriedadeAlteradaTipoComplexo);

            this.NomePropriedadeEntidade = nomePropriedadeEntidade;
            this.NomePropriedadeTipoComplexo = propriedadeAlteradaTipoComplexo;
        }
    }
}