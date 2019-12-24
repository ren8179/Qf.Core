using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Qf.Core.EFCore;
using Qf.Core.EFCore.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.SysTodoList.Infrastructure.MySql
{
    public static class DbContextConfigurationContextMySQLExtensions
    {
        public static DbContextOptionsBuilder UseMySQL(
           [NotNull] this DbContextConfigurationContext context,
           [CanBeNull] Action<MySqlDbContextOptionsBuilder> mySQLOptionsAction = null)
        {
            if (context.ExistingConnection != null)
            {
                return context.DbContextOptions.UseMySql(context.ExistingConnection, mySQLOptionsAction);
            }
            else
            {
                return context.DbContextOptions.UseMySql(context.ConnectionString, mySQLOptionsAction);
            }
        }
    }
    public static class QfDbContextOptionsMySQLExtensions
    {
        public static void UseMySQL(
                [NotNull] this QfDbContextOptions options,
                [CanBeNull] Action<MySqlDbContextOptionsBuilder> mySQLOptionsAction = null)
        {
            options.PreConfigure(context =>
            {
                context.DbContextOptions.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning));
            });
            options.Configure(context =>
            {
                context.UseMySQL(mySQLOptionsAction);
            });
        }

        public static void UseMySQL<TDbContext>(
            [NotNull] this QfDbContextOptions options,
            [CanBeNull] Action<MySqlDbContextOptionsBuilder> mySQLOptionsAction = null)
            where TDbContext : QfDbContext<TDbContext>
        {
            options.PreConfigure(context =>
            {
                context.DbContextOptions.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning));
            });
            options.Configure<TDbContext>(context =>
            {
                context.UseMySQL(mySQLOptionsAction);
            });
        }
    }
}
