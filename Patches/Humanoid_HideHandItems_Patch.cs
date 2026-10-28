using HarmonyLib;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.HideHandItems))]
    internal static class Humanoid_HideHandItems_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(Humanoid __instance, ref ItemDrop.ItemData ___m_hiddenRightItem)
        {
            if (__instance is not Player)
            {
                return;
            }

            ItemDrop.ItemData item = ___m_hiddenRightItem;

            if (item?.m_dropPrefab == null || item.m_dropPrefab.name != Plugin.Constants.FilledPrefabName)
            {
                return;
            }

            __instance.UnequipItem(item, triggerEquipEffects: false);
        }
    }
}