using System.Globalization;

namespace Snebur.Utils.Tests;

public class JsonUtilTests
{
    private sealed class Poco
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    [Fact]
    public void Serializar_E_Desserializar_DotNet_Profile()
    {
        var obj = new Poco { Id = 7, Name = "Alpha" };
        var json = JsonUtil.Serializar(obj, EnumTipoSerializacao.DotNet, CultureInfo.InvariantCulture, isIdentar: false);
        json.Should().NotBeNullOrWhiteSpace();

        var back = JsonUtil.Deserializar<Poco>(json, EnumTipoSerializacao.DotNet);
        back.Should().NotBeNull();
        back!.Id.Should().Be(7);
        back.Name.Should().Be("Alpha");
    }

    [Fact]
    public void TryDeserializar_InvalidJson_ReturnsDefault()
    {
        var result = JsonUtil.TryDeserializar<Poco>("{ not json }", EnumTipoSerializacao.DotNet);
        result.Should().BeNull();
    }
}

