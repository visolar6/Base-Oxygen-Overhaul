using UnityEngine;
using BaseOxygenOverhaul.Types;

namespace BaseOxygenOverhaul.Mono.OxygenGenerator
{
    /// <summary>
    /// Manages the oxygen generator behavior.
    /// </summary>
    public class OxygenGeneratorManager : MonoBehaviour
    {
        public OxygenGeneratorSize type;

        private OxygenGeneratorAudioVisual audioVisual;

        private void Start()
        {
            audioVisual = GetComponent<OxygenGeneratorAudioVisual>();
            if (audioVisual == null)
            {
                Plugin.Log.LogWarning($"No audio-visual component found for {type} on {gameObject.name}");
            }
        }
    }
}