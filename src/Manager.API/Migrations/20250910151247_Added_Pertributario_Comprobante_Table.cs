using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manager.API.Migrations
{
    /// <inheritdoc />
    public partial class Added_Pertributario_Comprobante_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pertributarios",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    mes = table.Column<int>(type: "int", nullable: false),
                    anio = table.Column<int>(type: "int", nullable: false),
                    TipoComprobante = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsInactive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pertributarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pertributarios_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "manager",
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comprobantes",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ruc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RazonSocial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Periodo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CarSunat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEmision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaVencimiento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoComprobante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Serie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Anio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroFinalRango = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoDocIdentidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroDocIdentidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreProveedor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BiGravadoDG = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IgvDG = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BiGravadoDGNG = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IgvDGNG = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BiGravadoDNG = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IgvDNG = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValorAdqNG = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Isc = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Icbper = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OtrosTributos = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Moneda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoCambio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FechaEmisionMod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoCPMod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerieCPMod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodDam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroCPMod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Clasificacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdProyecto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PorcPart = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Imb = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CarOrigen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Detraccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoNota = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoComprobante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Incal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Clus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerTributarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsInactive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comprobantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comprobantes_Pertributarios_PerTributarioId",
                        column: x => x.PerTributarioId,
                        principalSchema: "manager",
                        principalTable: "Pertributarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comprobantes_PerTributarioId",
                schema: "manager",
                table: "Comprobantes",
                column: "PerTributarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pertributarios_ClienteId_mes_anio_TipoComprobante",
                schema: "manager",
                table: "Pertributarios",
                columns: new[] { "ClienteId", "mes", "anio", "TipoComprobante" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comprobantes",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "Pertributarios",
                schema: "manager");
        }
    }
}
