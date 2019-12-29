using Dapper;
using MySql.Data.MySqlClient;
using Qf.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qf.SysTodoList.Infrastructure
{
    public abstract class BasicQueriesBase
    {
        protected string _connectionString = string.Empty;
        public BasicQueriesBase(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }
        protected IDbConnection GetConnection() { 
            return new MySqlConnection(_connectionString);
        }

        /// <summary>
        /// 分页查询列表
        /// </summary>
        protected async Task<PageDto<T>> GetPageAsync<T>(string tableName, int page = 1, int pageSize = 10, string where = "", string fields = "*", string orderby = "CreationTime DESC")
        {
            using var connection = GetConnection();
            int skip = 0;
            if (page > 0)
            {
                skip = (page - 1) * pageSize;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(1) FROM {0} where {1};", tableName, where);
            sb.AppendFormat("SELECT {0} FROM {1} WHERE {2} ORDER BY {3} LIMIT {4}, {5} ; ", fields, tableName, where, orderby, skip, page * pageSize);
            connection.Open();
            var sql = sb.ToString();
            var reader = await connection.QueryMultipleAsync(sql);
            var result = new PageDto<T>
            {
                Records = reader.ReadFirst<int>(),
                Rows = reader.Read<T>()
            };
            result.Total = (int)Math.Ceiling((double)result.Records / pageSize);
            result.Page = page;
            return result;
        }
        /// <summary>
        /// 查出一条记录的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null)
        {
            using var connection = GetConnection();
            connection.Open();
            var result = await connection.QueryAsync<T>(sql, param);
            if (result == null || result.Count() == 0)  return default(T);
            return result.FirstOrDefault();
        }
        /// <summary>
        /// 查出多条记录的实体泛型集合
        /// </summary>
        /// <typeparam name="T">泛型T</typeparam>
        /// <returns></returns>
        protected async Task<List<T>> QueryAsync<T>(string sql, object param = null)
        {
            using var connection = GetConnection();
            connection.Open();
            var result = await connection.QueryAsync<T>(sql, param);
            if (result == null || result.Count() == 0)  return new List<T>();
            return result.AsList();
        }
    }
}
