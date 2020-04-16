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
        /// 分页查询列表
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="keyValue">属性值</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="page">当前页</param>
        /// <param name="pageSize">每页显示的条数</param>
        /// <returns></returns>
        Task<PageDto<TodoTaskDto>> GetPageListAsync(string fieldName, string keyValue, string startTime, string endTime, int page = 1, int pageSize = 20);
        /// <summary>
        /// 查询详细信息
        /// </summary>
        Task<TodoTaskDto> GetModelAsync(Guid id);
    }
}
