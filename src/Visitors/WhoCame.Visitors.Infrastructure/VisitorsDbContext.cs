using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WhoCame.Visitors.Domain;
using WhoCame.Visitors.Infrastructure.Configurations;

namespace WhoCame.Visitors.Infrastructure;

public class VisitorsDbContext(IConfiguration configuration): DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            .UseLoggerFactory(CreateLoggerFactory)
            .EnableSensitiveDataLogging()
            .UseSnakeCaseNamingConvention()
            .LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("visitors");
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VisitorsDbContext).Assembly);
    }

    private static readonly ILoggerFactory CreateLoggerFactory
        = LoggerFactory.Create(builder => { builder.AddConsole(); });
    
    public DbSet<Visitor> Visitors { get; set; } = null!;
}