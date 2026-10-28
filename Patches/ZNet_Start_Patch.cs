using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace PetCarrier.Patches
{

    [HarmonyPatch(typeof(ZNet), "Start")]
    internal static class ZNet_Start_Patch
    {
        /*
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        private static void Postfix()
        {
            // for debug logs
        }

        private static void DumpCreaturesMass()
        {
            var results = ZNetScene.instance.m_prefabs
            .Where(prefab => prefab != null)
            .Where(prefab => prefab.GetComponent<Character>() != null)
            .Where(prefab => prefab.GetComponent<BaseAI>() != null)
            .Select(prefab =>
            {
                Rigidbody body = prefab.GetComponent<Rigidbody>();
                Collider collider = prefab.GetComponent<Collider>();

                Vector3 scale = prefab.transform.lossyScale;
                float factor = Mathf.Pow(Mathf.Abs(scale.x * scale.y * scale.z), 1f / 3f);
                float mass = body.mass * factor;

                return new { Name = prefab.name, Mass = mass };
            })
            .OrderBy(result => result.Mass)
            .ToList();

            Plugin.Log.LogInfo($"=== Creature Mass & Volume ({results.Count}) ===");
            foreach (var result in results)
            {
                Plugin.Log.LogInfo($"{result.Name,-35} Mass: {result.Mass,10:F3}");
            }
            Plugin.Log.LogInfo("=== End Creature Mass & Volume ===");
        }
        */

    }

    /*

[Info   : CrateEmUp] === Creature Mass & Volume (189) ===
[Info   : CrateEmUp] OTAB_Bat_pup                        Mass:      0,500
[Info   : CrateEmUp] OTAB_Bat_Swamp_pup                  Mass:      0,500
[Info   : CrateEmUp] Bat                                 Mass:      1,000
[Info   : CrateEmUp] Bat_Swamp                           Mass:      1,000
[Info   : CrateEmUp] FrostWisp                           Mass:      1,000
[Info   : CrateEmUp] Tick                                Mass:      2,000
[Info   : CrateEmUp] OTAB_Leech_cave_spawn               Mass:      3,300
[Info   : CrateEmUp] OTAB_Leech_spawn                    Mass:      3,300
[Info   : CrateEmUp] Boar_piggy                          Mass:      4,000
[Info   : CrateEmUp] Chicken                             Mass:      4,000
[Info   : CrateEmUp] OTAB_Neck_hatchling                 Mass:      4,000
[Info   : CrateEmUp] Deathsquito                         Mass:      5,000
[Info   : CrateEmUp] Greyling                            Mass:      5,000
[Info   : CrateEmUp] Mistile                             Mass:      5,000
[Info   : CrateEmUp] SeekerBrood                         Mass:      5,000
[Info   : CrateEmUp] OTAB_Fawn                           Mass:      5,000
[Info   : CrateEmUp] OTAB_Leveret                        Mass:      5,000
[Info   : CrateEmUp] OTAB_Ulv_cub                        Mass:      5,000
[Info   : CrateEmUp] Frysling                            Mass:      8,000
[Info   : CrateEmUp] Neck                                Mass:      8,000
[Info   : CrateEmUp] Surtling                            Mass:      8,000
[Info   : CrateEmUp] Asksvin                             Mass:     10,000
[Info   : CrateEmUp] Blob                                Mass:     10,000
[Info   : CrateEmUp] BlobAspect                          Mass:     10,000
[Info   : CrateEmUp] BlobElite                           Mass:     10,000
[Info   : CrateEmUp] BlobFrost                           Mass:     10,000
[Info   : CrateEmUp] BlobLava                            Mass:     10,000
[Info   : CrateEmUp] BlobMork                            Mass:     10,000
[Info   : CrateEmUp] BlobMorkMini                        Mass:     10,000
[Info   : CrateEmUp] BlobTar                             Mass:     10,000
[Info   : CrateEmUp] Boar                                Mass:     10,000
[Info   : CrateEmUp] Boar_spiritcaller                   Mass:     10,000
[Info   : CrateEmUp] BogWitchKvastur                     Mass:     10,000
[Info   : CrateEmUp] Charred_Archer                      Mass:     10,000
[Info   : CrateEmUp] Charred_Archer_Fader                Mass:     10,000
[Info   : CrateEmUp] Charred_Mage                        Mass:     10,000
[Info   : CrateEmUp] Charred_Melee                       Mass:     10,000
[Info   : CrateEmUp] Charred_Melee_Dyrnwyn               Mass:     10,000
[Info   : CrateEmUp] Charred_Melee_Fader                 Mass:     10,000
[Info   : CrateEmUp] Charred_Twitcher                    Mass:     10,000
[Info   : CrateEmUp] Charred_Twitcher_Summoned           Mass:     10,000
[Info   : CrateEmUp] Deer                                Mass:     10,000
[Info   : CrateEmUp] Deer_White                          Mass:     10,000
[Info   : CrateEmUp] Dverger                             Mass:     10,000
[Info   : CrateEmUp] DvergerAshlands                     Mass:     10,000
[Info   : CrateEmUp] DvergerDeepNorth                    Mass:     10,000
[Info   : CrateEmUp] DvergerMage                         Mass:     10,000
[Info   : CrateEmUp] DvergerMageFire                     Mass:     10,000
[Info   : CrateEmUp] DvergerMageIce                      Mass:     10,000
[Info   : CrateEmUp] DvergerMageSupport                  Mass:     10,000
[Info   : CrateEmUp] DvergerTest                         Mass:     10,000
[Info   : CrateEmUp] Elaking                             Mass:     10,000
[Info   : CrateEmUp] ElakingLantern                      Mass:     10,000
[Info   : CrateEmUp] ElakingMole                         Mass:     10,000
[Info   : CrateEmUp] Fenring_Cultist                     Mass:     10,000
[Info   : CrateEmUp] Fenring_Cultist_Hildir              Mass:     10,000
[Info   : CrateEmUp] Fenring_Cultist_Hildir_nochest      Mass:     10,000
[Info   : CrateEmUp] Ghost                               Mass:     10,000
[Info   : CrateEmUp] Ghost_old                           Mass:     10,000
[Info   : CrateEmUp] Ghost_sleeping                      Mass:     10,000
[Info   : CrateEmUp] Ghost_Void                          Mass:     10,000
[Info   : CrateEmUp] Goblin                              Mass:     10,000
[Info   : CrateEmUp] Goblin_Gem                          Mass:     10,000
[Info   : CrateEmUp] GoblinArcher                        Mass:     10,000
[Info   : CrateEmUp] GoblinDeepNorth                     Mass:     10,000
[Info   : CrateEmUp] GoblinShaman                        Mass:     10,000
[Info   : CrateEmUp] GoblinShaman_Hildir                 Mass:     10,000
[Info   : CrateEmUp] GoblinShaman_Hildir_nochest         Mass:     10,000
[Info   : CrateEmUp] Greydwarf                           Mass:     10,000
[Info   : CrateEmUp] Greydwarf_Frozen                    Mass:     10,000
[Info   : CrateEmUp] Hare                                Mass:     10,000
[Info   : CrateEmUp] Hen                                 Mass:     10,000
[Info   : CrateEmUp] JotunWarrior                        Mass:     10,000
[Info   : CrateEmUp] JotunWarriorDualWield               Mass:     10,000
[Info   : CrateEmUp] JotunWitch                          Mass:     10,000
[Info   : CrateEmUp] Leech                               Mass:     10,000
[Info   : CrateEmUp] Leech_cave                          Mass:     10,000
[Info   : CrateEmUp] Seeker                              Mass:     10,000
[Info   : CrateEmUp] Skeleton                            Mass:     10,000
[Info   : CrateEmUp] Skeleton_aspect                     Mass:     10,000
[Info   : CrateEmUp] Skeleton_DeepNorth                  Mass:     10,000
[Info   : CrateEmUp] Skeleton_Friendly                   Mass:     10,000
[Info   : CrateEmUp] Skeleton_Meadows                    Mass:     10,000
[Info   : CrateEmUp] Skeleton_Meadows_noarcher           Mass:     10,000
[Info   : CrateEmUp] Skeleton_Mountains                  Mass:     10,000
[Info   : CrateEmUp] Skeleton_Mountains_noarcher         Mass:     10,000
[Info   : CrateEmUp] Skeleton_NoArcher                   Mass:     10,000
[Info   : CrateEmUp] Skeleton_Poison                     Mass:     10,000
[Info   : CrateEmUp] Skeleton_Swamps                     Mass:     10,000
[Info   : CrateEmUp] Skeleton_Swamps_noarcher            Mass:     10,000
[Info   : CrateEmUp] Tendril_back                        Mass:     10,000
[Info   : CrateEmUp] Ulv                                 Mass:     10,000
[Info   : CrateEmUp] Wolf                                Mass:     10,000
[Info   : CrateEmUp] Wolf_cub                            Mass:     10,000
[Info   : CrateEmUp] Wolf_spiritcaller                   Mass:     10,000
[Info   : CrateEmUp] Writhan                             Mass:     10,000
[Info   : CrateEmUp] OTAB_Volture_chick                  Mass:     10,000
[Info   : CrateEmUp] OTAB_Kvastur                        Mass:     10,000
[Info   : CrateEmUp] OTAB_GrowingMorgen1                 Mass:     12,500
[Info   : CrateEmUp] OTAB_GrowingStoneGolem1             Mass:     12,500
[Info   : CrateEmUp] Greydwarf_Shaman                    Mass:     15,000
[Info   : CrateEmUp] Greydwarf_Shaman_Frozen             Mass:     15,000
[Info   : CrateEmUp] Hatchling                           Mass:     15,000
[Info   : CrateEmUp] Wraith                              Mass:     15,000
[Info   : CrateEmUp] OTAB_Serpent_spawn                  Mass:     15,000
[Info   : CrateEmUp] Asksvin_hatchling                   Mass:     18,000
[Info   : CrateEmUp] Draugr                              Mass:     20,000
[Info   : CrateEmUp] Draugr_Ranged                       Mass:     20,000
[Info   : CrateEmUp] Draugr_Ranged_sleeping              Mass:     20,000
[Info   : CrateEmUp] Draugr_sleeping                     Mass:     20,000
[Info   : CrateEmUp] FallenValkyrie                      Mass:     20,000
[Info   : CrateEmUp] FallenWarrior                       Mass:     20,000
[Info   : CrateEmUp] Greydwarf_Elite                     Mass:     20,000
[Info   : CrateEmUp] Lox_Calf                            Mass:     20,000
[Info   : CrateEmUp] Moose_calf                          Mass:     20,000
[Info   : CrateEmUp] ShadowPerson                        Mass:     20,000
[Info   : CrateEmUp] Volture                             Mass:     20,000
[Info   : CrateEmUp] OTAB_GrowingAbomination1            Mass:     25,000
[Info   : CrateEmUp] OTAB_GrowingMorgen2                 Mass:     25,000
[Info   : CrateEmUp] OTAB_GrowingStoneGolem2             Mass:     25,000
[Info   : CrateEmUp] BonemawSerpent                      Mass:     30,000
[Info   : CrateEmUp] Draugr_Elite                        Mass:     30,000
[Info   : CrateEmUp] Draugr_Elite_sleeping               Mass:     30,000
[Info   : CrateEmUp] Fenring                             Mass:     30,000
[Info   : CrateEmUp] Serpent                             Mass:     30,000
[Info   : CrateEmUp] Skeleton_Hildir                     Mass:     30,000
[Info   : CrateEmUp] Skeleton_Hildir_nochest             Mass:     30,000
[Info   : CrateEmUp] OTAB_Bjorn_cub                      Mass:     30,000
[Info   : CrateEmUp] OTAB_MiniEikthyr                    Mass:     33,000
[Info   : CrateEmUp] OTAB_GrowingMorgen3                 Mass:     37,500
[Info   : CrateEmUp] OTAB_GrowingStoneGolem3             Mass:     37,500
[Info   : CrateEmUp] Seal_Pup                            Mass:     40,000
[Info   : CrateEmUp] Aspect_Eikthyr                      Mass:     50,000
[Info   : CrateEmUp] Barka                               Mass:     50,000
[Info   : CrateEmUp] Eikthyr                             Mass:     50,000
[Info   : CrateEmUp] Gjall                               Mass:     50,000
[Info   : CrateEmUp] GoblinBrute                         Mass:     50,000
[Info   : CrateEmUp] GoblinBrute_Hildir                  Mass:     50,000
[Info   : CrateEmUp] GoblinBruteBros                     Mass:     50,000
[Info   : CrateEmUp] GoblinBruteBros_nochest             Mass:     50,000
[Info   : CrateEmUp] Morgen                              Mass:     50,000
[Info   : CrateEmUp] Morgen_NonSleeping                  Mass:     50,000
[Info   : CrateEmUp] SeekerBrute                         Mass:     50,000
[Info   : CrateEmUp] StoneGolem                          Mass:     50,000
[Info   : CrateEmUp] Troll                               Mass:     50,000
[Info   : CrateEmUp] Troll_sleeping                      Mass:     50,000
[Info   : CrateEmUp] Troll_Summoned                      Mass:     50,000
[Info   : CrateEmUp] OTAB_GrowingAbomination2            Mass:     50,000
[Info   : CrateEmUp] Bjorn                               Mass:     60,000
[Info   : CrateEmUp] Bjorn_sleeping                      Mass:     60,000
[Info   : CrateEmUp] Bjorn_spiritcaller                  Mass:     60,000
[Info   : CrateEmUp] Lox                                 Mass:     60,000
[Info   : CrateEmUp] Moose                               Mass:     60,000
[Info   : CrateEmUp] Moose_spiritcaller                  Mass:     60,000
[Info   : CrateEmUp] Unbjorn                             Mass:     60,000
[Info   : CrateEmUp] OTAB_MiniBonemass                   Mass:     66,000
[Info   : CrateEmUp] OTAB_MiniFader                      Mass:     66,000
[Info   : CrateEmUp] OTAB_MiniSeekerQueen                Mass:     66,000
[Info   : CrateEmUp] OTAB_MiniYagluth                    Mass:     66,000
[Info   : CrateEmUp] OTAB_MiniElder                      Mass:     66,000
[Info   : CrateEmUp] OTAB_GrowingAbomination3            Mass:     75,000
[Info   : CrateEmUp] Abomination                         Mass:    100,000
[Info   : CrateEmUp] Aspect_Bonemass                     Mass:    100,000
[Info   : CrateEmUp] Aspect_Fader                        Mass:    100,000
[Info   : CrateEmUp] Aspect_SeekerQueen                  Mass:    100,000
[Info   : CrateEmUp] Aspect_Yagluth                      Mass:    100,000
[Info   : CrateEmUp] Bonemass                            Mass:    100,000
[Info   : CrateEmUp] Fader                               Mass:    100,000
[Info   : CrateEmUp] FrozenKing                          Mass:    100,000
[Info   : CrateEmUp] FrozenKing_p2                       Mass:    100,000
[Info   : CrateEmUp] FrozenKing_p3                       Mass:    100,000
[Info   : CrateEmUp] GoblinKing                          Mass:    100,000
[Info   : CrateEmUp] Hive                                Mass:    100,000
[Info   : CrateEmUp] Seal                                Mass:    100,000
[Info   : CrateEmUp] SeekerQueen                         Mass:    100,000
[Info   : CrateEmUp] Aspect_Elder                        Mass:    100,000
[Info   : CrateEmUp] gd_king                             Mass:    100,000
[Info   : CrateEmUp] Aspect_TentaRoot                    Mass:    200,000
[Info   : CrateEmUp] Tendril                             Mass:    200,000
[Info   : CrateEmUp] TentaRoot                           Mass:    200,000
[Info   : CrateEmUp] TentaRoot_wild                      Mass:    200,000
[Info   : CrateEmUp] TrollFrost                          Mass:    200,000
[Info   : CrateEmUp] staff_greenroots_tentaroot          Mass:    300,000
[Info   : CrateEmUp] OTAB_MiniModer                      Mass:    330,000
[Info   : CrateEmUp] Aspect_Moder                        Mass:    500,000
[Info   : CrateEmUp] Dragon                              Mass:    500,000
[Info   : CrateEmUp] TheHive                             Mass:   1000,000
[Info   : CrateEmUp] piece_TrainingDummy                 Mass:  15000,000
[Info   : CrateEmUp] TrainingDummy                       Mass:  15000,000
[Info   : CrateEmUp] === End Creature Mass & Volume ===

    */

}
