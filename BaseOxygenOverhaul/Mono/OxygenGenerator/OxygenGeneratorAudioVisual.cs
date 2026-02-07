using UnityEngine;

namespace BaseOxygenOverhaul.Mono.OxygenGenerator
{
    /// <summary>
    /// Handles the audio and visual effects for oxygen generators.
    /// </summary>
    public abstract class OxygenGeneratorAudioVisual : MonoBehaviour
    {
        protected virtual void Start()
        {
            Plugin.Log.LogInfo($"Starting {GetType().Name} on {gameObject.name}");
        }
    }
}