using Dapper;
using Microsoft.Data.SqlClient;
using Qf.Core;
using Qf.SysTodoList.Application.Dto;
using Qf.SysTodoList.Domain;
using System;
using System.Linq;
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
            var whereStr = " ";
            if (type.HasValue)
                whereStr += $" Type={(int)type.Value} ";
            return await GetPageAsync<TodoTaskDto>("TodoTask", page, pageSize, whereStr);
        }
        /// <summary>
        /// 查询详细信息
        /// </summary>
        public async Task<TodoTaskDto> GetModelAsync(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var result = await connection.QueryAsync<TodoTaskDto>(
                "SELECT * FROM TodoTask WHERE Id=@id", new { id });
            if (result == null || result.Count() == 0)
                return null;
            return result.FirstOrDefault();
        }
    }
}
