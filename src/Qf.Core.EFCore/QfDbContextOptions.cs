using JetBrains.Annotations;
using Qf.Core.EFCore.DependencyInjection;
using Qf.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.EFCore
{
    public class QfDbContextOptions
    {
        internal List<Action<DbContextConfigurationContext>> DefaultPreConfigureActions { get; set; }

        internal Action<DbContextConfigurationContext> DefaultConfigureAction { get; set; }

        internal Dictionary<Type, List<object>> PreConfigureActions { get; set; }

        internal Dictionary<Type, object> ConfigureActions { get; set; }

        public QfDbContextOptions()
        {
            DefaultPreConfigureActions = new List<Action<DbContextConfigurationContext>>();
            PreConfigureActions = new Dictionary<Type, List<object>>();
            ConfigureActions = new Dictionary<Type, object>();
        }

        public void PreConfigure([NotNull] Action<DbContextConfigurationContext> action)
        {
            Check.NotNull(action, nameof(action));

            DefaultPreConfigureActions.Add(action);
        }

        public void Configure([NotNull] Action<DbContextConfigurationContext> action)
        {
            Check.NotNull(action, nameof(action));

            DefaultConfigureAction = action;
        }

        public void PreConfigure<TDbContext>([NotNull] Action<DbContextConfigurationContext<TDbContext>> action)
            where TDbContext : QfDbContext<TDbContext>
        {
            Check.NotNull(action, nameof(action));

            var actions = PreConfigureActions.GetOrDefault(typeof(TDbContext));
            if (actions == null)
            {
                PreConfigureActions[typeof(TDbContext)] = actions = new List<object>();
            }

            actions.Add(action);
        }

        public void Configure<TDbContext>([NotNull] Action<DbContextConfigurationContext<TDbContext>> action)
            where TDbContext : QfDbContext<TDbContext>
        {
            Check.NotNull(action, nameof(action));

            ConfigureActions[typeof(TDbContext)] = action;
        }
    }
}
