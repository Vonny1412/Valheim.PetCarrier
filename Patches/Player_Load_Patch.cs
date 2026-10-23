using HarmonyLib;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Player), "Load", new[] { typeof(ZPackage) })]
    internal static class Player_Load_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(Player __instance)
        {
            if (!ZNetScene.instance)
            {
                return;
            }

            foreach (ItemDrop.ItemData item in __instance.GetInventory().GetAllItems())
            {
                if (item.m_dropPrefab && item.m_dropPrefab.name == Plugin.Constants.FilledPrefabName)
                {
                    CrateItemData.SetCrateSharedData(item);
                }
            }

        }
    }
}
