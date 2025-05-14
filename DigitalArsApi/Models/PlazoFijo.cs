namespace DigitalArsApi.Models;

public class PlazoFijo
{
    public int Id { get; set; }
    public int IdTransaccion { get; set; }
    public decimal Monto { get; set; }
    public decimal TNA { get; set; }
    public DateTime F_Inicio { get; set; }
    public DateTime F_Fin { get; set; }
    public decimal? InteresEsperado { get; set; }
    public int IdEstado { get; set; }

    public EstadoPlazoFijo Estado { get; set; } = null!;
    public Transaccion Transaccion { get; set; } = null!;
}
