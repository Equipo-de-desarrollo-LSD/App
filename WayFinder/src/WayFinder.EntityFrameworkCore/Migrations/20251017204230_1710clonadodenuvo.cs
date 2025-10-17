using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayFinder.Migrations
{
    /// <inheritdoc />
    public partial class _1710clonadodenuvo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ultimaActualizacion",
                table: "AppDestinosTuristicos",
                newName: "UltimaActualizacion");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AppDestinosTuristicos",
                newName: "Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AppDestinosTuristicos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UltimaActualizacion",
                table: "AppDestinosTuristicos",
                newName: "ultimaActualizacion");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AppDestinosTuristicos",
                newName: "id");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "AppDestinosTuristicos",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
