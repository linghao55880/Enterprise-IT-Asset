using ITAssetManagement.Models;

namespace ITAssetManagement.Services;

public class MockDataService
{
    public List<Asset> Assets { get; } = new();
    public List<Credential> Credentials { get; } = new();
    public List<User> Users { get; } = new();
    public List<ApprovalRequest> Approvals { get; } = new();
    public List<AuditLog> AuditLogs { get; } = new();

    public MockDataService()
    {
        SeedUsers();
        SeedAssets();
        SeedCredentials();
        SeedApprovals();
        SeedAuditLogs();
    }

    private void SeedUsers()
    {
        Users.AddRange(new[]
        {
            new User { Id = "U001", Username = "admin", DisplayName = "系统管理员", Email = "admin@company.com", Phone = "13800138001", Role = "超级管理员", Department = "信息技术部", IsActive = true, LastLogin = DateTime.Now.AddDays(-1), CreatedAt = DateTime.Now.AddMonths(-6) },
            new User { Id = "U002", Username = "assetmgr", DisplayName = "资产管理员", Email = "assetmgr@company.com", Phone = "13800138002", Role = "资产管理员", Department = "信息技术部", IsActive = true, LastLogin = DateTime.Now.AddDays(-2), CreatedAt = DateTime.Now.AddMonths(-5) },
            new User { Id = "U003", Username = "secmgr", DisplayName = "安全管理员", Email = "secmgr@company.com", Phone = "13800138003", Role = "安全管理员", Department = "信息安全部", IsActive = true, LastLogin = DateTime.Now.AddDays(-3), CreatedAt = DateTime.Now.AddMonths(-5) },
            new User { Id = "U004", Username = "auditor", DisplayName = "审计员", Email = "auditor@company.com", Phone = "13800138004", Role = "审计员", Department = "审计部", IsActive = true, LastLogin = DateTime.Now.AddDays(-5), CreatedAt = DateTime.Now.AddMonths(-4) },
            new User { Id = "U005", Username = "depthead", DisplayName = "部门负责人", Email = "depthead@company.com", Phone = "13800138005", Role = "部门负责人", Department = "信息技术部", IsActive = true, LastLogin = DateTime.Now.AddDays(-7), CreatedAt = DateTime.Now.AddMonths(-4) },
            new User { Id = "U006", Username = "member1", DisplayName = "张三", Email = "zhangsan@company.com", Phone = "13800138006", Role = "普通成员", Department = "研发部", IsActive = true, LastLogin = DateTime.Now.AddDays(-1), CreatedAt = DateTime.Now.AddMonths(-3) },
            new User { Id = "U007", Username = "member2", DisplayName = "李四", Email = "lisi@company.com", Phone = "13800138007", Role = "普通成员", Department = "运维部", IsActive = true, LastLogin = DateTime.Now.AddDays(-2), CreatedAt = DateTime.Now.AddMonths(-3) }
        });
    }

