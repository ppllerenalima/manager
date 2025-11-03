using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manager.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTienePermisoToCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "tienePermiso",
                schema: "manager",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tienePermiso",
                schema: "manager",
                table: "Clientes");
        }
    }
}
