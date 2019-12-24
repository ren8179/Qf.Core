using Qf.Core;
using Qf.SysTodoList.Application.Dto;
using Qf.SysTodoList.Domain;
using Qf.SysTodoList.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Qf.SysTodoList.Application.Queries
{
    public class TodoTaskQueries : BasicQueriesBase, ITodoTaskQueries
    {
        public TodoTaskQueries(string constr) : base(constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }
        /// <summary>
        /// 分页查询列表
        /// </summary>
        public async Task<PageDto<TodoTaskDto>> GetPageListAsync(TodoType? type, int page = 1, int pageSize = 20)
        {
            var whereStr = "1=1";
            if (type.HasValue)
                whereStr += $" AND Type={(int)type.Value} ";
            return await GetPageListAsync<TodoTaskDto>("TodoTask", page, pageSize, whereStr);
        }
        /// <summary>
        /// 查询详细信息
        /// </summary>
        public async Task<TodoTaskDto> GetModelAsync(Guid id)
        {
            return await QueryFirstOrDefaultAsync<TodoTaskDto>("SELECT * FROM TodoTask WHERE Id=@id", new { id });
        }
    }
}
