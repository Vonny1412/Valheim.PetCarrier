using HarmonyLib;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Shutdown))]
    internal static class ZNetScene_Shutdown_Patch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            CreatureSnapshot.Clear();
            CrateIcon.Clear();
            CreatureMass.Clear();
            CratePacking.Clear();
        }
    }
}
