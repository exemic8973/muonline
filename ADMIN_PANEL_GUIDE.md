# OpenMU Admin Panel — 私人服务器配置手册

## 访问
浏览器打开: `http://localhost:57058/`

---

## ① 宝石255个堆叠 ✅ (已完成)
Bless/Soul/Life/Creation/Guardian 的 Durability 已设为 255
(需要 `docker restart openmu-startup` 后生效，新获得的宝石会叠加)

---

## ② 所有翅膀免费进商店

1. 左栏 → **Configuration** → **Items**
2. 搜索框输入 `Wing of` → 点击每条进去编辑:
   - 翻到 **Item → Character Classes** → 全部删除(点×)
   - 翻到 **Attributes Requirements** → 全部删除(点×)
3. 左栏 → **Setup** → 搜 `Wandering Merchant Martin`
4. 点 **Merchant Store** → **Add Item**
5. 搜索 `Wing` → 每个翅膀都加进去:
   - Price: **0**
   - Level: **15**
   - Durability: **255**
6. 点 **Save** — 去 Lorencia 找流浪商人即可免费拿+15翅膀

---

## ③ 每职业多套+15满追加装备

在同一个流浪商人的 Merchant Store 里，**Add Item**:

### Dark Knight (骑士) Dragon Set
| 部位 | 搜索词 | Group | Number |
|------|--------|-------|--------|
| 头盔 | Dragon Helm | 7 | 1 |
| 铠甲 | Dragon Armor | 8 | 1 |
| 裤子 | Dragon Pants | 9 | 1 |
| 手套 | Dragon Gloves | 10 | 1 |
| 靴子 | Dragon Boots | 11 | 1 |

全部设 Price=0, Level=15, Durability=255

### Dark Wizard (法师) Legendary Set
| 部位 | 搜索词 | Group | Number |
|------|--------|-------|--------|
| 头盔 | Legendary Helm | 7 | 3 |
| 铠甲 | Legendary Armor | 8 | 3 |
| 裤子 | Legendary Pants | 9 | 3 |
| 手套 | Legendary Gloves | 10 | 3 |
| 靴子 | Legendary Boots | 11 | 3 |

### Fairy Elf (精灵) Sphinx Set
| 部位 | Group | Number |
|------|-------|--------|
| 全部 | 7,8,9,10,11 | 7 |

### 所有职业通用: Adamantine (Maya) Set
| 部位 | Group | Number |
|------|-------|--------|
| 全部 | 7,8,9,10,11 | 26 |

### Excellent 300+ 级套装
| 套装 | Group | Number |
|------|-------|--------|
| Black Dragon | 7,8,9,10,11 | 16 |
| Grand Soul | 7,8,9,10,11 | 18 |
| Dark Soul | 7,8,9,10,11 | 22 |
| Great Dragon | 7,8,9,10,11 | 21 |

全部 Price=0, Level=15, Durability=255

---

## ④ 角色额外属性点

左栏 → **Accounts** → 选账号 → **Characters** → 选角色
→ 改 **LevelUpPoints** = 5000 → **Save**

(也可以用 Ctrl+点加号 100点一次快速加)

---

## ⑤ 宝石补给

左栏 → **Accounts** → 选账号 → **Characters** → 选角色
→ **Inventory** → **Add Item**
→ 搜 `Jewel of Bless` → Durability: **255**
→ 同样方法加 Soul/Life/Creation/Guardian → 每种一个就=255个
