using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manager.API.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "manager");

            migrationBuilder.CreateTable(
                name: "CuentaBaseSOLs",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ruc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsInactive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentaBaseSOLs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsInactive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApePaterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApeMaterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsInactive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TokenBases",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CuentaBaseSolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsInactive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenBases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenBases_CuentaBaseSOLs_CuentaBaseSolId",
                        column: x => x.CuentaBaseSolId,
                        principalSchema: "manager",
                        principalTable: "CuentaBaseSOLs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ruc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Razonsocial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dt_registro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrupoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsInactive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalSchema: "manager",
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalSchema: "manager",
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolClaims",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "manager",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsInactive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "manager",
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioClaims",
                schema: "manager",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioClaims_Usuarios_UserId",
                        column: x => x.UserId,
                        principalSchema: "manager",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioLogins",
                schema: "manager",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UsuarioLogins_Usuarios_UserId",
                        column: x => x.UserId,
                        principalSchema: "manager",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRoles",
                schema: "manager",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "manager",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Usuarios_UserId",
                        column: x => x.UserId,
                        principalSchema: "manager",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioTokens",
                schema: "manager",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UsuarioTokens_Usuarios_UserId",
                        column: x => x.UserId,
                        principalSchema: "manager",
                        principalTable: "Usuarios",
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
                    BiGravadoDG = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    IgvDG = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    BiGravadoDGNG = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    IgvDGNG = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    BiGravadoDNG = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    IgvDNG = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    ValorAdqNG = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    Isc = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    Icbper = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    OtrosTributos = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    Total = table.Column<decimal>(type: "decimal(12,4)", precision: 12, scale: 4, nullable: true),
                    Moneda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoCambio = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    FechaEmisionMod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoCPMod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerieCPMod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodDam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroCPMod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Clasificacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdProyecto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PorcPart = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    Imb = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    CarOrigen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Detraccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoNota = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoComprobante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Incal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Clus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TieneGlosa = table.Column<bool>(type: "bit", nullable: false),
                    Glosa = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "IX_Clientes_GrupoId",
                schema: "manager",
                table: "Clientes",
                column: "GrupoId");

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

            migrationBuilder.CreateIndex(
                name: "IX_RolClaims_RoleId",
                schema: "manager",
                table: "RolClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "manager",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ClienteId",
                schema: "manager",
                table: "Tickets",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenBases_CuentaBaseSolId",
                schema: "manager",
                table: "TokenBases",
                column: "CuentaBaseSolId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_ClienteId",
                schema: "manager",
                table: "Tokens",
                column: "ClienteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioClaims_UserId",
                schema: "manager",
                table: "UsuarioClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioLogins_UserId",
                schema: "manager",
                table: "UsuarioLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_RoleId",
                schema: "manager",
                table: "UsuarioRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "manager",
                table: "Usuarios",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PersonaId",
                schema: "manager",
                table: "Usuarios",
                column: "PersonaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "manager",
                table: "Usuarios",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comprobantes",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "RolClaims",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "Tickets",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "TokenBases",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "UsuarioClaims",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "UsuarioLogins",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "UsuarioRoles",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "UsuarioTokens",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "Pertributarios",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "CuentaBaseSOLs",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "Usuarios",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "Clientes",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "Personas",
                schema: "manager");

            migrationBuilder.DropTable(
                name: "Grupos",
                schema: "manager");
        }
    }
}
