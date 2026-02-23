using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalLifeManager.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToHabitEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_HabitEntries_UserId_HabitId_Date",
                table: "HabitEntries",
                columns: new[] { "UserId", "HabitId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HabitEntries_UserId_HabitId_Date",
                table: "HabitEntries");
        }
    }
}
