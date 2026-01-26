using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fidodino.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScoreEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScoreEvents",
                columns: table => new
                {
                    ScoreEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScoreDelta = table.Column<int>(type: "integer", nullable: false),
                    CompositeDelta = table.Column<long>(type: "bigint", nullable: true),
                    TimeRange = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AppliedToRedis = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreEvents", x => x.ScoreEventId);
                    table.ForeignKey(
                        name: "FK_ScoreEvents_GameSessions_GameSessionId",
                        column: x => x.GameSessionId,
                        principalTable: "GameSessions",
                        principalColumn: "GameSessionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoreEvents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEvents_GameSessionId",
                table: "ScoreEvents",
                column: "GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEvents_UserId",
                table: "ScoreEvents",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoreEvents");
        }
    }
}
