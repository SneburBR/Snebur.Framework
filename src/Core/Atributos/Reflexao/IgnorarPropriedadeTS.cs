using System;

using System.Xml.Serialization;

namespace Snebur.Dominio.Atributos
{
    /// <summary>
    /// Foi incluído no Newtonsoft.Json para ignorar a serialização dos atributos que tiverem a classe XmlIgnoreAttribute.
    /// Assim, a propriedade que tiver este atributo não é enviado para o lado cliente.
    /// Isso foi feito para o AcessoDados não ter conhecimento da biblioteca do Newtonsoft.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnorarPropriedadeTSAttribute : XmlIgnoreAttribute
    {
    }
}