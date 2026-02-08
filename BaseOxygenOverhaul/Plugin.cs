using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.Reflection;
using UnityEngine;
using BaseOxygenOverhaul.Utilities;
using BaseOxygenOverhaul.Prefabs;

namespace BaseOxygenOverhaul
{
    [BepInPlugin(GUID, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Options Options { get; } = OptionsPanelHandler.RegisterModOptions<Options>();

        public static ManualLogSource Log;

        public static AssetBundle AssetBundle { get; } = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly.GetExecutingAssembly(), "baseoxygenoverhaul");

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
            Log?.LogInfo("Patching localization");
            LanguagesHandler.Patch();

            Log?.LogInfo("Patching hooks");
            _harmony.PatchAll();

            Log?.LogInfo("Patching global");
            Global.Patch();

            Log?.LogInfo("Patching items");
            SmallOxygenGenerator.Register();
            LargeOxygenGenerator.Register();
            LargeOxygenGeneratorFragment.Register();
        }
    }
}
