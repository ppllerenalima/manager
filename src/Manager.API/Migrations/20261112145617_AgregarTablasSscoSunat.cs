using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manager.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablasSscoSunat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SujetoSinCapacidadOperativas",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ruc = table.Column<string>(type: "varchar(11)", unicode: false, maxLength: 11, nullable: false),
                    RazonSocial = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DomicilioFiscal = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NumeroResolucion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FechaEmisionResolucion = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFirmezaResolucion = table.Column<DateOnly>(type: "date", nullable: true),
                    DocumentoRepresentanteLegal = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    NombreRepresentanteLegal = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FechaPublicacion = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaActualizacionSistema = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsInactive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SujetoSinCapacidadOperativas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SunatPadronHistorials",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombrePadron = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TextoFechaSunat = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    RegistrosProcesados = table.Column<int>(type: "int", nullable: false),
                    FechaEjecucion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsInactive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SunatPadronHistorials", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SujetoSinCapacidadOperativas_Ruc",
                schema: "manager",
                table: "SujetoSinCapacidadOperativas",
                column: "Ruc");

            migrationBuilder.CreateIndex(
                name: "IX_SujetoSinCapacidadOperativas_Ruc_NumeroResolucion",
                schema: "manager",
                table: "SujetoSinCapacidadOperativas",
                columns: new[] { "Ruc", "NumeroResolucion" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SujetoSinCapacidadOperativas",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "SunatPadronHistorials",
                schema: "manager");
        }
    }
}
