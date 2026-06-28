using Client.Main.Models;

namespace Client.Main.Core.Utilities
{
    /// <summary>
    /// Maps re-indexed <see cref="PlayerAction"/> values (compact, no gaps) back to the
    /// classic Season 6 BMD action indices that the Player.bmd model uses.
    ///
    /// The PlayerAction enum was compressed to remove the gaps present in the original
    /// S6 action table, but the Player.bmd binary file still uses the original numbering.
    /// Without this mapping, walking/running/attacking animations play from the wrong
    /// BMD action slot (or a null one), causing the "stiff slide" effect.
    /// </summary>
    public static class BmdActionIndexMap
    {
        /// <summary>
        /// For every value of the current <see cref="PlayerAction"/> enum, returns the
        /// corresponding action index in a classic S6 Player.bmd file.
        /// Array index = new PlayerAction value; array value = BMD action slot.
        /// </summary>
        /// <remarks>
        /// Generated from the old enum preserved as a comment in PlayerAction.cs.
        /// Where the old enum had "Unknown" or unnamed gaps the new compact value
        /// is mapped to -1 (no-op) so it won't crash but won't animate either.
        /// </remarks>
        public static readonly int[] ToBmdIndex = new int[(int)PlayerAction.MaxPlayerAction];

        static BmdActionIndexMap()
        {
            // Default: all entries -1 (no valid BMD action)
            for (int i = 0; i < ToBmdIndex.Length; i++)
                ToBmdIndex[i] = -1;

            // ─── New → Old direct correspondences (same name = same BMD slot) ───
            // Stops (0–14)
            Map((int)PlayerAction.Set,                      0);
            Map((int)PlayerAction.PlayerStopMale,            1);
            Map((int)PlayerAction.PlayerStopFemale,          2);
            Map((int)PlayerAction.PlayerStopSummoner,        3);
            Map((int)PlayerAction.PlayerStopSword,           4);   // was ActionUnknown5 (Stop/Idle Sword) in old
            Map((int)PlayerAction.PlayerStopTwoHandSword,    10);
            Map((int)PlayerAction.PlayerStopSpear,           11);
            Map((int)PlayerAction.PlayerStopScythe,          12);
            Map((int)PlayerAction.PlayerStopBow,             13);
            Map((int)PlayerAction.PlayerStopCrossbow,        14);
            Map((int)PlayerAction.PlayerStopWand,            15);
            Map((int)PlayerAction.PlayerStopFly,             17);
            Map((int)PlayerAction.PlayerStopFlyCrossbow,     19);
            Map((int)PlayerAction.PlayerStopRide,            18);  // StopFlying2 in old (index 18)
            Map((int)PlayerAction.PlayerStopRideWeapon,      -1);  // no direct old counterpart

            // Walks (15–24 in new → 47–58 in old)
            Map((int)PlayerAction.PlayerWalkMale,            47);
            Map((int)PlayerAction.PlayerWalkFemale,          48);
            Map((int)PlayerAction.PlayerWalkSword,           49);
            Map((int)PlayerAction.PlayerWalkTwoHandSword,    50);
            Map((int)PlayerAction.PlayerWalkSpear,           51);
            Map((int)PlayerAction.PlayerWalkScythe,          52);
            Map((int)PlayerAction.PlayerWalkBow,             53);
            Map((int)PlayerAction.PlayerWalkCrossbow,        54);
            Map((int)PlayerAction.PlayerWalkWand,            55);
            Map((int)PlayerAction.PlayerWalkSwim,            58);

            // Runs (25–37 in new → 75–84 in old)
            Map((int)PlayerAction.PlayerRun,                 75);
            Map((int)PlayerAction.PlayerRunSword,            76);
            Map((int)PlayerAction.PlayerRunTwoSword,         26);  // PlayerRunTwoSword in old
            Map((int)PlayerAction.PlayerRunTwoHandSword,     77);
            Map((int)PlayerAction.PlayerRunSpear,            78);
            Map((int)PlayerAction.PlayerRunBow,              79);
            Map((int)PlayerAction.PlayerRunCrossbow,         80);
            Map((int)PlayerAction.PlayerRunWand,             59);
            Map((int)PlayerAction.PlayerRunSwim,             84);
            Map((int)PlayerAction.PlayerFly,                 85);
            Map((int)PlayerAction.PlayerFlyCrossbow,         86);
            Map((int)PlayerAction.PlayerRunRide,             92);
            Map((int)PlayerAction.PlayerRunRideWeapon,       -1);  // no direct old counterpart

            // Attacks (38–59)
            Map((int)PlayerAction.PlayerAttackFist,          107);
            Map((int)PlayerAction.PlayerAttackSwordRight1,   109);
            Map((int)PlayerAction.PlayerAttackSwordRight2,   110);
            Map((int)PlayerAction.PlayerAttackSwordLeft1,    111);
            Map((int)PlayerAction.PlayerAttackSwordLeft2,    112);
            Map((int)PlayerAction.PlayerAttackTwoHandSword1, 113);
            Map((int)PlayerAction.PlayerAttackTwoHandSword2, 45);
            Map((int)PlayerAction.PlayerAttackTwoHandSword3, 46);
            Map((int)PlayerAction.PlayerAttackSpear1,        117);
            Map((int)PlayerAction.PlayerAttackScythe1,       119);
            Map((int)PlayerAction.PlayerAttackScythe2,       120);
            Map((int)PlayerAction.PlayerAttackScythe3,       121);
            Map((int)PlayerAction.PlayerAttackBow,           122);
            Map((int)PlayerAction.PlayerAttackCrossbow,      123);
            Map((int)PlayerAction.PlayerAttackFlyBow,        124);
            Map((int)PlayerAction.PlayerAttackFlyCrossbow,   125);
            Map((int)PlayerAction.PlayerAttackRideSword,     135);
            Map((int)PlayerAction.PlayerAttackRideTwoHandSword, 135); // same index, different weapon context
            Map((int)PlayerAction.PlayerAttackRideSpear,     136);
            Map((int)PlayerAction.PlayerAttackRideScythe,    -1);
            Map((int)PlayerAction.PlayerAttackRideBow,       -1);
            Map((int)PlayerAction.PlayerAttackRideCrossbow,  142);

            // Skill attacks (60–74)
            Map((int)PlayerAction.PlayerAttackSkillSword1,   133);
            Map((int)PlayerAction.PlayerAttackSkillSword2,   -1);  // no direct old idx
            Map((int)PlayerAction.PlayerAttackSkillSword3,   144);
            Map((int)PlayerAction.PlayerAttackSkillSword4,   146);
            Map((int)PlayerAction.PlayerAttackSkillSword5,   146); // same
            Map((int)PlayerAction.PlayerAttackSkillWheel,    147);
            Map((int)PlayerAction.PlayerAttackSkillFuryStrike, 148);
            Map((int)PlayerAction.PlayerSkillVitality,       149);
            Map((int)PlayerAction.PlayerSkillRider,          152);
            Map((int)PlayerAction.PlayerSkillRiderFly,       151);
            Map((int)PlayerAction.PlayerAttackSkillSpear,    153);
            Map((int)PlayerAction.PlayerAttackDeathstab,     154);
            Map((int)PlayerAction.PlayerSkillHellBegin,      155);
            Map((int)PlayerAction.PlayerSkillHellStart,      156);

            // Fenrir / DarkLord (75–129)
            Map((int)PlayerAction.PlayerAttackEnd,           74);
            Map((int)PlayerAction.PlayerFlyRideWeapon,       -1);
            Map((int)PlayerAction.PlayerDarklordStand,       28);
            Map((int)PlayerAction.PlayerDarklordWalk,        57);
            Map((int)PlayerAction.PlayerStopRideHorse,       297); // PlayerStopRide in old
            Map((int)PlayerAction.PlayerRunRideHorse,        92);
            // Fenrir entries — old enum had explicit names at 85-126 range
            Map((int)PlayerAction.PlayerAttackStrike,        -1);
            Map((int)PlayerAction.PlayerAttackTeleport,      -1);
            Map((int)PlayerAction.PlayerAttackDarkhorse,     -1);
            Map((int)PlayerAction.PlayerFenrirAttack,        90);
            Map((int)PlayerAction.PlayerFenrirRun,           110); // approximate
            Map((int)PlayerAction.PlayerFenrirStand,         122); // approximate
            Map((int)PlayerAction.PlayerFenrirWalk,          126); // approximate

            // More attack variants (130–145)
            Map((int)PlayerAction.PlayerAttackBowUp,         -1);
            Map((int)PlayerAction.PlayerAttackCrossbowUp,    -1);
            Map((int)PlayerAction.PlayerAttackFlyBowUp,      -1);
            Map((int)PlayerAction.PlayerAttackFlyCrossbowUp, -1);
            Map((int)PlayerAction.PlayerAttackRideBowUp,     -1);
            Map((int)PlayerAction.PlayerAttackRideCrossbowUp, -1);
            Map((int)PlayerAction.PlayerStopTwoHandSwordTwo, 142); // approximate
            Map((int)PlayerAction.PlayerWalkTwoHandSwordTwo, 143);
            Map((int)PlayerAction.PlayerRunTwoHandSwordTwo,  144);
            Map((int)PlayerAction.PlayerAttackTwoHandSwordTwo, 145);
            Map((int)PlayerAction.PlayerShock,               348); // shock animation

            // Magic skills (146–175)
            Map((int)PlayerAction.PlayerSkillHand1,          158);
            Map((int)PlayerAction.PlayerSkillHand2,          159);
            Map((int)PlayerAction.PlayerSkillWeapon1,        267);
            Map((int)PlayerAction.PlayerSkillWeapon2,        -1);
            Map((int)PlayerAction.PlayerSkillElf1,           162);
            Map((int)PlayerAction.PlayerSkillTeleport,       163);
            Map((int)PlayerAction.PlayerSkillFlash,          164);
            Map((int)PlayerAction.PlayerSkillInferno,        165);
            Map((int)PlayerAction.PlayerSkillHell,           167);
            Map((int)PlayerAction.PlayerRideSkill,           152);
            Map((int)PlayerAction.PlayerSkillSleep,          -1);
            Map((int)PlayerAction.PlayerSkillSleepUni,       -1);
            Map((int)PlayerAction.PlayerSkillSleepDino,      -1);
            Map((int)PlayerAction.PlayerSkillSleepFenrir,    -1);
            Map((int)PlayerAction.PlayerSkillChainLightning, -1);
            Map((int)PlayerAction.PlayerSkillChainLightningUni, -1);
            Map((int)PlayerAction.PlayerSkillChainLightningDino, -1);
            Map((int)PlayerAction.PlayerSkillChainLightningFenrir, -1);
            Map((int)PlayerAction.PlayerSkillLightningOrb,   -1);
            Map((int)PlayerAction.PlayerSkillLightningOrbUni, -1);
            Map((int)PlayerAction.PlayerSkillLightningOrbDino, -1);
            Map((int)PlayerAction.PlayerSkillLightningOrbFenrir, -1);
            Map((int)PlayerAction.PlayerSkillDrainLife,      -1);
            Map((int)PlayerAction.PlayerSkillDrainLifeUni,   -1);
            Map((int)PlayerAction.PlayerSkillDrainLifeDino,  -1);
            Map((int)PlayerAction.PlayerSkillDrainLifeFenrir, -1);
            Map((int)PlayerAction.PlayerSkillSummon,         -1);
            Map((int)PlayerAction.PlayerSkillSummonUni,      -1);
            Map((int)PlayerAction.PlayerSkillSummonDino,     -1);
            Map((int)PlayerAction.PlayerSkillSummonFenrir,   -1);
            Map((int)PlayerAction.PlayerSkillBlowOfDestruction, 174);
            Map((int)PlayerAction.PlayerSkillSwellOfMp,      176);
            Map((int)PlayerAction.PlayerSkillMultishotBowStand, 177);
            Map((int)PlayerAction.PlayerSkillMultishotBowFlying, 178);
            Map((int)PlayerAction.PlayerSkillMultishotCrossbowStand, 179);
            Map((int)PlayerAction.PlayerSkillMultishotCrossbowFlying, 180);
            Map((int)PlayerAction.PlayerSkillRecovery,       181);
            Map((int)PlayerAction.PlayerSkillGiganticstorm,  182);
            Map((int)PlayerAction.PlayerSkillFlamestrike,    183);
            Map((int)PlayerAction.PlayerSkillLightningShock, 184);

            // Emotes / Social (186–229)
            Map((int)PlayerAction.PlayerDefense1,            334);
            Map((int)PlayerAction.PlayerGreeting1,           318);
            Map((int)PlayerAction.PlayerGreetingFemale1,     319);
            Map((int)PlayerAction.PlayerGoodbye1,            320);
            Map((int)PlayerAction.PlayerGoodbyeFemale1,      321);
            Map((int)PlayerAction.PlayerClap1,               322);
            Map((int)PlayerAction.PlayerClapFemale1,         323);
            Map((int)PlayerAction.PlayerCheer1,              324);
            Map((int)PlayerAction.PlayerCheerFemale1,        325);
            Map((int)PlayerAction.PlayerDirection1,          326);
            Map((int)PlayerAction.PlayerDirectionFemale1,    327);
            Map((int)PlayerAction.PlayerGesture1,            328);
            Map((int)PlayerAction.PlayerGestureFemale1,      329);
            Map((int)PlayerAction.PlayerUnknown1,            330);
            Map((int)PlayerAction.PlayerUnknownFemale1,      335);
            Map((int)PlayerAction.PlayerCry1,                332);
            Map((int)PlayerAction.PlayerCryFemale1,          333);
            Map((int)PlayerAction.PlayerAwkward1,            211);
            Map((int)PlayerAction.PlayerAwkwardFemale1,      335);
            Map((int)PlayerAction.PlayerSee1,                336);
            Map((int)PlayerAction.PlayerSeeFemale1,          337);
            Map((int)PlayerAction.PlayerWin1,                338);
            Map((int)PlayerAction.PlayerWinFemale1,          339);
            Map((int)PlayerAction.PlayerSmile1,              340);
            Map((int)PlayerAction.PlayerSmileFemale1,        341);
            Map((int)PlayerAction.PlayerSleep1,              342);
            Map((int)PlayerAction.PlayerSleepFemale1,        343);
            Map((int)PlayerAction.PlayerCold1,               344);
            Map((int)PlayerAction.PlayerColdFemale1,         345);
            Map((int)PlayerAction.PlayerAgain1,              346);
            Map((int)PlayerAction.PlayerAgainFemale1,        -1);
            Map((int)PlayerAction.PlayerRespect1,            331);
            Map((int)PlayerAction.PlayerSalute1,             349);
            Map((int)PlayerAction.PlayerScissors,            350);
            Map((int)PlayerAction.PlayerRock,                351);
            Map((int)PlayerAction.PlayerPaper,               352);
            Map((int)PlayerAction.PlayerHustle,              353);
            Map((int)PlayerAction.PlayerProvocation,         354);
            Map((int)PlayerAction.PlayerLookAround,          355);
            Map((int)PlayerAction.PlayerCheers,              356);
            Map((int)PlayerAction.PlayerKoreaHandclap,       357);
            Map((int)PlayerAction.PlayerPointDance,          358);
            Map((int)PlayerAction.PlayerRush1,               359);
            Map((int)PlayerAction.PlayerComeUp,              347);

            // Death / Sit / Healing (231–246)
            Map((int)PlayerAction.PlayerDie1,                314);
            Map((int)PlayerAction.PlayerDie2,                315);
            Map((int)PlayerAction.PlayerSit1,                215);
            Map((int)PlayerAction.PlayerSit2,                216);
            Map((int)PlayerAction.PlayerSitFemale1,          360);
            Map((int)PlayerAction.PlayerSitFemale2,          361);
            Map((int)PlayerAction.PlayerHealing1,            365);
            Map((int)PlayerAction.PlayerHealingFemale1,      366);
            Map((int)PlayerAction.PlayerPoseMale1,           369);
            Map((int)PlayerAction.PlayerPoseFemale1,         368);
            Map((int)PlayerAction.PlayerJack1,               373);
            Map((int)PlayerAction.PlayerJack2,               374);
            Map((int)PlayerAction.PlayerSanta1,              375);
            Map((int)PlayerAction.PlayerSanta2,              376);
            Map((int)PlayerAction.PlayerChangeUp,            377);
            Map((int)PlayerAction.PlayerRecoverSkill,        378);

            // Darkside / Dragon / Phoenix (247–256)
            Map((int)PlayerAction.PlayerSkillThrust,         -1);
            Map((int)PlayerAction.PlayerSkillStamp,          -1);
            Map((int)PlayerAction.PlayerSkillGiantswing,     -1);
            Map((int)PlayerAction.PlayerSkillDarksideReady,  217);
            Map((int)PlayerAction.PlayerSkillDarksideAttack, 218);
            Map((int)PlayerAction.PlayerSkillDragonkick,     219);
            Map((int)PlayerAction.PlayerSkillDragonlore,     220);
            Map((int)PlayerAction.PlayerSkillPhoenixShot,    221);
            Map((int)PlayerAction.PlayerSkillAttUpOurforces, 160);
            Map((int)PlayerAction.PlayerSkillHpUpOurforces,  168);

            // Rage Fighter (257–283)
            // These are newer than the old enum; map to nearest equivalent or -1
            Map((int)PlayerAction.PlayerRageUniAttack,       -1);
            Map((int)PlayerAction.PlayerRageUniAttackOneRight, -1);
            Map((int)PlayerAction.PlayerRageUniRun,          -1);
            Map((int)PlayerAction.PlayerRageUniRunOneRight,  -1);
            Map((int)PlayerAction.PlayerRageUniStopOneRight, -1);
            Map((int)PlayerAction.PlayerRageFenrir,          -1);
            Map((int)PlayerAction.PlayerRageFenrirTwoSword,  -1);
            Map((int)PlayerAction.PlayerRageFenrirOneRight,  -1);
            Map((int)PlayerAction.PlayerRageFenrirOneLeft,   -1);
            Map((int)PlayerAction.PlayerRageFenrirWalk,      -1);
            Map((int)PlayerAction.PlayerRageFenrirWalkOneRight, -1);
            Map((int)PlayerAction.PlayerRageFenrirWalkOneLeft, -1);
            Map((int)PlayerAction.PlayerRageFenrirWalkTwoSword, -1);
            Map((int)PlayerAction.PlayerRageFenrirRun,       -1);
            Map((int)PlayerAction.PlayerRageFenrirRunTwoSword, -1);
            Map((int)PlayerAction.PlayerRageFenrirRunOneRight, -1);
            Map((int)PlayerAction.PlayerRageFenrirRunOneLeft, -1);
            Map((int)PlayerAction.PlayerRageFenrirStand,     -1);
            Map((int)PlayerAction.PlayerRageFenrirStandTwoSword, -1);
            Map((int)PlayerAction.PlayerRageFenrirStandOneRight, -1);
            Map((int)PlayerAction.PlayerRageFenrirStandOneLeft, -1);
            Map((int)PlayerAction.PlayerRageFenrirDamage,    -1);
            Map((int)PlayerAction.PlayerRageFenrirDamageTwoSword, -1);
            Map((int)PlayerAction.PlayerRageFenrirDamageOneRight, -1);
            Map((int)PlayerAction.PlayerRageFenrirDamageOneLeft, -1);
            Map((int)PlayerAction.PlayerRageFenrirAttackRight, -1);
            Map((int)PlayerAction.PlayerStopRagefighter,     4);

            // ─── Post-processing: for any unmapped value, keep -1 ───
        }

        private static void Map(int newIndex, int oldIndex)
        {
            if (newIndex >= 0 && newIndex < ToBmdIndex.Length)
                ToBmdIndex[newIndex] = oldIndex;
        }

        /// <summary>
        /// Translates a new <see cref="PlayerAction"/> value to the classic BMD action index.
        /// Returns <paramref name="newAction"/> unchanged if no mapping exists (the default).
        /// </summary>
        public static int GetBmdIndex(int newActionValue)
        {
            if (newActionValue < 0 || newActionValue >= ToBmdIndex.Length)
                return newActionValue;
            int mapped = ToBmdIndex[newActionValue];
            return mapped >= 0 ? mapped : newActionValue;
        }
    }
}
