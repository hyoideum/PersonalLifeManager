using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalLifeManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHabit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Habits",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IdDeleted",
                table: "Habits",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "IdDeleted",
                table: "Habits");
        }
    }
}
