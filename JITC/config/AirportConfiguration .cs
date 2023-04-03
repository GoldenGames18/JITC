using JITC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JITC.config
{
    public class AirportConfiguration : IEntityTypeConfiguration<Models.Route>
    {
        public void Configure(EntityTypeBuilder<Models.Route> builder)
        {
            builder.HasOne(r => r.StartAirport)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.EndAirport)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
