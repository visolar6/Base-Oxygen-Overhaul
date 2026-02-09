using System.IO;
using System.Reflection;
using BaseOxygenOverhaul.Prefabs;
using Nautilus.FMod;
using Nautilus.Handlers;
using Nautilus.Utility;
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

        public static class FMODSoundIds
        {
            public static string SmallOxygenGeneratorAmbient = "SmallOxygenGeneratorAmbient";
            public static string LargeOxygenGeneratorAmbient = "LargeOxygenGeneratorAmbient";
        }

        public static class StoryGoals
        {
            public static StoryGoal EnterBaseOxygenOverhaulStoryGoal = new StoryGoal("EnterBaseOxygenOverhaul", Story.GoalType.Encyclopedia, 1f);
        }

        public static void Patch()
        {
            foreach (var key in System.Enum.GetNames(typeof(EncyclopediaKeys)))
                AddEncylopediaEntry(key, GetEncyclopediaKeyPath((EncyclopediaKeys)System.Enum.Parse(typeof(EncyclopediaKeys), key)));

            RegisterFMODSound();

            StoryGoalHandler.RegisterCustomEvent(
                key: StoryGoals.EnterBaseOxygenOverhaulStoryGoal.key,
                customEventCallback: () =>
                {
                    PDAEncyclopedia.AddAndPlaySound(GetEncyclopediaKey(EncyclopediaKeys.OxygenGeneration));
                }
            );
        }

        public static string GetEncyclopediaKey(EncyclopediaKeys key) => System.Enum.GetName(typeof(EncyclopediaKeys), key);
        public static string GetEncyclopediaKeyPath(EncyclopediaKeys key)
        {
            switch (key)
            {
                case EncyclopediaKeys.OxygenGeneration:
                case EncyclopediaKeys.SmallOxygenGenerator:
                case EncyclopediaKeys.LargeOxygenGenerator:
                    return "Tech/Habitats";
                default:
                    return "Tech";
            }
        }

        private static void AddEncylopediaEntry(string baseKey, string path, FMODAsset unlockSound = null, FMODAsset voiceLog = null)
        {
            var titleKey = $"Ency_{baseKey}";
            var descKey = $"EncyDesc_{baseKey}";
            var imageKey = $"{baseKey}/{baseKey}Image";
            var popupImageKey = $"{baseKey}/{baseKey}Popup";
            Plugin.Log.LogInfo($"Registering encyclopedia entry with key {baseKey}, title {titleKey}, desc {descKey}, image {imageKey}, popup {popupImageKey}");

            PDAHandler.AddEncyclopediaEntry(
                key: baseKey,
                path: path,
                title: titleKey,
                desc: descKey,
                image: Plugin.AssetBundle.LoadAsset<Texture2D>(imageKey),
                popupImage: Plugin.AssetBundle.LoadAsset<Sprite>(popupImageKey),
                unlockSound: unlockSound ?? KnownTechHandler.DefaultUnlockData.BasicUnlockSound,
                voiceLog: voiceLog
            );
        }

        private static void RegisterFMODSound()
        {
            var soundSource = new ModFolderSoundSource(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"));
            var soundBuilder = new FModSoundBuilder(soundSource);

            soundBuilder.CreateNewEvent(FMODSoundIds.SmallOxygenGeneratorAmbient, AudioUtils.BusPaths.SFX)
                .SetMode3D(0.5f, 1f, true)
                .SetSound(FMODSoundIds.SmallOxygenGeneratorAmbient)
                .Register();

            soundBuilder.CreateNewEvent(FMODSoundIds.LargeOxygenGeneratorAmbient, AudioUtils.BusPaths.SFX)
                .SetMode3D(0.75f, 1.5f, true)
                .SetSound(FMODSoundIds.LargeOxygenGeneratorAmbient)
                .Register();
        }
    }
}