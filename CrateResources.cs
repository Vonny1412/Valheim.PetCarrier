using System.IO;
using System.Reflection;
using UnityEngine;

namespace PetCarrier
{
    internal static class CrateResources
    {
        private static Sprite m_creatureUnavailable;
        private static Sprite m_creatureUnknown;


        internal static Sprite CreatureUnavailable
        {
            get
            {
                if (m_creatureUnavailable)
                {
                    return m_creatureUnavailable;
                }
                m_creatureUnavailable = CreateSprite($"{nameof(PetCarrier)}.Resources.CreatureUnavailable.png");
                return m_creatureUnavailable;
            }
        }

        internal static Sprite CreatureUnknown
        {
            get
            {
                if (m_creatureUnknown)
                {
                    return m_creatureUnknown;
                }
                m_creatureUnknown = CreateSprite($"{nameof(PetCarrier)}.Resources.CreatureUnknown.png");
                return m_creatureUnknown;
            }
        }

        // Explicitly resolve the byte[] overload to avoid the ReadOnlySpan<byte>
        // overload resolution issue when targeting .NET Framework 4.8.
        private static readonly MethodInfo s_loadImageMethod = typeof(ImageConversion).GetMethod(nameof(ImageConversion.LoadImage), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(Texture2D), typeof(byte[]), typeof(bool) }, null);

        private static Sprite CreateSprite(string png)
        {
            Assembly assembly = typeof(Plugin).Assembly;
            using Stream stream = assembly.GetManifestResourceStream(png);
            if (stream == null)
            {
                Plugin.Log.LogWarning($"{png} resource not found.");
                return null;
            }

            byte[] bytes = new byte[stream.Length];
            using (MemoryStream memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                bytes = memory.ToArray();
            }

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (s_loadImageMethod == null || !(bool)s_loadImageMethod.Invoke(null, new object[] { texture, bytes, false }))
            {
                Object.Destroy(texture);
                return null;
            }

            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            return sprite;
        }

        private static void DestroySprite(Sprite sprite)
        {
            if (!sprite)
            {
                return;
            }
            Texture2D texture = sprite.texture;
            Object.Destroy(sprite);
            if (texture)
            {
                Object.Destroy(texture);
            }
        }

        internal static void Clear()
        {
            DestroySprite(m_creatureUnavailable);
            m_creatureUnavailable = null;
            DestroySprite(m_creatureUnknown);
            m_creatureUnknown = null;
        }

    }
}
