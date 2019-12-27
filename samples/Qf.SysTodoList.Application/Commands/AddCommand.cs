using MediatR;
using Qf.SysTodoList.Domain;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Qf.SysTodoList.Application.Commands
{
    [DataContract]
    public class AddCommand : IRequest<bool>
    {
        public dynamic Model { get; set; }
    }
}
