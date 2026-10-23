using BepInEx.Configuration;
using Jotunn.Extensions;
using System.Collections.Generic;

namespace PetCarrier
{
    public sealed partial class Plugin
    {
        public class Configs
        {

            private const string Section_Common = "Common";

            public static ConfigEntry<float> CrateBaseWeight { get; private set; }
            public static ConfigEntry<float> CrateWeightMassFactor { get; private set; }
            public static ConfigEntry<int> CrateMaxStackSize { get; private set; }
            public static ConfigEntry<bool> CrateTeleportable { get; private set; }
            public static ConfigEntry<int> CratePrice { get; private set; }
            public static ConfigEntry<bool> LockCreaturesToWorld { get; private set; }


            

            private static readonly List<ConfigurationManagerAttributes> WorldLocks = new List<ConfigurationManagerAttributes>();

            public static void SetWorldLock(bool locked)
            {
                foreach (var attributes in WorldLocks)
                {
                    attributes.ReadOnly = locked;
                }
            }

            private static ConfigEntry<T> BindConfig<T>(ConfigFile config, string section, string key, T value, string description, AcceptableValueBase acceptableValues = null)
            {
                var attributes = new ConfigurationManagerAttributes
                {
                    ReadOnly = false
                };

                WorldLocks.Add(attributes);

                return config.BindConfigInOrder<T>(
                    section,
                    key,
                    value,
                    description,
                    acceptableValues: acceptableValues,
                    configAttributes: attributes,
                    synced: true
                );
            }

            public static void Initialize(ConfigFile config)
            {
                string section = Section_Common;

                CrateBaseWeight = BindConfig<float>(
                    config, section, "BaseWeight", 10f,
                    "Base weight of a pet crate, excluding the weight of its occupant.",
                    new AcceptableValueRange<float>(0f, 100f)
                );

                CrateWeightMassFactor = BindConfig<float>(
                    config, section, "WeightMassFactor", 1f,
                    "Multiplier applied to a creature's mass when calculating the weight of a filled crate.",
                    new AcceptableValueRange<float>(0f, 100f)
                );

                CrateMaxStackSize = BindConfig<int>(
                    config, section, "MaxStackSize", 10,
                    "Max stack size of empty crates."
                );

                CrateTeleportable = BindConfig<bool>(
                    config, section, "Teleportable", true,
                    "Whether filled pet crates can be transported through portals."
                );

                CratePrice = BindConfig<int>(
                    config, section, "Price", 100,
                    "Purchase price of an empty pet crate at Haldor.",
                    new AcceptableValueRange<int>(0, 1000000)
                );

                LockCreaturesToWorld = BindConfig<bool>(
                    config, section, "LockCreaturesToWorld", false,
                    "Prevents cross-world creature transfers. Creatures packed in this world cannot be released in other worlds, and creatures packed in other worlds cannot be released here."
                );


                

            }

        }
    }
}
