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

        public static string GetSkillName(int skillId)
        {
            var bmdName = GetSkillDefinition(skillId)?.Name;
            if (!string.IsNullOrEmpty(bmdName) && bmdName.Length > 1 && !bmdName.StartsWith("Unknown"))
                return bmdName;
            return s_fallbackNames.TryGetValue(skillId, out var name) ? name : $"Skill {skillId}";
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
