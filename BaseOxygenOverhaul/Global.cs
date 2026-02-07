using Story;

namespace BaseOxygenOverhaul
{
    public static class Global
    {
        public static class Keys
        {
            public const string OxygenGeneration = "OxygenGeneration";
            public const string EncyOxygenGeneration = "Ency_OxygenGeneration";
            public const string EncyDescOxygenGeneration = "EncyDesc_OxygenGeneration";
            public const string EncyLargeOxygenGeneratorFragment = "Ency_LargeOxygenGeneratorFragment";
        }

        public static class StoryGoals
        {
            public static StoryGoal EnterBaseOxygenOverhaulStoryGoal = new StoryGoal("EnterBaseOxygenOverhaul", Story.GoalType.Encyclopedia, 1f);
        }
    }
}