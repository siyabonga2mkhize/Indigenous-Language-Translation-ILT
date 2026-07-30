using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhraseBookk.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedDateToFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SavedDate",
                table: "Favorites",
                newName: "CreatedDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Favorites",
                newName: "SavedDate");
        }
    }
}
