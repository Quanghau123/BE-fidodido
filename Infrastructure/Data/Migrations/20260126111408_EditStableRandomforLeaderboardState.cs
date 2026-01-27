using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fidodino.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditStableRandomforLeaderboardState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        -- 1. Fix dữ liệu rỗng / null trước
        UPDATE "LeaderboardStates"
        SET "StableRandom" = '0'
        WHERE "StableRandom" IS NULL
           OR "StableRandom" = '';

        -- 2. Ép kiểu sang integer
        ALTER TABLE "LeaderboardStates"
        ALTER COLUMN "StableRandom" TYPE integer
        USING "StableRandom"::integer;
    """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StableRandom",
                table: "LeaderboardStates",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
