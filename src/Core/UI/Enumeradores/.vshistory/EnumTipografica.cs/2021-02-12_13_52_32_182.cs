﻿ using Snebur.Dominio.Atributos;

namespace Snebur.UI
{
    public enum EnumTipografia
    {
        [Rotulo("Vazio")]
        Vazio = BaseEnumApresentacao.Vazio,
        h1,
        h2,
        h3,
        h4,
        h5,
        h6,
        h7,
        Titulo,
        SubTitulo,
        SubTitulo2,
        Corpo,
        Corpo2,
        Descricao,
        Descricao2,
        BotaoCaixaAlta,
        LinhaCaixaAlta
    }

    public enum Peso
    {
        SuperLeve =1,
        Leve =2,
        Normal =3,
        Negrito =4,
        Pesado = 5,
        SuperPesado = 6
    }
}
