using HarmonyLib;
using PetCarrier.Components;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Player), "UpdateCrouch")]
    internal static class Player_UpdateCrouch_Patch
    {
        [HarmonyPrefix]
        private static void Prefix(Player __instance, ref bool ___m_crouchToggled)
        {
            if (__instance != Player.m_localPlayer)
            {
                return;
            }

            if (CratePlacement.IsPlacing())
            {
                // disable crouching while holding the crate
                ___m_crouchToggled = false;
            }
        }
    }
}