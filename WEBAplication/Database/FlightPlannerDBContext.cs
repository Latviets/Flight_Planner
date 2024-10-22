using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Database
{
    public class FlightPlannerDBContext: DbContext
    {
        public FlightPlannerDBContext(DbContextOptions<FlightPlannerDBContext> options) : base(options) {}

        public DbSet<Flight> Flights { get; set; }
        public DbSet<Airport> Airports { get; set; }
    }
}
