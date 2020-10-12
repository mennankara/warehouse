using System;
using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Internals.Repository.DataAccess;

namespace Warehouse.Domain.Tests.RepositoryTests.DataAccessMocks
{
    internal class DbContextMock : WarehouseDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }
    }
}