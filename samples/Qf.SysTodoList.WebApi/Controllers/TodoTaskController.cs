using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Qf.SysTodoList.Domain;
using Qf.SysTodoList.Application.Commands;
using Qf.SysTodoList.Application.Dto;
using Qf.SysTodoList.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Qf.SysTodoList.WebApi.Controllers
{
    [ApiController]
    [Route(Program.AppName + "/[controller]")]
    public class TodoTaskController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        private readonly ITodoTaskQueries _queries;

        /// <summary>
        /// Initializes a new instance of the <see cref="TodoTaskController"/> class.
        /// </summary>
        public TodoTaskController(IMediator mediator,
            ITodoTaskQueries queries,
            ILogger<TodoTaskController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        [HttpGet("getpagelist")]
        [ProducesResponseType(typeof(IEnumerable<TodoTaskDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<TodoTaskDto>>> GetPageListAsync(TodoType? type, int page = 1, int pageSize = 20)
        {
            var list = await _queries.GetPageListAsync(type, page, pageSize);
            return Ok(list);
        }
        /// <summary>
        /// 查询收货地址详细信息
        /// </summary>
        [HttpGet("getmodel")]
        [ProducesResponseType(typeof(TodoTaskDto), (int)HttpStatusCode.OK)]
        public async Task<TodoTaskDto> GetModelAsync(Guid id)
        {
            var model = await _queries.GetModelAsync(id);
            return model;
        }
        /// <summary>
        /// 创建任务
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTodoTaskCommand input)
        {
            var commandResult = await _mediator.Send(input);
            if (!commandResult) return BadRequest();
            return Ok(commandResult);
        }

        /// <summary>
        /// 测试自动添加
        /// </summary>
        /// <returns></returns>
        [HttpGet("TestAdd")]
        public async Task<IActionResult> TestAdd()
        {
            try
            {
                var model = new TodoTask
                {
                    CreationTime = DateTime.Now,
                    Id = Guid.NewGuid(),
                    Status = TodoStatus.Default,
                    Title = "测试AddCommand",
                    Type = TodoType.NearTerm
                };
                var command = new AddCommand { Model = model };
                await _mediator.Send(command);
                return Ok("成功");
            }
            catch (Exception e)
            {
                return Ok("error");
            }
        }
    }
}
