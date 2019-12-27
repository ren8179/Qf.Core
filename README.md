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

### samples 示例项目解决方案

### 参考项目
* [abp](https://github.com/abpframework/abp)
* [eShopOnContainers](https://github.com/dotnet-architecture/eShopOnContainers)