using Microsoft.EntityFrameworkCore;
using OmnisNexus.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace OmnisNexus.Tests
{
    internal class TestDbHelper
    {
        public static (ApplicationDbContext Db, TestDbContextFactory Factory) CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var db = new ApplicationDbContext(options);
            var factory = new TestDbContextFactory(options);

            return (db, factory);
        }
    }
}
