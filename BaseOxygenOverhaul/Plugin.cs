using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using BaseOxygenOverhaul.Utilities;
using BaseOxygenOverhaul.Prefabs;
using UWE;

namespace BaseOxygenOverhaul
{
    [BepInPlugin(GUID, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Options Options { get; } = OptionsPanelHandler.RegisterModOptions<Options>();

        public static ManualLogSource Log;

        internal const string GUID = "com.visolar6.baseoxygenoverhaul";

        internal const string Name = "Base Oxygen Overhaul";

        internal const string Version = "1.0.0";

        private readonly Harmony _harmony = new Harmony(GUID);

        /// <summary>
        /// Awakes the plugin (on game start).
        /// </summary>
        public void Awake()
        {
            Log = Logger;
        }

        public void Start()
        {
            Log?.LogInfo("Patching databank");
            PDAHandler.AddEncyclopediaEntry(
                key: Global.Keys.OxygenGeneration,
                path: "Tech/Habitats",
                title: Global.Keys.EncyOxygenGeneration,
                desc: Global.Keys.EncyDescOxygenGeneration,
                image: null,
                popupImage: null,
                unlockSound: null,
                voiceLog: null
            );

            Log?.LogInfo("Patching story goals");
            StoryGoalHandler.RegisterCustomEvent(
                key: Global.StoryGoals.EnterBaseOxygenOverhaulStoryGoal.key,
                customEventCallback: () =>
                {
                    PDAEncyclopedia.AddAndPlaySound(Global.Keys.OxygenGeneration);
                }
            );

            Log?.LogInfo("Patching hooks");
            _harmony.PatchAll();

            Log?.LogInfo("Patching localization");
            LanguagesHandler.GlobalPatch();

            Log?.LogInfo("Patching items");
            SmallOxygenGenerator.Register();
            /* Not ready yet - need to design and implement large oxygen generator first
            LargeOxygenGenerator.Register();
            LargeOxygenGeneratorFragment.Register();
            */
        }
    }
}