    private void SeedAssets()
    {
        Assets.AddRange(new[]
        {
            new Asset { Id = "A001", AssetCode = "SRV-2024-001", Name = "核心数据库服务器-01", Category = "物理服务器", SubCategory = "机架式服务器", Status = "在用", IpAddress = "192.168.10.10", ConfigInfo = "CPU: 2x Intel Xeon Gold 6248R, 内存: 512GB DDR4, 硬盘: 8x 1.92TB SSD RAID10", OsVersion = "Windows Server 2022 Datacenter", Purpose = "承载ERP核心数据库", Location = "A栋-3楼-机房A-机柜R01", ResponsiblePerson = "assetmgr", PurchaseDate = new DateTime(2024, 1, 15), PurchasePrice = 128000, Supplier = "戴尔科技", WarrantyExpire = new DateTime(2027, 1, 15), Notes = "核心业务系统，需重点监控", CreatedAt = DateTime.Now.AddMonths(-6) },
            new Asset { Id = "A002", AssetCode = "SRV-2024-002", Name = "应用服务器-01", Category = "物理服务器", SubCategory = "机架式服务器", Status = "在用", IpAddress = "192.168.10.11", ConfigInfo = "CPU: 2x Intel Xeon Silver 4314, 内存: 256GB DDR4, 硬盘: 4x 960GB SSD RAID5", OsVersion = "CentOS Stream 9", Purpose = "运行OA及协同办公系统", Location = "A栋-3楼-机房A-机柜R02", ResponsiblePerson = "assetmgr", PurchaseDate = new DateTime(2024, 2, 20), PurchasePrice = 86000, Supplier = "华为", WarrantyExpire = new DateTime(2027, 2, 20), Notes = "", CreatedAt = DateTime.Now.AddMonths(-5) },
            new Asset { Id = "A003", AssetCode = "VM-2024-003", Name = "开发测试虚拟机-01", Category = "虚拟机", SubCategory = "VMware虚拟机", Status = "在用", IpAddress = "192.168.20.50", ConfigInfo = "vCPU: 8核, 内存: 32GB, 磁盘: 200GB", OsVersion = "Ubuntu 22.04 LTS", Purpose = "开发环境测试", Location = "虚拟化集群-Cluster01", ResponsiblePerson = "member1", PurchaseDate = null, PurchasePrice = null, Supplier = "VMware", WarrantyExpire = null, Notes = "按需启停", CreatedAt = DateTime.Now.AddMonths(-4) },
            new Asset { Id = "A004", AssetCode = "VM-2024-004", Name = "预发布环境虚拟机", Category = "虚拟机", SubCategory = "VMware虚拟机", Status = "在用", IpAddress = "192.168.20.51", ConfigInfo = "vCPU: 16核, 内存: 64GB, 磁盘: 500GB", OsVersion = "Windows Server 2019", Purpose = "UAT预发布验证", Location = "虚拟化集群-Cluster01", ResponsiblePerson = "member2", PurchaseDate = null, PurchasePrice = null, Supplier = "VMware", WarrantyExpire = null, Notes = "", CreatedAt = DateTime.Now.AddMonths(-4) },
            new Asset { Id = "A005", AssetCode = "APP-2024-005", Name = "企业ERP系统", Category = "业务系统", SubCategory = "ERP系统", Status = "在用", IpAddress = null, ConfigInfo = "部署于A001及A002", OsVersion = null, Purpose = "企业资源计划管理", Location = "", ResponsiblePerson = "depthead", PurchaseDate = new DateTime(2023, 6, 1), PurchasePrice = 500000, Supplier = "用友网络", WarrantyExpire = new DateTime(2026, 6, 1), Notes = "包含财务、供应链、生产制造模块", CreatedAt = DateTime.Now.AddMonths(-6) },
            new Asset { Id = "A006", AssetCode = "APP-2024-006", Name = "协同办公OA系统", Category = "业务系统", SubCategory = "OA系统", Status = "在用", IpAddress = null, ConfigInfo = "B/S架构，支持移动端", OsVersion = null, Purpose = "内部流程审批及协同办公", Location = "", ResponsiblePerson = "depthead", PurchaseDate = new DateTime(2023, 3, 10), PurchasePrice = 120000, Supplier = "泛微网络", WarrantyExpire = new DateTime(2026, 3, 10), Notes = "", CreatedAt = DateTime.Now.AddMonths(-6) },
            new Asset { Id = "A007", AssetCode = "NET-2024-007", Name = "核心交换机-01", Category = "网络设备", SubCategory = "核心交换机", Status = "在用", IpAddress = "192.168.1.1", ConfigInfo = "48口万兆光口 + 4口40G上联", OsVersion = "VRP 8.0", Purpose = "数据中心核心交换", Location = "A栋-3楼-机房A-网络柜N01", ResponsiblePerson = "assetmgr", PurchaseDate = new DateTime(2023, 8, 15), PurchasePrice = 45000, Supplier = "华为", WarrantyExpire = new DateTime(2026, 8, 15), Notes = "双电源冗余", CreatedAt = DateTime.Now.AddMonths(-5) },
            new Asset { Id = "A008", AssetCode = "NET-2024-008", Name = "防火墙-01", Category = "网络设备", SubCategory = "下一代防火墙", Status = "在用", IpAddress = "192.168.1.254", ConfigInfo = "吞吐量: 10Gbps, 并发连接: 500万", OsVersion = "FortiOS 7.4", Purpose = "边界安全防护", Location = "A栋-3楼-机房A-网络柜N02", ResponsiblePerson = "secmgr", PurchaseDate = new DateTime(2023, 9, 1), PurchasePrice = 68000, Supplier = "Fortinet", WarrantyExpire = new DateTime(2026, 9, 1), Notes = "含IPS/AV/URL过滤授权", CreatedAt = DateTime.Now.AddMonths(-5) },
            new Asset { Id = "A009", AssetCode = "NET-2024-009", Name = "无线AP控制器", Category = "网络设备", SubCategory = "无线控制器", Status = "在用", IpAddress = "192.168.1.10", ConfigInfo = "管理AP数量: 256个", OsVersion = "ArubaOS 8.10", Purpose = "统一无线接入管理", Location = "A栋-3楼-机房A-网络柜N01", ResponsiblePerson = "assetmgr", PurchaseDate = new DateTime(2023, 5, 20), PurchasePrice = 32000, Supplier = "Aruba", WarrantyExpire = new DateTime(2026, 5, 20), Notes = "", CreatedAt = DateTime.Now.AddMonths(-5) },
            new Asset { Id = "A010", AssetCode = "CLOUD-2024-010", Name = "阿里云-生产环境ECS", Category = "云服务", SubCategory = "云服务器ECS", Status = "在用", IpAddress = "47.102.x.x", ConfigInfo = "规格: ecs.g7.2xlarge, 带宽: 100Mbps", OsVersion = "Alibaba Cloud Linux 3", Purpose = "官网及移动端API服务", Location = "阿里云-华东1(杭州)", ResponsiblePerson = "member2", PurchaseDate = new DateTime(2024, 1, 1), PurchasePrice = 24000, Supplier = "阿里云", WarrantyExpire = new DateTime(2025, 1, 1), Notes = "按年续费", CreatedAt = DateTime.Now.AddMonths(-6) },
            new Asset { Id = "A011", AssetCode = "CLOUD-2024-011", Name = "阿里云-RDS数据库", Category = "云服务", SubCategory = "云数据库RDS", Status = "在用", IpAddress = null, ConfigInfo = "MySQL 8.0, 4核16GB, 500GB SSD", OsVersion = "MySQL 8.0", Purpose = "线上业务数据库", Location = "阿里云-华东1(杭州)", ResponsiblePerson = "assetmgr", PurchaseDate = new DateTime(2024, 1, 1), PurchasePrice = 18000, Supplier = "阿里云", WarrantyExpire = new DateTime(2025, 1, 1), Notes = "主备架构", CreatedAt = DateTime.Now.AddMonths(-6) },
            new Asset { Id = "A012", AssetCode = "CLOUD-2024-012", Name = "腾讯云-对象存储COS", Category = "云服务", SubCategory = "对象存储", Status = "在用", IpAddress = null, ConfigInfo = "标准存储: 5TB, 下行流量包: 2TB/月", OsVersion = null, Purpose = "文件及图片存储", Location = "腾讯云-华南(广州)", ResponsiblePerson = "member1", PurchaseDate = new DateTime(2023, 10, 1), PurchasePrice = 6000, Supplier = "腾讯云", WarrantyExpire = new DateTime(2025, 10, 1), Notes = "", CreatedAt = DateTime.Now.AddMonths(-5) },
            new Asset { Id = "A013", AssetCode = "SRV-2023-013", Name = "备份服务器", Category = "物理服务器", SubCategory = "塔式服务器", Status = "在用", IpAddress = "192.168.10.20", ConfigInfo = "CPU: Intel Xeon E-2388G, 内存: 128GB, 硬盘: 12x 4TB HDD RAID6", OsVersion = "TrueNAS Scale", Purpose = "数据备份及归档", Location = "A栋-3楼-机房B", ResponsiblePerson = "assetmgr", PurchaseDate = new DateTime(2023, 7, 10), PurchasePrice = 52000, Supplier = "HPE", WarrantyExpire = new DateTime(2026, 7, 10), Notes = "", CreatedAt = DateTime.Now.AddMonths(-6) },
            new Asset { Id = "A014", AssetCode = "NET-2023-014", Name = "VPN网关设备", Category = "网络设备", SubCategory = "VPN网关", Status = "闲置", IpAddress = "192.168.1.253", ConfigInfo = "支持IPSec/SSL VPN, 并发用户: 500", OsVersion = "", Purpose = "远程接入", Location = "A栋-3楼-机房A-网络柜N02", ResponsiblePerson = "secmgr", PurchaseDate = new DateTime(2023, 4, 15), PurchasePrice = 28000, Supplier = "深信服", WarrantyExpire = new DateTime(2026, 4, 15), Notes = "待替换", CreatedAt = DateTime.Now.AddMonths(-6) },
            new Asset { Id = "A015", AssetCode = "APP-2023-015", Name = "财务报销系统", Category = "业务系统", SubCategory = "财务系统", Status = "在用", IpAddress = null, ConfigInfo = "", OsVersion = null, Purpose = "员工费用报销及审批", Location = "", ResponsiblePerson = "depthead", PurchaseDate = new DateTime(2023, 1, 10), PurchasePrice = 80000, Supplier = "金蝶", WarrantyExpire = new DateTime(2026, 1, 10), Notes = "", CreatedAt = DateTime.Now.AddMonths(-6) }
        });
    }

