using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OmnisNexus.Models;

namespace OmnisNexus.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Community> Communities { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Channel> Channels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Membership>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Membership>()
                .HasOne(m => m.Community)
                .WithMany(c => c.Memberships)
                .HasForeignKey(m => m.CommunityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(m => m.Channel)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Channel>()
                .HasOne(c => c.Community)
                .WithMany(c => c.Channels)
                .HasForeignKey(c => c.CommunityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Community>()
                .HasOne(c => c.Owner)
                .WithMany()
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Membership>()
                .HasIndex(m => new { m.UserId, m.CommunityId })
                .IsUnique();
        }
    }
}
