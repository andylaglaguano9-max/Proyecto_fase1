using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VeterinariaApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Duenos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Correo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duenos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Especies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veterinarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Especialidad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veterinarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mascotas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Peso = table.Column<decimal>(type: "numeric", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DuenoId = table.Column<int>(type: "integer", nullable: false),
                    EspecieId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mascotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mascotas_Duenos_DuenoId",
                        column: x => x.DuenoId,
                        principalTable: "Duenos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mascotas_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Citas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaCita = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MascotaId = table.Column<int>(type: "integer", nullable: false),
                    VeterinarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Citas_Mascotas_MascotaId",
                        column: x => x.MascotaId,
                        principalTable: "Mascotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Citas_Veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tratamientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Costo = table.Column<decimal>(type: "numeric", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CitaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tratamientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tratamientos_Citas_CitaId",
                        column: x => x.CitaId,
                        principalTable: "Citas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Duenos",
                columns: new[] { "Id", "Activo", "Apellido", "Correo", "Direccion", "FechaCreacion", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 1, true, "Perez", "juan@example.com", "", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(995), "Juan", "123456789" },
                    { 2, true, "Gomez", "maria@example.com", "", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(1844), "Maria", "987654321" },
                    { 3, true, "Ruiz", "carlos@example.com", "", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(1845), "Carlos", "555555555" },
                    { 4, true, "Lopez", "ana@example.com", "", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(1846), "Ana", "444444444" },
                    { 5, true, "Diaz", "luis@example.com", "", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(1847), "Luis", "333333333" }
                });

            migrationBuilder.InsertData(
                table: "Especies",
                columns: new[] { "Id", "Activo", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2026, 6, 12, 3, 43, 44, 376, DateTimeKind.Utc).AddTicks(5532), "Perro" },
                    { 2, true, new DateTime(2026, 6, 12, 3, 43, 44, 376, DateTimeKind.Utc).AddTicks(6053), "Gato" },
                    { 3, true, new DateTime(2026, 6, 12, 3, 43, 44, 376, DateTimeKind.Utc).AddTicks(6054), "Ave" },
                    { 4, true, new DateTime(2026, 6, 12, 3, 43, 44, 376, DateTimeKind.Utc).AddTicks(6054), "Roedor" },
                    { 5, true, new DateTime(2026, 6, 12, 3, 43, 44, 376, DateTimeKind.Utc).AddTicks(6055), "Reptil" }
                });

            migrationBuilder.InsertData(
                table: "Veterinarios",
                columns: new[] { "Id", "Activo", "Apellido", "Especialidad", "FechaCreacion", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 1, true, "Sanchez", "General", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(2461), "Dr. Roberto", "" },
                    { 2, true, "Martinez", "Cirugía", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(3004), "Dra. Laura", "" },
                    { 3, true, "Ramirez", "Dermatología", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(3005), "Dr. Pedro", "" },
                    { 4, true, "Herrera", "Oftalmología", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(3006), "Dra. Sofia", "" },
                    { 5, true, "Torres", "Traumatología", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(3007), "Dr. Miguel", "" }
                });

            migrationBuilder.InsertData(
                table: "Mascotas",
                columns: new[] { "Id", "Activo", "DuenoId", "EspecieId", "FechaCreacion", "FechaNacimiento", "Nombre", "Peso" },
                values: new object[,]
                {
                    { 1, true, 1, 1, new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(3413), new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rex", 15.5m },
                    { 2, true, 2, 2, new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(4339), new DateTime(2021, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Miau", 4.2m },
                    { 3, true, 3, 3, new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(4343), new DateTime(2022, 8, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Piolin", 0.5m },
                    { 4, true, 4, 4, new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(4344), new DateTime(2023, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Hams", 0.3m },
                    { 5, true, 5, 5, new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(4345), new DateTime(2019, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Igu", 2.1m }
                });

            migrationBuilder.InsertData(
                table: "Citas",
                columns: new[] { "Id", "Activo", "Estado", "FechaCita", "FechaCreacion", "MascotaId", "Motivo", "VeterinarioId" },
                values: new object[,]
                {
                    { 1, true, "Programada", new DateTime(2026, 6, 13, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(4907), new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(4770), 1, "Control anual", 1 },
                    { 2, true, "Programada", new DateTime(2026, 6, 14, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(5476), new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(5476), 2, "Vacunación", 2 },
                    { 3, true, "Programada", new DateTime(2026, 6, 15, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(5481), new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(5481), 3, "Revisión ala", 3 },
                    { 4, true, "Programada", new DateTime(2026, 6, 16, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(5483), new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(5482), 4, "Corte de uñas", 4 },
                    { 5, true, "Programada", new DateTime(2026, 6, 17, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(5484), new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(5484), 5, "Chequeo piel", 5 }
                });

            migrationBuilder.InsertData(
                table: "Tratamientos",
                columns: new[] { "Id", "Activo", "CitaId", "Costo", "Descripcion", "FechaCreacion" },
                values: new object[,]
                {
                    { 1, true, 1, 30.00m, "Revisión general completa", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(5829) },
                    { 2, true, 2, 25.50m, "Aplicación vacuna antirrábica", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(6360) },
                    { 3, true, 3, 15.00m, "Curación ala", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(6362) },
                    { 4, true, 4, 10.00m, "Corte de uñas", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(6363) },
                    { 5, true, 5, 20.00m, "Limpieza de escamas", new DateTime(2026, 6, 12, 3, 43, 44, 377, DateTimeKind.Utc).AddTicks(6364) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MascotaId",
                table: "Citas",
                column: "MascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_VeterinarioId",
                table: "Citas",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_DuenoId",
                table: "Mascotas",
                column: "DuenoId");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_EspecieId",
                table: "Mascotas",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Tratamientos_CitaId",
                table: "Tratamientos",
                column: "CitaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tratamientos");

            migrationBuilder.DropTable(
                name: "Citas");

            migrationBuilder.DropTable(
                name: "Mascotas");

            migrationBuilder.DropTable(
                name: "Veterinarios");

            migrationBuilder.DropTable(
                name: "Duenos");

            migrationBuilder.DropTable(
                name: "Especies");
        }
    }
}
