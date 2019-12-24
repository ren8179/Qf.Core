using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Qf.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.Core.EFCore
{
    public abstract class QfDbContext<TDbContext> : DbContext, IEfCoreDbContext, ITransientDependency
        where TDbContext : DbContext
    {
        public ILogger<QfDbContext<TDbContext>> Logger { get; set; }

        protected QfDbContext(DbContextOptions<TDbContext> options)
            : base(options)
        {
            Logger = NullLogger<QfDbContext<TDbContext>>.Instance;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            try
            {
                var result = base.SaveChanges(acceptAllChangesOnSuccess);

                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new EPTException(ex.Message, ex);
            }
            finally
            {
                ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            try
            {
                
                var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new EPTException(ex.Message, ex);
            }
            finally
            {
                ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

    }
}
