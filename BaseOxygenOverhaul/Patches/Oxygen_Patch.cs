using HarmonyLib;
using BaseOxygenOverhaul.Handlers;

namespace BaseOxygenOverhaul.Patches
{
    [HarmonyPatch(typeof(Oxygen))]
    internal static class Oxygen_Patch
    {
        /// <summary>
        /// Blocks oxygen addition when the player is inside a base, unless in creative/free mode or No Oxygen cheat is active.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Oxygen.AddOxygen))]
        private static bool AddOxygen_Prefix(Oxygen __instance, float amount, ref float __result)
        {
            if (!BaseOxygen.OnPlayerOxygenAdd(ref __instance))
            {
                __result = 0f;
                return false; // Block oxygen addition
            }

            return true; // Allow normal oxygen addition
        }
    }
}
