using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Qf.Core.Extensions;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qf.Core.Web.Extension
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            this.next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            Check.NotNull(context, nameof(context));
            try
            {
                context.Request.EnableBuffering();
                await next(context).ConfigureAwait(false);
            }
            catch (EPTException ex)
            {
                HandleException(context.Response, 200, ex.Message);
                await LogRequestInfo(context.Request,ex.Message);
            }
            catch (Exception ex)
            {
                var statusCode = context.Response.StatusCode;
                if (ex is ArgumentException) statusCode = 200;
                HandleException(context.Response, statusCode, ex.Message);
                await LogRequestInfo(context.Request, ex.Message,ex);
            }
            finally
            {
                var statusCode = context.Response.StatusCode;
                if (statusCode >= 400)
                {
                    var msg = GetMsg(statusCode);
                    if (!string.IsNullOrEmpty(msg))
                        HandleException(context.Response, statusCode, msg);
                }
            }
        }

        private void HandleException(HttpResponse response, int statusCode, string msg)
        {
            if (!response.HasStarted)
            {
                response.ContentType = "application/json;charset=utf-8";
                response.WriteAsync(JsonSerializer.Serialize(new ResultMsg
                {
                    Code = statusCode,
                    Success = false,
                    Msg = msg
                }).ToLower());
            }
        }
        private string GetMsg(int statusCode)
        {
            switch (statusCode)
            {
                case 400: return "请求失败";
                case 401: return "未授权";
                case 404: return "未找到服务";
                case 502: return "无效响应";
                default: return "未知错误";
            }
        }
        private async Task LogRequestInfo(HttpRequest request, string errMsg, Exception ex = null)
        {
            var msg = "";
            if (request != null && request.Path != null && !request.Path.Value.Contains("swagger"))
            {
                var body = await ReadRequestBodyAsync(request.BodyReader).ConfigureAwait(false);
                msg = $"[{request.Method}] {request.Path}{request.QueryString} {body}";
            }
            if (ex == null)
                _logger.LogWarning($"{msg} \r\n {errMsg}");
            else
                _logger.LogError(ex, $"{msg} \r\n {errMsg}");
        }

        private async Task<string> ReadRequestBodyAsync(PipeReader reader)
        {
            var result = "";
            while (true)
            {
                ReadResult readResult = await reader.ReadAsync();
                var buffer = readResult.Buffer;
                if (buffer.IsEmpty)
                    break;
                result = AsString(buffer);

                // At this point, buffer will be updated to point one byte after the last
                // \n character.
                reader.AdvanceTo(buffer.Start, buffer.End);

                if (readResult.IsCompleted)
                {
                    break;
                }
            }

            return result;
        }
        private static string AsString(in ReadOnlySequence<byte> readOnlySequence)
        {
            // Separate method because Span/ReadOnlySpan cannot be used in async methods
            ReadOnlySpan<byte> span = readOnlySequence.ToArray().AsSpan();
            return Encoding.UTF8.GetString(span);
        }
    }
    public static class ErrorHandlingExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
