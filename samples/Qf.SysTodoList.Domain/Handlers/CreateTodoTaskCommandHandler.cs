using MediatR;
using Microsoft.Extensions.Logging;
using Qf.Core.Infrastructure;
using Qf.SysTodoList.Domain.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.SysTodoList.Domain.Handlers
{
    public class CreateTodoTaskCommandHandler : IRequestHandler<CreateTodoTaskCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IRepository<TodoTask, Guid> _repository;

        public CreateTodoTaskCommandHandler(ILogger<CreateTodoTaskCommandHandler> logger,
            IRepository<TodoTask, Guid> repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<bool> Handle(CreateTodoTaskCommand message, CancellationToken cancellationToken)
        {
            var model = new TodoTask
            {
                Title = message.Title,
                Type = message.Type,
                Status = message.Status,
                CreationTime = DateTime.Now
            };
            await _repository.AddAsync(model, autoSave: true, cancellationToken);
            return true;
        }
    }
}
