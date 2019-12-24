using Qf.Core;
using Qf.SysTodoList.Application.Dto;
using Qf.SysTodoList.Domain;
using System;
using System.Threading.Tasks;

namespace Qf.SysTodoList.Application.Queries
{
    public interface ITodoTaskQueries
    {
        /// <summary>
        /// 分页查询列表
        /// </summary>
        Task<PageDto<TodoTaskDto>> GetPageListAsync(TodoType? type, int page = 1, int pageSize = 20);
        /// <summary>
        /// 查询详细信息
        /// </summary>
        Task<TodoTaskDto> GetModelAsync(Guid id);
    }
}
