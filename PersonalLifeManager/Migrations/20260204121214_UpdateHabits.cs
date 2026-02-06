using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalLifeManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHabits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdDeleted",
                table: "Habits",
                newName: "IsDeleted");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Habits",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Habits",
                newName: "IdDeleted");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Habits",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
