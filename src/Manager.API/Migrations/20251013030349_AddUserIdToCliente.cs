using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manager.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "manager",
                table: "Clientes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("87AF8AF7-3920-4F31-4CED-08DDF9CCA3D9"));

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UserId",
                schema: "manager",
                table: "Clientes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Usuarios_UserId",
                schema: "manager",
                table: "Clientes",
                column: "UserId",
                principalSchema: "manager",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Usuarios_UserId",
                schema: "manager",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_UserId",
                schema: "manager",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "manager",
                table: "Clientes");
        }
    }
}
