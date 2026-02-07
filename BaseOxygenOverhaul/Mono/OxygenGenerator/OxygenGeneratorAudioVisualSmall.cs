using UnityEngine;

namespace BaseOxygenOverhaul.Mono.OxygenGenerator
{
    /// <summary>
    /// Handles the audio and visual effects for small oxygen generators.
    /// </summary>
    public class OxygenGeneratorAudioVisualSmall : OxygenGeneratorAudioVisual
    {
        protected override void Start()
        {
            base.Start();

            Plugin.Log.LogInfo($"Initializing audio-visual effects for small oxygen generator on {gameObject.name}");
        }
    }
}