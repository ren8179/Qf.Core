using MediatR;
using Microsoft.Extensions.Logging;
using Qf.Core.AutoMapper;
using Qf.Core.Infrastructure;
using Qf.SysTodoList.Application.Commands;
using Qf.SysTodoList.Application.Dto;
using Qf.SysTodoList.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.SysTodoList.Application.Handlers
{
    public class CreateTodoTaskCommandHandler : IRequestHandler<CreateTodoTaskCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IObjectMapper _objectMapper;
        private readonly IRepository<TodoTask, Guid> _repository;

        public CreateTodoTaskCommandHandler(ILogger<CreateTodoTaskCommandHandler> logger,
            IObjectMapper objectMapper,
            IRepository<TodoTask, Guid> repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _objectMapper = objectMapper ?? throw new ArgumentNullException(nameof(objectMapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<bool> Handle(CreateTodoTaskCommand message, CancellationToken cancellationToken)
        {
            var model = _objectMapper.Map<CreateTodoTaskInput, TodoTask>(message.Data);
            model.CreationTime = DateTime.Now;
            await _repository.AddAsync(model, autoSave: true, cancellationToken);
            return true;
        }
    }
}
