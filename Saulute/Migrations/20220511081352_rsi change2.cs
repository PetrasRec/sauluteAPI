using Microsoft.EntityFrameworkCore.Migrations;

namespace Saulute.Migrations
{
    public partial class rsichange2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BeaconId",
                table: "Rsi",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rsi_BeaconId",
                table: "Rsi",
                column: "BeaconId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rsi_Beacons_BeaconId",
                table: "Rsi",
                column: "BeaconId",
                principalTable: "Beacons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rsi_Beacons_BeaconId",
                table: "Rsi");

            migrationBuilder.DropIndex(
                name: "IX_Rsi_BeaconId",
                table: "Rsi");

            migrationBuilder.DropColumn(
                name: "BeaconId",
                table: "Rsi");
        }
    }
}
