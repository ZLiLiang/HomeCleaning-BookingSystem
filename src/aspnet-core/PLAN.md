# 后端技术架构与核心业务表设计方案

基于 .NET 8 LTS 与 ABP Framework 构建模块化后端，采用 OpenIddict 统一多端鉴权机制。本系统为自营家政服务平台，不采用 SaaS 多租户（Tenant）隔离机制，直接通过基于角色的权限控制（Role-Based Access Control）来划分 B 端管理员、服务人员以及 C 端消费用户的资源权限。

## 阶段与执行步骤

1. **[Phase 1] 基础环境预研与搭建**
   - 创建基于 .NET 8 (LTS) 和 ABP Framework (v8.x) 的初始空项目。
   - 数据库初始化配置：连接 SQL Server 2016 实例，启用 EF Core 核心功能与自动审计日志（Audit Logging）。

2. **[Phase 1] 多端鉴权系统接入 (OpenIddict)**
   - **B 端权限管理**：利用 ABP 原生集成的 OpenIddict，采用标准 OAuth 2.0 框架下发 Token，通过授权 `admin` / `cleaner` 等角色角色控制页面和接口。
   - **C 端微信授权**：以自定义扩展（Extension Grant Type）方式接入 OpenIddict 管道。服务器接收前端发送的 `code`，调用微信接口获取 `openid`，后台直接将其静默注册/登录并自动授予 `Customer` 角色，完成一键Token颁发。

3. **[Phase 2] 核心业务模型与数据库设计 (DDD)**
   - `ServiceItem` (服务项目表)：名称、基础服务价、计费单位类型（工时/面积/件）、图文介绍资源链接、关联适用的退款规则 `RefundRuleId`。
   - `CapacitySchedule` (运力排期表)：日期（Date）、细分时段（TimeSlot，如 09:00-11:00）、单时段接待上限人数（MaxCapacity）、已占用配额（UsedCapacity），并强制附加 EF Core 提供的 `byte[] RowVersion` 并发控制锁定字段。
   - `Order` (服务订单表)：存储订单号、挂载用户ID、服务项目ID、具体预约的上门日期与时段、计价金额和实收金额、枚举状态（等待付款、已生效、待评价、售后退款中等）。将订单发起时的项目快照资产（产品当时的名称、适用的动态违约金规则、已核销的券面信息等）用单独的一个 `SnapshotData (JSON)` 列落库记录。
   - `CouponEntity` (卡券体系模型组)：分为 `CouponTemplate` (面值规则、使用起止期、适用项目约束) 和 `UserCoupon` (具体客户领券映射，需包含未使用/已过期/已占用订单编号等状态字段)。

## 阶段决策点
- **权限边界区分：** 剔除多租户设计，统一由 OpenIddict 结合 ABP User Roles 主导。
- **快照持久方案：** 选择单表 JSON 长文本列存储订单当时的交易商品与退款规则，取代强关联快照表，此举极大优化高频读取效率。
- **并发锁颗粒度：** 采用精细化粒度记录级别保护运力资源（只锁定具体 `[日期+时段段内]` 的可调度人员总量）。

## 任务执行清单

### Phase 1 基础环境与鉴权基建
- [x] 初始化 ABP 后端骨架（项目名 `CY.HomeCleaning`）。
- [x] 连接字符串切换为 LocalDB（数据库 `CYHomeCleaning`）。
- [x] 执行 `DbMigrator` 完成数据库迁移与种子初始化。
- [x] OpenIddict 发现文档、`/connect/token` 端点可用性验证通过。
- [x] 新增开发联调客户端 `HomeCleaning_Dev`（支持 password + refresh_token）。
- [x] 验证 `password grant` 下发 Token 成功。

### Phase 1 权限体系（先做第一项）
- [x] 新增 B/C 端权限定义（Backoffice/Customer 权限树）。
- [x] 新增角色常量（`admin`、`operator`、`customer`）。
- [x] 新增角色权限种子（角色自动创建 + 权限自动赋予）。
- [x] 增加策略授权（BackofficeOnly、CustomerOnly）。
- [x] 增加安全探针接口并验证：`admin` 访问 Backoffice 返回 `200`，访问 CustomerOnly 返回 `403`。

### Phase 1 微信小程序扩展授权（再做第二项）
- [x] 新增微信 MiniApp 配置与认证服务抽象（支持 Mock 模式）。
- [x] 新增自定义 Grant Type：`wechat_miniapp`。
- [x] 新增 OpenIddict 客户端 `HomeCleaning_WeChatMiniApp`。
- [x] 在 Host 层注册扩展授权处理器并完成编译通过。
- [x] 使用 `grant_type=wechat_miniapp` 实测 Token 下发成功。

### Phase 2 核心业务表（待开始）
- [ ] 落地 `ServiceItem`、`CapacitySchedule`、`Order`、`CouponTemplate`、`UserCoupon` 实体设计。
- [ ] 增加 `Order.SnapshotData (JSON)` 映射与持久化策略。
- [ ] 增加 `CapacitySchedule.RowVersion` 并发字段与防超卖测试。

### 下一步待办
- [ ] 将微信配置从 Mock 切换到真实 `AppId/AppSecret`，并关闭 `EnableMockMode`。（已完成配置切换与启动校验，待真实 `code` 联调通过后勾选）
- [ ] 补充 C 端最小业务闭环 API（如“我的订单”）并加 Customer 权限控制。
