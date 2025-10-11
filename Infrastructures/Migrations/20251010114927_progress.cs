using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class progress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCourtSessions");

            migrationBuilder.DropColumn(
                name: "Outcome",
                table: "CourtSessions");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "AiAnalysisSummary",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "PrecedentComparisonResult",
                table: "Cases");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AIConversationSessions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AIConversationSessions");

            migrationBuilder.AddColumn<string>(
                name: "Outcome",
                table: "CourtSessions",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Courts",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Courts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AiAnalysisSummary",
                table: "Cases",
                type: "varchar(4000)",
                maxLength: 4000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PrecedentComparisonResult",
                table: "Cases",
                type: "varchar(4000)",
                maxLength: 4000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserCourtSessions",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourtSessionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourtSessions", x => new { x.UserId, x.CourtSessionId });
                    table.ForeignKey(
                        name: "FK_UserCourtSessions_CourtSessions_CourtSessionId",
                        column: x => x.CourtSessionId,
                        principalTable: "CourtSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCourtSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourtSessions_CourtSessionId",
                table: "UserCourtSessions",
                column: "CourtSessionId");
        }
    }
}
