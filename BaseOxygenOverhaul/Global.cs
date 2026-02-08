using Nautilus.Handlers;
using Story;
using UnityEngine;

namespace BaseOxygenOverhaul
{
    public static class Global
    {
        public enum EncyclopediaKeys
        {
            OxygenGeneration,
            SmallOxygenGenerator,
            LargeOxygenGenerator,
        }

        public enum AssetKeys
        {
            SmallOxygenGeneratorPrefab,
            SmallOxygenGeneratorIcon,
            SmallOxygenGeneratorImage,
            SmallOxygenGeneratorPopup,

            LargeOxygenGeneratorPrefab,
            LargeOxygenGeneratorIcon,
            LargeOxygenGeneratorImage,
            LargeOxygenGeneratorPopup,
        }

        public static class StoryGoals
        {
            public static StoryGoal EnterBaseOxygenOverhaulStoryGoal = new StoryGoal("EnterBaseOxygenOverhaul", Story.GoalType.Encyclopedia, 1f);
        }

        public static void Patch()
        {
            foreach (var key in System.Enum.GetNames(typeof(EncyclopediaKeys)))
                AddEncylopediaEntry(key);

            StoryGoalHandler.RegisterCustomEvent(
                key: StoryGoals.EnterBaseOxygenOverhaulStoryGoal.key,
                customEventCallback: () =>
                {
                    PDAEncyclopedia.AddAndPlaySound(GetEncyclopediaKey(EncyclopediaKeys.OxygenGeneration));
                }
            );
        }

        public static string GetEncyclopediaKey(EncyclopediaKeys key) => System.Enum.GetName(typeof(EncyclopediaKeys), key);

        private static void AddEncylopediaEntry(string baseKey, FMODAsset unlockSound = null, FMODAsset voiceLog = null)
        {
            var titleKey = $"Ency_{baseKey}";
            var descKey = $"EncyDesc_{baseKey}";
            var imageKey = $"{baseKey}/{baseKey}Image";
            var popupImageKey = $"{baseKey}/{baseKey}Popup";

            PDAHandler.AddEncyclopediaEntry(
                key: baseKey,
                path: "Tech/Habitats",
                title: titleKey,
                desc: descKey,
                image: Plugin.AssetBundle.LoadAsset<Texture2D>(imageKey),
                popupImage: Plugin.AssetBundle.LoadAsset<Sprite>(popupImageKey),
                unlockSound: unlockSound ?? KnownTechHandler.DefaultUnlockData.BasicUnlockSound,
                voiceLog: voiceLog
            );
        }
    }
}