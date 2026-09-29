using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusReservationERP.Migrations
{
    /// <inheritdoc />
    public partial class AddFilteredUniqueIndexForBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_TripId_SeatNumber",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TripId_SeatNumber",
                table: "Bookings",
                columns: new[] { "TripId", "SeatNumber" },
                unique: true,
                filter: "[Bookingstatus] <> 'Cancelled' ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_TripId_SeatNumber",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TripId_SeatNumber",
                table: "Bookings",
                columns: new[] { "TripId", "SeatNumber" },
                unique: true);
        }
    }
}
