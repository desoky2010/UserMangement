using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserMangement.Migrations
{
    /// <inheritdoc />
    public partial class UserBlock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BlockedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "UserBlock",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserBlock",
                table: "AspNetUsers");
        }
    }
}
