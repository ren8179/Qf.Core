using AutoMapper;
using Qf.SysTodoList.Application.Dto;
using Qf.SysTodoList.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.SysTodoList.Application
{
    public class ApplicationAutoMapperProfile : Profile
    {
        public ApplicationAutoMapperProfile()
        {
            CreateMap<CreateTodoTaskInput, TodoTask>();
        }
    }
}
