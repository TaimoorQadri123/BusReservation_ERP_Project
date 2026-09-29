using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusReservationERP.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRouteWithCities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationLocation",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "OriginLocation",
                table: "Routes");

            migrationBuilder.AddColumn<int>(
                name: "DestinationCityId",
                table: "Routes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OriginCityId",
                table: "Routes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DestinationCityId",
                table: "Routes",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginCityId",
                table: "Routes",
                column: "OriginCityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Cities_DestinationCityId",
                table: "Routes",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Cities_OriginCityId",
                table: "Routes",
                column: "OriginCityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Cities_DestinationCityId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Cities_OriginCityId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_DestinationCityId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_OriginCityId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "DestinationCityId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "OriginCityId",
                table: "Routes");

            migrationBuilder.AddColumn<string>(
                name: "DestinationLocation",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginLocation",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
