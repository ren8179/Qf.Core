using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Qf.Core.Infrastructure;
using Qf.SysTodoList.Application.Commands;
using Qf.SysTodoList.Domain;
using Qf.SysTodoList.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Qf.SysTodoList.Application.Handlers
{
    public class AddCommandHandler : IRequestHandler<AddCommand, bool>
    {
        private readonly ILogger _logger; 
        private readonly TodoDbContext _dbContext;

        public AddCommandHandler(ILogger<AddCommandHandler> logger,
            IRepository<TodoTask, Guid> repository, TodoDbContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
            _dbContext = dbContext;
        }
        public async Task<bool> Handle(AddCommand request, CancellationToken cancellationToken)
        {
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            try
            {
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        await _dbContext.AddAsync(request.Model);
                        _dbContext.SaveChanges();
                        transaction.Commit();
                    }
                });
            }
            catch (Exception e)
            {
                _logger.LogError(" OrderActionLogCommandHandler 操作异常" + e.Message);
                return false;

            }
            return true;
        }
    }
}
