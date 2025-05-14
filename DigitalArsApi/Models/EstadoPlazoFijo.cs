namespace DigitalArsApi.Models;

using System.Text.Json.Serialization;
public class EstadoPlazoFijo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;

    [JsonIgnore]
    public ICollection<PlazoFijo> PlazosFijos { get; set; } = new List<PlazoFijo>();
}

