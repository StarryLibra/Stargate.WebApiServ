![Logo](docs/Images/Stargate.WebApiServ-Logo-ReadMe.jpg)

# Stargate.WebApiServ - 文档
> 供查询数据的 HTTP(S) WebAPI 网站。

## 特殊功能链接：
#### Hello World!
/
#### 欢迎页面
/welcome
#### Swagger文档
/swagger
/swagger/index.html
/swagger/v1/swagger.json
#### 健康检查(HealthCheck)
/healthz
/healthz/ready
#### 日志面板
/logdashboard
#### (迷你)性能监控
/mini-profiler-resources/results-index
/mini-profiler-resources/results-list
/mini-profiler-resources/results
/mini-profiler-resources/results?id={guid of specific profiler}

## 技术简介：
*   核心框架：.NET 6.0, ASP.NET Core Runtime 6.0
*   持久层框架：EntityFramework Core 6.0
*   日志框架：Serilog 2.10, LogDashboard 1.4
*   性能监控：MiniProfiler 4.2
*   RESTful接口文档：Swashbuckle AspNetCore 6.2

### 许可证
[MIT许可证](LICENSE)
