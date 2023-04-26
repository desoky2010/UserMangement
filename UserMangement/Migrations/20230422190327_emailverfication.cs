using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserMangement.Migrations
{
    /// <inheritdoc />
    public partial class emailverfication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpire",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerficationEmailToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpire",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VerficationEmailToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "AspNetUsers");
        }
    }
}
