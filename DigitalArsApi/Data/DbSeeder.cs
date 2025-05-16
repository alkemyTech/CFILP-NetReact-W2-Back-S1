using DigitalArsApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DigitalArsApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(DigitalArsContext context)
    {
        // Tipos
        if (!await context.Tipos.AnyAsync())
        {
            await context.Tipos.AddRangeAsync(
                new Tipo { Nombre = "Depósito", Descripcion = "Movimiento de fondos de propia cuenta" },
                new Tipo { Nombre = "Transferencia", Descripcion = "Movimiento de fondos entre cuentas de Usuarios" }
            );
            await context.SaveChangesAsync();
        }

        // Roles
        if (!await context.Roles.AnyAsync())
        {
            await context.Roles.AddRangeAsync(
                new Rol { Id = 1, Nombre = "Administrador" },
                new Rol { Id = 2, Nombre = "Usuario" }
            );
            await context.SaveChangesAsync();
        }

        // Usuarios
        if (!await context.Usuarios.AnyAsync())
        {
            var hasher = new PasswordHasher<Usuario>();

            var usuario1 = new Usuario
            {
                DNI = 32599611,
                Nombre = "Cesar",
                Apellido = "Barrera",
                Email = "cesar@barrera.com",
                Fecha = DateTime.Now,
                F_Update = DateTime.Now
            };
            usuario1.Password = hasher.HashPassword(usuario1, "cesar123");

            var usuario2 = new Usuario
            {
                DNI = 45566115,
                Nombre = "Luciano",
                Apellido = "Cayssials",
                Email = "luciano@cayssials.com",
                Fecha = DateTime.Now,
                F_Update = DateTime.Now
            };
            usuario2.Password = hasher.HashPassword(usuario2, "ana123");

            await context.Usuarios.AddRangeAsync(usuario1, usuario2);
            await context.SaveChangesAsync();
        }

        // EstadosPlazoFijo
        if (!await context.EstadosPlazoFijo.AnyAsync())
        {
            await context.EstadosPlazoFijo.AddRangeAsync(
                new EstadoPlazoFijo { Nombre = "Activo" },
                new EstadoPlazoFijo { Nombre = "Cancelado" },
                new EstadoPlazoFijo { Nombre = "Vencido" }
            );
            await context.SaveChangesAsync();
        }

        // RolesUsuarios
        // Usuario 1: Cesar con "Usuario" (Id = 2)
        var userCesar = await context.Usuarios.FirstOrDefaultAsync(u => u.DNI == 32599611);
        var rolUser = await context.Roles.FirstOrDefaultAsync(r => r.Id == 2);

        if (userCesar != null && rolUser != null)
        {
            var yaAsignado = await context.Set<Dictionary<string, object>>("RolesUsuarios")
                .AnyAsync(ru => ru["DNI"].Equals(userCesar.DNI) && ru["IdRol"].Equals(rolUser.Id));

            if (!yaAsignado)
            {
                context.Set<Dictionary<string, object>>("RolesUsuarios").Add(new Dictionary<string, object>
                {
                    { "DNI", userCesar.DNI },
                    { "IdRol", rolUser.Id },
                    { "Fecha", DateTime.Now }
                });
            }
        }

        // Usuario 2: Luciano con Rol "Administrador" (Id = 1)
        var userLuciano = await context.Usuarios.FirstOrDefaultAsync(u => u.DNI == 45566115);
        var rolAdmin = await context.Roles.FirstOrDefaultAsync(r => r.Id == 1);

        if (userLuciano != null && rolAdmin != null)
        {
            var yaAsignado = await context.Set<Dictionary<string, object>>("RolesUsuarios")
                .AnyAsync(ru => ru["DNI"].Equals(userLuciano.DNI) && ru["IdRol"].Equals(rolAdmin.Id));

            if (!yaAsignado)
            {
                context.Set<Dictionary<string, object>>("RolesUsuarios").Add(new Dictionary<string, object>
                {
                    { "DNI", userLuciano.DNI },
                    { "IdRol", rolAdmin.Id },
                    { "Fecha", DateTime.Now }
                });
            }
        }

        await context.SaveChangesAsync();
    }
}