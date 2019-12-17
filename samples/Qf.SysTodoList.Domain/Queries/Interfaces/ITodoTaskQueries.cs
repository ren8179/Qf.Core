using Qf.Core;
using Qf.SysTodoList.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qf.SysTodoList.Domain.Queries
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
