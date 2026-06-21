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

        public DbSet<Ciudad> Ciudades { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Dueno> Duenos { get; set; }
        public DbSet<Especie> Especies { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Veterinario> Veterinarios { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Tratamiento> Tratamientos { get; set; }

        // Nuevas 5 tablas para Fase 3 (Superbase de datos - Clínico)
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<DetalleFactura> DetallesFactura { get; set; }
        public DbSet<Vacuna> Vacunas { get; set; }

        // Nuevas 4 tablas para Fase 3 (Módulo Tienda PetShop)
        public DbSet<CategoriaProducto> CategoriasProducto { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Delete behavior can be customized here if needed.
            // Using soft delete usually means we don't rely heavily on DB cascade delete,
            // but we can leave defaults for now.
        }
    }
}
