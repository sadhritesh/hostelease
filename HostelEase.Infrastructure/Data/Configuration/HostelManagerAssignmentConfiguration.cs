using HostelEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace HostelEase.Infrastructure.Data.Configuration
{
    public class HostelManagerAssignmentConfiguration : IEntityTypeConfiguration<HostelManagerAssignment>
    {
        public void Configure(EntityTypeBuilder<HostelManagerAssignment> builder)
        {
            builder.ToTable("HostelManagerAssignments");

            builder.HasKey(x => x.AssignmentId);

            builder.Property(x => x.HostelId).IsRequired();

            builder.Property(x => x.ManagerUserId).IsRequired();

            builder.Property(x => x.AssignedDate)
                .HasDefaultValueSql("GETUTCDATE()");
            
            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);
            
            //relation bw HostelManagerAssignment and Hostel
            builder.HasOne(x => x.Hostel)
                .WithMany(x => x.ManagerAssignments)
                .HasForeignKey(x => x.HostelId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
