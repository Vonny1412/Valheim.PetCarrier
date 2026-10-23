using HarmonyLib;
using PetCarrier.Components;
using UnityEngine;

namespace PetCarrier.Patches
{
    [HarmonyPatch(typeof(Player), "PlayerAttackInput")]
    internal static class Player_PlayerAttackInput_Patch
    {

        private static bool suppressAttackUntilRelease;

        [HarmonyPrefix]
        private static bool Prefix(
            Player __instance,
            ref bool ___m_attack,
            ref bool ___m_attackHold,
            ref float ___m_queuedAttackTimer)
        {
            if (__instance != Player.m_localPlayer)
            {
                return true;
            }

            if (CratePlacement.IsPlacing())
            {
                if (!InventoryGui.IsVisible() && Input.GetMouseButtonDown(0))
                {
                    suppressAttackUntilRelease = true;
                    CratePlacement.TryPlace();
                }

                ___m_attack = false;
                ___m_attackHold = false;
                ___m_queuedAttackTimer = 0f;
                return false;
            }

            if (suppressAttackUntilRelease)
            {
                ___m_attack = false;
                ___m_attackHold = false;
                ___m_queuedAttackTimer = 0f;
                if (!Input.GetMouseButton(0))
                {
                    suppressAttackUntilRelease = false;
                }
                return false;
            }

            return true;
        }

    }

}
