namespace DigitalArsApi.Models;

using System.Text.Json.Serialization;
public class Rol
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;

    [JsonIgnore]
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}

