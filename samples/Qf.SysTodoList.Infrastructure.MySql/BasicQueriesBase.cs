using Dapper;
using MySql.Data.MySqlClient;
using Qf.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qf.SysTodoList.Infrastructure
{
    public abstract class BasicQueriesBase
    {
        /// <summary>
        /// 超时时间 单位:秒
        /// </summary>
        private const int TimeOut = 120;
        /// <summary>
        /// 默认显示字段
        /// </summary>
        protected readonly string DefaultFields = "*";
        /// <summary>
        /// 默认表名
        /// </summary>
        protected readonly string DefaultTableName = "";

        protected string _connectionString = string.Empty;
        public BasicQueriesBase(string constr, string tableName, string fields = "*")
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
            DefaultTableName = tableName;
            DefaultFields = fields;
        }
        protected IDbConnection GetConnection() { 
            return new MySqlConnection(_connectionString);
        }

        /// <summary>
        /// 分页查询列表
        /// </summary>
        protected async Task<PageDto<T>> GetPageAsync<T>(string tableName, int page = 1, int pageSize = 10, string where = "", string fields = "*", string orderby = "CreationTime DESC", CancellationToken cancellationToken = default)
        {
            int skip = 0;
            if (page > 0)
            {
                skip = (page - 1) * pageSize;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(1) FROM {0} where {1};", tableName, where);
            sb.AppendFormat("SELECT {0} FROM {1} WHERE {2} ORDER BY {3} LIMIT {4}, {5} ; ", fields, tableName, where, orderby, skip, page * pageSize);
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = GetConnection();
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
        protected async Task<T> QueryFirstOrDefaultAsync<T>(string tableName = null, string fields = null, string where = "", object param = null, CancellationToken cancellationToken = default)
        {
            var sql = $"SELECT {fields ?? DefaultFields} FROM {tableName ?? DefaultTableName} {where};";
            return await QueryFirstOrDefaultAsync<T>(sql, param, cancellationToken);
        }
        /// <summary>
        /// 查出一条记录的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, CancellationToken cancellationToken = default)
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
        protected async Task<List<T>> QueryAsync<T>(string sql, object param = null, CancellationToken cancellationToken = default)
        {
            using var connection = GetConnection();
            connection.Open();
            var result = await connection.QueryAsync<T>(sql, param);
            if (result == null || result.Count() == 0)  return new List<T>();
            return result.AsList();
        }
    }
}
