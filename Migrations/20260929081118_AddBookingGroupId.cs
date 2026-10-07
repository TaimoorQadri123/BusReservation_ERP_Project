using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusReservationERP.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingGroupId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookingGroupId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingGroupId",
                table: "Bookings");
        }
    }
}
