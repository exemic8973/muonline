#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Client.Data.BMD;
using Client.Main.Localization;
using Microsoft.Extensions.Logging;

namespace Client.Main.Core.Utilities
{
    /// <summary>
    /// Static database for skill definitions loaded from skill_eng.bmd.
    /// </summary>
    public static class SkillDatabase
    {
        private static readonly ILogger? _logger = MuGame.AppLoggerFactory?.CreateLogger("SkillDatabase");

        /// <summary>Lookup cache: SkillId → skill definition.</summary>
        private static Dictionary<int, SkillBMD> _skillDefinitions = [];

        public static async Task Initialize()
        {
            _skillDefinitions = await InitializeSkillData();
        }

        /// <summary>
        /// Loads skill_eng.bmd from an embedded resource and builds the definition table.
        /// </summary>
        private static async Task<Dictionary<int, SkillBMD>> InitializeSkillData()
        {
            var skillPath = Path.Combine(Constants.DataPath, "Local", "skill.bmd");
            var reader = new SkillBMDReader();
            var skills = await reader.Load(skillPath);
            _logger?.LogInformation($"Loaded {skills.Count} skills from skill_eng.bmd");
            return skills;
        }

        #region Public API ------------------------------------------------------

        /// <summary>
        /// Gets skill definition by skill ID.
        /// </summary>
        public static SkillBMD? GetSkillDefinition(int skillId)
        {
            _skillDefinitions.TryGetValue(skillId, out var def);
            return def;
        }

        /// <summary>
        /// Gets skill name by skill ID.
        /// </summary>
        /// <summary>
        /// English skill names keyed by the <b>real</b> MU/OpenMU skill IDs
        /// (authoritative source: <see cref="Client.Data.BMD.SkillDefinitions"/>).
        /// The previous table used a fan-made sequential numbering that did not match
        /// the IDs the server actually sends, so e.g. Lightning (id 3) was shown as
        /// "Power Wave". Keep this in sync with SkillDefinitions.
        /// </summary>
        private static readonly Dictionary<int, string> s_fallbackNames = new()
        {
            // Wizard
            { 1, "Poison" }, { 2, "Meteorite" }, { 3, "Lightning" }, { 4, "Fire Ball" },
            { 5, "Flame" }, { 6, "Teleport" }, { 7, "Ice" }, { 8, "Twister" },
            { 9, "Evil Spirit" }, { 10, "Hellfire" }, { 11, "Power Wave" }, { 12, "Aqua Beam" },
            { 13, "Cometfall" }, { 14, "Inferno" }, { 15, "Teleport Ally" }, { 16, "Soul Barrier" },
            { 17, "Energy Ball" },
            // Knight
            { 18, "Defense" }, { 19, "Falling Slash" }, { 20, "Lunge" }, { 21, "Uppercut" },
            { 22, "Cyclone" }, { 23, "Slash" }, { 24, "Triple Shot" }, { 26, "Heal" },
            { 27, "Greater Defense" }, { 28, "Greater Damage" },
            // Elf summons
            { 30, "Summon Goblin" }, { 31, "Summon Stone Golem" }, { 32, "Summon Assassin" },
            { 33, "Summon Elite Yeti" }, { 34, "Summon Dark Knight" }, { 35, "Summon Bali" },
            { 36, "Summon Soldier" },
            // Mixed / Master
            { 38, "Decay" }, { 39, "Ice Storm" }, { 40, "Nova" }, { 41, "Twisting Slash" },
            { 42, "Rageful Blow" }, { 43, "Death Stab" }, { 44, "Crescent Moon Slash" },
            { 45, "Lance" }, { 46, "Starfall" }, { 47, "Impale" }, { 48, "Swell Life" },
            { 49, "Fire Breath" }, { 51, "Ice Arrow" }, { 52, "Penetration" },
            { 55, "Fire Slash" }, { 56, "Power Slash" }, { 57, "Spiral Slash" },
            { 60, "Force" }, { 61, "Fire Burst" }, { 62, "Earthshake" }, { 63, "Summon" },
            { 64, "Increase Critical Damage" }, { 65, "Electric Spike" }, { 66, "Force Wave" },
            { 67, "Stun" }, { 68, "Cancel Stun" }, { 69, "Swell Mana" }, { 70, "Invisibility" },
            { 71, "Cancel Invisibility" }, { 72, "Abolish Magic" }, { 73, "Mana Rays" },
            { 74, "Fire Blast" }, { 76, "Plasma Storm" }, { 77, "Infinity Arrow" },
            { 78, "Fire Scream" }, { 79, "Explosion" },
            // Summoner
            { 200, "Summon Monster" }, { 201, "Magic Attack Immunity" }, { 202, "Physical Attack Immunity" },
            { 203, "Potion of Bless" }, { 204, "Potion of Soul" }, { 210, "Spell of Protection" },
            { 211, "Spell of Restriction" }, { 212, "Spell of Pursuit" }, { 213, "Shield-Burn" },
            { 214, "Drain Life" }, { 215, "Chain Lightning" }, { 217, "Damage Reflection" },
            { 218, "Berserker" }, { 219, "Sleep" }, { 221, "Weakness" }, { 222, "Innovation" },
            { 223, "Explosion" }, { 224, "Requiem" }, { 225, "Pollution" },
            // Master skills
            { 230, "Lightning Shock" }, { 232, "Strike of Destruction" }, { 233, "Expansion of Wizardry" },
            { 234, "Recovery" }, { 235, "Multi-Shot" }, { 236, "Flame Strike" }, { 237, "Gigantic Storm" },
            { 238, "Chaotic Diseier" },
            // Rage Fighter
            { 260, "Killing Blow" }, { 261, "Beast Uppercut" }, { 262, "Chain Drive" },
            { 263, "Dark Side" }, { 264, "Dragon Roar" }, { 265, "Dragon Slasher" },
            { 266, "Ignore Defense" }, { 267, "Increase Health" }, { 268, "Increase Block" },
            { 269, "Charge" }, { 270, "Phoenix Shot" },
            // Special
            { 495, "Earth Prison" }, { 565, "Blood Howling" },
        };

