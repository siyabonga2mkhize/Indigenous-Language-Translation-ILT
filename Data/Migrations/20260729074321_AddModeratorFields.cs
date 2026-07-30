using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhraseBookk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModeratorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsModerator",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModeratorLanguage",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsModerator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ModeratorLanguage",
                table: "AspNetUsers");
        }
    }
}
