using Bogus;
using Bogus.Extensions.Brazil;
using Snebur.Linq;
using static Bogus.DataSets.Name;

namespace Snebur.Utilidade;

public static class DesenvolvedorUtil
{
    //private static Faker _faker;

    //private static Faker Faker = ThreadUtil.RetornarRetornarValor(ref _faker, () => new Faker("pt_BR"));

#pragma warning disable IDE0032  
    private static Random? _random;
#pragma warning restore IDE0032 
    private static Random Random
        => LazyUtil.RetornarValorLazyComBloqueio(ref _random, () => new Random());

    public static PesssoaFisicaFalsa PesssoaFisicaFalsa()
    {
        var faker = new Faker("pt_BR");
        var pessoa = faker.Person;
        var enderecoFalso = EnderecoFalso();
        var genero = pessoa.Gender == Gender.Female ? 'F' : 'M';
        var rg = Random.Next(1000000, 999999999);
        return new PesssoaFisicaFalsa
        {
            Cpf = pessoa.Cpf(),
            Email = pessoa.Email,
            Genero = genero,
            Rg = rg.ToString(),
            Nome = pessoa.FullName,
            Telefone = pessoa.Phone,
            DataNascimento = pessoa.DateOfBirth,
            EnderecoFalso = enderecoFalso
        };
    }
    public static PesssoaJuridicaFalsa PesssoaJuridicaFalsa()
    {
        var faker = new Faker("pt_BR");
        var empresa = faker.Company;
        var pessoa = faker.Person;
        var enderecoFalso = EnderecoFalso();
        var inscricaoEstudual = Random.Next(1000000, 999999999);

        return new PesssoaJuridicaFalsa
        {
            Cnpj = empresa.Cnpj(),
            Email = pessoa.Email,
            InscricaoEstadual = inscricaoEstudual.ToString(),
            Nome = pessoa.FullName,
            RazaoSocial = empresa.CompanyName(),
            Telefone = pessoa.Phone,
            EnderecoFalso = enderecoFalso
        };
    }

    public static EnderecoFalso EnderecoFalso()
    {
        var faker = new Faker("pt_BR");
        var cidade = faker.Address.City();

        var logradouro = faker.Address.StreetName();
        var estado = faker.Address.StateAbbr();
        var cep = faker.Address.ZipCode();
        var bairro = faker.Address.City();
        var numero = DesenvolvedorUtil.Random.Next(100, 10000);

        return new EnderecoFalso
        {
            Cidade = cidade,
            Logradouro = logradouro,
            Bairro = bairro,
            Numero = numero,
            Cep = cep,
            Estado = estado
        };
    }

    public static string GerarRg()
    {
        return DesenvolvedorUtil.Random.Next(10000000, 99999999).ToString();
    }
    public static string GerarInscricaoEstadual()
    {
        return DesenvolvedorUtil.Random.Next(10000000, 99999999).ToString();
    }

    public static TEnum RandowEnum<TEnum>() where TEnum : System.Enum
    {
        return EnumUtil.RetornarValoresEnum<TEnum>().Random();
    }
}

public class EnderecoFalso
{
    public required string Cidade { get; init; }
    public required string Logradouro { get; init; }
    public required string Bairro { get; init; }
    public required int Numero { get; init; }
    public required string Cep { get; init; }
    public required string Estado { get; init; }
}

public class PesssoaFisicaFalsa
{
    public required string Nome { get; init; }
    public required string Cpf { get; init; }
    public required string Rg { get; init; }
    public required string Email { get; init; }
    public required string Telefone { get; init; }
    public required Char Genero { get; init; }
    public required EnderecoFalso EnderecoFalso { get; init; }
    public required DateTime DataNascimento { get; init; }
}

public class PesssoaJuridicaFalsa
{
    public required string Nome { get; init; }
    public required string RazaoSocial { get; init; }
    public required string Cnpj { get; init; }
    public required string InscricaoEstadual { get; init; }
    public required string Email { get; init; }
    public required string Telefone { get; init; }
    public required EnderecoFalso EnderecoFalso { get; init; }
}