using Microsoft.EntityFrameworkCore;
using TuneSpot.Model;

public class MusicDbContext : DbContext
{
    public DbSet<Song> Songs { get; set; }
    public DbSet<Fingerprint> Fingerprints { get; set; }

    // Constructor for DI (inject in services)
    public MusicDbContext(DbContextOptions<MusicDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fingerprint>().HasKey(f => f.Id);
        modelBuilder.Entity<Song>().HasKey(s => s.Id);
        modelBuilder.Entity<Fingerprint>().HasIndex(f => f.Hash);  // This ensures fast lookups
    }
}