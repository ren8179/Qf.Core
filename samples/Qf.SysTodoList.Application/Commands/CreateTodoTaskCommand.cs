using MediatR;
using Qf.SysTodoList.Application.Dto;
using Qf.SysTodoList.Domain;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Qf.SysTodoList.Application.Commands
{
    [DataContract]
    public class CreateTodoTaskCommand : IRequest<bool>
    {
        [DataMember]
        public CreateTodoTaskInput Data { get; private set; }
        public CreateTodoTaskCommand(CreateTodoTaskInput data)
        {
            Data = data;
        }
    }
}
