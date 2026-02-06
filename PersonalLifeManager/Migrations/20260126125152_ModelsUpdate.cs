using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalLifeManager.Migrations
{
    /// <inheritdoc />
    public partial class ModelsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Streak",
                table: "Habits",
                newName: "CurrentStreak");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Habits_UserId",
                table: "Habits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitEntries_HabitId",
                table: "HabitEntries",
                column: "HabitId");

            migrationBuilder.AddForeignKey(
                name: "FK_HabitEntries_Habits_HabitId",
                table: "HabitEntries",
                column: "HabitId",
                principalTable: "Habits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Habits_Users_UserId",
                table: "Habits",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HabitEntries_Habits_HabitId",
                table: "HabitEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Habits_Users_UserId",
                table: "Habits");

            migrationBuilder.DropIndex(
                name: "IX_Habits_UserId",
                table: "Habits");

            migrationBuilder.DropIndex(
                name: "IX_HabitEntries_HabitId",
                table: "HabitEntries");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "CurrentStreak",
                table: "Habits",
                newName: "Streak");
        }
    }
}
