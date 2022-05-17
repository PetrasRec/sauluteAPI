using Microsoft.EntityFrameworkCore.Migrations;

namespace Saulute.Migrations
{
    public partial class UserRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRooms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    BeaconId = table.Column<int>(nullable: false),
                    RoomId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<string>(nullable: true),
                    Corner1 = table.Column<double>(nullable: false),
                    Corner2 = table.Column<double>(nullable: false),
                    Corner3 = table.Column<double>(nullable: false),
                    Corner4 = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRooms_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRooms_OwnerId",
                table: "UserRooms",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRooms");
        }
    }
}
