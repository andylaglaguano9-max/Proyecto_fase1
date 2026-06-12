using Microsoft.EntityFrameworkCore;
using VeterinariaApp.Models;
using System;

namespace VeterinariaApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Dueno> Duenos { get; set; }
        public DbSet<Especie> Especies { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Veterinario> Veterinarios { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Tratamiento> Tratamientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Data Seeding - Especies
            modelBuilder.Entity<Especie>().HasData(
                new Especie { Id = 1, Nombre = "Perro" },
                new Especie { Id = 2, Nombre = "Gato" },
                new Especie { Id = 3, Nombre = "Ave" },
                new Especie { Id = 4, Nombre = "Roedor" },
                new Especie { Id = 5, Nombre = "Reptil" }
            );

            // Data Seeding - Dueños
            modelBuilder.Entity<Dueno>().HasData(
                new Dueno { Id = 1, Nombre = "Juan", Apellido = "Perez", Telefono = "123456789", Correo = "juan@example.com" },
                new Dueno { Id = 2, Nombre = "Maria", Apellido = "Gomez", Telefono = "987654321", Correo = "maria@example.com" },
                new Dueno { Id = 3, Nombre = "Carlos", Apellido = "Ruiz", Telefono = "555555555", Correo = "carlos@example.com" },
                new Dueno { Id = 4, Nombre = "Ana", Apellido = "Lopez", Telefono = "444444444", Correo = "ana@example.com" },
                new Dueno { Id = 5, Nombre = "Luis", Apellido = "Diaz", Telefono = "333333333", Correo = "luis@example.com" }
            );

            // Data Seeding - Veterinarios
            modelBuilder.Entity<Veterinario>().HasData(
                new Veterinario { Id = 1, Nombre = "Dr. Roberto", Apellido = "Sanchez", Especialidad = "General" },
                new Veterinario { Id = 2, Nombre = "Dra. Laura", Apellido = "Martinez", Especialidad = "Cirugía" },
                new Veterinario { Id = 3, Nombre = "Dr. Pedro", Apellido = "Ramirez", Especialidad = "Dermatología" },
                new Veterinario { Id = 4, Nombre = "Dra. Sofia", Apellido = "Herrera", Especialidad = "Oftalmología" },
                new Veterinario { Id = 5, Nombre = "Dr. Miguel", Apellido = "Torres", Especialidad = "Traumatología" }
            );

            // Data Seeding - Mascotas
            modelBuilder.Entity<Mascota>().HasData(
                new Mascota { Id = 1, Nombre = "Rex", FechaNacimiento = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), Peso = 15.5m, DuenoId = 1, EspecieId = 1 },
                new Mascota { Id = 2, Nombre = "Miau", FechaNacimiento = new DateTime(2021, 5, 10, 0, 0, 0, DateTimeKind.Utc), Peso = 4.2m, DuenoId = 2, EspecieId = 2 },
                new Mascota { Id = 3, Nombre = "Piolin", FechaNacimiento = new DateTime(2022, 8, 15, 0, 0, 0, DateTimeKind.Utc), Peso = 0.5m, DuenoId = 3, EspecieId = 3 },
                new Mascota { Id = 4, Nombre = "Hams", FechaNacimiento = new DateTime(2023, 2, 20, 0, 0, 0, DateTimeKind.Utc), Peso = 0.3m, DuenoId = 4, EspecieId = 4 },
                new Mascota { Id = 5, Nombre = "Igu", FechaNacimiento = new DateTime(2019, 11, 30, 0, 0, 0, DateTimeKind.Utc), Peso = 2.1m, DuenoId = 5, EspecieId = 5 }
            );

            // Data Seeding - Citas
            modelBuilder.Entity<Cita>().HasData(
                new Cita { Id = 1, FechaCita = DateTime.UtcNow.AddDays(1), Motivo = "Control anual", MascotaId = 1, VeterinarioId = 1 },
                new Cita { Id = 2, FechaCita = DateTime.UtcNow.AddDays(2), Motivo = "Vacunación", MascotaId = 2, VeterinarioId = 2 },
                new Cita { Id = 3, FechaCita = DateTime.UtcNow.AddDays(3), Motivo = "Revisión ala", MascotaId = 3, VeterinarioId = 3 },
                new Cita { Id = 4, FechaCita = DateTime.UtcNow.AddDays(4), Motivo = "Corte de uñas", MascotaId = 4, VeterinarioId = 4 },
                new Cita { Id = 5, FechaCita = DateTime.UtcNow.AddDays(5), Motivo = "Chequeo piel", MascotaId = 5, VeterinarioId = 5 }
            );

            // Data Seeding - Tratamientos
            modelBuilder.Entity<Tratamiento>().HasData(
                new Tratamiento { Id = 1, Descripcion = "Revisión general completa", Costo = 30.00m, CitaId = 1 },
                new Tratamiento { Id = 2, Descripcion = "Aplicación vacuna antirrábica", Costo = 25.50m, CitaId = 2 },
                new Tratamiento { Id = 3, Descripcion = "Curación ala", Costo = 15.00m, CitaId = 3 },
                new Tratamiento { Id = 4, Descripcion = "Corte de uñas", Costo = 10.00m, CitaId = 4 },
                new Tratamiento { Id = 5, Descripcion = "Limpieza de escamas", Costo = 20.00m, CitaId = 5 }
            );
        }
    }
}
