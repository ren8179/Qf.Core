using Dapper;
using Microsoft.Data.SqlClient;
using Qf.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Qf.SysTodoList.Application.Queries
{
    public abstract class BasicQueriesBase
    {
        protected string _connectionString = string.Empty;
        public BasicQueriesBase(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }
        /// <summary>
        /// 分页查询列表
        /// </summary>
        protected async Task<PageDto<T>> GetPageAsync<T>(string tableName, int page = 1, int pageSize = 10, string where = "", string fields = "*")
        {
            using var connection = new SqlConnection(_connectionString);
            var p = new DynamicParameters();
            string proName = "ProcGetPageData";
            p.Add("TableName", tableName);
            p.Add("PrimaryKey", "Id");
            p.Add("Fields", fields);
            p.Add("Condition", " 1=1 " + where);
            p.Add("CurrentPage", page);
            p.Add("PageSize", pageSize);
            p.Add("Sort", "CreationTime DESC");
            p.Add("RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            connection.Open();
            var result = new PageDto<T>
            {
                Rows = await connection.QueryAsync<T>(proName, p, commandType: CommandType.StoredProcedure),
                Records = p.Get<int>("RecordCount"),
            };
            result.Total = (int)Math.Ceiling((double)result.Records / pageSize);
            result.Page = page;
            return result;
        }
    }
}
