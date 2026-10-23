using System.Reflection;
using UnityEngine;

namespace PetCarrier
{
    internal static class CrateItemData
    {

        internal static void GetSource(ItemDrop.ItemData item, out long worldUID, out string worldName, out bool hasLock, out bool isLocked)
        {
            worldUID = default;
            worldName = default;
            hasLock = false;
            isLocked = false;

            if (!item.m_customData.TryGetValue(Plugin.ZDOVars.s_creatureSource, out string source))
            {
                hasLock = Plugin.Configs.LockCreaturesToWorld.Value;
                isLocked = hasLock;
                return;
            }

            var split = source.Split(new[] { '|' }, 3);
            if (split.Length != 3 || !long.TryParse(split[0], out long parsedUID) || (split[2] != "0" && split[2] != "1"))
            {
                hasLock = true;
                isLocked = true;
                return;
            }

            worldUID = parsedUID;
            worldName = split[1];
            hasLock = split[2] == "1" || Plugin.Configs.LockCreaturesToWorld.Value;
            isLocked = hasLock && worldUID != ZNet.instance.GetWorldUID();
        }

        internal static void SetSource(ZDO zdo)
        {
            var worldUID = ZNet.instance.GetWorldUID();
            var worldName = ZNet.instance.GetWorldName();
            var worldLock = Plugin.Configs.LockCreaturesToWorld.Value ? "1" : "0";
            zdo.Set(Plugin.ZDOVars.z_creatureSource, $"{worldUID}|{worldName}|{worldLock}");
        }

        internal static bool IsLocked(ItemDrop.ItemData item)
        {
            GetSource(item, out long _, out string _, out bool _, out bool isLocked);
            return isLocked;
        }

        internal static bool TransferFromZDO(ItemDrop.ItemData item, ZDO zdo)
        {
            string prefab = zdo.GetString(Plugin.ZDOVars.z_creaturePrefab, "");
            string data = zdo.GetString(Plugin.ZDOVars.z_creatureData, "");
            string name = zdo.GetString(Plugin.ZDOVars.z_creatureName, "");
            string source = zdo.GetString(Plugin.ZDOVars.z_creatureSource, "");

            if (string.IsNullOrEmpty(prefab) || string.IsNullOrEmpty(data))
            {
                return false;
            }

            item.m_customData[Plugin.ZDOVars.s_creaturePrefab] = prefab;
            item.m_customData[Plugin.ZDOVars.s_creatureData] = data;

            if (!string.IsNullOrEmpty(name))
            {
                item.m_customData[Plugin.ZDOVars.s_creatureName] = name;
            }
            if (!string.IsNullOrEmpty(source))
            {
                item.m_customData[Plugin.ZDOVars.s_creatureSource] = source;
            }

            return true;
        }

        internal static bool TransferToZDO(ItemDrop.ItemData item, ZDO zdo)
        {
            if (item == null || zdo == null)
            {
                return false;
            }

            if (!item.m_customData.TryGetValue(Plugin.ZDOVars.s_creaturePrefab, out string prefab) || string.IsNullOrEmpty(prefab))
            {
                return false;
            }

            if (!item.m_customData.TryGetValue(Plugin.ZDOVars.s_creatureData, out string data) || string.IsNullOrEmpty(data))
            {
                return false;
            }

            zdo.Set(Plugin.ZDOVars.z_creaturePrefab, prefab);
            zdo.Set(Plugin.ZDOVars.z_creatureData, data);

            if (item.m_customData.TryGetValue(Plugin.ZDOVars.s_creatureName, out string name) && !string.IsNullOrEmpty(name))
            {
                zdo.Set(Plugin.ZDOVars.z_creatureName, name);
            }

            if (item.m_customData.TryGetValue(Plugin.ZDOVars.s_creatureSource, out string source) && !string.IsNullOrEmpty(source))
            {
                zdo.Set(Plugin.ZDOVars.z_creatureSource, source);
            }

            return true;
        }

        internal static void SetCrateSharedData(ItemDrop.ItemData item)
        {
            item.m_customData.TryGetValue(Plugin.ZDOVars.s_creaturePrefab, out string creaturePrefabName);
            item.m_customData.TryGetValue(Plugin.ZDOVars.s_creatureData, out string creatureData);
            item.m_customData.TryGetValue(Plugin.ZDOVars.s_creatureName, out string creatureName);

            if (string.IsNullOrEmpty(creatureData) || string.IsNullOrEmpty(creaturePrefabName))
            {
                Plugin.Log.LogWarning("Filled crate has missing data.");
                creaturePrefabName = null; // trigger error foreground icon
            }

            var creatureUnknown = false;
            if (string.IsNullOrEmpty(creatureName))
            {
                creatureName = "???";
                creatureUnknown = true;
            }

            MakeSharedUnique(item);
            item.m_shared.m_description = Plugin.Constants.TranslationKey_CrateFilledDescription;
            item.m_shared.m_weight = Plugin.Configs.CrateBaseWeight.Value + (CreatureMass.Get(creaturePrefabName) * Plugin.Configs.CrateWeightMassFactor.Value);
            item.m_shared.m_name = Localization.instance.Localize(Plugin.Constants.TranslationKey_CrateFilled, creatureName);

            Sprite foreground = null;
            if (!creatureUnknown)
            {
                GameObject creaturePrefab = ZNetScene.instance.GetPrefab(creaturePrefabName);
                if (creaturePrefab)
                {
                    foreground = CreatureSnapshot.Get(creaturePrefab);
                }
                if (!foreground)
                {
                    foreground = CrateResources.CreatureUnavailable;
                }
            } else
            {
                foreground = CrateResources.CreatureUnknown;
            }

            Sprite icon = CrateIcon.Bake(item, creaturePrefabName, foreground);
            if (icon)
            {
                item.m_shared.m_icons[0] = icon;
            }

            GetSource(item, out long _, out string worldName, out bool hasLock, out bool isLocked);
            if (hasLock && !string.IsNullOrEmpty(worldName))
            {
                var color = isLocked ? "red" : "yellow";
                item.m_shared.m_description += $"\n\n<color={color}>" + Localization.instance.Localize(Plugin.Constants.TranslationKey_LockedToWorld, worldName) + "</color>";
            }
        }

        private static void MakeSharedUnique(ItemDrop.ItemData item)
        {
            var original = item.m_shared;
            var copy = new ItemDrop.ItemData.SharedData();

            foreach (FieldInfo field in typeof(ItemDrop.ItemData.SharedData).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                field.SetValue(copy, field.GetValue(original));
            }

            copy.m_icons = (Sprite[])original.m_icons.Clone();
            item.m_shared = copy;
        }

    }
}