    private void SeedCredentials()
    {
        Credentials.AddRange(new[]
        {
            new Credential { Id = "C001", AssetId = "A001", Title = "核心数据库服务器-管理员账号", CredentialType = "OS账号", Username = "Administrator", Password = "P@ssw0rd!2024", Url = null, Notes = "本地管理员账号", GroupName = "服务器账号", ExpiresAt = null, CreatedAt = DateTime.Now.AddMonths(-5) },
            new Credential { Id = "C002", AssetId = "A001", Title = "SQL Server数据库-sa账号", CredentialType = "数据库账号", Username = "sa", Password = "Sql#Secure789", Url = null, Notes = "数据库超级管理员", GroupName = "数据库账号", ExpiresAt = DateTime.Now.AddMonths(3), CreatedAt = DateTime.Now.AddMonths(-5) },
            new Credential { Id = "C003", AssetId = "A002", Title = "应用服务器-Linux root", CredentialType = "OS账号", Username = "root", Password = "Root@CentOS99", Url = null, Notes = "SSH root账号", GroupName = "服务器账号", ExpiresAt = null, CreatedAt = DateTime.Now.AddMonths(-4) },
            new Credential { Id = "C004", AssetId = "A005", Title = "ERP系统-管理员", CredentialType = "Web系统账号", Username = "erpadmin", Password = "Erp#Admin456", Url = "https://erp.company.com", Notes = "ERP后台管理", GroupName = "业务系统账号", ExpiresAt = null, CreatedAt = DateTime.Now.AddMonths(-5) },
            new Credential { Id = "C005", AssetId = "A006", Title = "OA系统-管理员", CredentialType = "Web系统账号", Username = "oaadmin", Password = "OA#Manager2024", Url = "https://oa.company.com", Notes = "OA后台管理", GroupName = "业务系统账号", ExpiresAt = null, CreatedAt = DateTime.Now.AddMonths(-5) },
            new Credential { Id = "C006", AssetId = "A007", Title = "核心交换机-管理账号", CredentialType = "网络设备账号", Username = "admin", Password = "Huawei@Switch01", Url = "https://192.168.1.1", Notes = "Web及SSH管理", GroupName = "网络设备账号", ExpiresAt = null, CreatedAt = DateTime.Now.AddMonths(-4) },
            new Credential { Id = "C007", AssetId = "A008", Title = "防火墙-管理员", CredentialType = "网络设备账号", Username = "admin", Password = "Forti@Gate2024", Url = "https://192.168.1.254", Notes = "全权限管理员", GroupName = "网络设备账号", ExpiresAt = null, CreatedAt = DateTime.Now.AddMonths(-4) },
            new Credential { Id = "C008", AssetId = "A010", Title = "阿里云ECS-SSH密钥", CredentialType = "远程工具", Username = "root", Password = "Aliyun#ECS99Root", Url = null, Notes = "生产环境ECS登录", GroupName = "云账号", ExpiresAt = null, CreatedAt = DateTime.Now.AddMonths(-5) },
            new Credential { Id = "C009", AssetId = "A011", Title = "阿里云RDS-数据库账号", CredentialType = "数据库账号", Username = "dbadmin", Password = "RDS#DbAdmin88", Url = "rm-xxx.mysql.rds.aliyuncs.com", Notes = "RDS管理账号", GroupName = "云账号", ExpiresAt = DateTime.Now.AddMonths(2), CreatedAt = DateTime.Now.AddMonths(-5) },
            new Credential { Id = "C010", AssetId = "A003", Title = "开发测试机-普通用户", CredentialType = "OS账号", Username = "devuser", Password = "Dev#User123", Url = null, Notes = "开发测试普通账号", GroupName = "服务器账号", ExpiresAt = null, CreatedAt = DateTime.Now.AddMonths(-3) },
            new Credential { Id = "C011", AssetId = null, Title = "公司WiFi-访客网络", CredentialType = "网络设备账号", Username = "guest", Password = "Guest@2024!", Url = null, Notes = "访客WiFi密码", GroupName = "网络设备账号", ExpiresAt = null, CreatedAt = DateTime.Now.AddMonths(-2) },
            new Credential { Id = "C012", AssetId = null, Title = "VPN-远程接入", CredentialType = "远程工具", Username = "vpnuser", Password = "VPN@Remote2024", Url = "https://vpn.company.com", Notes = "SSL VPN登录", GroupName = "远程工具", ExpiresAt = null, CreatedAt = DateTime.Now.AddMonths(-3) }
        });
    }

