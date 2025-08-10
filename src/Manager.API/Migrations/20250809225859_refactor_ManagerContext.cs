using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manager.API.Migrations
{
    /// <inheritdoc />
    public partial class refactor_ManagerContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Persona_PersonaId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Grupo_GrupoId",
                schema: "manager",
                table: "Clientes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Persona",
                table: "Persona");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Grupo",
                table: "Grupo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "Persona",
                newName: "Personas",
                newSchema: "manager");

            migrationBuilder.RenameTable(
                name: "Grupo",
                newName: "Grupos",
                newSchema: "manager");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "UsuarioTokens",
                newSchema: "manager");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "Usuarios",
                newSchema: "manager");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "UsuarioRoles",
                newSchema: "manager");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "UsuarioLogins",
                newSchema: "manager");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "UsuarioClaims",
                newSchema: "manager");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "Roles",
                newSchema: "manager");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "RolClaims",
                newSchema: "manager");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_PersonaId",
                schema: "manager",
                table: "Usuarios",
                newName: "IX_Usuarios_PersonaId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "manager",
                table: "UsuarioRoles",
                newName: "IX_UsuarioRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "manager",
                table: "UsuarioLogins",
                newName: "IX_UsuarioLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "manager",
                table: "UsuarioClaims",
                newName: "IX_UsuarioClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "manager",
                table: "RolClaims",
                newName: "IX_RolClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Personas",
                schema: "manager",
                table: "Personas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Grupos",
                schema: "manager",
                table: "Grupos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioTokens",
                schema: "manager",
                table: "UsuarioTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                schema: "manager",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioRoles",
                schema: "manager",
                table: "UsuarioRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioLogins",
                schema: "manager",
                table: "UsuarioLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioClaims",
                schema: "manager",
                table: "UsuarioClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                schema: "manager",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolClaims",
                schema: "manager",
                table: "RolClaims",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CuentaBaseSOL",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsInactive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentaBaseSOL", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Grupos_GrupoId",
                schema: "manager",
                table: "Clientes",
                column: "GrupoId",
                principalSchema: "manager",
                principalTable: "Grupos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolClaims_Roles_RoleId",
                schema: "manager",
                table: "RolClaims",
                column: "RoleId",
                principalSchema: "manager",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioClaims_Usuarios_UserId",
                schema: "manager",
                table: "UsuarioClaims",
                column: "UserId",
                principalSchema: "manager",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioLogins_Usuarios_UserId",
                schema: "manager",
                table: "UsuarioLogins",
                column: "UserId",
                principalSchema: "manager",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioRoles_Roles_RoleId",
                schema: "manager",
                table: "UsuarioRoles",
                column: "RoleId",
                principalSchema: "manager",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioRoles_Usuarios_UserId",
                schema: "manager",
                table: "UsuarioRoles",
                column: "UserId",
                principalSchema: "manager",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Personas_PersonaId",
                schema: "manager",
                table: "Usuarios",
                column: "PersonaId",
                principalSchema: "manager",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioTokens_Usuarios_UserId",
                schema: "manager",
                table: "UsuarioTokens",
                column: "UserId",
                principalSchema: "manager",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Grupos_GrupoId",
                schema: "manager",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_RolClaims_Roles_RoleId",
                schema: "manager",
                table: "RolClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioClaims_Usuarios_UserId",
                schema: "manager",
                table: "UsuarioClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioLogins_Usuarios_UserId",
                schema: "manager",
                table: "UsuarioLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioRoles_Roles_RoleId",
                schema: "manager",
                table: "UsuarioRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioRoles_Usuarios_UserId",
                schema: "manager",
                table: "UsuarioRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Personas_PersonaId",
                schema: "manager",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioTokens_Usuarios_UserId",
                schema: "manager",
                table: "UsuarioTokens");

            migrationBuilder.DropTable(
                name: "CuentaBaseSOL",
                schema: "manager");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioTokens",
                schema: "manager",
                table: "UsuarioTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                schema: "manager",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioRoles",
                schema: "manager",
                table: "UsuarioRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioLogins",
                schema: "manager",
                table: "UsuarioLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioClaims",
                schema: "manager",
                table: "UsuarioClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                schema: "manager",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolClaims",
                schema: "manager",
                table: "RolClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Personas",
                schema: "manager",
                table: "Personas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Grupos",
                schema: "manager",
                table: "Grupos");

            migrationBuilder.RenameTable(
                name: "UsuarioTokens",
                schema: "manager",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                schema: "manager",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "UsuarioRoles",
                schema: "manager",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "UsuarioLogins",
                schema: "manager",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "UsuarioClaims",
                schema: "manager",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "manager",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "RolClaims",
                schema: "manager",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "Personas",
                schema: "manager",
                newName: "Persona");

            migrationBuilder.RenameTable(
                name: "Grupos",
                schema: "manager",
                newName: "Grupo");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_PersonaId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_PersonaId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioRoles_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioLogins_UserId",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioClaims_UserId",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RolClaims_RoleId",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Persona",
                table: "Persona",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Grupo",
                table: "Grupo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Persona_PersonaId",
                table: "AspNetUsers",
                column: "PersonaId",
                principalTable: "Persona",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Grupo_GrupoId",
                schema: "manager",
                table: "Clientes",
                column: "GrupoId",
                principalTable: "Grupo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
