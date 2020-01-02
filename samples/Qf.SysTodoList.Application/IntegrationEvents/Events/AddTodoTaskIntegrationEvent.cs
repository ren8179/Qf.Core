using Qf.SysTodoList.Application.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.SysTodoList.Application.IntegrationEvents.Events
{
    public class AddTodoTaskIntegrationEvent
    {
        public CreateTodoTaskInput Data { get; set; }

        public AddTodoTaskIntegrationEvent(CreateTodoTaskInput data)
        {
            Data = data;
        }
    }
}
