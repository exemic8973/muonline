# MuOnline 私人服务器备忘录

> 最后更新: 2025-06-28

## 服务器

- **OpenMU (Docker)**
- ConnectServer: `localhost:44405`
- Admin Panel: `http://localhost:57058/`
- 经验倍率: 1000x
- 宝石堆叠: 255 个
- 自动启动: 是

## 默认账号

```
test1 / test1          →  普通玩家, 5000属性点
testgm / testgm        →  GM, 5000属性点
test0~test9            →  普通测试账号
```

## 游戏操作

| 按键 | 功能 |
|------|------|
| `I` / `V` | 打开背包 |
| `C` | 角色属性 |
| `M` | 移动命令 |
| `ESC` | 暂停菜单 |
| `Enter` | 聊天 |
| `Space` | 拾取物品 |
| `F2` | 聊天日志视图 |
| `F3` | 技能选择面板 |
| `F4` | 聊天日志尺寸 |
| `F5` | 聊天日志显隐 |
| `F6` | 聊天日志透明度 |
| `1`~`9` | 在技能面板中选技能 |
| 右键怪物 | 施放当前选中技能 |
| 右键物品 | 使用/装备 |
| 左键拖出物品到窗口外 | 丢弃 |

## 技能系统

1. 点击底部技能槽（单独大格）或按 `F3` 打开技能面板
2. 鼠标悬停查看技能详情，`1~9` 或点击选中技能
3. 右键怪物/目标施放技能
4. 基础技能（能量球）默认可用，其他技能需通过 Admin Panel 学习
   - 打开 `http://localhost:57058/`
   - `Accounts → 你的账号 → Characters → 角色 → Skills → Add Skill`

## Admin Panel 常用操作

### 为角色添加技能
Accounts → 选账号 → Characters → 选角色 → Skills → Add Skill

### 给角色刷装备/属性
Accounts → 选账号 → Characters → 选角色 → Inventory → Add Item

### 修改角色属性点
Accounts → 选账号 → Characters → 选角色 → LevelUpPoints → 填数字

## 已知限制

- **技能名/物品名 显示:** 来自韩国版客户端数据 (EUC-KR编码)，显示韩文。如有中文版资源文件可替换
- **聊天消息/掉落提示 语言:** 由服务器发送，默认英文
- **底部QWE/ASD/123 快捷栏:** 原客户端的这些贴图为纯装饰，暂无可交互功能
- **装备进商店:** 需要在 Admin Panel 手动添加（NPC → 流动商人 → Merchant Store）

## 启动命令

```batch
cd /d D:\AI_Coding\NextMU\muonline
dotnet run --project MuWinDX\MuWinDX.csproj -c Debug -p:MonoGameFramework=MonoGame.Framework.WindowsDX
```
