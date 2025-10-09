using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class fixCert7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Lawyers_LawyerId1",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_LawyerId1",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "LawyerId1",
                table: "Certificates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LawyerId1",
                table: "Certificates",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_LawyerId1",
                table: "Certificates",
                column: "LawyerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Lawyers_LawyerId1",
                table: "Certificates",
                column: "LawyerId1",
                principalTable: "Lawyers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}