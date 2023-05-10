using Microsoft.EntityFrameworkCore;

namespace MovieSearch.Models;

// WTF
public class MovieDb : DbContext
{
    public MovieDb(DbContextOptions<MovieDb> options) : base(options) { }

    public DbSet<MovieInfo> Infos => Set<MovieInfo>();
}
