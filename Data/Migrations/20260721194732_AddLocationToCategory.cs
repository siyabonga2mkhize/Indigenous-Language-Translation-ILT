using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhraseBookk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapLocationName",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "MapLocationName",
                table: "Categories");
        }
    }
}
