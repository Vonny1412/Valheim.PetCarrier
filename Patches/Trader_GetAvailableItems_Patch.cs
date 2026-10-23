using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Trader), nameof(Trader.GetAvailableItems))]
    internal static class Trader_GetAvailableItems_Patch
    {
        private static Trader.TradeItem crateTradeItem;

        [HarmonyPostfix]
        private static void Postfix(Trader __instance, ref List<Trader.TradeItem> __result)
        {
            if (!__instance.name.StartsWith("Haldor"))
            {
                return;
            }

            GameObject prefab = ZNetScene.instance?.GetPrefab(Plugin.Constants.EmptyPrefabName);
            if (!prefab)
            {
                return;
            }

            ItemDrop itemDrop = prefab.GetComponent<ItemDrop>();
            if (!itemDrop)
            {
                return;
            }

            foreach (Trader.TradeItem tradeItem in __result)
            {
                if (tradeItem.m_prefab == itemDrop)
                {
                    return;
                }
            }

            crateTradeItem ??= new Trader.TradeItem
            {
                m_prefab = itemDrop,
                m_stack = 1,
                m_requiredGlobalKey = "",
                m_tooltip = Plugin.Constants.TranslationKey_TraderTooltip,
                m_buyPlayerEffects = new EffectList()
            };

            crateTradeItem.m_prefab = itemDrop;
            crateTradeItem.m_price = Plugin.Configs.CratePrice.Value;

            __result.Add(crateTradeItem);
        }
    }
}