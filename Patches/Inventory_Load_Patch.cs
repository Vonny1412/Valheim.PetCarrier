using HarmonyLib;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.Load), typeof(ZPackage))]
    internal static class Inventory_Load_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(Inventory __instance)
        {
            if (!ZNetScene.instance)
            {
                return;
            }

            foreach (ItemDrop.ItemData item in __instance.GetAllItems())
            {
                if (item.m_dropPrefab && item.m_dropPrefab.name == Plugin.Constants.FilledPrefabName)
                {
                    CrateItemData.SetCrateSharedData(item);
                }
            }
        }
    }
}