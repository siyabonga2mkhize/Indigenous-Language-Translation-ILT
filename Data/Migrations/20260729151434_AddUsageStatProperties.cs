using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhraseBookk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsageStatProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsageStats_Categories_CategoryId",
                table: "UsageStats");

            migrationBuilder.DropIndex(
                name: "IX_UsageStats_CategoryId",
                table: "UsageStats");

            migrationBuilder.RenameColumn(
                name: "ViewedAt",
                table: "UsageStats",
                newName: "Timestamp");

            migrationBuilder.AlterColumn<string>(
                name: "LanguageSelected",
                table: "UsageStats",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "UsageStats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "UsageStats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "UsageStats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "UsageStats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhraseId",
                table: "UsageStats",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalApprovedTranslations",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsageStats_PhraseId",
                table: "UsageStats",
                column: "PhraseId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsageStats_Phrases_PhraseId",
                table: "UsageStats",
                column: "PhraseId",
                principalTable: "Phrases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsageStats_Phrases_PhraseId",
                table: "UsageStats");

            migrationBuilder.DropIndex(
                name: "IX_UsageStats_PhraseId",
                table: "UsageStats");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "UsageStats");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "UsageStats");

            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "UsageStats");

            migrationBuilder.DropColumn(
                name: "PhraseId",
                table: "UsageStats");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "UsageStats",
                newName: "ViewedAt");

            migrationBuilder.AlterColumn<int>(
                name: "LanguageSelected",
                table: "UsageStats",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "UsageStats",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalApprovedTranslations",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_UsageStats_CategoryId",
                table: "UsageStats",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsageStats_Categories_CategoryId",
                table: "UsageStats",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
