namespace DigitalArsApi.Models;

public class Cuenta
{
    public int Numero { get; set; }
    public int DNI { get; set; }
    public decimal Saldo { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime? F_Update { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public ICollection<Transaccion> TransaccionesOrigen { get; set; } = new List<Transaccion>();
    public ICollection<Transaccion> TransaccionesDestino { get; set; } = new List<Transaccion>();
}

