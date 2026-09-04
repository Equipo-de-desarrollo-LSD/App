using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayFinder.Migrations
{
    /// <inheritdoc />
    public partial class Agregar_IndiceUnico_Eventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Eventos",
                table: "Eventos");

            migrationBuilder.RenameTable(
                name: "Eventos",
                newName: "AppEventos");

            migrationBuilder.AlterColumn<string>(
                name: "IdExterno",
                table: "AppEventos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppEventos",
                table: "AppEventos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppEventos_DestinoTuristicoId_IdExterno",
                table: "AppEventos",
                columns: new[] { "DestinoTuristicoId", "IdExterno" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppEventos",
                table: "AppEventos");

            migrationBuilder.DropIndex(
                name: "IX_AppEventos_DestinoTuristicoId_IdExterno",
                table: "AppEventos");

            migrationBuilder.RenameTable(
                name: "AppEventos",
                newName: "Eventos");

            migrationBuilder.AlterColumn<string>(
                name: "IdExterno",
                table: "Eventos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Eventos",
                table: "Eventos",
                column: "Id");
        }
    }
}
