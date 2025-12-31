using Microsoft.EntityFrameworkCore;
using PastebinLite.Models;

namespace PastebinLite.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Paste> Pastes => Set<Paste>();
}
