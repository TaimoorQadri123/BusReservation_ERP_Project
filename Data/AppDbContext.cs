using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BusReservationERP.Models;

namespace BusReservationERP.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Bus> Buses { get; set; }
        public DbSet<BusRoute> Routes { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public DbSet<RouteStop> RouteStops { get; set; }
        public DbSet<City> Cities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.TripId, b.SeatNumber })
                .IsUnique()
                .HasFilter("[Bookingstatus] <> 'Cancelled' ");

            modelBuilder.Entity<Trip>()
                .Property(t => t.Fare)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<BusRoute>()
                .HasOne(r => r.OriginCity)
                .WithMany()
                .HasForeignKey(r => r.OriginCityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusRoute>()
                .HasOne(r => r.DestinationCity)
                .WithMany()
                .HasForeignKey(r => r.DestinationCityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.BoardingStop)
                .WithMany()
                .HasForeignKey(b => b.BoardingStopId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.DropStop)
                .WithMany()
                .HasForeignKey(b => b.DropStopId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}