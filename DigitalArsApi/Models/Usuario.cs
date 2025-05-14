namespace DigitalArsApi.Models;

public class Usuario
{
    public int DNI { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public DateTime? F_Update { get; set; }

    public ICollection<Rol> Roles { get; set; } = new List<Rol>();
    public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
}

