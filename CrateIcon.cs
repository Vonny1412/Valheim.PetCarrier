using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PetCarrier
{
    internal static class CrateIcon
    {
        private const int Resolution = 128;

        private const float CreatureScale = 0.55f;
        private const float CreatureOffsetX = -10f;
        private const float CreatureOffsetY = 9f;

        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        internal static Sprite Bake(ItemDrop.ItemData item, string prefabName, Sprite foreground = null)
        {
            prefabName ??= "";
            if (Cache.TryGetValue(prefabName, out Sprite cached) && cached)
            {
                return cached;
            }

            if (!foreground)
            {
                Plugin.Log.LogWarning("Crate foreground icon not found.");
                return null;
            }

            Sprite background = item.m_shared.m_icons[0];
            if (!background)
            {
                Plugin.Log.LogWarning("Crate background icon not found.");
                return null;
            }

            RenderTexture rt = RenderTexture.GetTemporary(Resolution, Resolution, 0, RenderTextureFormat.ARGB32);
            RenderTexture previous = RenderTexture.active;

            try
            {
                RenderTexture.active = rt;
                GL.Clear(true, true, Color.clear);
                GL.PushMatrix();
                try
                {
                    GL.LoadPixelMatrix(0, Resolution, Resolution, 0);

                    DrawSprite(background, new Rect(0, 0, Resolution, Resolution));

                    float size = Resolution * CreatureScale;
                    float x = (Resolution - size) / 2f + CreatureOffsetX;
                    float y = (Resolution - size) / 2f + CreatureOffsetY;

                    DrawSprite(foreground, new Rect(x, y, size, size));
                }
                finally
                {
                    GL.PopMatrix();
                }

                Texture2D texture = new Texture2D(Resolution, Resolution, TextureFormat.RGBA32, false);
                texture.ReadPixels(new Rect(0, 0, Resolution, Resolution), 0, 0);
                texture.Apply();

                Sprite bakedSprite = Sprite.Create(texture, new Rect(0, 0, Resolution, Resolution), new Vector2(0.5f, 0.5f), 100f);
                Cache[prefabName] = bakedSprite;
                return bakedSprite;
            }
            finally
            {
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(rt);
            }
        }

        private static void DrawSprite(Sprite sprite, Rect destination)
        {
            Rect source = sprite.textureRect;
            Texture texture = sprite.texture;

            Rect uv = new Rect(
                source.x / texture.width,
                source.y / texture.height,
                source.width / texture.width,
                source.height / texture.height
            );

            Graphics.DrawTexture(destination, texture, uv, 0, 0, 0, 0);
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

        private static void ExportTexture(Texture2D texture, string prefabName)
        {
            string directory = Path.Combine(BepInEx.Paths.CachePath, Plugin.ModGuid);
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, $"{prefabName}.png");
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Plugin.Log.LogMessage($"Exported crate icon: {path}");
        }
        
    }
}
