
namespace PetCarrier
{
    public sealed partial class Plugin
    {
        public static class Constants
        {

            internal const string EmptyPrefabName = "PetCarrier_Crate_Empty";
            internal const string FilledPrefabName = "PetCarrier_Crate_Filled";
            internal const string VisualPrefabName = "CargoCrate";
            internal const float VisualSizeFactor = 1.0f;

            internal const float CrateSpriteSize = 0.66f * VisualSizeFactor;
            internal const float CrateSpriteOffset = 0.41f * VisualSizeFactor;
            internal const float CrateSpriteHeight = 0.35f * VisualSizeFactor;

            internal const float EquipPositionX = -0.40f; // - left,  + right
            internal const float EquipPositionY = -0.60f; // - down,  + up
            internal const float EquipPositionZ = +0.20f; // - back,  + forward



            internal const string TranslationKey_CrateEmpty = "$petcarrier_crate_empty";
            internal const string TranslationKey_CrateEmptyDescription = "$petcarrier_crate_empty_description";
            internal const string TranslationKey_CrateFilled = "$petcarrier_crate_filled";
            internal const string TranslationKey_CrateFilledDescription = "$petcarrier_crate_filled_description";
            internal const string TranslationKey_MessageNotTamed = "$petcarrier_message_is_not_tamed";
            internal const string TranslationKey_MessageIsAlerted = "$petcarrier_message_is_alerted";
            internal const string TranslationKey_MessageIsAggravated = "$petcarrier_message_is_aggravated";
            internal const string TranslationKey_MessageIsPregnant = "$petcarrier_message_is_pregnant";
            internal const string TranslationKey_MessageIsLocked = "$petcarrier_message_is_locked";
            internal const string TranslationKey_TraderTooltip = "$petcarrier_trader_tooltip";
            internal const string TranslationKey_LockedToWorld = "$petcarrier_locked_to_world";
            
        }
    }
}