using Bogus;
using Bogus.Extensions.Brazil;
using System;
using System.Linq;
using static Bogus.DataSets.Name;

namespace Snebur.Utilidade
{
    public static class DesenvolvedorUtil
    {
        //private static Faker _faker;

        //private static Faker Faker = ThreadUtil.RetornarRetornarValor(ref _faker, () => new Faker("pt_BR"));

        private static Random _random;
        private static Random Random => ThreadUtil.RetornarRetornarValor(ref _random, () => new Random());

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
        public string Cidade { get; internal set; }
        public string Logradouro { get; internal set; }
        public string Bairro { get; internal set; }
        public int Numero { get; internal set; }
        public string Cep { get; internal set; }
        public string Estado { get; internal set; }
    }



    public class PesssoaFisicaFalsa
    {
        public string Nome { get; internal set; }
        public string Cpf { get; internal set; }
        public string Rg { get; internal set; }
        public string Email { get; internal set; }
        public string Telefone { get; internal set; }
        public Char Genero { get; internal set; }
        public EnderecoFalso EnderecoFalso { get; set; }
        public DateTime DataNascimento { get; internal set; }
    }

    public class PesssoaJuridicaFalsa
    {
        public string Nome { get; internal set; }
        public string RazaoSocial { get; internal set; }
        public string Cnpj { get; internal set; }
        public string InscricaoEstadual { get; internal set; }
        public string Email { get; internal set; }
        public string Telefone { get; internal set; }
        public EnderecoFalso EnderecoFalso { get; set; }

    }

}
