using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalLifeManager.Migrations
{
    /// <inheritdoc />
    public partial class HabitEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Habits",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "HabitEntries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "HabitEntries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "HabitEntries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "HabitEntries");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Habits",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
