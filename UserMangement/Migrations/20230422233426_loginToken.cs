using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserMangement.Migrations
{
    /// <inheritdoc />
    public partial class loginToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoginToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginToken",
                table: "AspNetUsers");
        }
    }
}
