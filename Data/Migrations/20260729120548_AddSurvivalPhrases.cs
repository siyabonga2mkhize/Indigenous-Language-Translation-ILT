using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhraseBookk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSurvivalPhrases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSurvival",
                table: "Phrases",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSurvival",
                table: "Phrases");
        }
    }
}
