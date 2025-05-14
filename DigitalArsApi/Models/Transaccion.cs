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

    public Cuenta? CuentaOrigen { get; set; }
    public Cuenta CuentaDestino { get; set; } = null!;
    public Tipo Tipo { get; set; } = null!;

    public PlazoFijo? PlazoFijo { get; set; } // 1:1 con PlazoFijo
}

