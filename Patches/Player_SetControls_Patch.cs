using HarmonyLib;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.SetControls))]
    internal static class Player_SetControls_Patch
    {
        [HarmonyPrefix]
        private static void Prefix(ItemDrop.ItemData ___m_rightItem, ref bool block, ref bool blockHold)
        {
            if (___m_rightItem?.m_dropPrefab == null || ___m_rightItem.m_dropPrefab.name != Plugin.Constants.FilledPrefabName)
            {
                return;
            }

            block = false;
            blockHold = false;
        }
    }
}