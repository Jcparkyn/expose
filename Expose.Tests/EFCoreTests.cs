namespace Expose.Tests;

using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Xunit.Abstractions;

public class EFCoreTests(ITestOutputHelper output)
{
    [Fact]
    public void Compose_WithEFCore_Where()
    {
        var options = Connect();

        // Expressions to be composed
        Expression<Func<int, bool>> isNegative = x => x < 0;
        Expression<Func<int, int>> mod2 = x => x % 2;

        // Composing the expression
        var wrapped = Expose.Compose(
            (MyEntity e) => isNegative.Call(e.Age) || mod2.Call(e.Age) == 1
        );

        using var context = new MyDbContext(options);
        var result = context.MyEntities
            .Where(wrapped)
            .ToList();

        result.Should().HaveCount(2);
        result.Should().ContainSingle(e => e.Age == -1);
        result.Should().ContainSingle(e => e.Age == 1);
    }

    [Fact]
    public void Compose_WithEFCore_Select()
    {
        var options = Connect();

        Expression<Func<MyEntity, string>> getFullName = e => e.FirstName + " " + e.LastName;

        using var context = new MyDbContext(options);
        var result = context.MyEntities
            .Select(Expose.Compose((MyEntity e) => new
            {
                e.Id,
                FullName = getFullName.Call(e),
            }))
            .Single(e => e.FullName == "Mike Stack");
        result.FullName.Should().Be("Mike Stack");
    }

    private DbContextOptions<MyDbContext> Connect()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        // Setup in-memory database
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseSqlite(connection)
            .UseLoggerFactory(output.BuildLoggerFactory(LogLevel.Information))
            .EnableSensitiveDataLogging()
            .Options;

        using (var context = new MyDbContext(options))
        {
            context.Database.EnsureCreated();
            // Seed data
            context.MyEntities.Add(new MyEntity { Age = -1, FirstName = "Jeff", LastName = "Bridges" });
            context.MyEntities.Add(new MyEntity { Age = 0, FirstName = "Mike", LastName = "Stack" });
            context.MyEntities.Add(new MyEntity { Age = 1, FirstName = "Andy", LastName = "Bennet" });
            context.SaveChanges();
        }

        return options;
    }
}


public class MyEntity
{
    [Key]
    public int Id { get; set; }
    public int Age { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}

public class MyDbContext : DbContext
{
    public DbSet<MyEntity> MyEntities { get; set; }

    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    if (!optionsBuilder.IsConfigured)
    //    {
    //        optionsBuilder.UseSqlite("Data Source=InMemorySample;Mode=Memory;Cache=Shared");
    //    }
    //}
}

record WhyDoINeedToDoThis(ILoggerFactory factory) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return factory.CreateLogger(categoryName);
    }

    public void Dispose()
    {
        factory.Dispose();
    }
}