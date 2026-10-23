using HarmonyLib;

namespace PetCarrier.Patches
{
    [HarmonyPatch]
    internal static class ConfigWorldLock_Patch
    {
        [HarmonyPatch(typeof(Game), "Start")]
        [HarmonyPostfix]
        private static void OnGameStart()
        {
            Plugin.Configs.SetWorldLock(true);
        }

        [HarmonyPatch(typeof(Game), "OnDestroy")]
        [HarmonyPostfix]
        private static void OnGameDestroy()
        {
            Plugin.Configs.SetWorldLock(false);
        }
    }
}