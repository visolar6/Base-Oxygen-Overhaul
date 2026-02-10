using BaseOxygenOverhaul.Utilities;
using HarmonyLib;

namespace BaseOxygenOverhaul.Patches
{
    [HarmonyPatch(typeof(Player))]
    public static class Player_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetCurrentSub")]
        public static void Postfix(Player __instance, SubRoot sub, bool forced = false)
        {
            if (sub != null && sub.isBase)
            {
                // Only trigger when entering a base (not vehicle), and only if sub is not null
                Global.StoryGoals.EnterBaseOxygenOverhaulStoryGoal.Trigger();

                // Set the BaseOxygenHandler's current base reference to the new base
                BaseOxygenHandler._base = sub.GetComponent<Base>();
                // Reset timers so checks trigger immediately on first frame
                BaseOxygenHandler.ResetAllTimers();
            }
            else if (sub == null)
            {
                // Player left their base, reset BaseOxygenHandler's timers and base reference
                BaseOxygenHandler._base = null;
                BaseOxygenHandler.ResetAllTimers();
            }
        }
    }
}
