using Microsoft.EntityFrameworkCore;
using TwitterStub.Models;

namespace TwitterStub.Repository
{
    public class TwitterStubContext : DbContext
    {
        public TwitterStubContext()
        {

        }

        public TwitterStubContext(DbContextOptions<TwitterStubContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tweet>(entity =>
            {
                entity.Property(e => e.text).HasMaxLength(1000);
                entity.HasKey(x => x.id);
                entity.HasIndex(x => x.created_at);
                entity.HasIndex(x => x.text);
            });

            modelBuilder.Entity<Entities>().
                HasKey(x => x.entities_id);

            modelBuilder.Entity<Url>(entity =>
            {
                entity.HasKey(x => x.url_id);
                entity.HasIndex(x => x.expanded_url);
                entity.HasIndex(x => x.display_url);
            });

            modelBuilder.Entity<Hashtag>(entity =>
            {
                entity.HasKey(x => x.hashtag_id);
                entity.HasIndex(x => x.tag);
            });



            base.OnModelCreating(modelBuilder);

        }

        public DbSet<Tweet> Tweets { get; set; }

        public DbSet<Url> Urls { get; set; }

        public DbSet<Hashtag> Hashtags { get; set; }
    }
}
