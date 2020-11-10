using Qf.Core;
using Qf.SysTodoList.Application.Dto;
using Qf.SysTodoList.Domain;
using Qf.SysTodoList.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.SysTodoList.Application.Queries
{
    public class TodoTaskQueries : BasicQueriesBase, ITodoTaskQueries
    {
        public TodoTaskQueries(string constr) : base(constr, "TodoTask")
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }
        /// <summary>
        /// 分页查询列表
        /// </summary>
        public async Task<PageDto<TodoTaskDto>> GetPageListAsync(TodoType? type, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var whereStr = "1=1";
            if (type.HasValue)
                whereStr += $" AND Type={(int)type.Value} ";
            return await GetPageListAsync<TodoTaskDto>("TodoTask", page, pageSize, whereStr, cancellationToken: cancellationToken);
        }
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
        public async Task<PageDto<TodoTaskDto>> GetPageListAsync(string fieldName, string keyValue, string startTime, string endTime, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            string whereStr = "1=1";
            if (!string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(keyValue))
            {
                whereStr += $" and {fieldName} like '%{keyValue}%'";
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                whereStr += $" and CreationTime>='{startTime}'";
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                whereStr += $" and CreationTime<='{endTime}'";
            }
            return await GetPageListAsync<TodoTaskDto>("TodoTask", page, pageSize, whereStr, cancellationToken: cancellationToken);
        }
        /// <summary>
        /// 查询详细信息
        /// </summary>
        public async Task<TodoTaskDto> GetModelAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await QueryFirstOrDefaultAsync<TodoTaskDto>("SELECT * FROM TodoTask WHERE Id=@id", new { id }, cancellationToken);
        }
    }
}
