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
        public static class EncyclopediaKeys
        {
            public const string OxygenGeneration = "OxygenGeneration";
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
            PDAHandler.AddEncyclopediaEntry(
                key: EncyclopediaKeys.OxygenGeneration,
                path: "Tech/Habitats",
                title: null,
                desc: null,
                image: null,
                popupImage: null,
                unlockSound: null,
                voiceLog: null
            );

            var audioPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", "Audio");
            var soundSource = new ModFolderSoundSource(audioPath);
            var soundBuilder = new FModSoundBuilder(soundSource);
            soundBuilder.CreateNewEvent(FMODSoundIds.SmallOxygenGeneratorAmbient, AudioUtils.BusPaths.SFX)
                .SetMode3D(1f, 4f, false)
                .SetFadeDuration(0.5f)
                .SetSound(FMODSoundIds.SmallOxygenGeneratorAmbient)
                .Register();
            soundBuilder.CreateNewEvent(FMODSoundIds.LargeOxygenGeneratorAmbient, AudioUtils.BusPaths.SFX)
                .SetMode3D(2f, 6f, false)
                .SetFadeDuration(1f)
                .SetSound(FMODSoundIds.LargeOxygenGeneratorAmbient)
                .Register();

            StoryGoalHandler.RegisterCustomEvent(
                key: StoryGoals.EnterBaseOxygenOverhaulStoryGoal.key,
                customEventCallback: () =>
                {
                    PDAEncyclopedia.AddAndPlaySound(EncyclopediaKeys.OxygenGeneration);
                }
            );
        }
    }
}