using HarmonyLib;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Humanoid), "SetupEquipment")]
    internal static class Humanoid_SetupEquipment_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(Humanoid __instance, ItemDrop.ItemData ___m_rightItem)
        {
            if (!(__instance is Player player))
            {
                return;
            }

            ZNetView nview = player.GetComponent<ZNetView>();
            if (!nview || !nview.IsValid() || !nview.IsOwner())
            {
                return;
            }

            string prefabName = "";
            if (___m_rightItem?.m_dropPrefab && ___m_rightItem.m_dropPrefab.name == Plugin.Constants.FilledPrefabName)
            {
                ___m_rightItem.m_customData.TryGetValue(Plugin.ZDOVars.s_creaturePrefab, out prefabName);
            }
            prefabName = prefabName ?? "";

            ZDO zdo = nview.GetZDO();
            int key = Plugin.ZDOVars.z_equippedCreaturePrefab;
            if (zdo.GetString(key, "") != prefabName)
            {
                zdo.Set(key, prefabName);
            }
        }
    }
}