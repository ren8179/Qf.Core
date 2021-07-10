# Qf.Core

### 介绍
基于.net 5 的微服务开发框架,使用简化的DDD+CQRS设计

<p align="center">
  <img  src="https://raw.githubusercontent.com/ren8179/Qf.Core/master/doc/DDD%2BCQRS%E5%9F%BA%E7%A1%80%E6%A1%86%E6%9E%B6%E7%A4%BA%E6%84%8F%E5%9B%BE.png">
</p>

### framework 微服务框架解决方案
* **Qf.Core**

    核心类库,部分代码参考自[abp](https://github.com/abpframework/abp)项目,用来实现框架的基础功能

    你可以运行以下命令在你的项目中使用类库
    > PM> Install-Package Qf.Core -Version 2.0.1.2

* **Qf.Core.AutoMapper**

    基于AutoMapper的对象映射,具体用法请查看 [Qf.Core.AutoMapper的用法](https://github.com/ren8179/Qf.Core/wiki/Qf.Core.AutoMapper%E7%9A%84%E7%94%A8%E6%B3%95)
    
    你可以运行以下命令在你的项目中使用类库
    > PM> Install-Package Qf.Core.AutoMapper -Version 2.0.1.2

* **Qf.Core.EFCore**

    基于EntityFrameworkCore的仓储基类,默认使用统一工作单元,自动注入默认仓储.
    
    你可以运行以下命令在你的项目中使用类库
    > PM> Install-Package Qf.Core.EFCore -Version 2.0.1.2

* **Qf.Core.Web**

    asp.net core mvc 项目扩展,添加了微信登录([WeChat](https://github.com/ren8179/Qf.Core/tree/master/framework/src/Qf.Core.Web/Authentication/WeChat)),自定义授权认证([BearerAuthorize](https://github.com/ren8179/Qf.Core/tree/master/framework/src/Qf.Core.Web/Authorization)),全局异常处理([ErrorHandling](https://github.com/ren8179/Qf.Core/blob/master/framework/src/Qf.Core.Web/Extension/ErrorHandlingExtensions.cs)),返回值封装([WebApiResult](https://github.com/ren8179/Qf.Core/tree/master/framework/src/Qf.Core.Web/Filters)))
    
    你可以运行以下命令在你的项目中使用类库
    > PM> Install-Package Qf.Core.Web -Version 2.0.1.2

* **Qf.Extensions.Configuration.Encryption**

  asp.net core mvc 项目扩展,提供配置文件加密功能
  
  你可以运行以下命令在你的项目中使用类库
  > PM> Install-Package Qf.Extensions.Configuration.Encryption -Version 1.0.3.3
  
  用法如下:
  
```
            var builder = new ConfigurationBuilder();
            if (isNotDebug)
            {
                builder.AddEncryptionFile("appsettings.encryption", optional: false, reloadOnChange: true);
            }
            else
            {
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            }
            return builder.Build();
```
   
### gateway Ocelot网关
* **Qf.APIGateway** 基于Ocelot实现的API网关
* **Qf.HttpReports** 基于HttpReports.Dashboard实现的API接口请求监控服务,建议部署为独立的站点

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

### 捐赠
如果您觉得 **Qf.Core项目** 对您有所帮助，可以请作者媳妇儿喝一杯咖啡

![微信赞赏码](https://github.com/ren8179/blog/blob/master/wxzsm.jpg)
