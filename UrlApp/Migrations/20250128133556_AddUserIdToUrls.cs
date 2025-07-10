using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClickCount",
                table: "Urls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Urls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Urls_UserId",
                table: "Urls",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Urls_Users_UserId",
                table: "Urls",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Urls_Users_UserId",
                table: "Urls");

            migrationBuilder.DropIndex(
                name: "IX_Urls_UserId",
                table: "Urls");

            migrationBuilder.DropColumn(
                name: "ClickCount",
                table: "Urls");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Urls");
        }
    }
}
