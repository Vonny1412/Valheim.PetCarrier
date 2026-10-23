
using HarmonyLib;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(ItemDrop), nameof(ItemDrop.DropItem))]
    internal static class ItemDrop_DropItem_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(ItemDrop __result)
        {
            if (!__result ||
                !__result.m_itemData.m_dropPrefab ||
                __result.m_itemData.m_dropPrefab.name != Plugin.Constants.FilledPrefabName)
            {
                return;
            }

            ZNetView nview = __result.GetComponent<ZNetView>();

            if (!nview || !nview.IsValid() || !nview.IsOwner())
            {
                return;
            }

            if (!CrateItemData.TransferToZDO(__result.m_itemData, nview.GetZDO()))
            {
                Plugin.Log.LogWarning("Failed to transfer creature data to dropped crate ZDO.");
            }
        }
    }
}
