using MediatR;
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
        public string Title { get; set; }
        [DataMember]
        public TodoType Type { get; set; }
        [DataMember]
        public TodoStatus Status { get; set; }
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
