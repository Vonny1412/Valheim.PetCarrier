using HarmonyLib;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Player), "OnDestroy")]
    internal static class Player_OnDestroy_Patch
    {
        [HarmonyPrefix]
        private static void Prefix(Player __instance)
        {
            long playerId = __instance.GetPlayerID();

            if (playerId != 0L)
            {
                VisEquipment_UpdateEquipmentVisuals_Patch.RemovePlayer(playerId);
            }
        }
    }
}