    private void SeedApprovals()
    {
        Approvals.AddRange(new[]
        {
            new ApprovalRequest { Id = "R001", RequestType = "密码访问", RequesterId = "U006", RequesterName = "张三", TargetId = "C001", TargetName = "核心数据库服务器-管理员账号", Reason = "需要排查数据库连接异常问题", Status = "待审批", ApproverId = null, ApproverName = null, ApprovalComment = null, CreatedAt = DateTime.Now.AddDays(-2), ApprovedAt = null },
            new ApprovalRequest { Id = "R002", RequestType = "资产报废", RequesterId = "U007", RequesterName = "李四", TargetId = "A014", TargetName = "VPN网关设备", Reason = "设备已过保且性能不足，建议报废处理", Status = "待审批", ApproverId = null, ApproverName = null, ApprovalComment = null, CreatedAt = DateTime.Now.AddDays(-3), ApprovedAt = null },
            new ApprovalRequest { Id = "R003", RequestType = "权限提升", RequesterId = "U006", RequesterName = "张三", TargetId = null, TargetName = "阿里云ECS管理权限", Reason = "需要临时权限进行生产环境发布", Status = "已通过", ApproverId = "U005", ApproverName = "部门负责人", ApprovalComment = "同意，发布后请及时撤销权限", CreatedAt = DateTime.Now.AddDays(-5), ApprovedAt = DateTime.Now.AddDays(-4) },
            new ApprovalRequest { Id = "R004", RequestType = "密码访问", RequesterId = "U007", RequesterName = "李四", TargetId = "C007", TargetName = "防火墙-管理员", Reason = "需要修改防火墙策略", Status = "已拒绝", ApproverId = "U003", ApproverName = "安全管理员", ApprovalComment = "不符合最小权限原则，请通过工单系统提交变更", CreatedAt = DateTime.Now.AddDays(-6), ApprovedAt = DateTime.Now.AddDays(-5) },
            new ApprovalRequest { Id = "R005", RequestType = "资产报废", RequesterId = "U002", RequesterName = "资产管理员", TargetId = "A013", TargetName = "备份服务器", Reason = "测试数据，请勿审批", Status = "已拒绝", ApproverId = "U005", ApproverName = "部门负责人", ApprovalComment = "拒绝，该设备仍在保且运行正常", CreatedAt = DateTime.Now.AddDays(-7), ApprovedAt = DateTime.Now.AddDays(-6) }
        });
    }

