using Microsoft.EntityFrameworkCore.Migrations;

namespace Saulute.Migrations
{
    public partial class rsichange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IsRequested",
                table: "Rsi",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRequested",
                table: "Rsi");
        }
    }
}
