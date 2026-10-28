using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace PetCarrier
{
    internal static class CrateZDO
    {

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        private static readonly HashSet<int> ExcludedHashes = new HashSet<int>
        {
            ZDOVars.s_bodyVelocity,
            ZDOVars.s_alert,
            ZDOVars.s_haveTargetHash,
            ZDOVars.s_patrol,
            ZDOVars.s_patrolPoint,
            ZDOVars.s_spawnPoint,
            ZDOVars.s_follow,
            ZDOVars.s_velHash,
            ZDOVars.s_lookTarget,
            ZDOVars.s_tiltrot,
            ZDOVars.s_noise
        };

        private static string ToDataString<T>(T value)
        {
            switch (value)
            {
                case float f:
                    return f.ToString("R", Culture);

                case Vector3 v:
                    return string.Join(",",
                        v.x.ToString("R", Culture),
                        v.y.ToString("R", Culture),
                        v.z.ToString("R", Culture));

                case Quaternion q:
                    return string.Join(",",
                        q.x.ToString("R", Culture),
                        q.y.ToString("R", Culture),
                        q.z.ToString("R", Culture),
                        q.w.ToString("R", Culture));

                case byte[] bytes:
                    return Convert.ToBase64String(bytes);

                default:
                    return Convert.ToString(value, Culture) ?? "";
            }
        }

        private static void FilterZDOData<T>(Dictionary<string, string> data, List<KeyValuePair<int, T>> entries)
        {
            string type = typeof(T).Name;

            foreach (var entry in entries)
            {
                if (ExcludedHashes.Contains(entry.Key))
                {
                    continue;
                }

                //string keyName = ZDOKeyNames.Names.TryGetValue(entry.Key, out string name) ? name : "Unknown";
                string value = ToDataString(entry.Value);
                //Plugin.Log.LogMessage($"ZDO [{type}] {keyName} ({entry.Key}) = {value}");

                data[$"{type}:{entry.Key}"] = value;
            }
        }

        internal static Dictionary<string, string> GetData(ZDO zdo)
        {
            ZDOExtraData.GetData(
                zdo.m_uid,
                out var floats,
                out var vec3s,
                out var quats,
                out var ints,
                out var longs,
                out var strings,
                out var byteArrays,
                out var connectionData
            );

            var data = new Dictionary<string, string>();

            FilterZDOData(data, floats);
            FilterZDOData(data, vec3s);
            FilterZDOData(data, quats);
            FilterZDOData(data, ints);
            FilterZDOData(data, longs);
            FilterZDOData(data, strings);
            FilterZDOData(data, byteArrays);

            // Connections deliberately excluded.
            // They may reference other world objects.

            return data;
        }

        internal static bool ValidateData(Dictionary<string, string> data)
        {
            try
            {
                foreach (var entry in data)
                {
                    string[] parts = entry.Key.Split(':');
                    if (parts.Length != 2)
                    {
                        continue;
                    }

                    int.Parse(parts[1], Culture);
                    string value = entry.Value;

                    switch (parts[0])
                    {
                        case "Single":
                            float.Parse(value, Culture);
                            break;

                        case "Int32":
                            int.Parse(value, Culture);
                            break;

                        case "Int64":
                            long.Parse(value, Culture);
                            break;

                        case "Vector3":
                            if (value.Split(',').Length != 3)
                                return false;

                            foreach (string component in value.Split(','))
                                float.Parse(component, Culture);
                            break;

                        case "Quaternion":
                            if (value.Split(',').Length != 4)
                                return false;

                            foreach (string component in value.Split(','))
                                float.Parse(component, Culture);
                            break;

                        case "Byte[]":
                            Convert.FromBase64String(value);
                            break;

                        case "String":
                            break;

                        default:
                            Plugin.Log.LogWarning($"Unknown ZDO data type: {entry.Key} = {entry.Value}");
                            return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Plugin.Log.LogWarning($"Invalid ZDO data: {ex}");
                return false;
            }
        }

        internal static string Stringify(Dictionary<string, string> data)
        {
            ZPackage pkg = new ZPackage();
            pkg.Write(data.Count);

            foreach (var entry in data)
            {
                pkg.Write(entry.Key);
                pkg.Write(entry.Value);
            }

            return pkg.GetBase64();
        }

        internal static Dictionary<string, string> ParseData(string data)
        {
            ZPackage pkg = new ZPackage(data);
            int count = pkg.ReadInt();

            var result = new Dictionary<string, string>(count);

            for (int i = 0; i < count; i++)
            {
                string key = pkg.ReadString();
                string value = pkg.ReadString();

                result.Add(key, value);
            }

            return result;
        }

        internal static void RestoreData(ZDO zdo, Dictionary<string, string> data)
        {
            foreach (var entry in data)
            {
                string[] parts = entry.Key.Split(':');
                if (parts.Length != 2)
                {
                    continue;
                }

                if (!int.TryParse(parts[1], NumberStyles.Integer, Culture, out int hash))
                {
                    continue;
                }

                string value = entry.Value;

                switch (parts[0])
                {
                    case "Single":
                        zdo.Set(hash, float.Parse(value, Culture));
                        break;

                    case "Vector3":
                        {
                            string[] v = value.Split(',');
                            zdo.Set(hash, new Vector3(
                                float.Parse(v[0], Culture),
                                float.Parse(v[1], Culture),
                                float.Parse(v[2], Culture)
                            ));
                            break;
                        }

                    case "Quaternion":
                        {
                            string[] q = value.Split(',');
                            zdo.Set(hash, new Quaternion(
                                float.Parse(q[0], Culture),
                                float.Parse(q[1], Culture),
                                float.Parse(q[2], Culture),
                                float.Parse(q[3], Culture)
                            ));
                            break;
                        }

                    case "Int32":
                        zdo.Set(hash, int.Parse(value, Culture));
                        break;

                    case "Int64":
                        zdo.Set(hash, long.Parse(value, Culture));
                        break;

                    case "String":
                        zdo.Set(hash, value);
                        break;

                    case "Byte[]":
                        zdo.Set(hash, Convert.FromBase64String(value));
                        break;
                }
            }
        }

    }
}
