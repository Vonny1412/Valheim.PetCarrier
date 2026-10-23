using HarmonyLib;
using PetCarrier.Components;
using System.Collections.Generic;
using UnityEngine;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(VisEquipment), "UpdateEquipmentVisuals")]
    internal static class VisEquipment_UpdateEquipmentVisuals_Patch
    {

        private static readonly Dictionary<long, (GameObject instance, string prefab)>LastEquippedPrefabs = new Dictionary<long, (GameObject instance, string prefab)>();
        internal static void RemovePlayer(long playerId)
        {
            LastEquippedPrefabs.Remove(playerId);
        }

        [HarmonyPostfix]
        private static void Postfix(VisEquipment __instance, GameObject ___m_rightItemInstance)
        {
            Player player = __instance.GetComponent<Player>();
            if (!player)
            {
                return;
            }

            long playerId = player.GetPlayerID();
            if (playerId == 0L)
            {
                return;
            }

            if (!___m_rightItemInstance)
            {
                LastEquippedPrefabs.Remove(playerId);
                return;
            }

            Transform visual = ___m_rightItemInstance.transform.Find("Visual");
            if (!visual)
            {
                LastEquippedPrefabs.Remove(playerId);
                return;
            }

            ZNetView nview = player.GetComponent<ZNetView>();
            if (!nview || !nview.IsValid())
            {
                return;
            }

            int equippedHash = nview.GetZDO().GetInt(ZDOVars.s_rightItem);
            int crateHash = Plugin.Constants.FilledPrefabName.GetStableHashCode();
            if (equippedHash != crateHash)
            {
                LastEquippedPrefabs.Remove(playerId);
                return;
            }


            string prefabName = nview.GetZDO().GetString(Plugin.ZDOVars.z_equippedCreaturePrefab, "");
            CrateAttachedSprite sprite = visual.GetComponent<CrateAttachedSprite>();

            bool unchanged = LastEquippedPrefabs.TryGetValue(playerId, out var last) && last.instance == ___m_rightItemInstance && last.prefab == prefabName;

            if (unchanged && sprite)
            {
                return;
            }

            if (!sprite)
            {
                sprite = visual.gameObject.AddComponent<CrateAttachedSprite>();
            }

            Sprite creatureSprite;

            if (string.IsNullOrEmpty(prefabName))
            {
                creatureSprite = CrateResources.CreatureUnknown;
            }
            else
            {
                GameObject creaturePrefab = ZNetScene.instance ? ZNetScene.instance.GetPrefab(prefabName) : null;
                creatureSprite = creaturePrefab ? CreatureSnapshot.Get(creaturePrefab) : CrateResources.CreatureUnavailable;
            }

            sprite.SetSprite(creatureSprite);
            LastEquippedPrefabs[playerId] = (___m_rightItemInstance, prefabName);
        }

        /*
        [HarmonyPostfix]
        private static void Postfix(VisEquipment __instance, GameObject ___m_rightItemInstance)
        {
            Player player = Player.m_localPlayer;
            if (!player || __instance.gameObject != player.gameObject || !___m_rightItemInstance)
            {
                return;
            }

            CrateAttachedSprite sprite = ___m_rightItemInstance.GetComponentInChildren<CrateAttachedSprite>(true);
            if (sprite)
            {
                return;
            }

            ItemDrop.ItemData item = Traverse.Create(player).Field("m_rightItem").GetValue<ItemDrop.ItemData>();
            if (item == null || item.m_dropPrefab == null || item.m_dropPrefab.name != Plugin.Constants.FilledPrefabName)
            {
                return;
            }

            Transform visual = ___m_rightItemInstance.transform.Find("Visual");
            if (!visual)
            {
                return;
            }

            sprite = visual.gameObject.AddComponent<CrateAttachedSprite>();
            sprite.SetFromItem(item);
        }
        */



    }
}