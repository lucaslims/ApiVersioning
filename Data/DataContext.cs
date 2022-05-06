using Microsoft.EntityFrameworkCore;
using ApiVersioning.Models;

namespace ApiVersioning.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<AppUser> AppUsers { get; set; }
}