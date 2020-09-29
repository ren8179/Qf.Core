using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qf.SysTodoList.WebApi.Controllers
{
    [ApiController]
    [Route(Program.AppName + "/[controller]")]
    public class ToolsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolsController"/> class.
        /// </summary>
        public ToolsController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// 通过网关Ocelot,获取到客户端IP
        /// 需要开启Ocelot中的 UpstreamHeaderTransform 配置项
        /// </summary>
        [HttpGet("getClientIP")]
        [AllowAnonymous]
        public IActionResult GetClientIP()
        {
            var clientIP = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];
            return Ok(clientIP);
        }
    }
}
