using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manager.API.Migrations
{
    /// <inheritdoc />
    public partial class Token_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenClientes_Clientes_ClienteId",
                schema: "manager",
                table: "TokenClientes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TokenClientes",
                schema: "manager",
                table: "TokenClientes");

            migrationBuilder.RenameTable(
                name: "TokenClientes",
                schema: "manager",
                newName: "Tokens",
                newSchema: "manager");

            migrationBuilder.RenameIndex(
                name: "IX_TokenClientes_ClienteId",
                schema: "manager",
                table: "Tokens",
                newName: "IX_Tokens_ClienteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tokens",
                schema: "manager",
                table: "Tokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Clientes_ClienteId",
                schema: "manager",
                table: "Tokens",
                column: "ClienteId",
                principalSchema: "manager",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_Clientes_ClienteId",
                schema: "manager",
                table: "Tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tokens",
                schema: "manager",
                table: "Tokens");

            migrationBuilder.RenameTable(
                name: "Tokens",
                schema: "manager",
                newName: "TokenClientes",
                newSchema: "manager");

            migrationBuilder.RenameIndex(
                name: "IX_Tokens_ClienteId",
                schema: "manager",
                table: "TokenClientes",
                newName: "IX_TokenClientes_ClienteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TokenClientes",
                schema: "manager",
                table: "TokenClientes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenClientes_Clientes_ClienteId",
                schema: "manager",
                table: "TokenClientes",
                column: "ClienteId",
                principalSchema: "manager",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
