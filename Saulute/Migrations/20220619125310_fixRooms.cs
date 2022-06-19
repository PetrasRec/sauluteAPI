using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Saulute.Migrations
{
    public partial class fixRooms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rsi");

            migrationBuilder.AddColumn<int>(
                name: "SupervisedUserId",
                table: "UserRooms",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupervisedUserId1",
                table: "UserRooms",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRooms_SupervisedUserId",
                table: "UserRooms",
                column: "SupervisedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRooms_SupervisedUserId1",
                table: "UserRooms",
                column: "SupervisedUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRooms_SupervisedUsers_SupervisedUserId",
                table: "UserRooms",
                column: "SupervisedUserId",
                principalTable: "SupervisedUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRooms_SupervisedUsers_SupervisedUserId1",
                table: "UserRooms",
                column: "SupervisedUserId1",
                principalTable: "SupervisedUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRooms_SupervisedUsers_SupervisedUserId",
                table: "UserRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRooms_SupervisedUsers_SupervisedUserId1",
                table: "UserRooms");

            migrationBuilder.DropIndex(
                name: "IX_UserRooms_SupervisedUserId",
                table: "UserRooms");

            migrationBuilder.DropIndex(
                name: "IX_UserRooms_SupervisedUserId1",
                table: "UserRooms");

            migrationBuilder.DropColumn(
                name: "SupervisedUserId",
                table: "UserRooms");

            migrationBuilder.DropColumn(
                name: "SupervisedUserId1",
                table: "UserRooms");

            migrationBuilder.CreateTable(
                name: "Rsi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BeaconId = table.Column<int>(type: "int", nullable: true),
                    IsRequested = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rsi = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rsi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rsi_Beacons_BeaconId",
                        column: x => x.BeaconId,
                        principalTable: "Beacons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rsi_BeaconId",
                table: "Rsi",
                column: "BeaconId");
        }
    }
}
