using Dapper;
using JetBrains.Annotations;
using Microsoft.Data.SqlClient;
using Qf.Core;
using Qf.Core.Extensions;
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

        protected BasicQueriesBase(string constr, string tableName,string fields = "*")
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
            DefaultTableName = tableName;
            DefaultFields = fields;
        }
        protected IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// 分页查询列表
        /// </summary>
        protected async Task<PageDto<T>> GetPageAsync<T>(int page = 1, int pageSize = 10, string where = "", string tableName = null, string fields = null, string sort = "CreationTime DESC", CancellationToken cancellationToken = default)
        {
            string proName = "ProcGetPageData";
            var p = new DynamicParameters();
            p.Add("TableName", tableName ?? DefaultTableName);
            p.Add("PrimaryKey", "Id");
            p.Add("Fields", fields ?? DefaultFields);
            p.Add("Condition", " 1=1 " + where);
            p.Add("CurrentPage", page);
            p.Add("PageSize", pageSize);
            p.Add("Sort", sort);
            p.Add("RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = GetConnection();
            connection.Open();
            var result = new PageDto<T>
            {
                Rows = await connection.QueryAsync<T>(proName, p, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut),
                Records = p.Get<int>("RecordCount"),
            };
            result.Total = (int)Math.Ceiling((double)result.Records / pageSize);
            result.Page = page;
            return result;
        }
        /// <summary>
        /// 分页查询列表
        /// </summary>
        protected async Task<PageDto<T>> GetPageAsync<T>([NotNull] DynamicParameters para, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            Check.NotNull(para, nameof(para));
            string proName = "ProcGetPageData";
            para.Add("RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = GetConnection();
            connection.Open();
            var result = new PageDto<T>
            {
                Rows = await connection.QueryAsync<T>(proName, para, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut),
                Records = para.Get<int>("RecordCount"),
            };
            result.Total = (int)Math.Ceiling((double)result.Records / pageSize);
            result.Page = page;
            return result;
        }
        /// <summary>
        /// 通用分页方法
        /// </summary>
        /// <param name="fields">列</param>
        /// <param name="tableName">表</param>
        /// <param name="where">条件</param>
        /// <param name="orderby">排序</param>
        /// <param name="page">当前页</param>
        /// <param name="pageSize">当前页显示条数</param>
        /// <returns></returns>
        protected async Task<PageDto<T>> GetPageListAsync<T>(string tableName, int page = 1, int pageSize = 10, string where = "", string fields = "*", string orderby = "CreationTime DESC", CancellationToken cancellationToken = default)
        {
            int skip = 1;
            if (page > 0)
            {
                skip = (page - 1) * pageSize + 1;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(1) FROM {0} where {1};", tableName, where);
            sb.AppendFormat(@"SELECT  {0}
                                FROM(SELECT ROW_NUMBER() OVER(ORDER BY {3}) AS RowNum,{0}
                                          FROM  {1}
                                          WHERE {2}
                                        ) AS result
                                WHERE  RowNum >= {4}   AND RowNum <= {5}
                                ORDER BY {3}", fields, tableName, where, orderby, skip, page * pageSize);
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = GetConnection();
            connection.Open();
            var reader = await connection.QueryMultipleAsync(sb.ToString(), commandTimeout: TimeOut);
            var result = new PageDto<T>
            {
                Records = reader.ReadFirst<int>(),
                Rows = reader.Read<T>(),
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
            var sql = $"SELECT {fields ?? DefaultFields} FROM {tableName ?? DefaultTableName} WITH(NOLOCK) {where}";
            return await QueryFirstOrDefaultAsync<T>(sql, param, cancellationToken);
        }
        /// <summary>
        /// 查出一条记录的实体
        /// </summary>
        protected async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = GetConnection();
            connection.Open();
            var result = await connection.QueryAsync<T>(sql, param, commandTimeout: TimeOut);
            if (result == null || result.Count() == 0) return default(T);
            return result.FirstOrDefault();
        }
        /// <summary>
        /// 查出多条记录的实体泛型集合
        /// </summary>
        protected async Task<List<T>> QueryAsync<T>(string tableName = null, string fields = null, string where = "", object param = null, CancellationToken cancellationToken = default)
        {
            var sql = $"SELECT {fields ?? DefaultFields} FROM {tableName ?? DefaultTableName} WITH(NOLOCK) {where}";
            return await QueryAsync<T>(sql, param, cancellationToken);
        }
        /// <summary>
        /// 查出多条记录的实体泛型集合
        /// </summary>
        protected async Task<List<T>> QueryAsync<T>([NotNull] string sql, [NotNull] object param, CancellationToken cancellationToken = default)
        {
            Check.NotNull(param, nameof(param));
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = GetConnection();
            connection.Open();
            var result = await connection.QueryAsync<T>(sql, param, commandTimeout: TimeOut);
            if (result == null || result.Count() == 0) return new List<T>();
            return result.AsList();
        }
    }
}