        /// <summary>
        /// Chinese skill names keyed by the <b>real</b> MU/OpenMU skill IDs
        /// (must stay aligned with <see cref="s_fallbackNames"/> and
        /// <see cref="Client.Data.BMD.SkillDefinitions"/>).
        /// </summary>
        private static readonly Dictionary<int, string> s_namesZh = new()
        {
            // Wizard
            { 1, "毒咒" }, { 2, "陨石术" }, { 3, "闪电" }, { 4, "火球术" },
            { 5, "火焰术" }, { 6, "瞬间移动" }, { 7, "冰冻术" }, { 8, "龙卷风" },
            { 9, "邪灵" }, { 10, "地狱火" }, { 11, "力量波" }, { 12, "水流冲击" },
            { 13, "彗星坠落" }, { 14, "地狱烈焰" }, { 15, "团队传送" }, { 16, "灵魂屏障" },
            { 17, "能量球" },
            // Knight
            { 18, "防御" }, { 19, "上挑斩" }, { 20, "突刺" }, { 21, "上勾拳" },
            { 22, "旋风斩" }, { 23, "斩击" }, { 24, "三重箭" }, { 26, "治愈术" },
            { 27, "强化防御" }, { 28, "强化攻击" },
            // Elf summons
            { 30, "召唤哥布林" }, { 31, "召唤石魔像" }, { 32, "召唤刺客" },
            { 33, "召唤精英雪人" }, { 34, "召唤黑暗骑士" }, { 35, "召唤巴利" },
            { 36, "召唤士兵" },
            // Mixed / Master
            { 38, "衰亡术" }, { 39, "暴风雪" }, { 40, "新星" }, { 41, "旋转斩" },
            { 42, "狂怒重击" }, { 43, "死亡之刺" }, { 44, "弦月斩" },
            { 45, "长枪" }, { 46, "流星坠落" }, { 47, "穿刺" }, { 48, "强化生命" },
            { 49, "火焰吐息" }, { 51, "冰箭" }, { 52, "穿透" },
            { 55, "火焰斩" }, { 56, "强力斩" }, { 57, "螺旋斩" },
            { 60, "原力" }, { 61, "火焰爆发" }, { 62, "大地震击" }, { 63, "召唤" },
            { 64, "提升致命伤害" }, { 65, "雷电尖刺" }, { 66, "原力波" },
            { 67, "眩晕" }, { 68, "解除眩晕" }, { 69, "强化魔力" }, { 70, "隐身" },
            { 71, "解除隐身" }, { 72, "魔法消除" }, { 73, "魔力射线" },
            { 74, "火焰冲击" }, { 76, "等离子风暴" }, { 77, "无限之箭" },
            { 78, "火焰尖啸" }, { 79, "爆裂" },
            // Summoner
            { 200, "召唤怪物" }, { 201, "魔法攻击免疫" }, { 202, "物理攻击免疫" },
            { 203, "祝福药水" }, { 204, "灵魂药水" }, { 210, "保护咒语" },
            { 211, "束缚咒语" }, { 212, "追踪咒语" }, { 213, "护盾燃烧" },
            { 214, "吸取生命" }, { 215, "连锁闪电" }, { 217, "伤害反射" },
            { 218, "狂战士" }, { 219, "沉睡" }, { 221, "弱化" }, { 222, "革新" },
            { 223, "爆裂" }, { 224, "安魂曲" }, { 225, "污染" },
            // Master skills
            { 230, "闪电震击" }, { 232, "毁灭打击" }, { 233, "魔法增幅" },
            { 234, "恢复" }, { 235, "多重射击" }, { 236, "烈焰打击" }, { 237, "巨型风暴" },
            { 238, "混沌使者" },
            // Rage Fighter
            { 260, "致命打击" }, { 261, "野兽上勾拳" }, { 262, "连环踢" },
            { 263, "黑暗面" }, { 264, "龙吼" }, { 265, "屠龙斩" },
            { 266, "无视防御" }, { 267, "提升生命" }, { 268, "提升格挡" },
            { 269, "冲锋" }, { 270, "凤凰射击" },
            // Special
            { 495, "大地牢笼" }, { 565, "血之咆哮" },
        };

