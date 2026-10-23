using UnityEngine;

namespace PetCarrier.Components
{
    internal class CrateAttachedSprite : MonoBehaviour
    {

        private readonly SpriteRenderer[] m_renderers = new SpriteRenderer[4];

        private void Awake()
        {
            var height = Plugin.Constants.CrateSpriteHeight;
            var offset = Plugin.Constants.CrateSpriteOffset;
            CreateSide(0, new Vector3(0f, height, offset), 0f);
            CreateSide(1, new Vector3(offset, height, 0f), 90f);
            CreateSide(2, new Vector3(0f, height, -offset), 180f);
            CreateSide(3, new Vector3(-offset, height, 0f), 270f);
        }

        private void Start()
        {
            var nview = GetComponent<ZNetView>();
            if (!nview || !nview.IsValid())
            {
                return;
            }

            var zdo = nview.GetZDO();
            var creaturePrefabName = zdo.GetString(Plugin.ZDOVars.z_creaturePrefab, "");
            var creaturePrefabData = zdo.GetString(Plugin.ZDOVars.z_creatureData, "");
            var creatureName = zdo.GetString(Plugin.ZDOVars.z_creatureName, "");
            if (!string.IsNullOrEmpty(creaturePrefabName) && !string.IsNullOrEmpty(creaturePrefabData))
            {
                GameObject creaturePrefab = ZNetScene.instance ? ZNetScene.instance.GetPrefab(creaturePrefabName) : null;
                if (creaturePrefab)
                {
                    if (!string.IsNullOrEmpty(creatureName))
                    {
                        SetSprite(CreatureSnapshot.Get(creaturePrefab));
                    }
                    else
                    {
                        SetSprite(CrateResources.CreatureUnknown);
                    }
                }
                else
                {
                    SetSprite(CrateResources.CreatureUnavailable);
                }
            }
            else
            {
                SetSprite(CrateResources.CreatureUnavailable);
            }
        }

        internal void SetSprite(Sprite sprite)
        {
            foreach (var renderer in m_renderers)
            {
                renderer.sprite = sprite;
            }
        }

        private void CreateSide(int index, Vector3 position, float rotation)
        {
            GameObject visual = new GameObject($"PetCarrier_Crate_Sprite_{index}");
            visual.transform.SetParent(transform, false);

            visual.transform.localPosition = position;
            visual.transform.localRotation = Quaternion.Euler(0f, rotation, 0f);
            visual.transform.localScale = Vector3.one * Plugin.Constants.CrateSpriteSize;

            m_renderers[index] = visual.AddComponent<SpriteRenderer>();
        }

        internal void SetFromItem(ItemDrop.ItemData item)
        {
            if (item == null)
            {
                SetSprite(CrateResources.CreatureUnavailable);
                return;
            }

            item.m_customData.TryGetValue(Plugin.ZDOVars.s_creaturePrefab, out string creaturePrefabName);
            item.m_customData.TryGetValue(Plugin.ZDOVars.s_creatureData, out string creatureData);
            item.m_customData.TryGetValue(Plugin.ZDOVars.s_creatureName, out string creatureName);

            if (string.IsNullOrEmpty(creaturePrefabName) || string.IsNullOrEmpty(creatureData))
            {
                SetSprite(CrateResources.CreatureUnavailable);
                return;
            }

            GameObject creaturePrefab = ZNetScene.instance ? ZNetScene.instance.GetPrefab(creaturePrefabName) : null;

            if (!creaturePrefab)
            {
                SetSprite(CrateResources.CreatureUnavailable);
            }
            else if (string.IsNullOrEmpty(creatureName))
            {
                SetSprite(CrateResources.CreatureUnknown);
            }
            else
            {
                SetSprite(CreatureSnapshot.Get(creaturePrefab));
            }
        }

    }
}
