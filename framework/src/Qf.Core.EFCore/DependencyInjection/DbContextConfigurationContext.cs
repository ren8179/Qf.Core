using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Qf.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Qf.Core.EFCore.DependencyInjection
{
    public class DbContextConfigurationContext : IServiceProviderAccessor
    {
        public IServiceProvider ServiceProvider { get; }

        public string ConnectionString { get; }

        public string ConnectionStringName { get; }

        public DbConnection ExistingConnection { get; }

        public DbContextOptionsBuilder DbContextOptions { get; protected set; }

        public DbContextConfigurationContext(
            [NotNull] string connectionString,
            [NotNull] IServiceProvider serviceProvider,
            [CanBeNull] string connectionStringName,
            [CanBeNull]DbConnection existingConnection)
        {
            ConnectionString = connectionString;
            ServiceProvider = serviceProvider;
            ConnectionStringName = connectionStringName;
            ExistingConnection = existingConnection;

            DbContextOptions = new DbContextOptionsBuilder();
        }
    }
    public class DbContextConfigurationContext<TDbContext> : DbContextConfigurationContext
        where TDbContext : QfDbContext<TDbContext>
    {
        public new DbContextOptionsBuilder<TDbContext> DbContextOptions => (DbContextOptionsBuilder<TDbContext>)base.DbContextOptions;

        public DbContextConfigurationContext(
            string connectionString,
            [NotNull] IServiceProvider serviceProvider,
            [CanBeNull] string connectionStringName,
            [CanBeNull] DbConnection existingConnection)
            : base(
                  connectionString,
                  serviceProvider,
                  connectionStringName,
                  existingConnection)
        {
            base.DbContextOptions = new DbContextOptionsBuilder<TDbContext>();
        }
    }
}
