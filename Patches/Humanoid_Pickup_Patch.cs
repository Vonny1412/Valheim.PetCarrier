using HarmonyLib;
using UnityEngine;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Humanoid), "Pickup")]
    internal static class Humanoid_Pickup_Patch
    {
        [HarmonyPrefix]
        private static void Prefix(GameObject go)
        {
            if (!go || !go.TryGetComponent<ItemDrop>(out var itemDrop))
            {
                return;
            }

            if (Utils.GetPrefabName(go.name) != Plugin.Constants.FilledPrefabName)
            {
                return;
            }

            ZNetView nview = go.GetComponent<ZNetView>();
            if (!nview || !nview.IsValid())
            {
                return;
            }

            ZDO zdo = nview.GetZDO();

            itemDrop.Load();

            if (!CrateItemData.TransferFromZDO(itemDrop.m_itemData, zdo))
            {
                Plugin.Log.LogWarning("Failed to transfer creature data to crate item.");
                // do not return here
            }

            CrateItemData.SetCrateSharedData(itemDrop.m_itemData);
        }
    }
}
