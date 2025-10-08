using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayFinder.Migrations
{
    /// <inheritdoc />
    public partial class Created_Destinos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "coordenadas_latitud",
                table: "AppDestinosTuristicos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "coordenadas_longitud",
                table: "AppDestinosTuristicos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "pais_nombre",
                table: "AppDestinosTuristicos",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "pais_poblacion",
                table: "AppDestinosTuristicos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "coordenadas_latitud",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "coordenadas_longitud",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "pais_nombre",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "pais_poblacion",
                table: "AppDestinosTuristicos");
        }
    }
}
