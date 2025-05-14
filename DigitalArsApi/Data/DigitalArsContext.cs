using Microsoft.EntityFrameworkCore;
using DigitalArsApi.Models;

namespace DigitalArsApi.Data
{
    public class DigitalArsContext : DbContext
    {
        public DigitalArsContext(DbContextOptions<DigitalArsContext> options)
            : base(options) { }

        public DbSet<Tipo> Tipos { get; set; } = null!;
        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Cuenta> Cuentas { get; set; } = null!;
        public DbSet<Transaccion> Transacciones { get; set; } = null!;
        public DbSet<EstadoPlazoFijo> EstadosPlazoFijo { get; set; } = null!;
        public DbSet<PlazoFijo> PlazosFijos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Claves primarias
            modelBuilder.Entity<Tipo>().HasKey(t => t.Id);
            modelBuilder.Entity<Rol>().HasKey(r => r.Id);
            modelBuilder.Entity<Usuario>().HasKey(u => u.DNI);
            modelBuilder.Entity<Cuenta>().HasKey(c => c.Numero);
            modelBuilder.Entity<Transaccion>().HasKey(t => t.Id);
            modelBuilder.Entity<EstadoPlazoFijo>().HasKey(e => e.Id);
            modelBuilder.Entity<PlazoFijo>().HasKey(p => p.Id);

            // Usuario -> Rol (N:N) a través de la tabla RolesUsuarios
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Usuarios)
                .UsingEntity<Dictionary<string, object>>(
                    "RolesUsuarios", // Nombre de la tabla intermedia
                    j => j
                        .HasOne<Rol>()
                        .WithMany()
                        .HasForeignKey("IdRol")
                        .HasConstraintName("FK_RolesUsuarios_Roles")
                        .OnDelete(DeleteBehavior.Restrict),
                    j => j
                        .HasOne<Usuario>()
                        .WithMany()
                        .HasForeignKey("DNI")
                        .HasConstraintName("FK_RolesUsuarios_Usuarios")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("DNI", "IdRol"); // Clave primaria compuesta
                        j.Property<DateTime>("Fecha").HasDefaultValueSql("CURRENT_TIMESTAMP");
                    });

            // Cuenta -> Usuario (1:N)
            modelBuilder.Entity<Cuenta>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Cuentas)
                .HasForeignKey(c => c.DNI)
                .OnDelete(DeleteBehavior.Cascade);

            // Transaccion -> Cuenta (Origen)
            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.CuentaOrigen)
                .WithMany(c => c.TransaccionesOrigen)
                .HasForeignKey(t => t.CtaOrigen)
                .OnDelete(DeleteBehavior.Restrict);

            // Transaccion -> Cuenta (Destino)
            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.CuentaDestino)
                .WithMany(c => c.TransaccionesDestino)
                .HasForeignKey(t => t.CtaDestino)
                .OnDelete(DeleteBehavior.Restrict);

            // Transaccion -> Tipo (N:1)
            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Tipo)
                .WithMany(tp => tp.Transacciones)
                .HasForeignKey(t => t.IdTipo)
                .OnDelete(DeleteBehavior.Restrict);

            // PlazoFijo -> Transaccion (1:1)
            modelBuilder.Entity<PlazoFijo>()
                .HasOne(p => p.Transaccion)
                .WithOne(t => t.PlazoFijo)
                .HasForeignKey<PlazoFijo>(p => p.IdTransaccion)
                .OnDelete(DeleteBehavior.Cascade);

            // PlazoFijo -> EstadoPlazoFijo (N:1)
            modelBuilder.Entity<PlazoFijo>()
                .HasOne(p => p.Estado)
                .WithMany(e => e.PlazosFijos)
                .HasForeignKey(p => p.IdEstado)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}