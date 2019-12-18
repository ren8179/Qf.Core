using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Qf.Core.DependencyInjection;
using Qf.Core.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qf.Core.Uow
{
    public class UnitOfWorkInterceptor : QfInterceptor, ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UnitOfWorkDefaultOptions _defaultOptions;

        public UnitOfWorkInterceptor(IUnitOfWorkManager unitOfWorkManager, IOptions<UnitOfWorkDefaultOptions> options)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _defaultOptions = options.Value;
        }

        public override void Intercept(IQfMethodInvocation invocation)
        {
            if (!UnitOfWorkHelper.IsUnitOfWorkMethod(invocation.Method, out var unitOfWorkAttribute))
            {
                invocation.Proceed();
                return;
            }

            using (var uow = _unitOfWorkManager.Begin(CreateOptions(invocation, unitOfWorkAttribute)))
            {
                invocation.Proceed();
                uow.Complete();
            }
        }

        public override async Task InterceptAsync(IQfMethodInvocation invocation)
        {
            if (!UnitOfWorkHelper.IsUnitOfWorkMethod(invocation.Method, out var unitOfWorkAttribute))
            {
                await invocation.ProceedAsync();
                return;
            }

            using (var uow = _unitOfWorkManager.Begin(CreateOptions(invocation, unitOfWorkAttribute)))
            {
                await invocation.ProceedAsync();
                await uow.CompleteAsync();
            }
        }

        private UnitOfWorkOptions CreateOptions(IQfMethodInvocation invocation, [CanBeNull] UnitOfWorkAttribute unitOfWorkAttribute)
        {
            var options = new UnitOfWorkOptions();

            unitOfWorkAttribute?.SetOptions(options);

            if (unitOfWorkAttribute?.IsTransactional == null)
            {
                options.IsTransactional = _defaultOptions.CalculateIsTransactional(
                    autoValue: !invocation.Method.Name.StartsWith("Get", StringComparison.InvariantCultureIgnoreCase)
                );
            }

            return options;
        }
    }
}
