using Qf.SysTodoList.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.SysTodoList.Application.Dto
{
    public class TodoTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public TodoType Type { get; set; }
        public TodoStatus Status { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
