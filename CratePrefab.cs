using BepInEx.Logging;
using PetCarrier.Components;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace PetCarrier
{
    internal static class CratePrefab
    {
        internal static void Register(ManualLogSource log)
        {
            GameObject source = PrefabManager.Instance.GetPrefab(Plugin.Constants.VisualPrefabName);

            if (source == null)
            {
                log.LogWarning($"Prefab '{Plugin.Constants.VisualPrefabName}' not found.");
                return;
            }

            Transform sourceVisual = source.transform.Find("default (4)");

            if (sourceVisual == null)
            {
                log.LogWarning($"Visual not found in '{Plugin.Constants.VisualPrefabName}'.");
                return;
            }

            var itemDropEmpty = CreateAndRegisterPrefab(Plugin.Constants.EmptyPrefabName, sourceVisual, equipable: false);
            itemDropEmpty.m_autoDestroy = false;
            itemDropEmpty.m_autoPickup = true;

            var itemDropEmptyFloating = itemDropEmpty.gameObject.AddComponent<Floating>();
            itemDropEmptyFloating.m_waterLevelOffset = 0.5f;

            var itemDropEmptyShared = itemDropEmpty.m_itemData.m_shared;
            itemDropEmptyShared.m_name = Plugin.Constants.TranslationKey_CrateEmpty;
            itemDropEmptyShared.m_description = Plugin.Constants.TranslationKey_CrateEmptyDescription;
            itemDropEmptyShared.m_itemType = ItemDrop.ItemData.ItemType.Misc;
            itemDropEmptyShared.m_maxStackSize = Plugin.Configs.CrateMaxStackSize.Value; 
            itemDropEmptyShared.m_weight = Plugin.Configs.CrateBaseWeight.Value;
            itemDropEmptyShared.m_teleportable = true; // empty should always be transportable
            itemDropEmptyShared.m_autoStack = false;
            itemDropEmptyShared.m_canBeReparied = false;
            itemDropEmptyShared.m_maxQuality = 1;
            itemDropEmptyShared.m_value = 0;
            itemDropEmptyShared.m_maxDurability = 1;
            itemDropEmptyShared.m_useDurability = false;

            var itemDropFilled = CreateAndRegisterPrefab(Plugin.Constants.FilledPrefabName, sourceVisual, equipable: true);
            itemDropFilled.m_autoDestroy = false;
            itemDropFilled.m_autoPickup = false;
            itemDropFilled.gameObject.AddComponent<CrateAttachedSprite>();

            var itemDropFilledFloating = itemDropFilled.gameObject.AddComponent<Floating>();
            itemDropFilledFloating.m_waterLevelOffset = 0.33f;

            var itemDropFilledShared = itemDropFilled.m_itemData.m_shared;
            itemDropFilledShared.m_name = Plugin.Constants.TranslationKey_CrateFilled;
            itemDropFilledShared.m_description = Plugin.Constants.TranslationKey_CrateFilledDescription;
            itemDropFilledShared.m_itemType = ItemDrop.ItemData.ItemType.Tool;
            itemDropFilledShared.m_animationState = ItemDrop.ItemData.AnimationState.Feaster;
            itemDropFilledShared.m_maxStackSize = 1;
            itemDropFilledShared.m_weight = Plugin.Configs.CrateBaseWeight.Value;
            itemDropFilledShared.m_teleportable = Plugin.Configs.CrateTeleportable.Value;
            itemDropFilledShared.m_autoStack = false;
            itemDropFilledShared.m_canBeReparied = false;
            itemDropFilledShared.m_maxQuality = 1;
            itemDropFilledShared.m_value = 0;
            itemDropFilledShared.m_maxDurability = 1;
            itemDropFilledShared.m_useDurability = false;
            itemDropFilledShared.m_equipDuration = 0f;
        }

        private static ItemDrop CreateAndRegisterPrefab(string prefabName, Transform sourceVisual, bool equipable = false)
        {
            GameObject prefab = new GameObject(prefabName);
            prefab.SetActive(false);

            ZNetView nview = prefab.AddComponent<ZNetView>();
            nview.m_persistent = true;
            nview.m_type = ZDO.ObjectType.Solid;

            Rigidbody body = prefab.AddComponent<Rigidbody>();
            body.mass = 1f;

            ItemDrop itemDrop = prefab.AddComponent<ItemDrop>();
            itemDrop.m_itemData.m_shared = new ItemDrop.ItemData.SharedData();

            GameObject visual = Object.Instantiate(sourceVisual.gameObject, prefab.transform, false);
            visual.name = "Visual";
            visual.transform.localScale *= Plugin.Constants.VisualSizeFactor;

            Object.DestroyImmediate(visual.GetComponent<BoxCollider>());

            MeshFilter meshFilter = visual.GetComponent<MeshFilter>();
            Bounds bounds = meshFilter.sharedMesh.bounds;
            Vector3 scale = visual.transform.localScale;

            BoxCollider collider = prefab.AddComponent<BoxCollider>();
            collider.center = visual.transform.localPosition + Vector3.Scale(bounds.center, scale);
            collider.size = Vector3.Scale(bounds.size, new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z)));

            itemDrop.m_itemData.m_shared.m_icons = new[]
            {
                RenderManager.Instance.Render(
                    new RenderManager.RenderRequest(prefab)
                    {
                        Rotation = Quaternion.Euler(12f, 15f, 0f), //RenderManager.IsometricRotation,
                        UseCache = false,
                        Width = 64,
                        Height = 64
                    }
                )
            };

            if (equipable)
            {
                GameObject attach = new GameObject("attach");
                attach.transform.SetParent(prefab.transform, false);

                GameObject attachVisual = Object.Instantiate(visual.gameObject, attach.transform, false);
                attachVisual.name = "Visual";

                Quaternion visualRotation = Quaternion.Euler(5.58f, 2.77f, 116.38f);
                Vector3 position = new Vector3(Plugin.Constants.EquipPositionX, Plugin.Constants.EquipPositionY, Plugin.Constants.EquipPositionZ);
                attachVisual.transform.localPosition = visualRotation * position;
                attachVisual.transform.localRotation = visualRotation;

                attach.SetActive(false);
            }

            ItemManager.Instance.AddItem(new CustomItem(prefab, fixReference: false));
            prefab.SetActive(true);

            return itemDrop;
        }





    }
}
