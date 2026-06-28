#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Client.Data.BMD;
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
        /// <summary>Hardcoded skill names for corrupted BMD entries.</summary>
        private static readonly Dictionary<int, string> s_fallbackNames = new()
        {
            // Dark Wizard
            { 1, "Energy Ball" }, { 2, "Fire Ball" }, { 3, "Power Wave" },
            { 4, "Lightning" }, { 5, "Cyclone" }, { 6, "Explosion" },
            { 7, "Flame" }, { 8, "Inferno" }, { 9, "Evil Spirit" },
            { 10, "Hellfire" }, { 11, "Teleport" }, { 12, "Ice" },
            { 13, "Meteorite" }, { 14, "Blast" }, { 15, "Poison" },
            { 16, "Moonlight" }, { 17, "Soul Barrier" }, { 18, "Decay" },
            { 19, "Twister" }, { 20, "Aqua Beam" },
            // Dark Knight
            { 21, "Slash" }, { 22, "Power Slash" }, { 23, "Cyclone" },
            { 24, "Sword Skill" }, { 25, "Twisting Slash" }, { 26, "Rageful Blow" },
            { 27, "Death Stab" }, { 28, "Impale" }, { 29, "Blade Skill" },
            { 30, "Blood Storm" }, { 31, "Devil Eye" }, { 32, "Comet Fall" },
            // Fairy Elf
            { 33, "Arrow Bomb" }, { 34, "Penetration" }, { 35, "Ice Arrow" },
            { 36, "Healing" }, { 37, "Greater Defense" }, { 38, "Greater Damage" },
            { 39, "Summon Goblin" }, { 40, "Summon Stone Golem" },
            { 41, "Summon Assassin" }, { 42, "Summon Elite Yeti" },
            { 43, "Summon Dark Knight" }, { 44, "Summon Bali" },
            { 45, "Summon Soldier" }, { 46, "Summon Beholder" },
            { 47, "Heal" },
            // Magic Gladiator
            { 48, "Fire Slash" }, { 49, "Flame Strike" }, { 50, "Gigantic Storm" },
            // Dark Lord
            { 51, "Fire Breath" }, { 52, "Power Slash" }, { 53, "Critical Slash" },
            { 55, "Summon Dark Horse" }, { 56, "Summon Dark Raven" },
            { 57, "Cure" }, { 58, "Refresh" }, { 59, "Greater Fortitude" },
            // Summoner
            { 60, "Chain Lightning" }, { 61, "Drain Life" }, { 62, "Lightning Orb" },
            { 63, "Sleep" }, { 64, "Summon Satyros" }, { 65, "Summon Queen" },
            { 66, "Summon Golem" }, { 67, "Summon Wizard" },
            { 68, "Summon Priest" }, { 69, "Summon Storm" },
            // Rage Fighter
            { 70, "Iron Defense" }, { 71, "Critical Damage" },
            { 74, "Beast Charge" }, { 75, "Dragon Kick" }, { 76, "Dragon Lore" },
            { 77, "Phoenix Shot" },
            // Common / Scrolls
            { 200, "Horse Riding" }, { 201, "Horse Attack" },
        };

        /// <summary>Chinese skill names, keyed by skill ID (mirrors <see cref="s_fallbackNames"/>).</summary>
        private static readonly Dictionary<int, string> s_namesZh = new()
        {
            // Dark Wizard
            { 1, "能量球" }, { 2, "火球术" }, { 3, "力量波" },
            { 4, "闪电" }, { 5, "旋风" }, { 6, "爆裂" },
            { 7, "火焰" }, { 8, "地狱火" }, { 9, "邪灵" },
            { 10, "地狱烈焰" }, { 11, "瞬间移动" }, { 12, "冰冻术" },
            { 13, "陨石术" }, { 14, "爆炸" }, { 15, "毒云术" },
            { 16, "月光" }, { 17, "灵魂护盾" }, { 18, "衰退术" },
            { 19, "龙卷风" }, { 20, "水流冲击" },
            // Dark Knight
            { 21, "斩击" }, { 22, "强力斩" }, { 23, "旋风斩" },
            { 24, "剑术" }, { 25, "旋转斩" }, { 26, "狂怒重击" },
            { 27, "死亡之刺" }, { 28, "穿刺" }, { 29, "刀刃技" },
            { 30, "血色风暴" }, { 31, "恶魔之眼" }, { 32, "彗星坠落" },
            // Fairy Elf
            { 33, "爆裂箭" }, { 34, "穿透" }, { 35, "冰箭" },
            { 36, "治愈术" }, { 37, "强化防御" }, { 38, "强化攻击" },
            { 39, "召唤哥布林" }, { 40, "召唤石魔像" },
            { 41, "召唤刺客" }, { 42, "召唤精英雪人" },
            { 43, "召唤黑暗骑士" }, { 44, "召唤巴利" },
            { 45, "召唤士兵" }, { 46, "召唤眼魔" },
            { 47, "治愈" },
            // Magic Gladiator
            { 48, "火焰斩" }, { 49, "烈焰打击" }, { 50, "巨型风暴" },
            // Dark Lord
            { 51, "火焰吐息" }, { 52, "强力斩" }, { 53, "致命斩" },
            { 55, "召唤黑马" }, { 56, "召唤黑鸦" },
            { 57, "净化" }, { 58, "恢复" }, { 59, "强化勇气" },
            // Summoner
            { 60, "连锁闪电" }, { 61, "吸取生命" }, { 62, "闪电球" },
            { 63, "沉睡" }, { 64, "召唤萨提洛斯" }, { 65, "召唤女王" },
            { 66, "召唤魔像" }, { 67, "召唤巫师" },
            { 68, "召唤祭司" }, { 69, "召唤风暴" },
            // Rage Fighter
            { 70, "钢铁防御" }, { 71, "致命伤害" },
            { 74, "野兽冲锋" }, { 75, "龙踢" }, { 76, "龙之传说" },
            { 77, "凤凰射击" },
            // Common / Scrolls
            { 200, "骑马" }, { 201, "骑乘攻击" },
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
                Client.Main.Localization.Loc.CurrentLanguage, "zh", StringComparison.OrdinalIgnoreCase);

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
        /// Gets skill AG cost.
        /// </summary>
        public static ushort GetSkillAGCost(int skillId) =>
            GetSkillDefinition(skillId)?.AbilityGaugeCost ?? 0;

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