    private void SeedAuditLogs()
    {
        AuditLogs.AddRange(new[]
        {
            new AuditLog { Id = "L001", Timestamp = DateTime.Now.AddDays(-1).AddHours(-2), UserId = "U001", UserName = "系统管理员", Action = "登录", ResourceType = "系统", ResourceId = null, Result = "成功", SourceIp = "192.168.1.100", Detail = "用户登录系统" },
            new AuditLog { Id = "L002", Timestamp = DateTime.Now.AddDays(-1).AddHours(-1), UserId = "U001", UserName = "系统管理员", Action = "创建", ResourceType = "资产", ResourceId = "A015", Result = "成功", SourceIp = "192.168.1.100", Detail = "新增资产: 财务报销系统" },
            new AuditLog { Id = "L003", Timestamp = DateTime.Now.AddDays(-1), UserId = "U002", UserName = "资产管理员", Action = "登录", ResourceType = "系统", ResourceId = null, Result = "成功", SourceIp = "192.168.1.101", Detail = "用户登录系统" },
            new AuditLog { Id = "L004", Timestamp = DateTime.Now.AddDays(-1), UserId = "U002", UserName = "资产管理员", Action = "更新", ResourceType = "资产", ResourceId = "A001", Result = "成功", SourceIp = "192.168.1.101", Detail = "修改资产: 核心数据库服务器-01" },
            new AuditLog { Id = "L005", Timestamp = DateTime.Now.AddDays(-2), UserId = "U003", UserName = "安全管理员", Action = "登录", ResourceType = "系统", ResourceId = null, Result = "成功", SourceIp = "192.168.1.102", Detail = "用户登录系统" },
            new AuditLog { Id = "L006", Timestamp = DateTime.Now.AddDays(-2), UserId = "U003", UserName = "安全管理员", Action = "查看密码", ResourceType = "凭据", ResourceId = "C007", Result = "成功", SourceIp = "192.168.1.102", Detail = "查看凭据密码: 防火墙-管理员" },
            new AuditLog { Id = "L007", Timestamp = DateTime.Now.AddDays(-2).AddHours(-1), UserId = "U006", UserName = "张三", Action = "登录", ResourceType = "系统", ResourceId = null, Result = "成功", SourceIp = "192.168.1.105", Detail = "用户登录系统" },
            new AuditLog { Id = "L008", Timestamp = DateTime.Now.AddDays(-2).AddHours(-1), UserId = "U006", UserName = "张三", Action = "创建", ResourceType = "审批申请", ResourceId = "R001", Result = "成功", SourceIp = "192.168.1.105", Detail = "提交密码访问申请" },
            new AuditLog { Id = "L009", Timestamp = DateTime.Now.AddDays(-3), UserId = "U007", UserName = "李四", Action = "登录", ResourceType = "系统", ResourceId = null, Result = "成功", SourceIp = "192.168.1.106", Detail = "用户登录系统" },
            new AuditLog { Id = "L010", Timestamp = DateTime.Now.AddDays(-3), UserId = "U007", UserName = "李四", Action = "创建", ResourceType = "审批申请", ResourceId = "R002", Result = "成功", SourceIp = "192.168.1.106", Detail = "提交资产报废申请" },
            new AuditLog { Id = "L011", Timestamp = DateTime.Now.AddDays(-4), UserId = "U005", UserName = "部门负责人", Action = "登录", ResourceType = "系统", ResourceId = null, Result = "成功", SourceIp = "192.168.1.104", Detail = "用户登录系统" },
            new AuditLog { Id = "L012", Timestamp = DateTime.Now.AddDays(-4), UserId = "U005", UserName = "部门负责人", Action = "审批通过", ResourceType = "审批申请", ResourceId = "R003", Result = "成功", SourceIp = "192.168.1.104", Detail = "审批通过权限提升申请" },
            new AuditLog { Id = "L013", Timestamp = DateTime.Now.AddDays(-5), UserId = "U003", UserName = "安全管理员", Action = "审批拒绝", ResourceType = "审批申请", ResourceId = "R004", Result = "成功", SourceIp = "192.168.1.102", Detail = "拒绝密码访问申请" },
            new AuditLog { Id = "L014", Timestamp = DateTime.Now.AddDays(-5), UserId = "U001", UserName = "系统管理员", Action = "创建", ResourceType = "用户", ResourceId = "U007", Result = "成功", SourceIp = "192.168.1.100", Detail = "创建新用户: 李四" },
            new AuditLog { Id = "L015", Timestamp = DateTime.Now.AddDays(-6), UserId = "U002", UserName = "资产管理员", Action = "删除", ResourceType = "资产", ResourceId = "A014", Result = "失败", SourceIp = "192.168.1.101", Detail = "删除失败: 资产不存在或无权操作" },
            new AuditLog { Id = "L016", Timestamp = DateTime.Now.AddDays(-6), UserId = "U004", UserName = "审计员", Action = "登录", ResourceType = "系统", ResourceId = null, Result = "成功", SourceIp = "192.168.1.103", Detail = "用户登录系统" },
            new AuditLog { Id = "L017", Timestamp = DateTime.Now.AddDays(-6).AddHours(-1), UserId = "U004", UserName = "审计员", Action = "查看", ResourceType = "审计日志", ResourceId = null, Result = "成功", SourceIp = "192.168.1.103", Detail = "导出审计日志报表" },
            new AuditLog { Id = "L018", Timestamp = DateTime.Now.AddDays(-7), UserId = "U001", UserName = "系统管理员", Action = "更新", ResourceType = "用户", ResourceId = "U006", Result = "成功", SourceIp = "192.168.1.100", Detail = "修改用户信息: 张三" },
            new AuditLog { Id = "L019", Timestamp = DateTime.Now.AddDays(-7).AddHours(-1), UserId = "U006", UserName = "张三", Action = "查看密码", ResourceType = "凭据", ResourceId = "C010", Result = "成功", SourceIp = "192.168.1.105", Detail = "查看凭据密码: 开发测试机-普通用户" },
            new AuditLog { Id = "L020", Timestamp = DateTime.Now.AddDays(-8), UserId = "U002", UserName = "资产管理员", Action = "创建", ResourceType = "凭据", ResourceId = "C012", Result = "成功", SourceIp = "192.168.1.101", Detail = "新增凭据: VPN-远程接入" }
        });
    }

    public void AddAuditLog(string userId, string userName, string action, string resourceType, string? resourceId, string result, string? sourceIp = null, string? detail = null)
    {
        AuditLogs.Insert(0, new AuditLog
        {
            Id = Guid.NewGuid().ToString("N")[..8].ToUpper(),
            Timestamp = DateTime.Now,
            UserId = userId,
            UserName = userName,
            Action = action,
            ResourceType = resourceType,
            ResourceId = resourceId,
            Result = result,
            SourceIp = sourceIp,
            Detail = detail
        });
    }
}
