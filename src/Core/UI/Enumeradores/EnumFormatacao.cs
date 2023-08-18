﻿using Snebur.Dominio.Atributos;

namespace Snebur.UI
{
    [IgnorarEnumTS]
    public enum EnumFormatacao
    {
        Nenhuma, 

        Bytes,// "bytes",

        Cep, // "cep",

        Cpf, // "cpf",

        Cnpj, // "cnpj",

        CpfCnpj, // "cpfcnpj",

        Telefone, // "telefone",

        Moeda, // "moeda",

        MoedaComSinal, // "moedacomsinal"

        Inteiro, // "inteiro",

        Decimal, // "decimal",

        Decimal1, // "decimal1",

        Decimal3, // "decimal3",

        Data, // "data",

        Hora, // "hora",

        Dimensao,

        Margem,

        DimensaoCm,

        DimensaoPixels,

        DataHora, // "datahora",

        HoraDescricao, // "datahoradescricao",

        HoraDescricaoMin, // "datahoradescricaomin",

        DataSemantica,

        DataHoraSemantica, // "datahorasemantico",

        DataSemanticaHora, // "datahorasemantico",

        SimNao, // "simnao",

        Trim, // "trim",

        TamanhoArquivo, // "tamanhoarquivo",

        Porcentagem, // "porcentagem",

        Porcentagem1, // "porcentagem1",

        Porcentagem2, // "porcentagem2",

        NaoQuebrar, // "naoquebrar",

        Pixel, // "pixel",

        //Celular , // "celular",

        Tempo, // "tempo",

        TempoSemantico, // "temposemantico",

        Centimetro, // "centimetro",

        Grau, // "grau",

        Grau1, // "grau1",

        Grau2, // "grau2",

        PrimeiraLetraMaiuscula, // "primeiraletramaiuscula",

        CaixaAlta, // "caixaalta",

        CaixaBaixa, // "caixabaixa"

        Nome,

        PositivoNegativo, //"positivonegativo",

        PositivoNegativoDecimal,//"positivonegativodecimal"

        PortentagemPositivoNegativo, //"porcentagempositivonegativo",

        Portentagem1PositivoNegativo, //"porcentagem1positivonegativo",

        Absoluto,

        EntreParentes,

        QuebrarLinhasHtml, //"quebrarlinhahtml",

        Peso, //"peso",

        Prazo, // "prazo",

        DoisPontosFinal
    }
}
