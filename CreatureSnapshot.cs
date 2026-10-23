using Jotunn.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace PetCarrier
{
    internal static class CreatureSnapshot
    {
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        internal static Sprite Get(GameObject creature)
        {
            if (!creature)
            {
                return CrateResources.CreatureUnavailable;
            }

            string prefabName = Utils.GetPrefabName(creature);
            if (Cache.TryGetValue(prefabName, out Sprite snapshot) && snapshot)
            {
                return snapshot;
            }

            snapshot = RenderManager.Instance.Render(
                new RenderManager.RenderRequest(creature)
                {
                    Rotation = RenderManager.IsometricRotation,
                    UseCache = false,
                    Width = 128,
                    Height = 128
                }
            );

            if (snapshot)
            {
                Cache[prefabName] = snapshot;
            }

            return snapshot;
        }

        internal static void Clear()
        {
            foreach (Sprite sprite in Cache.Values)
            {
                if (!sprite)
                {
                    continue;
                }
                Texture2D texture = sprite.texture;
                Object.Destroy(sprite);
                if (texture)
                {
                    Object.Destroy(texture);
                }
            }
            Cache.Clear();
        }

    }
}
