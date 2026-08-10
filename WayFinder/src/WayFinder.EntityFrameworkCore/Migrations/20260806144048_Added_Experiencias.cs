using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayFinder.Migrations
{
    /// <inheritdoc />
    public partial class Added_Experiencias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExperienciasViajes",
                table: "ExperienciasViajes");

            migrationBuilder.RenameTable(
                name: "ExperienciasViajes",
                newName: "AppExperienciasViajes");

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "AppExperienciasViajes",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Contenido",
                table: "AppExperienciasViajes",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppExperienciasViajes",
                table: "AppExperienciasViajes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppExperienciasViajes_CreatorId",
                table: "AppExperienciasViajes",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppExperienciasViajes_DestinoTuristicoId",
                table: "AppExperienciasViajes",
                column: "DestinoTuristicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppExperienciasViajes_AbpUsers_CreatorId",
                table: "AppExperienciasViajes",
                column: "CreatorId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppExperienciasViajes_AppDestinosTuristicos_DestinoTuristicoId",
                table: "AppExperienciasViajes",
                column: "DestinoTuristicoId",
                principalTable: "AppDestinosTuristicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppExperienciasViajes_AbpUsers_CreatorId",
                table: "AppExperienciasViajes");

            migrationBuilder.DropForeignKey(
                name: "FK_AppExperienciasViajes_AppDestinosTuristicos_DestinoTuristicoId",
                table: "AppExperienciasViajes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppExperienciasViajes",
                table: "AppExperienciasViajes");

            migrationBuilder.DropIndex(
                name: "IX_AppExperienciasViajes_CreatorId",
                table: "AppExperienciasViajes");

            migrationBuilder.DropIndex(
                name: "IX_AppExperienciasViajes_DestinoTuristicoId",
                table: "AppExperienciasViajes");

            migrationBuilder.RenameTable(
                name: "AppExperienciasViajes",
                newName: "ExperienciasViajes");

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "ExperienciasViajes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Contenido",
                table: "ExperienciasViajes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExperienciasViajes",
                table: "ExperienciasViajes",
                column: "Id");
        }
    }
}
