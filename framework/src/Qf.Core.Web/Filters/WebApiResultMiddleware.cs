using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Qf.Core.Web.Filters
{
    public class WebApiResultMiddleware : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var attrib = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes(typeof(DontWrapResultAttribute), false);
            if (attrib != null && attrib.Length > 0) return;
            if (context.Result == null)
                context.Result = new ObjectResult(new ResultMsg { Code = 404, Msg = "未找到资源", Success = false });
            else
            {
                if (context.Result is BadRequestObjectResult)
                {
                    var errResult = context.Result as BadRequestObjectResult;
                    context.Result = new ObjectResult(new ResultMsg { Code = 400, Msg = "请求失败", Result = errResult.Value, Success = false });
                }
                else if (context.Result is EmptyResult)
                {
                    context.Result = new ObjectResult(new ResultMsg { Code = 404, Msg = "未找到资源", Success = false });
                }
                else if (context.Result is ObjectResult)
                {
                    var objectResult = context.Result as ObjectResult;
                    if (objectResult.Value == null)
                    {
                        context.Result = new ObjectResult(new ResultMsg { Code = 404, Msg = "未找到资源", Success = false });
                    }
                    else if (objectResult.StatusCode >= 400)
                    {
                        context.Result = new ObjectResult(new ResultMsg { Code = 400, Msg = "请求失败", Result = objectResult.Value, Success = false });
                    }
                    else
                    {
                        context.Result = new ObjectResult(new ResultMsg { Code = 200, Msg = "", Result = objectResult.Value, Success = true });
                    }
                }
                else if (context.Result is ContentResult)
                {
                    context.Result = new ObjectResult(new ResultMsg { Code = 200, Msg = "", Result = (context.Result as ContentResult).Content, Success = true });
                }
                else if (context.Result is StatusCodeResult)
                {
                    context.Result = new ObjectResult(new ResultMsg { Code = (context.Result as StatusCodeResult).StatusCode, Msg = "", Success = false });
                }
            }
        }
    }
}
