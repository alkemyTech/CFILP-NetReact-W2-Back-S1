namespace DigitalArsApi.Models;

using System.Text.Json.Serialization;
public class Tipo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    [JsonIgnore]
    public ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
}

