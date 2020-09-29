using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Qf.SysTodoList.Application.Commands;
using Qf.SysTodoList.Application.Dto;
using Qf.SysTodoList.Application.Queries;
using Qf.SysTodoList.Domain;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
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

        /// <summary>
        /// 获取分页列表
        /// </summary>
        [HttpGet("getpagelist")]
        [ProducesResponseType(typeof(IEnumerable<TodoTaskDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<TodoTaskDto>>> GetPageListAsync(TodoType? type, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var list = await _queries.GetPageListAsync(type, page, pageSize, cancellationToken);
            return Ok(list);
        }
        /// <summary>
        /// 获取任务详情
        /// </summary>
        [HttpGet("getmodel")]
        [ProducesResponseType(typeof(TodoTaskDto), (int)HttpStatusCode.OK)]
        public async Task<TodoTaskDto> GetModelAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var model = await _queries.GetModelAsync(id, cancellationToken);
            return model;
        }
        /// <summary>
        /// 创建任务
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTodoTaskInput input, CancellationToken cancellationToken = default)
        {
            var commandResult = await _mediator.Send(new CreateTodoTaskCommand(input), cancellationToken);
            if (!commandResult) return BadRequest();
            return Ok(commandResult);
        }

        /// <summary>
        /// 测试自动添加
        /// </summary>
        /// <returns></returns>
        [HttpGet("TestAdd")]
        public async Task<IActionResult> TestAdd(CancellationToken cancellationToken = default)
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
                await _mediator.Send(command, cancellationToken);
                return Ok("成功");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "TestAdd");
                return Ok("error");
            }
        }
    }
}
