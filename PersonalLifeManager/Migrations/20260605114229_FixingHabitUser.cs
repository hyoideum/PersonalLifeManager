using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalLifeManager.Migrations
{
    /// <inheritdoc />
    public partial class FixingHabitUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habits_AspNetUsers_AppUserId",
                table: "Habits");

            migrationBuilder.DropIndex(
                name: "IX_Habits_AppUserId",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Habits");

            migrationBuilder.CreateIndex(
                name: "IX_Habits_UserId",
                table: "Habits",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Habits_AspNetUsers_UserId",
                table: "Habits",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habits_AspNetUsers_UserId",
                table: "Habits");

            migrationBuilder.DropIndex(
                name: "IX_Habits_UserId",
                table: "Habits");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Habits",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Habits_AppUserId",
                table: "Habits",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Habits_AspNetUsers_AppUserId",
                table: "Habits",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
