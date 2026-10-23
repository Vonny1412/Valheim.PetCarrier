using HarmonyLib;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Humanoid), "UseItem")]
    internal static class Humanoid_UseItem_Patch
    {
        [HarmonyPrefix]
        private static bool Prefix(Humanoid __instance, Inventory inventory, ItemDrop.ItemData item, bool fromInventoryGui)
        {
            if (!(__instance is Player player))
            {
                return true;
            }

            if (fromInventoryGui || item?.m_dropPrefab == null || item.m_dropPrefab.name != Plugin.Constants.EmptyPrefabName)
            {
                return true;
            }

            Character creature = player.GetHoverCreature();
            if (!creature)
            {
                return true;
            }

            CratePacking.TryPack(player, inventory, item, creature);
            return false;
        }
    }
}
