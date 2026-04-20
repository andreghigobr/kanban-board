using Kanban.Api.Domain.Entities;
using Kanban.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Tests;

public class TestAppDbContext : AppDbContext
{
    private readonly string _dbName;

    public TestAppDbContext(string dbName = "TestDb")
        : base(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(dbName).Options)
    {
        _dbName = dbName;
    }
}
