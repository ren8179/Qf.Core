using MediatR;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Qf.SysTodoList.Domain.Commands
{
    [DataContract]
    public class CreateTodoTaskCommand : IRequest<bool>
    {
        [DataMember]
        public string Title { get; private set; }
        [DataMember]
        public TodoType Type { get; private set; }
        [DataMember]
        public TodoStatus Status { get; private set; }
        public CreateTodoTaskCommand()
        {
        }
        public CreateTodoTaskCommand(string title, TodoType type, TodoStatus status)
        {
            Title = title;
            Type = type;
            Status = status;
        }
    }
}
