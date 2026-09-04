using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayFinder.Migrations
{
    /// <inheritdoc />
    public partial class Agregar_Tabla_Eventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinoTuristicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdExterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlTicket = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lugar = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Eventos");
        }
    }
}
