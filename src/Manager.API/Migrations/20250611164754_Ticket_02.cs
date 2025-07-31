using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manager.API.Migrations
{
    /// <inheritdoc />
    public partial class Ticket_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tickets",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodProceso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodEstadoProceso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DesProceso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerTributario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumTicket = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FecCargaImportacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoraCargaImportacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodEstadoEnvio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DesEstadoEnvio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodTipoAchivoReporte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomArchivoReporte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsInactive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "manager",
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ClienteId",
                schema: "manager",
                table: "Tickets",
                column: "ClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets",
                schema: "manager");
        }
    }
}
