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
            // Only trigger when entering a base (not vehicle), and only if sub is not null
            if (sub != null && sub.isBase) Global.StoryGoals.EnterBaseOxygenOverhaulStoryGoal.Trigger();
        }
    }
}
