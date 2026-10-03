using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiClinica.Migrations
{
    /// <inheritdoc />
    public partial class RenomeiaDataParaDataHora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Consultas",
                newName: "DataHora");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataHora",
                table: "Consultas",
                newName: "Data");
        }
    }
}
