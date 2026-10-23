using HarmonyLib;
using PetCarrier.Components;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
    internal static class Humanoid_EquipItem_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(Humanoid __instance, ItemDrop.ItemData item, bool __result)
        {
            if (!__result || !(__instance is Player player))
            {
                return;
            }

            if (player != Player.m_localPlayer)
            {
                return;
            }

            if (item?.m_dropPrefab == null || item.m_dropPrefab.name != Plugin.Constants.FilledPrefabName)
            {
                return;
            }

            CratePlacement.Begin(player, item);
        }
    }
}