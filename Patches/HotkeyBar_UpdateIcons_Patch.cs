using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(HotkeyBar), "UpdateIcons")]
    internal static class HotkeyBar_UpdateIcons_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(HotkeyBar __instance)
        {
            Player player = Player.m_localPlayer;
            if (!player)
            {
                return;
            }

            IList elements = Traverse.Create(__instance).Field("m_elements").GetValue<IList>();
            IList items = Traverse.Create(__instance).Field("m_items").GetValue<IList>();
            if (elements == null || items == null)
            {
                return;
            }

            foreach (ItemDrop.ItemData item in items)
            {
                if (!PetCarrier.Components.CratePlacement.IsPlacing(item))
                {
                    continue;
                }

                int index = item.m_gridPos.x;
                if (index < 0 || index >= elements.Count)
                {
                    continue;
                }

                GameObject equipped = Traverse.Create(elements[index]).Field("m_equiped").GetValue<GameObject>();
                if (equipped)
                {
                    equipped.SetActive(true);
                }
            }
        }
    }
}
