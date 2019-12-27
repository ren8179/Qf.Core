# Qf.Core

### 介绍
基于.net core 3.1 的微服务开发框架,使用简化的DDD+CQRS设计

<p align="center">
  <img  src="https://raw.githubusercontent.com/ren8179/Qf.Core/master/doc/DDD%2BCQRS%E5%9F%BA%E7%A1%80%E6%A1%86%E6%9E%B6%E7%A4%BA%E6%84%8F%E5%9B%BE.png">
</p>

### framework 微服务框架解决方案
* **Qf.Core**

    核心类库,部分代码参考自[abp](https://github.com/abpframework/abp)项目,用来实现框架的基础功能
* **Qf.Core.EFCore**

    基于EntityFrameworkCore的仓储基类,默认使用统一工作单元,自动注入默认仓储.
* **Qf.Core.Web**

    asp.net core mvc 项目扩展,添加了微信登录([WeChat](https://github.com/ren8179/Qf.Core/tree/master/framework/src/Qf.Core.Web/Authentication/WeChat)),自定义授权认证([BearerAuthorize](https://github.com/ren8179/Qf.Core/tree/master/framework/src/Qf.Core.Web/Authorization)),全局异常处理([ErrorHandling](https://github.com/ren8179/Qf.Core/blob/master/framework/src/Qf.Core.Web/Extension/ErrorHandlingExtensions.cs)),返回值封装([WebApiResult](https://github.com/ren8179/Qf.Core/tree/master/framework/src/Qf.Core.Web/Filters)))

### samples 示例项目TodoList解决方案
* **Qf.SysTodoList.Application** 应用层
* **Qf.SysTodoList.Domain** 领域层
* **Qf.SysTodoList.Infrastructure** 基础设施层(默认基于SqlServer实现)
* **Qf.SysTodoList.Infrastructure.MySql** 基于MySql的基础设施层
* **Qf.SysTodoList.Web** 用户前端
* **Qf.SysTodoList.WebApi** 数据接口

### 如何开始
你可以参考示例项目TodoList解决方案的项目分层,新建你自己的解决方案,也可以直接复制samples文件夹下的所有内容,然后重命名,添加你自己的领域对象.

- 在正式运行WebApi项目之前,请修改 `appsettings.json` 配置文件中的数据库连接字符串

- WebApi项目启动后,访问 `/swagger/index.html` 路径来查看接口文档

<p align="center">
  <img  src="https://raw.githubusercontent.com/ren8179/Qf.Core/master/doc/todolist-swagger.png">
</p>

### 参考项目
* [abp](https://github.com/abpframework/abp)
* [eShopOnContainers](https://github.com/dotnet-architecture/eShopOnContainers)