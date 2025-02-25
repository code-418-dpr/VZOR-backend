﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WhoCame.Visitors.Application.Database;
using WhoCame.Visitors.Domain;
using WhoCame.Visitors.Infrastructure.Configurations;

namespace WhoCame.Visitors.Infrastructure;

public class VisitorsReadDbContext(IConfiguration configuration): DbContext, IReadDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            .UseLoggerFactory(CreateLoggerFactory)
            .EnableSensitiveDataLogging()
            .UseSnakeCaseNamingConvention()
            .LogTo(Console.WriteLine, LogLevel.Information);
        
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("visitors");
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VisitorsWriteDbContext).Assembly);
    }

    private static readonly ILoggerFactory CreateLoggerFactory
        = LoggerFactory.Create(builder => { builder.AddConsole(); });

    public IQueryable<Visitor> Visitors => Set<Visitor>();
}