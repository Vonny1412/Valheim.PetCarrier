
using HarmonyLib;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Character), "Awake")]
    internal static class Character_Awake_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(Character __instance)
        {
            if (__instance is Player player)
            {
                CratePacking.RegisterPlayerRPC(player);
            }
            else
            {
                CratePacking.RegisterCreatureRPC(__instance);
            }
        }
    }
}
