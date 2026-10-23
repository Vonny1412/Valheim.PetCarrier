using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;

namespace PetCarrier
{
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Patch)]

    public sealed partial class Plugin : BaseUnityPlugin
    {
        private Harmony harmony;
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;

            Configs.Initialize(Config);

            harmony = new Harmony(Plugin.ModGuid);
            harmony.PatchAll(typeof(Plugin).Assembly);

            PrefabManager.OnVanillaPrefabsAvailable += OnVanillaPrefabsAvailable;
        }

        private void OnVanillaPrefabsAvailable()
        {
            PrefabManager.OnVanillaPrefabsAvailable -= OnVanillaPrefabsAvailable;
            CratePrefab.Register(Log);
        }

        private void OnDestroy()
        {
            PrefabManager.OnVanillaPrefabsAvailable -= OnVanillaPrefabsAvailable;
            harmony?.UnpatchSelf();
            CrateResources.Clear();
        }

    }
}