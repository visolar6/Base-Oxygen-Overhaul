using UnityEngine;

namespace BaseOxygenOverhaul.Mono.OxygenGenerator
{
    /// <summary>
    /// Handles the audio and visual effects for large oxygen generators.
    /// </summary>
    public class OxygenGeneratorAudioVisualLarge : OxygenGeneratorAudioVisual
    {
        protected override void Start()
        {
            base.Start();

            Plugin.Log.LogInfo($"Initializing audio-visual effects for large oxygen generator on {gameObject.name}");
        }
    }
}