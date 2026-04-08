# Conversation Handoff Prompt

## 1) 你是谁 / 目标
你是我的编程助手。请先快速理解以下项目背景，并基于“当前任务”继续执行，不要重复让我提供已经写明的信息。

## 2) 项目背景（稳定信息）
- 项目名：HomeCleaning-BookingSystem（后端解决方案：CY.HomeCleaning）
- 技术栈：.NET 8 (LTS) + ABP Framework 8.3.4 + OpenIddict + EF Core + SQL Server/LocalDB
- 运行环境：Windows，PowerShell，LocalDB（MSSQLLocalDB），HTTPS 本地开发证书
- 关键目录结构：
  - src/aspnet-core/src/CY.HomeCleaning.HttpApi.Host（API Host）
  - src/aspnet-core/src/CY.HomeCleaning.DbMigrator（迁移与种子）
  - src/aspnet-core/src/CY.HomeCleaning.Domain（领域逻辑、OpenIddict 种子、微信认证服务）
  - src/aspnet-core/src/CY.HomeCleaning.Application.Contracts（权限定义）
  - src/aspnet-core/PLAN.md（阶段计划与任务勾选清单）
- 已知约束（规范/兼容性/性能/安全）：
  - 不启用 SaaS 租户隔离，采用 Role 区分 B/C 端。
  - 统一由 OpenIddict 发放 Token。
  - 目前微信登录允许 Mock 模式（本地联调），后续切真实 AppId/AppSecret。
  - 角色权限通过种子落库，策略由 API Host 注册。

## 3) 当前任务（本次要继续的核心）
- 任务标题：对话交接文档落地（将本轮架构与鉴权改动完整沉淀到 handoff 模板）
- 期望结果（Definition of Done）：
  - PROMPT.md 已被完整填写，包含已完成内容、关键决策、关键文件和下一步执行指令。
  - 下次会话可直接按“下一步指令”继续开发，不再重复背景说明。
- 非目标（明确这次不做什么）：
  - 不在本步骤继续新增业务实体或数据库迁移。
  - 不在本步骤切换微信真实凭证。

## 4) 已完成进展（上次对话结论）
- [x] 已完成 1：ABP 后端骨架初始化（CY.HomeCleaning），并迁移到 src/aspnet-core 根目录。
- [x] 已完成 2：LocalDB 配置与 DbMigrator 迁移成功（数据库 CYHomeCleaning）。
- [x] 已完成 3：OpenIddict password grant 联调成功（HomeCleaning_Dev 客户端）。
- [x] 已完成 4：角色/权限体系落地（admin/operator/customer）+ 策略授权探针接口验证（200/403）。
- [x] 已完成 5：微信小程序扩展授权 wechat_miniapp 实现并实测成功下发 token。
- [ ] 未完成 1：将微信 MiniApp 从 Mock 模式切换到真实 AppId/AppSecret。
- [ ] 未完成 2：进入 Phase 2 核心业务表建模（ServiceItem/CapacitySchedule/Order/Coupon）。

## 5) 关键决策与原因（避免重复争论）
- 决策 A：  
  - 方案：不启用 Tenant，多租户关闭，采用 Role 进行 B/C 端隔离。
  - 原因：当前是自营业务，不是多商家 SaaS，角色隔离复杂度更低。
  - 备选方案为何放弃：Tenant 会增加租户数据边界、运维和权限复杂度，当前阶段收益不足。
- 决策 B：
  - 方案：OpenIddict 统一鉴权；B 端使用 password grant（开发阶段），C 端采用自定义 wechat_miniapp grant。
  - 原因：统一 token 体系，便于后续多端扩展；微信登录路径独立且符合小程序 code 换身份的业务流程。

- 决策 C：
  - 方案：本地阶段保留微信 Mock 模式（EnableMockMode=true）。
  - 原因：可以脱离微信外网依赖，先打通端到端鉴权链路。

## 6) 关键上下文（高价值信息）
- 关键报错原文：
  - The ConnectionString property has not been initialized.
  - Unable to locate a Local Database Runtime installation.
  - 文件被 CY.HomeCleaning.HttpApi.Host 进程锁定导致 MSB3021/MSB3027。
  - 扩展授权最初放在 Domain 层导致缺少 AspNetCore/OpenIddict Server 相关依赖。
