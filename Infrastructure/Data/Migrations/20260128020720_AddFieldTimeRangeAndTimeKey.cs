using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fidodino.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldTimeRangeAndTimeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeKey",
                table: "PlayTurns",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TimeRange",
                table: "PlayTurns",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TimeKey",
                table: "GameSessions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TimeRange",
                table: "GameSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeKey",
                table: "PlayTurns");

            migrationBuilder.DropColumn(
                name: "TimeRange",
                table: "PlayTurns");

            migrationBuilder.DropColumn(
                name: "TimeKey",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "TimeRange",
                table: "GameSessions");
        }
    }
}
