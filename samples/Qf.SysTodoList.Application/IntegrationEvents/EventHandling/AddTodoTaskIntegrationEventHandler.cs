using DotNetCore.CAP;
using MediatR;
using Microsoft.Extensions.Logging;
using Qf.SysTodoList.Application.Commands;
using Qf.SysTodoList.Application.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qf.SysTodoList.Application.IntegrationEvents.EventHandling
{
    public class AddTodoTaskIntegrationEventHandler : ICapSubscribe
    {
        private readonly ILogger<AddTodoTaskIntegrationEventHandler> _logger;
        private readonly IMediator _mediator;

        public AddTodoTaskIntegrationEventHandler(
            ILogger<AddTodoTaskIntegrationEventHandler> logger,
            IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// 
        /// </summary>
        [CapSubscribe(nameof(AddTodoTaskIntegrationEvent))]
        public async Task Handle(AddTodoTaskIntegrationEvent @event)
        {
            await _mediator.Send(new CreateTodoTaskCommand(@event.Data));
        }
    }
}