- 关键接口/函数签名：
  - ITokenExtensionGrant: Task<IActionResult> HandleAsync(ExtensionGrantContext context)
  - WeChatMiniAppTokenExtensionGrant.Name = "wechat_miniapp"
  - IWeChatMiniAppAuthService.LoginByCodeAsync(string code)
- 关键配置项：
  - ConnectionStrings:Default=Server=(LocalDb)\\MSSQLLocalDB;Database=CYHomeCleaning;...
  - OpenIddict:Applications:HomeCleaning_Dev
  - OpenIddict:Applications:HomeCleaning_WeChatMiniApp
  - WeChat:MiniApp:EnableMockMode=true
- 依赖版本（非常重要）：
  - .NET SDK 9.x（编译），目标框架 net8.0
  - ABP 8.3.4
  - OpenIddict（ABP 集成）
- 外部限制（API 配额、权限、网络）：
  - 当前可本地联调，不依赖微信公网（Mock 模式）。
  - 若切真实微信接口，需要外网可达 api.weixin.qq.com 及合法 AppId/AppSecret。

## 7) 相关文件与位置（让助手快速定位）
- src/aspnet-core/src/CY.HomeCleaning.Application.Contracts/Permissions/HomeCleaningPermissions.cs：权限常量定义（Backoffice/Customer）。
- src/aspnet-core/src/CY.HomeCleaning.Application.Contracts/Permissions/HomeCleaningPermissionDefinitionProvider.cs：权限树注册。
- src/aspnet-core/src/CY.HomeCleaning.Domain/Data/HomeCleaningRolePermissionDataSeedContributor.cs：角色创建与权限种子。
- src/aspnet-core/src/CY.HomeCleaning.Domain/OpenIddict/OpenIddictDataSeedContributor.cs：OpenIddict 客户端种子（Swagger/Dev/WeChatMiniApp）。
- src/aspnet-core/src/CY.HomeCleaning.HttpApi.Host/HomeCleaningHttpApiHostModule.cs：策略授权 + 扩展 grant 注册。
- src/aspnet-core/src/CY.HomeCleaning.HttpApi.Host/OpenIddict/WeChatMiniAppTokenExtensionGrant.cs：微信扩展授权处理器。
- src/aspnet-core/src/CY.HomeCleaning.Domain/WeChat/WeChatMiniAppAuthService.cs：微信 code 换取身份（含 Mock 模式）。
- src/aspnet-core/src/CY.HomeCleaning.HttpApi.Host/Controllers/SecurityProbeController.cs：策略授权验证接口。
- src/aspnet-core/src/CY.HomeCleaning.DbMigrator/appsettings.json：OpenIddict 客户端与微信配置种子来源。
- src/aspnet-core/src/CY.HomeCleaning.HttpApi.Host/appsettings.json：Host 运行时连接串与微信配置。
- src/aspnet-core/PLAN.md：任务总览与已勾选进度清单。

## 8) 待办清单（按优先级）
1. P0：切换微信真实配置（AppId/AppSecret），关闭 EnableMockMode，并完成真实 code 联调。
2. P1：开始 Phase 2 核心业务实体与迁移（ServiceItem/CapacitySchedule/Order/CouponTemplate/UserCoupon）。
3. P2：基于 Customer 角色新增“我的订单”等最小业务闭环 API，并接入权限控制。

## 9) 你接下来应当如何回答（输出偏好）
- 先给“最小可执行方案”，再给可选优化。
- 每次改动请输出：
  1) 修改了哪些文件  
  2) 改动摘要  
  3) 验证步骤  
  4) 回滚方式
- 如信息不足，先提出最多 3 个关键问题，不要泛泛追问。

## 10) 下一步指令（每次续聊只改这里）
请基于上述上下文，继续完成：  
“将 WeChat MiniApp 鉴权从 Mock 切换到真实配置：更新配置读取与安全存储方案（secrets/env），补充真实 code 联调步骤，并在通过后把 PLAN.md 对应待办项改为已完成。”