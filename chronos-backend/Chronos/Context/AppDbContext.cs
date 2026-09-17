using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronos.Models;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
        {

        }

        public DbSet<EmployeeModel> employees { get; set; }
        public DbSet<UserModel> User { get; set; }
    }
}
