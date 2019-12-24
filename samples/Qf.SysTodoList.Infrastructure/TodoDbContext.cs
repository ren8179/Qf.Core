using Microsoft.EntityFrameworkCore;
using Qf.Core.EFCore;
using Qf.SysTodoList.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.SysTodoList.Infrastructure
{
    [ConnectionStringName("Default")]
    public class TodoDbContext : QfDbContext<TodoDbContext>
    {
        public DbSet<TodoTask> Users { get; set; }
        public TodoDbContext(DbContextOptions<TodoDbContext> options)
           : base(options)
        {

        }

    }
}
