using Microsoft.EntityFrameworkCore.Migrations;

namespace Addicted.Migrations
{
    public partial class AddBeaconClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beacons",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Identification = table.Column<string>(type: "nvarchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beacons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    BeaconId1 = table.Column<string>(nullable: true),
                    Corner1 = table.Column<double>(nullable: false),
                    Corner2 = table.Column<double>(nullable: false),
                    Corner3 = table.Column<double>(nullable: false),
                    Corner4 = table.Column<double>(nullable: false),
                    BeaconId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Beacons_BeaconId",
                        column: x => x.BeaconId,
                        principalTable: "Beacons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rooms_Beacons_BeaconId1",
                        column: x => x.BeaconId1,
                        principalTable: "Beacons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BeaconId",
                table: "Rooms",
                column: "BeaconId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BeaconId1",
                table: "Rooms",
                column: "BeaconId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Beacons");
        }
    }
}
