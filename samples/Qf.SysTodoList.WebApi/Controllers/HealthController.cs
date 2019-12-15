using Microsoft.AspNetCore.Authorization;
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
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configurationRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthController"/> class.
        /// </summary>
        public HealthController(IConfiguration configuration)
        {
            _configurationRoot = configuration;
        }
        /// <summary>
        /// 检查服务状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get() => Ok("ok");

        /// <summary>
        /// 配置信息
        /// </summary>
        [HttpGet("config/{key}")]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetValueForKey(string key)
        {
            return Ok(_configurationRoot[key]);
        }
    }
}