        /// <summary>
        /// Returns true when a name from the BMD looks like clean, displayable text.
        /// The bundled skill.bmd ships Korean (EUC-KR) names which the active font may
        /// not contain glyphs for, so we only trust BMD names that survive as printable
        /// text and otherwise prefer the curated localized/English tables below.
        /// </summary>
        private static bool IsDisplayableBmdName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length <= 1 || name.StartsWith("Unknown"))
                return false;

            foreach (var ch in name)
            {
                // Reject control chars and the Unicode replacement character (mojibake marker).
                if (char.IsControl(ch) || ch == '�')
                    return false;
            }

            return true;
        }

        public static string GetSkillName(int skillId)
        {
            // Curated localized names are authoritative: they are guaranteed to render
            // with the active font and avoid Korean mojibake from the bundled BMD.
            bool isChinese = string.Equals(
                Loc.CurrentLanguage, "zh", StringComparison.OrdinalIgnoreCase);

            if (isChinese && s_namesZh.TryGetValue(skillId, out var zhName))
                return zhName;

            if (s_fallbackNames.TryGetValue(skillId, out var enName))
                return enName;

            var bmdName = GetSkillDefinition(skillId)?.Name;
            if (IsDisplayableBmdName(bmdName))
                return bmdName!;

            return $"Skill {skillId}";
        }

        /// <summary>
        /// Gets skill type (AREA/TARGET/SELF) by skill ID.
        /// </summary>
        public static SkillType GetSkillType(int skillId) =>
            SkillDefinitions.GetSkillType(skillId);

        /// <summary>
        /// Gets animation ID for skill by skill ID.
        /// Returns -1 if no specific animation.
        /// </summary>
        public static int GetSkillAnimation(int skillId) =>
            SkillDefinitions.GetSkillAnimation(skillId);

        /// <summary>
        /// Checks if skill is area type.
        /// </summary>
        public static bool IsAreaSkill(int skillId) =>
            SkillDefinitions.IsAreaSkill(skillId);

        /// <summary>
        /// Checks if skill is target type.
        /// </summary>
        public static bool IsTargetSkill(int skillId) =>
            SkillDefinitions.IsTargetSkill(skillId);

        /// <summary>
        /// Checks if skill is self-cast type.
        /// </summary>
        public static bool IsSelfSkill(int skillId) =>
            SkillDefinitions.IsSelfSkill(skillId);

        /// <summary>
        /// Gets all loaded skills.
        /// </summary>
        public static IReadOnlyDictionary<int, SkillBMD> GetAllSkills() => _skillDefinitions;

        /// <summary>
        /// Gets skill mana cost.
        /// </summary>
        public static ushort GetSkillManaCost(int skillId) =>
            GetSkillDefinition(skillId)?.ManaCost ?? 0;

        /// <summary>
        /// Gets skill AG cost. Always 0 — AG requirement disabled.
        /// </summary>
        public static ushort GetSkillAGCost(int skillId) => 0;

        /// <summary>
        /// Gets skill range/distance.
        /// </summary>
        public static uint GetSkillRange(int skillId) =>
            GetSkillDefinition(skillId)?.Distance ?? 0;

        /// <summary>
        /// Gets skill cooldown delay in milliseconds.
        /// </summary>
        public static int GetSkillCooldown(int skillId) =>
            GetSkillDefinition(skillId)?.Delay ?? 0;

        /// <summary>
        /// Gets required level for skill.
        /// </summary>
        public static ushort GetRequiredLevel(int skillId) =>
            GetSkillDefinition(skillId)?.RequiredLevel ?? 0;

        #endregion
    }
}
