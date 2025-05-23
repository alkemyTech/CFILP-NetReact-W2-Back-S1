using DigitalArsApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DigitalArsApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(DigitalArsContext context)
    {
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

        // Tipos
        if (!await context.Tipos.AnyAsync())
        {
            await context.Tipos.AddRangeAsync(
                new Tipo { Nombre = "Depósito", Descripcion = "Movimiento de fondos a cuenta propia" },
                new Tipo { Nombre = "Plazo Fijo", Descripcion = "Movimiento de fondos a Plazo Fijo" },
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

            var user1 = new Usuario
            {
                DNI = 20,
                Nombre = "Western Union",
                Email = "western@union.com",
                Fecha = DateTime.Now,
                F_Update = DateTime.Now
            };
            user1.Password = hasher.HashPassword(user1, "user1");

            var user2 = new Usuario
            {
                DNI = 40,
                Nombre = "Mercado Pago",
                Email = "mercado@pago.com",
                Fecha = DateTime.Now,
                F_Update = DateTime.Now
            };
            user2.Password = hasher.HashPassword(user2, "user2");

            var user3 = new Usuario
            {
                DNI = 60,
                Nombre = "Personal Pay",
                Email = "personal@pay.com",
                Fecha = DateTime.Now,
                F_Update = DateTime.Now
            };
            user3.Password = hasher.HashPassword(user3, "user3");

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
            usuario2.Password = hasher.HashPassword(usuario2, "luciano123");

            await context.Usuarios.AddRangeAsync(user1, user2, user3, usuario1, usuario2);
            await context.SaveChangesAsync();
        }

        // Cuentas
        if (!await context.Cuentas.AnyAsync())
        {
            var cuentaExtra1 = new Cuenta
            {
                Numero = 20,
                DNI = 20,
                Saldo = 0,
                Fecha = DateTime.Now,
                F_Update = DateTime.Now
            };

            var cuentaExtra2 = new Cuenta
            {
                Numero = 40,
                DNI = 40,
                Saldo = 0,
                Fecha = DateTime.Now,
                F_Update = DateTime.Now
            };

            var cuentaExtra3 = new Cuenta
            {
                Numero = 60,
                DNI = 60,
                Saldo = 0,
                Fecha = DateTime.Now,
                F_Update = DateTime.Now
            };

            await context.Cuentas.AddRangeAsync(cuentaExtra1, cuentaExtra2, cuentaExtra3);
            await context.SaveChangesAsync(); // Agregado SaveChangesAsync aquí
        }

        var usuarioCesar = await context.Usuarios.FirstOrDefaultAsync(u => u.DNI == 32599611);
        var usuarioLuciano = await context.Usuarios.FirstOrDefaultAsync(u => u.DNI == 45566115);

        if (usuarioCesar != null && !await context.Cuentas.AnyAsync(c => c.DNI == usuarioCesar.DNI))
        {
            var cuentaCesar = new Cuenta
            {
                Numero = 12345,
                DNI = usuarioCesar.DNI,
                Saldo = 50000.00m,
                Fecha = DateTime.Now,
                F_Update = DateTime.Now
            };

            await context.Cuentas.AddAsync(cuentaCesar);
        }

        if (usuarioLuciano != null && !await context.Cuentas.AnyAsync(c => c.DNI == usuarioLuciano.DNI))
        {
            var cuentaLuciano = new Cuenta
            {
                Numero = 67890,
                DNI = usuarioLuciano.DNI,
                Saldo = 30000.00m,
                Fecha = DateTime.Now,
                F_Update = DateTime.Now
            };

            await context.Cuentas.AddAsync(cuentaLuciano);
        }

        await context.SaveChangesAsync();

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