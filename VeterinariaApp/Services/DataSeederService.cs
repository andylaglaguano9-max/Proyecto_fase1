using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VeterinariaApp.Data;
using VeterinariaApp.Models;

namespace VeterinariaApp.Services
{
    public class DataSeederService
    {
        private readonly AppDbContext _context;

        public DataSeederService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedDataAsync()
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            // Verificar si existen los datos genéricos antiguos
            bool hasOldGenericData = await _context.Ciudades.AnyAsync(c => c.Nombre == "Ciudad 1") || 
                                     await _context.Especies.AnyAsync(e => e.Nombre == "Especie 1");

            if (hasOldGenericData)
            {
                // Limpiar todas las tablas para permitir la regeneración con datos reales
                await _context.Database.ExecuteSqlRawAsync(@"
                    TRUNCATE TABLE ""Citas"", ""Duenos"", ""Especies"", ""Mascotas"", ""Sucursales"", ""Tratamientos"", ""Usuarios"", ""Veterinarios"", ""Ciudades"", ""CategoriasProducto"", ""DetallesFactura"", ""DetallesVenta"", ""Facturas"", ""Medicamentos"", ""Productos"", ""Proveedores"", ""Vacunas"", ""Ventas"" RESTART IDENTITY CASCADE;
                ");
            }

            // Función local genérica para la inserción masiva de datos (Bulk Insert).
            // Estrategia crítica para poblar 500,000+ registros sin agotar los recursos del servidor.
            async Task BulkInsertAsync<T>(List<T> entities) where T : class
            {
                // Segmentación de la colección en lotes manejables (Batching)
                int batchSize = 10000;
                for (int i = 0; i < entities.Count; i += batchSize)
                {
                    // Obtención de la fracción correspondiente mediante LINQ
                    var batch = entities.Skip(i).Take(batchSize).ToList();
                    await _context.Set<T>().AddRangeAsync(batch);
                    
                    // Ejecución transaccional del lote en la base de datos PostgreSQL
                    await _context.SaveChangesAsync();
                    
                    // Liberación obligatoria de la memoria RAM del ChangeTracker para evitar
                    // excepciones de tipo OutOfMemory durante la ejecución prolongada.
                    _context.ChangeTracker.Clear();
                }
            }

            var rnd = new Random();

            string[] nombres = { "Juan", "María", "Carlos", "Ana", "Luis", "Elena", "Pedro", "Sofía", "Miguel", "Lucía", "Jorge", "Laura", "Andrés", "Carmen", "Diego", "Paula", "Fernando", "Marta", "Roberto", "Isabel" };
            string[] apellidos = { "Pérez", "Gómez", "Rodríguez", "López", "Martínez", "Fernández", "García", "Sánchez", "Romero", "Suárez", "Torres", "Ruiz", "Díaz", "Vargas", "Castro", "Ortiz", "Mora", "Flores", "Ríos", "Silva" };
            string[] ciudadesNombres = { "Quito", "Guayaquil", "Cuenca", "Santo Domingo", "Machala", "Durán", "Manta", "Portoviejo", "Loja", "Ambato", "Esmeraldas", "Riobamba", "Ibarra", "Latacunga", "Tulcán" };
            string[] especiesNombres = { "Perro", "Gato", "Loro", "Canario", "Hámster", "Conejo", "Tortuga", "Iguana", "Pez", "Caballo", "Vaca", "Cerdo", "Oveja", "Cabra", "Gallina", "Pato", "Erizo", "Hurón", "Serpiente", "Gecko" };
            string[] mascotasNombres = { "Max", "Luna", "Bella", "Toby", "Rocky", "Coco", "Kira", "Zeus", "Nala", "Simba", "Milo", "Chloe", "Nina", "Thor", "Mia", "Lola", "Buddy", "Pelusa", "Manchas", "Duke" };
            string[] motivosCita = { "Control general", "Vacunación anual", "Desparasitación", "Enfermedad", "Emergencia", "Corte de uñas", "Limpieza dental", "Rayos X", "Cirugía menor", "Revisión post-operatoria" };
            string[] categoriasProd = { "Alimentos Secos", "Alimentos Húmedos", "Juguetes", "Accesorios", "Higiene", "Ropa", "Camas", "Collares", "Transportadoras", "Vitaminas", "Snacks", "Champú", "Cepillos", "Comederos" };
            string[] descTratamientos = { "Aplicación de vacuna antirrábica", "Limpieza dental profunda", "Radiografía de tórax", "Curación de herida superficial", "Extracción de diente", "Consulta especialista", "Ecografía abdominal", "Análisis de sangre", "Tratamiento antibiótico", "Sesión de fisioterapia" };

            string GetRandomName() => nombres[rnd.Next(nombres.Length)];
            string GetRandomSurname() => apellidos[rnd.Next(apellidos.Length)];
            string GetFullName() => $"{GetRandomName()} {GetRandomSurname()}";

            bool oldDataExists = await _context.Ciudades.AnyAsync();
            if (!oldDataExists)
            {
                // 1. Seed Ciudades (50)
                var ciudades = new List<Ciudad>();
                for (int i = 0; i < 50; i++)
                {
                    ciudades.Add(new Ciudad { Nombre = i < ciudadesNombres.Length ? ciudadesNombres[i] : $"Ciudad {i}", Activo = true, FechaCreacion = DateTime.UtcNow });
                }
                await _context.Ciudades.AddRangeAsync(ciudades);
                await _context.SaveChangesAsync();

                // 2. Seed Usuarios (50)
                var usuarios = new List<Usuario>();
                usuarios.Add(new Usuario { Username = "admin", Password = "admin123", Rol = "Admin", Activo = true, FechaCreacion = DateTime.UtcNow });
                for (int i = 2; i <= 50; i++)
                {
                    string randomUser = $"{GetRandomName().ToLower()}_{GetRandomSurname().ToLower()}{rnd.Next(1, 99)}";
                    usuarios.Add(new Usuario { Username = randomUser, Password = "user123", Rol = "User", Activo = true, FechaCreacion = DateTime.UtcNow });
                }
                await _context.Usuarios.AddRangeAsync(usuarios);
                await _context.SaveChangesAsync();

                // 3. Seed Especies (50)
                var especies = new List<Especie>();
                for (int i = 0; i < 50; i++)
                {
                    especies.Add(new Especie { Nombre = i < especiesNombres.Length ? especiesNombres[i] : $"Especie Exótica {i}", Activo = true, FechaCreacion = DateTime.UtcNow });
                }
                await _context.Especies.AddRangeAsync(especies);
                await _context.SaveChangesAsync();

                // 4. Seed Sucursales (100)
                var sucursales = new List<Sucursal>();
                var ciudadesIds = ciudades.Select(c => c.Id).ToList();
                for (int i = 1; i <= 100; i++)
                {
                    sucursales.Add(new Sucursal
                    {
                        Nombre = $"Clínica Vet {GetRandomSurname()} - Sucursal {i}",
                        Direccion = $"Av. Principal {rnd.Next(100, 999)} y Calle Secundaria",
                        CiudadId = ciudadesIds[rnd.Next(ciudadesIds.Count)],
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                }
                await _context.Sucursales.AddRangeAsync(sucursales);
                await _context.SaveChangesAsync();

                // 5. Seed Veterinarios (1000)
                var veterinarios = new List<Veterinario>();
                var sucursalesIds = sucursales.Select(s => s.Id).ToList();
                string[] especialidades = { "General", "Cirugía", "Dermatología", "Oftalmología", "Cardiología", "Neurología", "Odontología", "Traumatología", "Exóticos" };
                for (int i = 1; i <= 1000; i++)
                {
                    veterinarios.Add(new Veterinario
                    {
                        Nombre = $"Dr. {GetRandomName()}",
                        Apellido = GetRandomSurname(),
                        Especialidad = especialidades[rnd.Next(especialidades.Length)],
                        Telefono = $"09{rnd.Next(10000000, 99999999)}",
                        SucursalId = sucursalesIds[rnd.Next(sucursalesIds.Count)],
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                }
                await _context.Veterinarios.AddRangeAsync(veterinarios);
                await _context.SaveChangesAsync();

                // 6. Seed Duenos (80000)
                var duenos = new List<Dueno>();
                for (int i = 1; i <= 80000; i++)
                {
                    string nombre = GetRandomName();
                    string apellido = GetRandomSurname();
                    duenos.Add(new Dueno
                    {
                        Nombre = nombre,
                        Apellido = apellido,
                        Telefono = $"09{rnd.Next(10000000, 99999999)}",
                        Direccion = $"Barrio {GetRandomSurname()}, Calle {rnd.Next(1, 100)}",
                        Correo = $"{nombre.ToLower()}.{apellido.ToLower()}{rnd.Next(1, 999)}@example.com",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                }
                await BulkInsertAsync(duenos);

                var duenosIds = await _context.Duenos.Select(d => d.Id).ToListAsync();
                var especiesIds = especies.Select(e => e.Id).ToList();

                // 7. Seed Mascotas (100000)
                var mascotas = new List<Mascota>();
                for (int i = 1; i <= 100000; i++)
                {
                    mascotas.Add(new Mascota
                    {
                        Nombre = mascotasNombres[rnd.Next(mascotasNombres.Length)],
                        FechaNacimiento = DateTime.UtcNow.AddDays(-rnd.Next(100, 3000)),
                        Peso = (decimal)(rnd.NextDouble() * 30 + 1),
                        DuenoId = duenosIds[rnd.Next(duenosIds.Count)],
                        EspecieId = especiesIds[rnd.Next(especiesIds.Count)],
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                }
                await BulkInsertAsync(mascotas);

                var mascotasIds = await _context.Mascotas.Select(m => m.Id).ToListAsync();
                var veterinariosIds = veterinarios.Select(v => v.Id).ToList();

                // 8. Seed Citas (150000)
                var citas = new List<Cita>();
                for (int i = 1; i <= 150000; i++)
                {
                    var vetId = veterinariosIds[rnd.Next(veterinariosIds.Count)];
                    int sucId = veterinarios[vetId - 1].SucursalId;

                    citas.Add(new Cita
                    {
                        FechaCita = DateTime.UtcNow.AddDays(rnd.Next(-300, 30)),
                        Motivo = motivosCita[rnd.Next(motivosCita.Length)],
                        Estado = i % 2 == 0 ? "Completada" : "Programada",
                        MascotaId = mascotasIds[rnd.Next(mascotasIds.Count)],
                        VeterinarioId = vetId,
                        SucursalId = sucId,
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                }
                await BulkInsertAsync(citas);

                var citasIds = await _context.Citas.Select(c => c.Id).ToListAsync();

                // 9. Seed Tratamientos (168750)
                var tratamientos = new List<Tratamiento>();
                for (int i = 1; i <= 168750; i++)
                {
                    tratamientos.Add(new Tratamiento
                    {
                        Descripcion = descTratamientos[rnd.Next(descTratamientos.Length)],
                        Costo = (decimal)(rnd.NextDouble() * 100 + 10),
                        CitaId = citasIds[rnd.Next(citasIds.Count)],
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                }
                await BulkInsertAsync(tratamientos);
            }

            // FASE 3: SUPERBASE DE DATOS (Nuevas Tablas)
            bool superDBExists = await _context.Proveedores.AnyAsync();
            if (!superDBExists)
            {
                // Seed Proveedores (100)
                var proveedores = new List<Proveedor>();
                string[] empresas = { "PharmaVet", "PetSupply", "SaludAnimal", "Distribuidora Mascotas", "VetCareCorp", "MedicaVet", "BioVet", "NutriPet" };
                for (int i = 1; i <= 100; i++)
                {
                    proveedores.Add(new Proveedor
                    {
                        Nombre = $"{empresas[rnd.Next(empresas.Length)]} S.A. {i}",
                        Contacto = GetFullName(),
                        Telefono = $"022{rnd.Next(100000, 999999)}",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                }
                await _context.Proveedores.AddRangeAsync(proveedores);
                await _context.SaveChangesAsync();
                var proveedoresIds = proveedores.Select(p => p.Id).ToList();

                // Seed Medicamentos (5000)
                var medicamentos = new List<Medicamento>();
                string[] medNames = { "Amoxicilina Vet", "Desparasitante Plus", "Vitamina C", "Calcio", "Ibuprofeno Canino", "Gotas Óticas", "Colirio", "Pomada Cicatrizante", "Antipulgas", "Shampoo medicado" };
                for (int i = 1; i <= 5000; i++)
                {
                    medicamentos.Add(new Medicamento
                    {
                        Nombre = $"{medNames[rnd.Next(medNames.Length)]} {rnd.Next(10, 500)}mg",
                        Descripcion = $"Tratamiento eficaz para diversas condiciones. Lote {rnd.Next(1000, 9999)}",
                        Precio = (decimal)(rnd.NextDouble() * 50 + 5),
                        Stock = rnd.Next(10, 500),
                        ProveedorId = proveedoresIds[rnd.Next(proveedoresIds.Count)],
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                }
                await BulkInsertAsync(medicamentos);
                
                var duenosIds = await _context.Duenos.Select(d => d.Id).ToListAsync();
                var mascotasIds = await _context.Mascotas.Select(m => m.Id).ToListAsync();
                var veterinariosIds = await _context.Veterinarios.Select(v => v.Id).ToListAsync();

                if (duenosIds.Any() && mascotasIds.Any() && veterinariosIds.Any())
                {
                    // Seed Facturas (50000)
                    var facturas = new List<Factura>();
                    for (int i = 1; i <= 50000; i++)
                    {
                        facturas.Add(new Factura
                        {
                            FechaEmision = DateTime.UtcNow.AddDays(-rnd.Next(1, 300)),
                            Total = 0,
                            Estado = rnd.Next(2) == 0 ? "Pagada" : "Pendiente",
                            DuenoId = duenosIds[rnd.Next(duenosIds.Count)],
                            Activo = true,
                            FechaCreacion = DateTime.UtcNow
                        });
                    }
                    await BulkInsertAsync(facturas);
                    var facturasIds = await _context.Facturas.Select(f => f.Id).ToListAsync();

                    // Seed DetallesFactura (100000)
                    var detalles = new List<DetalleFactura>();
                    for (int i = 1; i <= 100000; i++)
                    {
                        int cant = rnd.Next(1, 5);
                        decimal pu = (decimal)(rnd.NextDouble() * 40 + 10);
                        detalles.Add(new DetalleFactura
                        {
                            FacturaId = facturasIds[rnd.Next(facturasIds.Count)],
                            Concepto = $"Servicio médico o producto: {descTratamientos[rnd.Next(descTratamientos.Length)]}",
                            Cantidad = cant,
                            PrecioUnitario = pu,
                            Subtotal = cant * pu,
                            Activo = true,
                            FechaCreacion = DateTime.UtcNow
                        });
                    }
                    await BulkInsertAsync(detalles);

                    // Seed Vacunas (30000)
                    var vacunas = new List<Vacuna>();
                    string[] nomVacunas = { "Antirrábica", "Parvovirus", "Moquillo", "Múltiple Canina", "Triple Felina", "Leucemia Felina", "Bordetella", "Lyme" };
                    for (int i = 1; i <= 30000; i++)
                    {
                        vacunas.Add(new Vacuna
                        {
                            Nombre = nomVacunas[rnd.Next(nomVacunas.Length)],
                            Lote = $"LOTE-{rnd.Next(1000, 9999)}",
                            FechaAplicacion = DateTime.UtcNow.AddDays(-rnd.Next(1, 200)),
                            ProximaDosis = DateTime.UtcNow.AddDays(rnd.Next(30, 365)),
                            MascotaId = mascotasIds[rnd.Next(mascotasIds.Count)],
                            VeterinarioId = veterinariosIds[rnd.Next(veterinariosIds.Count)],
                            Activo = true,
                            FechaCreacion = DateTime.UtcNow
                        });
                    }
                    await BulkInsertAsync(vacunas);
                }
            }

            // FASE 3: MODULO TIENDA PETSHOP
            bool tiendaExists = await _context.CategoriasProducto.AnyAsync();
            if (!tiendaExists && superDBExists == false)
            {
                // Seed CategoriaProducto (20)
                var categorias = new List<CategoriaProducto>();
                for (int i = 0; i < 20; i++)
                {
                    categorias.Add(new CategoriaProducto
                    {
                        Nombre = i < categoriasProd.Length ? categoriasProd[i] : $"Categoría Extra {i}",
                        Descripcion = $"Productos de la categoría {i}",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                }
                await _context.CategoriasProducto.AddRangeAsync(categorias);
                await _context.SaveChangesAsync();
                var categoriasIds = categorias.Select(c => c.Id).ToList();

                var proveedoresIds = await _context.Proveedores.Select(p => p.Id).ToListAsync();

                // Seed Producto (10000)
                var productos = new List<Producto>();
                string[] prodNames = { "Collar de cuero", "Correa retráctil", "Plato de acero", "Comida Premium", "Hueso masticable", "Rascador", "Cama acolchada", "Cepillo", "Shampoo", "Juguete con sonido" };
                for (int i = 1; i <= 10000; i++)
                {
                    productos.Add(new Producto
                    {
                        Nombre = $"{prodNames[rnd.Next(prodNames.Length)]} Tipo {rnd.Next(1, 5)}",
                        Descripcion = $"Accesorio o alimento de alta calidad.",
                        Precio = (decimal)(rnd.NextDouble() * 100 + 1),
                        Stock = rnd.Next(0, 1000),
                        CategoriaProductoId = categoriasIds[rnd.Next(categoriasIds.Count)],
                        ProveedorId = proveedoresIds[rnd.Next(proveedoresIds.Count)],
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                }
                await BulkInsertAsync(productos);
                var productosIds = await _context.Productos.Select(p => p.Id).ToListAsync();

                var duenosIds = await _context.Duenos.Select(d => d.Id).ToListAsync();

                if (duenosIds.Any())
                {
                    // Seed Ventas (60000)
                    var ventas = new List<Venta>();
                    for (int i = 1; i <= 60000; i++)
                    {
                        ventas.Add(new Venta
                        {
                            FechaVenta = DateTime.UtcNow.AddDays(-rnd.Next(1, 365)),
                            Total = 0,
                            DuenoId = duenosIds[rnd.Next(duenosIds.Count)],
                            Activo = true,
                            FechaCreacion = DateTime.UtcNow
                        });
                    }
                    await BulkInsertAsync(ventas);
                    var ventasIds = await _context.Ventas.Select(v => v.Id).ToListAsync();

                    // Seed DetallesVenta (150000)
                    var detallesVenta = new List<DetalleVenta>();
                    for (int i = 1; i <= 150000; i++)
                    {
                        int cant = rnd.Next(1, 10);
                        decimal pu = (decimal)(rnd.NextDouble() * 50 + 2);
                        detallesVenta.Add(new DetalleVenta
                        {
                            VentaId = ventasIds[rnd.Next(ventasIds.Count)],
                            ProductoId = productosIds[rnd.Next(productosIds.Count)],
                            Cantidad = cant,
                            PrecioUnitario = pu,
                            Subtotal = cant * pu,
                            Activo = true,
                            FechaCreacion = DateTime.UtcNow
                        });
                    }
                    await BulkInsertAsync(detallesVenta);
                }
            }

            _context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }
}
