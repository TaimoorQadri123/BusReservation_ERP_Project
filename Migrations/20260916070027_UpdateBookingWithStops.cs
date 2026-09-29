using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusReservationERP.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingWithStops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoardingStopId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DropStopId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BoardingStopId",
                table: "Bookings",
                column: "BoardingStopId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DropStopId",
                table: "Bookings",
                column: "DropStopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_RouteStops_BoardingStopId",
                table: "Bookings",
                column: "BoardingStopId",
                principalTable: "RouteStops",
                principalColumn: "RouteStopId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_RouteStops_DropStopId",
                table: "Bookings",
                column: "DropStopId",
                principalTable: "RouteStops",
                principalColumn: "RouteStopId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_RouteStops_BoardingStopId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_RouteStops_DropStopId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_BoardingStopId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_DropStopId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BoardingStopId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DropStopId",
                table: "Bookings");
        }
    }
}
