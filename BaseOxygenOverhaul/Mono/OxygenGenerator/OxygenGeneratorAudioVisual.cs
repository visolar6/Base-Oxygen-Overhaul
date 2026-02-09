using UnityEngine;

namespace BaseOxygenOverhaul.Mono.OxygenGenerator
{
    /// <summary>
    /// Handles the audio and visual effects for oxygen generators.
    /// </summary>
    public abstract class OxygenGeneratorAudioVisual : MonoBehaviour
    {
        private const float UpdateInterval = 0.5f;
        private float updateTimer = 0f;

        protected virtual void Start() { }

        protected virtual void Update() { }
    }
}