using JITC.config;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JITC.Models
{
    public class JITCSDbContext : IdentityDbContext<User>
    {

        public JITCSDbContext(DbContextOptions<JITCSDbContext> options) : base(options)
        { }




        protected override void OnModelCreating(ModelBuilder builder)
        {

         

            builder.Entity<Helicopter>().HasData(new Helicopter() { HelicopterId= 1, HelicopterName = "Eurocopter AS 355 F1/F2 Ecureuil III", Engine = "Deux turbines du modèle de Rolls Royce 250-C20F", Speed = 220, Fly=0, Size= 5 });
            builder.Entity<Helicopter>().HasData(new Helicopter() { HelicopterId = 2, HelicopterName = "Bell 206 JetRanger", Engine = "une turbine du type Rolls Royce 250-C20B", Speed = 190, Fly = 0, Size = 4 });
            builder.Entity<Helicopter>().HasData(new Helicopter() { HelicopterId = 3, HelicopterName = "Robinson R44 Raven II", Engine = "un piston du type Lycoming modèle IO-540", Speed = 190, Fly = 0, Size = 3 });
            builder.Entity<Helicopter>().HasData(new Helicopter() { HelicopterId = 4, HelicopterName = "SwilaCoptère", Engine = "2 swila turbine Firewall", Speed = 220, Fly = 0, Size = 5 });

            builder.Entity<Airport>().HasData(new Airport() { AirportId = 1, Latitude = "51.193165894", Longitude = "2.858163234", Name ="Oostende" });
            builder.Entity<Airport>().HasData(new Airport() { AirportId = 2, Latitude = "50.90082973", Longitude = "4.483998064", Name = "Bruxelles" });
            builder.Entity<Airport>().HasData(new Airport() { AirportId = 3, Latitude = "50.4108095", Longitude = "4.444643", Name = "Charleroi" });
            builder.Entity<Airport>().HasData(new Airport() { AirportId = 4, Latitude = "50.63583079", Longitude = "5.439331576", Name = "Liège" });

            

            builder.ApplyConfiguration(new AirportConfiguration());

            base.OnModelCreating(builder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Helicopter> Helicopters { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Ticket> Ticket { get; set; }

        public DbSet<Report> Reports { get; set; }


    }
}
