using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Qf.Core.EFCore;
using Qf.Core.EFCore.DependencyInjection;
using System;

namespace Qf.SysTodoList.Infrastructure
{
    public static class DbContextConfigurationContextSqlServerExtensions
    {
        public static DbContextOptionsBuilder UseSqlServer(
            [NotNull] this DbContextConfigurationContext context,
            [CanBeNull] Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null)
        {
            if (context.ExistingConnection != null)
            {
                return context.DbContextOptions.UseSqlServer(context.ExistingConnection, sqlServerOptionsAction);
            }
            else
            {
                return context.DbContextOptions.UseSqlServer(context.ConnectionString, sqlServerOptionsAction);
            }
        }
    }
    public static class QfDbContextOptionsSqlServerExtensions
    {
        public static void UseSqlServer(
            [NotNull] this QfDbContextOptions options,
            [CanBeNull] Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null)
        {
            options.PreConfigure(context =>
            {
                context.DbContextOptions.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning));
            });
            options.Configure(context =>
            {
                context.UseSqlServer(sqlServerOptionsAction);
            });
        }

        public static void UseSqlServer<TDbContext>(
            [NotNull] this QfDbContextOptions options,
            [CanBeNull] Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null)
            where TDbContext : QfDbContext<TDbContext>
        {
            options.PreConfigure(context =>
            {
                context.DbContextOptions.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning));
            });
            options.Configure<TDbContext>(context =>
            {
                context.UseSqlServer(sqlServerOptionsAction);
            });
        }
    }
}
