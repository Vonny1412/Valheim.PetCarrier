using System;
using System.Collections.Generic;
using UnityEngine;

namespace PetCarrier
{
    internal static class CrateUnpacking
    {
        internal static bool TryUnpack(Player player, ItemDrop.ItemData item, Vector3 position)
        {
            if (!player || item == null ||!ZNetScene.instance || ZDOMan.instance == null)
            {
                return false;
            }

            Inventory inventory = player.GetInventory();

            if (!inventory.ContainsItem(item))
            {
                return false;
            }

            if (CrateItemData.IsLocked(item))
            {
                player.Message(MessageHud.MessageType.Center, Plugin.Constants.TranslationKey_MessageIsLocked);
                return true;
            }

            item.m_customData.TryGetValue(Plugin.ZDOVars.s_creaturePrefab, out string prefabName);
            item.m_customData.TryGetValue(Plugin.ZDOVars.s_creatureData, out string serializedData);
            if (string.IsNullOrEmpty(prefabName) || string.IsNullOrEmpty(serializedData))
            {
                Plugin.Log.LogWarning("Crate has missing creature data.");
                return false;
            }

            Dictionary<string, string> data;
            try
            {
                data = CrateZDO.ParseData(serializedData);
                if (!CrateZDO.ValidateData(data))
                {
                    Plugin.Log.LogWarning("Invalid creature ZDO data.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogWarning($"Could not parse crate data: {ex}");
                return false;
            }

            GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);
            if (!prefab || !prefab.GetComponent<Character>() || !prefab.GetComponent<ZNetView>())
            {
                Plugin.Log.LogWarning($"Creature prefab unavailable: {prefabName}");
                return false;
            }

            GameObject creature = null;
            try
            {
                creature = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
                ZNetView nview = creature.GetComponent<ZNetView>();
                if (!nview || !nview.IsValid() || !nview.IsOwner())
                {
                    Plugin.Log.LogWarning("Spawned creature has no valid owned ZNetView.");
                    return false;
                }

                CrateZDO.RestoreData(nview.GetZDO(), data);
                Character character = creature.GetComponent<Character>();
                character.SetTamed(nview.GetZDO().GetBool(ZDOVars.s_tamed, false));
                character.SetLevel(nview.GetZDO().GetInt(ZDOVars.s_level, 1));







                if (player.IsItemEquiped(item))
                {
                    player.UnequipItem(item, triggerEquipEffects: false);
                }



                Vector2i slot = item.m_gridPos;
                if (!inventory.RemoveOneItem(item))
                {
                    Plugin.Log.LogWarning("Could not remove crate from inventory.");
                    return false;
                }

                Plugin.Log.LogInfo($"Unpacked creature: {prefabName}");

                SpawnReleaseEffect(character, position);

                // The spawned creature must remain in the world.
                creature = null;

                return true;
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to unpack creature: {ex}");
                return false;
            }
            finally
            {
                // Roll back a failed spawn.
                if (creature)
                {
                    ZNetView nview = creature.GetComponent<ZNetView>();
                    if (nview && nview.IsValid() && nview.IsOwner())
                    {
                        nview.Destroy();
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(creature);
                    }
                }
            }
        }

        private static void SpawnReleaseEffect(Character character, Vector3 position)
        {
            if (!character || character.IsFlying())
                return;

            GameObject prefab = ZNetScene.instance.GetPrefab("vfx_Place_mud_road");
            if (!prefab)
            {
                return;
            }

            UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
        }

    }
}
