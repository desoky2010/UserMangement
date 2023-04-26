using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserMangement.Migrations
{
    /// <inheritdoc />
    public partial class loginexpire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LoginTokenExpire",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginTokenExpire",
                table: "AspNetUsers");
        }
    }
}
