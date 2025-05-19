namespace DigitalArsApi.Models;

public class Transaccion
{
    public int Id { get; set; }
    public int? CtaOrigen { get; set; }
    public int CtaDestino { get; set; }
    public int IdTipo { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public string? Descripcion { get; set; }

    // Relaciones: MARCAR COMO OPCIONAL
    public Cuenta? CuentaOrigen { get; set; }  // ya era nullable
    public Cuenta? CuentaDestino { get; set; } // <-- cambiar a nullable
    public Tipo? Tipo { get; set; }             // <-- cambiar a nullable

    public PlazoFijo? PlazoFijo { get; set; }
}
