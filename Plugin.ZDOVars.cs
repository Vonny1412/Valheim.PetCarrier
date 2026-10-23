namespace PetCarrier
{
    public sealed partial class Plugin
    {
        public static class ZDOVars
        {

            internal static readonly string s_creaturePrefab = $"{Plugin.ModGuid}.z_creaturePrefab";
            internal static readonly string s_creatureData = $"{Plugin.ModGuid}.z_creatureData";
            internal static readonly string s_creatureName = $"{Plugin.ModGuid}.z_creatureName";
            internal static readonly string s_creatureSource = $"{Plugin.ModGuid}.z_creatureSource";

            internal static readonly int z_creaturePrefab = s_creaturePrefab.GetStableHashCode();
            internal static readonly int z_creatureData = s_creatureData.GetStableHashCode();
            internal static readonly int z_creatureName = s_creatureName.GetStableHashCode();
            internal static readonly int z_creatureSource = s_creatureSource.GetStableHashCode();
            internal static readonly int z_equippedCreaturePrefab = $"{Plugin.ModGuid}.z_equippedCreaturePrefab".GetStableHashCode();

        }
    }
}
