using HarmonyLib;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.AddKnownItem))]
    internal static class Player_AddKnownItem_Patch
    {
        [HarmonyPrefix]
        private static bool Prefix(ItemDrop.ItemData item)
        {
            return item?.m_dropPrefab == null || item.m_dropPrefab.name != Plugin.Constants.FilledPrefabName;
        }
    }
}