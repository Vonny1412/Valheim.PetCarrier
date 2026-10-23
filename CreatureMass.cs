using System.Collections.Generic;
using UnityEngine;

namespace PetCarrier
{
    internal static class CreatureMass
    {
        private static readonly Dictionary<string, float> Cache = new Dictionary<string, float>();

        internal static float Get(string prefabName)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                return 0f;
            }

            if (Cache.TryGetValue(prefabName, out float mass))
            {
                return mass;
            }

            GameObject prefab = ZNetScene.instance?.GetPrefab(prefabName);
            if (!prefab)
            {
                return 0f;
            }

            Rigidbody body = prefab.GetComponent<Rigidbody>();
            if (!body)
            {
                return 0f;
            }

            Vector3 scale = prefab.transform.lossyScale;
            float factor = Mathf.Pow(Mathf.Abs(scale.x * scale.y * scale.z), 1f / 3f);
            mass = body.mass * factor;

            Cache[prefabName] = mass;

            return mass;
        }

        internal static void Clear()
        {
            Cache.Clear();
        }
    }
}