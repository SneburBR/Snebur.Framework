using Snebur.Dominio.Atributos;
using System.ComponentModel;

namespace Snebur.Dominio
{
    [IgnorarGlobalizacao]
    public enum EnumTipoAplicacao
    {
        [Description("Web")]
        Web = 1,

        [Description("ApacheCordova")]
        ApacheCordova = 2,

        [Description("Servicos web")]
        DotNet_WebService = 3,

        [Description("Windows Service")]
        DotNet_WindowService = 4,

        [Description("Wpf")]
        DotNet_Wpf = 5,

        [Description("UnidadeTeste")]
        DotNet_UnitTest = 6,

        [Description("ExtensaoVisualStudio")]
        ExtensaoVisualStudio = 7,

        [Description("Desconhecido")]
        Desconhecido = 99,

        [Description("AspNet Core web service")]
        AspNetCore_WebService = 8,

        [Description("Maui .NET")]
        MaiuNet = 9
    }
}